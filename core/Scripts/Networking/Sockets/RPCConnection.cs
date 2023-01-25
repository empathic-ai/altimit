#define ALTIMIT_JSON
using Altimit;
using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using TaskExtensions = Altimit.TaskExtensions;
using System.Reflection;
#if ALTIMIT_JSON
using Newtonsoft.Json;
#endif

namespace Altimit.Networking
{
    // Routes messages to a given socket
    public class RPCConnection<TSelf, TPeer>
    {
        string sessionModuleTypeName;

        Dictionary<int, CancellationTokenSource> tokenSourcesByTaskID = new Dictionary<int, CancellationTokenSource>();
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public TSelf Self;
        public TPeer Peer;
        public IPeerSocket PeerSocket;
        ILogger Logger;

        int receiveID = 0;
        int maxSendID = 0;
        int maxTaskID = 0;

        Dictionary<int, TaskCompletionSource<object>> TaskCompletionSources = new Dictionary<int, TaskCompletionSource<object>>();

        List<NetworkMethodCall> methodCallQueue = new List<NetworkMethodCall>();

        public RPCConnection(TSelf self, IPeerSocket peerSocket, ILogger logger)
        {
            this.Logger = logger;
            this.PeerSocket = peerSocket;

            Self = self;
            Peer = ProxyTypeFactory.CreateProxyOfType<TPeer>();
            ((IProxy)Peer).ProxyMethodCalled = OnPeerMethodCalled;

            PeerSocket.OnBytesReceived += OnBytesReceived;
            PeerSocket.Disconnected += OnPeerDisconnected;
        }

        private object OnPeerMethodCalled(object sender, MethodCalledEventArgs e)
        {
            //OS.Log(e.MethodName + " CALLED");
            var taskReturnType = sender.GetType().GetMethodInfo(e.MethodName, e.MethodTypes).ReturnType.GetGenericArguments()[0];
            var methodInfo = this.GetType().GetMethod(nameof(this.SendTaskAsync)).MakeGenericMethod(taskReturnType);

            var task = methodInfo.Invoke(this, new object[] { e.MethodName, e.MethodTypes, e.MethodArgs });

            //new Type[] { typeof(string), typeof(Type[]), typeof(object[]) });
            //(nameof(this.SendAsync), new Type[] { typeof(string), typeof(Type[]), typeof(object[]) }, e.MethodName, e.MethodTypes, e.MethodArgs);
            //return SendAsync();

            // PeerSocket.SendBytes(new NetworkMethodCall(-1, -1, e.MethodName, e.MethodTypes, false, e.MethodArgs));
            return task;
        }

        void OnPeerDisconnected(IPeerSocket peer)
        {
            CancelTasks();
        }

        /*
        public T AddRemoteService<T>() where T : IService
        {
            return (T)AddRemoteService(typeof(T));
        }

        public IService AddRemoteService(Type type)
        {
            return AddService(RelayedServiceFactory.CreateRelayedService(type));
        }
        */

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

        /*
        // Sends a regular synchronous method over the network
        public void Send(string methodName, string[] typeNames, object[] args)
        {
            var methodTypes = typeNames.Select(x => TypeExtensions.GetTypeByAName(x)).ToArray();
            var sessionModuleType = TypeExtensions.GetTypeByAName(sessionModuleTypeName);

            // Create TaskCompletionSource with a TaskID and associate it with this method call's CancellationToken (if it exists)
            int taskID = maxTaskID++;
            CancellationToken cancellationToken = CreateCancellationToken(taskID, args);

            //
            //await CheckForTracking(sessionModuleType, methodName, methodTypes, args, cancellationToken);

            try
            {
                _ = SendAsync(sessionModuleType, methodName, methodTypes, args, taskID, cancellationToken);
            }
            catch (OperationCanceledException) { }
        }

        // Sends an async method without a return argument over the network
        public async Task SendAsync(string methodName, Type[] methodTypes, object[] methodArgs)
        {
            var methodTypes = methodTypes.Select(x => TypeExtensions.GetTypeByAName(x)).ToArray();
            var sessionModuleType = TypeExtensions.GetTypeByAName(sessionModuleTypeName);
            if (!sessionModuleType.GetMethodInfo(methodName, methodTypes).HasResponse)
            {
                Send(methodName, methodTypes, methodArgs);
                return;
            }

            // Create TaskCompletionSource with a TaskID and associate it with this method call's CancellationToken (if it exists)
            int taskID = maxTaskID++;
            TaskCompletionSource<object> taskCompletionSource = CreateTaskCompletionSource(taskID);
            CancellationToken cancellationToken = CreateCancellationToken(taskID, methodArgs);

            try
            {
                //
                //await CheckForTracking(methodName, methodTypes, args, cancellationToken);

                // Send message
                await SendAsync(sessionModuleType, methodName, methodTypes, methodArgs, taskID, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                // Await response
                await taskCompletionSource.Task;
                cancellationToken.ThrowIfCancellationRequested();
                tokenSourcesByTaskID.Remove(taskID);
            }
            catch (OperationCanceledException) { }
        }
        */

        // Sends an async method with a return argument over the network
        public async Task<T> SendTaskAsync<T>(string methodName, Type[] methodTypes, object[] methodArgs)
        {
            //var methodTypes = methodTypes.Select(x => TypeExtensions.GetTypeByAName(x)).ToArray();
            /*
            var sessionModuleType = TypeExtensions.GetTypeByAName(sessionModuleTypeName);
            if (!sessionModuleType.GetMethodInfo(methodName, methodTypes.Select(x => TypeExtensions.GetTypeByAName(x)).ToArray()).HasResponse)
            {
                Send(methodName, methodTypes, methodArgs);
                return default(T);
            }
            */

            // Create TaskCompletionSource with a TaskID and associate it with this method call's CancellationToken (if it exists)
            int taskID = maxTaskID++;
            TaskCompletionSource<object> taskCompletionSource = CreateTaskCompletionSource(taskID);
            CancellationToken cancellationToken = CreateCancellationToken(taskID, methodArgs);

            try
            {
                //
                //await CheckForTracking(sessionModuleType, methodName, methodTypes, args, cancellationToken);

                // Send message
                await SendAsync(methodName, methodTypes, methodArgs, taskID, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    // Await response and return result
                    var result = await taskCompletionSource.Task;
                    tokenSourcesByTaskID.Remove(taskID);

                    if (OS.LogMethodCalls)
                        Logger.Log($"Received result for {methodName}. " +
                            $"Result was of type {result.GetType().ToDereferencedString()} and value {result.ToDereferencedString()}.");

                    return result == null ? default(T) : (T)result;
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Failed to process result of method call named {0}.", methodName), e);
                }
            }
            catch (OperationCanceledException e)
            {
                OS.LogError(e);
                //return null;
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


        // The core method to serialize and transmit a method call over the network
        public async Task SendAsync(string methodName, Type[] methodTypes, object[] args, int taskID = -1, CancellationToken cancellationToken = default)
        {
            try
            {
                int methodSendID;
                // Serialize method call
                if (typeof(TPeer).GetMethodInfo(methodName, methodTypes).ProtocolType == ProtocolType.Sequential)
                {
                    methodSendID = maxSendID;
                    maxSendID++;
                }
                else
                {
                    methodSendID = -1;
                }

                var methodInfo = typeof(TPeer).GetMethodInfo(methodName, methodTypes);

                var methodCall = new NetworkMethodCall(methodSendID, taskID, methodName, methodTypes, false, args); //type.GetNativeTypeName(), 
                if (OS.LogMethodCalls)
                {
                    var messageString = methodCall.OrderID == -1 ? "message" : "message #" + methodCall.OrderID.ToString();
                    Logger.Log($"Sending {messageString} named {methodInfo} with arguments {methodCall.Args.ToNestedString()}.");
                }

                await Task.Run(() => SendMethodCallAsync(methodCall, cancellationToken).Wait(cancellationToken), cancellationToken);
            }
            catch (TaskCanceledException e)
            {
            }
            catch (Exception e)
            {
                Logger.LogError(new Exception($"Failed to send a method call with an ID of {methodName}", e));
            }
        }

        public async Task SendMethodCallAsync(NetworkMethodCall methodCall, CancellationToken cancellationToken = default)
        {
            try
            {
#if ALTIMIT_JSON
                var jsonString = JsonConvert.SerializeObject(methodCall);
                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                //OS.Log("SENT: " + jsonString);
#else
                var bytes = await Serializer.SerializeAsync(methodCall, cancellationToken);
#endif

                cancellationToken.ThrowIfCancellationRequested();

                /*
                if (OS.LogMethodCalls)
                {
                    var methodInfo = type.GetMethodInfo(methodCall.MethodName, methodCall.MethodTypes);
                    var messageString = methodCall.OrderID == -1 ? "message" : "message #" + methodCall.OrderID.ToString();
                    Logger.Log($"Sending {messageString} named {methodInfo} with arguments {methodCall.Args.LocalToString()}.");
                }*/

                // Send serialized method call
                PeerSocket.SendBytes(bytes);
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
                Logger.Log($"Received message: {PersistentDatabaseExtensions.ToNestedString(bytes)}.");

            //try
            //{
            // Convert bytes into a method call object, containing the method's ID and arguments
#if ALTIMIT_JSON
            var jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            OS.Log("RECEIVED: " + jsonString);
            var methodCall = JsonConvert.DeserializeObject<NetworkMethodCall>(jsonString);
#else
            var methodCall = await Serializer.DeserializeAsync<NetworkMethodCall>(bytes);
#endif
            tokenSource.Token.ThrowIfCancellationRequested();
            // Grab the session module type that includes the method
            //var methodCallType = TypeExtensions.GetTypeByAName(methodCall.TypeName);

            // Get the method info of this method call--if it's a response, the peer owns the method. Otherwise this app does
            var methodInfo = (methodCall.IsResponse ? typeof(TPeer) : typeof(TSelf)).GetMethodInfo(methodCall.MethodName, methodCall.MethodTypes);

            try
            {
                if (OS.LogMethodCalls)
                {
                    var messageString = methodCall.OrderID == -1 ? "peer message" : "peer message #" + methodCall.OrderID.ToString();
                    var responseString = methodCall.IsResponse ? "a response to" : "calling";
                    Logger.Log($"Received {messageString} which is {responseString} {methodInfo}. " +
                        $"Currently on thread {Thread.CurrentThread.ManagedThreadId} with localized arguments {PersistentDatabaseExtensions.ToNestedString(methodCall.Args)}. " +
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
                // TODO: Reimplement
                //
                /*
                if (methodCall.OrderID != -1)
                {
                    while (receiveID != methodCall.OrderID || (Peer.App.Get<ReplicationModule>().IsResolving))
                    {
                        await Task.Delay(25);
                    }
                }
                */

                tokenSource.Token.ThrowIfCancellationRequested();

                // If we're dealing with a response, PeerGenerator.SendAsync() is waiting for that response using a TaskCompletionSource
                // Find the TaskCompletionSource associated with this method call's task ID and set the result
                if (methodCall.IsResponse)
                {
                    object result = null;
                    if (methodInfo.ReturnType != typeof(Task))
                    {
                        result = methodCall.Args[0];
                        /*
                        if (!methodInfo.IsReturnForceValue)
                        {
                            result = await instanceDatabase.ReferenceProperty(result);
                        }*/
                    }
                    GetTaskCompletionSource(methodCall.TaskID).SetResult(result);

                    // If this method call has an order ID, set this peer's receive order ID to this method's order ID
                    if (methodCall.OrderID != -1)
                        receiveID = methodCall.OrderID + 1;
                }
                else
                {
                    //
                    /*
                    for (int i = 0; i < methodInfo.ParameterTypes.Length; i++)
                    {
                        if (!methodInfo.ParameterForceValues[i])
                            methodCall.Args[i] = await instanceDatabase.ReferenceProperty(methodCall.Args[i]);
                    }
                    */

                    if (OS.LogMethodCalls)
                        Logger?.Log(string.Format("Calling method #{0}: {1} with globalized arguments {2}.", methodCall.OrderID, methodInfo, methodCall.Args.ToDereferencedString()));

                    // Call the method the peer wanted us to call
                    //
                    var task = (Task)methodInfo.Invoke(Self, methodCall.Args);

                    // If this method call has an order ID, set this peer's receive order ID to this method's order ID
                    if (methodCall.OrderID != -1)
                        receiveID = methodCall.OrderID + 1;

                    // Rrepresents the return value
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
                            //await replicationService.EnsureTracked(result, tokenSource.Token);
                            //result = result.DereferenceProperty();
                        }
                    }
                    else if (returnType == typeof(Task))
                    {
                        throw new NotImplementedException();
                        //await task;
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
                            Logger.Log(string.Format("Sending message #{0} as response to method named {1} with return value {2}.", orderID, methodInfo, PersistentDatabaseExtensions.ToNestedString(methodArgs)));
                        }

                        await SendMethodCallAsync(new NetworkMethodCall(orderID, methodCall.TaskID, methodCall.MethodName, methodCall.MethodTypes, true, methodArgs), tokenSource.Token);
                        tokenSource.Token.ThrowIfCancellationRequested();
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

        public void Dispose()
        {
            //base.Dispose();

            CancelTasks();

            PeerSocket.OnBytesReceived -= OnBytesReceived;
            PeerSocket.Disconnected -= OnPeerDisconnected;
            PeerSocket.Disconnect();
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
