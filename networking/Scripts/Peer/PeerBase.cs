using System;
using System.Collections.Generic;

//TODO: Search for BTimer references and replace with console app solutions
namespace Altimit.Networking
{
    /// <summary>
    ///     This is an abstract implementation of <see cref="ISocketPeer" /> interface,
    ///     which handles acknowledgements and SendMessage overloads.
    ///     Extend this, if you want to implement custom protocols
    /// </summary>
    public abstract class PeerBase : ISocketPeer
    {
        public ILogger Logger { get; set; }
        public static bool DontCatchExceptionsInEditor = true;

        private static readonly object _idGenerationLock = new object();
        private static int _peerIdGenerator;

        public Action<byte[]> OnBytesReceived { get; set; }

        /// <summary>
        ///     Default timeout, after which response callback is invoked with
        ///     timeout status.
        /// </summary>
        public static int DefaultTimeoutSecs = 60;

        //private readonly Dictionary<int, ResponseCallback> _acks;

        protected readonly List<long[]> _ackTimeoutQueue;
        private readonly Dictionary<int, object> _data;

        private int _id = -1;

        private Dictionary<Type, object> _extensions;

        protected PeerBase()
        {
            _data = new Dictionary<int, object>();
            //_acks = new Dictionary<int, ResponseCallback>(30);
            _ackTimeoutQueue = new List<long[]>();
            _extensions = new Dictionary<Type, object>();

            //BTimer.Instance.OnTick += HandleAckDisposalTick;
            /*
            _timeoutMessage = new IncommingMessage(-1, 0, "Time out".ToBytes(), DeliveryMethod.Reliable, this)
            {
                Status = ResponseStatus.Timeout
            };*/
        }
        public Action<ISocketPeer> Connected { get; set; }
        public Action<ISocketPeer> Disconnected { get; set; }

        public ISocketPeer ServerPeer { get; private set; }

        /// <summary>
        ///     Saves data into peer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void SetProperty(int id, object data)
        {
            if (_data.ContainsKey(id))
                _data[id] = data;
            else
                _data.Add(id, data);
        }

        /// <summary>
        ///     Retrieves data from peer, which was stored with <see cref="SetProperty" />
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetProperty(int id)
        {
            object value;

            _data.TryGetValue(id, out value);

            return value;
        }

        /// <summary>
        ///     Retrieves data from peer, which was stored with <see cref="SetProperty" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object GetProperty(int id, object defaultValue)
        {
            var obj = GetProperty(id);
            return obj ?? defaultValue;
        }

        public T AddExtension<T>(T extension)
        {
            _extensions[typeof(T)] = extension;
            return extension;
        }

        public T GetExtension<T>()
        {
            object extension;
            _extensions.TryGetValue(typeof(T), out extension);
            if (extension == null)
                return default(T);

            return (T)extension;
        }

        public bool HasExtension<T>()
        {
            return _extensions.ContainsKey(typeof(T));
        }

        public void Dispose()
        {
            //BTimer.Instance.OnTick -= HandleAckDisposalTick;
        }

        /// <summary>
        ///     True, if connection is stil valid
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        ///     Sends a message to peer
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="deliveryMethod">Delivery method</param>
        /// <returns></returns>
        public abstract void SendBytes(byte[] bytes, bool isReliable = true);

        /// <summary>
        ///     Force disconnect
        /// </summary>
        /// <param name="reason"></param>
        public abstract void Disconnect();

        public void NotifyDisconnectEvent()
        {
            if (Disconnected != null)
                Disconnected(this);
        }
        private void StartAckTimeout(int ackId, int timeoutSecs)
        {
            // +1, because it might be about to tick in a few miliseconds
            //_ackTimeoutQueue.Add(new[] {ackId, BTimer.CurrentTick + timeoutSecs + 1});
        }

        public void HandleDataReceived(byte[] bytes)
        {
            OnBytesReceived?.Invoke(bytes);
        }


        #region Ack Disposal Stuff

        /// <summary>
        ///     Unique id
        /// </summary>
        public virtual int ID
        {
            get
            {
                if (_id < 0)
                    lock (_idGenerationLock)
                    {
                        if (_id < 0)
                            _id = _peerIdGenerator++;
                    }
                return _id;
            }
        }

        #endregion
    }
}