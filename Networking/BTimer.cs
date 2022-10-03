using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public class BTimer
    {
        public static long CurrentTick { get; protected set; }
        public delegate void DoneHandler(bool isSuccessful);

        private static BTimer _instance;

        private List<Action> _mainThreadActions;

        /// <summary>
        /// Event, which is invoked every second
        /// </summary>
        public event Action<long> OnTick;

        public event Action ApplicationQuit;

        private readonly object _mainThreadLock = new object();

        public static BTimer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BTimer();
                }
                return _instance;
            }
        }

        public BTimer()
        {
            Awake();
            Update();
        }

        // Use this for initialization
        private void Awake()
        {
            // Framework requires applications to run in background
            //Application.runInBackground = true;

            _mainThreadActions = new List<Action>();
            _instance = this;
            //DontDestroyOnLoad(this);

            //StartCoroutine(StartTicker());
            StartTicker();
        }

        async void Update()
        {
            while (true)
            {
                if (_mainThreadActions.Count > 0)
                {
                    lock (_mainThreadLock)
                    {
                        foreach (var actions in _mainThreadActions)
                        {
                            actions.Invoke();
                        }

                        _mainThreadActions.Clear();
                    }
                }
                await Task.Delay(100);
            }
        }

        /// <summary>
        ///     Waits while condition is false
        ///     If timed out, callback will be invoked with false
        /// </summary>
        /// <param name="condiction"></param>
        /// <param name="doneCallback"></param>
        /// <param name="timeoutSeconds"></param>
        public static void WaitUntil(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds)
        {
            WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds, true);
        }

        /// <summary>
        ///     Waits while condition is true
        ///     If timed out, callback will be invoked with false
        /// </summary>
        /// <param name="condiction"></param>
        /// <param name="doneCallback"></param>
        /// <param name="timeoutSeconds"></param>
        public static void WaitWhile(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds)
        {
            WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds);
        }

        private static async void WaitWhileTrueCoroutine(Func<bool> condition, DoneHandler callback,
            float timeoutSeconds, bool reverseCondition = false)
        {
            while ((timeoutSeconds > 0) && (condition.Invoke() == !reverseCondition))
            {
                timeoutSeconds -= 100/1000.0f;
                await Task.Delay(100);
            }

            callback.Invoke(timeoutSeconds > 0);
        }

        public static void AfterSeconds(float time, Action callback)
        {
            Instance.StartWaitingSeconds(time, callback);
        }

        public static void ExecuteOnMainThread(Action action)
        {
            Instance.OnMainThread(action);
        }

        public void OnMainThread(Action action)
        {
            lock (_mainThreadLock)
            {
                _mainThreadActions.Add(action);
            }
        }

        private async void StartWaitingSeconds(float time, Action callback)
        {
            await Task.Delay((int)(time*1000));
            callback.Invoke();
        }

        private async void StartTicker()
        {
            CurrentTick = 0;
            while (true)
            {
                await Task.Delay(1000);
                CurrentTick++;
                if (OnTick != null)
                    OnTick.Invoke(CurrentTick);
            }
        }

        void OnDestroy()
        {
        }

        void OnApplicationQuit()
        {
            if (ApplicationQuit != null)
                ApplicationQuit.Invoke();
        }
    }
}