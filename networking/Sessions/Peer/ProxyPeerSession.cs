using Altimit;
using Altimit.Networking;
using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using TaskExtensions = Altimit.TaskExtensions;

namespace Altimit.Networking
{
    // Represents a networked peer session
    // Modules of this session are generated using GenerateModule--the generated modules' methods call Send() on NetworkSession to broadcast messages over the network
    public class ProxyPeerSession : Session
    {
        protected override ISession container => this;
        ReplicationSM baseSM => Peer.Get<ReplicationSM>();
        InstanceDatabase instanceDatabase => App.Get<ReplicationAM>().InstanceDB;
        Dictionary<int, CancellationTokenSource> tokenSourcesByTaskID = new Dictionary<int, CancellationTokenSource>();
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public ISocketPeer SocketPeer;
        ILogger Logger;

        int receiveID = 0;
        int maxSendID = 0;
        int maxTaskID = 0;

        Dictionary<int, TaskCompletionSource<object>> TaskCompletionSources = new Dictionary<int, TaskCompletionSource<object>>();

        List<NetworkMethodCall> methodCallQueue = new List<NetworkMethodCall>();

        public ProxyPeerSession(App app, ISocketPeer peer, ILogger logger)
        {
            App = app;
            this.Logger = logger;
            this.SocketPeer = peer;
            AddProxyModule<IReplicationSM>();

            peer.OnBytesReceived += OnBytesReceived;
            peer.Disconnected += OnPeerDisconnected;
        }

        void OnPeerDisconnected(ISocketPeer peer)
        {
            CancelTasks();
        }

        
        public T AddProxyModule<T>() where T : ISessionModule
        {
            return (T)AddProxyModule(typeof(T));
        }

        public ISessionModule AddProxyModule(Type type)
        {
            return AddModule(ProxySMFactory.CreateModule(type));
        }

        public TaskCompletionSource<object> CreateTaskCompletionSource(int taskID)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            TaskCompletionSources.Add(taskID, taskCompletionSource);
            return taskCompletionSource;
        }

        public void RemoveTaskCompletionSource(int taskID)
        {
            TaskCompletionSources.Remove(taskID);
        }

        public TaskCompletionSource<object> GetTaskCompletionSource(int taskID)
        {
            return TaskCompletionSources[taskID];
        }

        public async Task<object> DynamicTask<T>(Task task)
        {
            var newTask = task as Task<T>;
            return await newTask;
        }

        // Sends a regular synchronous method over the network
        public async void Send(string sessionModuleTypeName, string methodName, string[] typeNames, object[] args)
        {
            var methodTypes = typeNames.Select(x => TypeExtensions.GetType(x)).ToArray();
            var sessionModuleType = TypeExtensions.GetType(sessionModuleTypeName);

            // Create TaskCompletionSource with a TaskID and associate it with this method call's CancellationToken (if it exists)
            int taskID = maxTaskID++;
            CancellationToken cancellationToken = CreateCancellationToken(taskID, args);
            
            await CheckForTracking(sessionModuleType, methodName, methodTypes, args, cancellationToken);

            try
            {
                _ = SendAsync(sessionModuleType, methodName, methodTypes, args, taskID, cancellationToken);
            }
            catch (OperationCanceledException) { }
        }

        // Sends an async method without a return argument over the network
        public async Task SendAsync(string sessionModuleTypeName, string methodName, string[] typeNames, object[] args)
        {
            var methodTypes = typeNames.Select(x => TypeExtensions.GetType(x)).ToArray();
            var sessionModuleType = TypeExtensions.GetType(sessionModuleTypeName);
            if (!sessionModuleType.GetMethodInfo(methodName, methodTypes).HasResponse)
            {
                Send(sessionModuleTypeName, methodName, typeNames, args);
                return;
            }

            // Create TaskCompletionSource with a TaskID and associate it with this method call's CancellationToken (if it exists)
            int taskID = maxTaskID++;
            TaskCompletionSource<object> taskCompletionSource = CreateTaskCompletionSource(taskID);
            CancellationToken cancellationToken = CreateCancellationToken(taskID, args);

            try
            {
                await CheckForTracking(sessionModuleType, methodName, methodTypes, args, cancellationToken);

                // Send message
                await SendAsync(sessionModuleType, methodName, methodTypes, args, taskID, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                // Await response
                await taskCompletionSource.Task;
                cancellationToken.ThrowIfCancellationRequested();
                tokenSourcesByTaskID.Remove(taskID);
            }
            catch (OperationCanceledException) { }
        }

        // Sends an async method with a return argument over the network
        public async Task<T> SendAsync<T>(string sessionModuleTypeName, string methodName, string[] typeNames, object[] args)
        {
            var methodTypes = typeNames.Select(x => TypeExtensions.GetType(x)).ToArray();
            var sessionModuleType = TypeExtensions.GetType(sessionModuleTypeName);
            if (!sessionModuleType.GetMethodInfo(methodName, typeNames.Select(x => TypeExtensions.GetType(x)).ToArray()).HasResponse)
            {
                Send(sessionModuleTypeName, methodName, typeNames, args);
                return default(T);
            }

            // Create TaskCompletionSource with a TaskID and associate it with this method call's CancellationToken (if it exists)
            int taskID = maxTaskID++;
            TaskCompletionSource<object> taskCompletionSource = CreateTaskCompletionSource(taskID);
            CancellationToken cancellationToken = CreateCancellationToken(taskID, args);

            try
            {
                await CheckForTracking(sessionModuleType, methodName, methodTypes, args, cancellationToken);

                // Send message
                await SendAsync(sessionModuleType, methodName, methodTypes, args, taskID, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    // Await response and return result
                    var result = await taskCompletionSource.Task;
                    tokenSourcesByTaskID.Remove(taskID);

                    if (OS.LogMethodCalls)
                        Logger.Log($"Received result for {methodName}. " +
                            $"Result was of type {result.GetType().GlobalToString()} and value {result.GlobalToString()}. " +
                            $"Expecting type {typeof(T)}.");

                    return result == null ? default(T) : (T)result;
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Failed to process result of method call named {0}.", methodName), e);
                }
            }
            catch (OperationCanceledException) {
                return default(T);
            }
        }

        public CancellationToken CreateCancellationToken(int taskID, object[] args)
        {
            CancellationToken? cancellationToken = null;
            // Find this method call's cancellation token (if it exists) so that when a response is received the token can be used
            for (int i = args.Length - 1; i >= Math.Max(0, args.Length - 2); i--)
            {
                if (args[i] != null && args[i].GetType() == typeof(CancellationToken))
                {
                    cancellationToken = (CancellationToken)args[i];
                    // Remove the cancellation token from the arguments to be serialized for this method call
                    // TODO: Possibly change this argument to an ID representing the token so it can be used on the receiving end
                    args[i] = null;
                    break;
                }
            }

            if (cancellationToken == null)
            {
                var tokenSource = new CancellationTokenSource();
                cancellationToken = tokenSource.Token;
                tokenSourcesByTaskID.Add(taskID, tokenSource);
            }
            else
            {
                var tokenSource = new CancellationTokenSource();
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, (CancellationToken)cancellationToken).Token;
                tokenSourcesByTaskID.Add(taskID, tokenSource);
            }

            return (CancellationToken)cancellationToken;
        }


        public async Task CheckForTracking(Type type, string methodName, Type[] methodTypes, object[] args, CancellationToken cancellationToken)
        {
            var methodInfo = type.GetMethodInfo(methodName, methodTypes);
            // Convert references to instance IDs
            for (int i = 0; i < methodInfo.ParameterTypes.Length; i++)
            {
                if (!methodInfo.ParameterForceValues[i])
                {
                    var arg = args[i];
                    await baseSM.EnsureTracked(arg, cancellationToken);
                    //baseSM.TryAddAndTrackObject(arg);
                    args[i] = arg.LocalizeProperty();
                }
            }
        }


        // The core method to serialize and transmit a method call over the network
        public async Task SendAsync(Type type, string methodName, Type[] methodTypes, object[] args, int taskID = -1, CancellationToken cancellationToken = default)
        {
            try
            {
                int methodSendID;
                // Serialize method call
                if (type.GetMethodInfo(methodName, methodTypes).ProtocolType == ProtocolType.Sequential)
                {
                    methodSendID = maxSendID;
                    maxSendID++;
                }
                else
                {
                    methodSendID = -1;
                }

                var methodInfo = type.GetMethodInfo(methodName, methodTypes);

                var methodCall = new NetworkMethodCall(type.GetNativeTypeName(), methodSendID, taskID, methodName, methodTypes, false, args);
                if (OS.LogMethodCalls)
                {
                    var messageString = methodCall.OrderID == -1 ? "message" : "message #" + methodCall.OrderID.ToString();
                    Logger.Log($"Sending {messageString} named {methodInfo} with arguments {methodCall.Args.LocalToString()}.");
                }

                await Task.Run(() => SendAsync(type, methodCall, cancellationToken).Wait(cancellationToken), cancellationToken);
            }
            catch (TaskCanceledException e)
            {
            }
            catch (Exception e)
            {
                Logger.LogError(new Exception($"Failed to send a method call with an ID of {methodName}", e));
            }
        }

        public async Task SendAsync(Type type, NetworkMethodCall methodCall, CancellationToken cancellationToken)
        {
            try
            {
                var bytes = await Serializer.SerializeAsync(methodCall, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                /*
                if (OS.LogMethodCalls)
                {
                    var methodInfo = type.GetMethodInfo(methodCall.MethodName, methodCall.MethodTypes);
                    var messageString = methodCall.OrderID == -1 ? "message" : "message #" + methodCall.OrderID.ToString();
                    Logger.Log($"Sending {messageString} named {methodInfo} with arguments {methodCall.Args.LocalToString()}.");
                }*/

                // Send serialized method call
                SocketPeer.SendBytes(bytes);
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                Logger.LogError(new Exception($"Failed to cancel task with an ID of {methodCall.TaskID}.", e));
            }
        }

        // When bytes are received from one of the peer sockets, deserialize them into a method call and call the method
        public async void OnBytesReceived(byte[] bytes)
        {
            if (OS.LogMethodCalls)
                Logger.Log($"Received message: {DBExtensions.LocalToString(bytes)}.");

            //try
            //{
            // Convert bytes into a method call object, containing the method's ID and arguments
            var methodCall = await Serializer.DeserializeAsync<NetworkMethodCall>(bytes);
            tokenSource.Token.ThrowIfCancellationRequested();
            // Grab the session module type that includes the method
            var methodCallType = TypeExtensions.GetType(methodCall.TypeName);

            // Get the method info of this method call--if it's a response, the peer owns the method. Otherwise this app does
            var methodInfo = methodCallType.GetMethodInfo(methodCall.MethodName, methodCall.MethodTypes);

            try
            {
                if (OS.LogMethodCalls)
                {
                    var messageString = methodCall.OrderID == -1 ? "peer message" : "peer message #" + methodCall.OrderID.ToString();
                    var responseString = methodCall.IsResponse ? "a response to" : "calling";
                    Logger.Log($"Received {messageString} which is {responseString} {methodInfo}. " +
                        $"Currently on thread {Thread.CurrentThread.ManagedThreadId} with localized arguments {DBExtensions.LocalToString(methodCall.Args)}. " +
                        $"Waiting to receive message #{receiveID}.");
                }

                /*
                int i = 0;
                while (i < methodCallQueue.Count) {
                    if (methodCall.OrderID < methodCallQueue[i].OrderID)
                    {
                        break;
                    }
                    i++;
                }
                methodCallQueue.Insert(i, methodCall);
                */

                // If this method call has an order ID, don't call it until its order ID is the next expected one
                if (methodCall.OrderID != -1)
                {
                    while (receiveID != methodCall.OrderID || (Peer.App.Get<ReplicationAM>().IsResolving))
                    {
                        await Task.Delay(25);
                    }
                }

                tokenSource.Token.ThrowIfCancellationRequested();

                // If we're dealing with a response, PeerGenerator.SendAsync() is waiting for that response using a TaskCompletionSource
                // Find the TaskCompletionSource associated with this method call's task ID and set the result
                if (methodCall.IsResponse)
                {
                    object result = null;
                    if (methodInfo.ReturnType != typeof(Task))
                    {
                        result = methodCall.Args[0];
                        if (!methodInfo.IsReturnForceValue)
                        {
                            result = await instanceDatabase.GlobalizeProperty(result);
                        }
                    }
                    GetTaskCompletionSource(methodCall.TaskID).SetResult(result);

                    // If this method call has an order ID, set this peer's receive order ID to this method's order ID
                    if (methodCall.OrderID != -1)
                        receiveID = methodCall.OrderID + 1;
                }
                else
                {
                    for (int i = 0; i < methodInfo.ParameterTypes.Length; i++)
                    {
                        if (!methodInfo.ParameterForceValues[i])
                            methodCall.Args[i] = await instanceDatabase.GlobalizeProperty(methodCall.Args[i]);
                    }

                    if (OS.LogMethodCalls)
                        Logger?.Log(string.Format("Calling method #{0}: {1} with globalized arguments {2}.", methodCall.OrderID, methodInfo, methodCall.Args.GlobalToString()));

                    // Call the method the peer wanted us to call
                    var task = (Task)methodInfo.Invoke(Peer.GetModule(methodCallType), methodCall.Args);

                    // If this method call has an order ID, set this peer's receive order ID to this method's order ID
                    if (methodCall.OrderID != -1)
                        receiveID = methodCall.OrderID + 1;

                    object result = null;
                    var returnType = methodInfo.ReturnType;

                    // If there's going to be a response and it contains information, localize that information
                    var isGenericTask = returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>);

                    if (isGenericTask)
                    {
                        var method = typeof(TaskExtensions).GetMethod(nameof(TaskExtensions.DynamicTask)).MakeGenericMethod(new Type[] { returnType.GetGenericArguments()[0] });
                        var taskObject = method.Invoke(null, new object[] { task });
                        result = await (Task<object>)taskObject;

                        tokenSource.Token.ThrowIfCancellationRequested();

                        if (!methodInfo.IsReturnForceValue)
                        {
                            await baseSM.EnsureTracked(result, tokenSource.Token);
                            result = result.LocalizeProperty();
                        }
                    } else if (returnType == typeof(Task))
                    {
                        await task;
                    }

                    // If there is a response, send the response
                    if (methodInfo.HasResponse && (isGenericTask || returnType == typeof(Task)))
                    {
                        int orderID;
                        if (methodInfo.ProtocolType == ProtocolType.Sequential)
                        {
                            orderID = maxSendID;
                            maxSendID++;
                        }
                        else
                        {
                            orderID = -1;
                        }

                        var methodArgs = isGenericTask ? new object[] { result } : new object[0];

                        if (OS.LogMethodCalls)
                        {
                            Logger.Log(string.Format("Sending message #{0} as response to method named {1} with return value {2}.", orderID, methodInfo, DBExtensions.LocalToString(methodArgs)));
                        }
                        var responseBytes = await Serializer.SerializeAsync(
                            new NetworkMethodCall(methodCall.TypeName, orderID, methodCall.TaskID, methodCall.MethodName, methodCall.MethodTypes, true, methodArgs));
                        tokenSource.Token.ThrowIfCancellationRequested();
                        SocketPeer.SendBytes(responseBytes);
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                throw new Exception(
                    string.Format("Failed to process {0} named {1}.", methodCall.IsResponse ? "response to a method call" : "method call", methodInfo),
                    e);
            }
            /*
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }*/
        }

        public override void Dispose()
        {
            base.Dispose();

            CancelTasks();

            SocketPeer.OnBytesReceived -= OnBytesReceived;
            SocketPeer.Disconnected -= OnPeerDisconnected;
            SocketPeer.Disconnect();
        }

        void CancelTasks()
        {
            foreach (var tokenSourceByTaskID in tokenSourcesByTaskID)
            {
                //logger.LogFormat("Cancelling task with an ID of {0}.", tokenSourceByTaskID.Key);
                tokenSourceByTaskID.Value.Cancel();
            }
            tokenSource.Cancel();
        }
    }
}
