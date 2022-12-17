using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit.Networking;

namespace Altimit
{
    public class AppModule<T> : AppModule where T : ISessionModule
    {
    }

    public interface IAppModule : IModule<App, IAppModule>
    {
    }

    public class SessionAppModule : AppModule
    {
        P2PClientAM P2PAppModule => App.Get<P2PClientAM>();

        public override async Task OnAdded()
        {
            await base.OnAdded();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public class AppModule : Module<App, IAppModule>, IAppModule
    {
        protected ILogger Logger => App.Logger;
        public App App => container;
        public Action OnDisposed;

        public AppModule()
        {
        }

        public override async Task OnAdded()
        {
            await base.OnAdded();
        }

        public override void Dispose()
        {
            base.Dispose();
            OnDisposed?.Invoke();
        }
    }
}
