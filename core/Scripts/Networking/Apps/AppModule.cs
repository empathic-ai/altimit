using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit.Networking;

namespace Altimit
{
    // Legacy implementation for binding services to app modules
    // TODO: Possibly remove in favor of services with no explicit binding to app modules,
    // As app modules could conceivably have a much more dynamic relationship to services
    /*
    public class AppModule<TSubService> : AppModule, IAsyncDisposable where TSubService : IConnectionSingleton
    {
    }

    public class SessionAppModule : AppModule
    {
        //P2PClientModule P2PAppModule => App.Get<P2PClientModule>();

        public override void Dispose()
        {
            base.Dispose();
        }
    }
    
    public class AppModule : Module<App, IAppModule>, IAppModule, IAsyncDisposable
    {
        protected ILogger Logger => App.Logger;
        public App App => container;
        public Action OnDisposed;

        public AppModule()
        {
        }

        public virtual async Task Init()
        {
        }

        public virtual async ValueTask DisposeAsync()
        {
            base.Dispose();
            OnDisposed?.Invoke();
        }
    }
    */
}
