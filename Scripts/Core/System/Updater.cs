using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Altimit
{
    /// <summary>
    ///     This is an object which gets spawned into the application once.
    ///     It's main purpose is to call update methods on a single thread
    /// </summary>
    public class Updater
    {
        private static Updater _instance;

        private Queue<Action> queue = new Queue<Action>();
        private List<IUpdateable> addUpdateables;
        private List<IUpdateable> removeUpdateables;

        private List<IUpdateable> updateables;

        public event Action ApplicationQuit; 

        public static Updater Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Updater();
                }
                return _instance;
            }
        }

        public void OnNextUpdate(Action action)
        {
            queue.Enqueue(action);
        }

        public Updater()
        {
            Awake();
            Update();
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
        }

        private void ProcessExit(object sender, EventArgs e)
        {
            OnApplicationQuit();
        }

        private void Awake()
        {
            updateables = new List<IUpdateable>();
            addUpdateables = new List<IUpdateable>();
            removeUpdateables = new List<IUpdateable>();
        }

        public async void Update()
        {
            try
            {
                if (addUpdateables.Count > 0)
                {
                    updateables.AddRange(addUpdateables);
                    addUpdateables.Clear();
                }
                if (removeUpdateables.Count > 0)
                {
                    updateables = updateables.Except(removeUpdateables).ToList();
                    removeUpdateables.Clear();
                }
                foreach (var updateable in updateables)
                    updateable.Update();

                while (queue.Count > 0)
                {
                    queue.Dequeue()?.Invoke();
                }
            } catch (Exception e)
            {
                OS.LogError(e);
            }
        }

        public void AddUpdateable(IUpdateable updatable)
        {
            if (addUpdateables.Contains(updatable))
                return;

            addUpdateables.Add(updatable);
        }

        public void RemoveUpdateable(IUpdateable updatable)
        {
            removeUpdateables.Add(updatable);
        }

        void OnApplicationQuit()
        {
            if (ApplicationQuit != null)
                ApplicationQuit.Invoke();
        }
    }
}