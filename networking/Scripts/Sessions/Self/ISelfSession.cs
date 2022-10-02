using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Altimit.Networking;

namespace Altimit
{
    public interface ISelfSession : ISession
    {
        App App { get; }

        Guid PeerAppID { get; }

        /*
         *         App App { get; }
IPeerSession GetPeerSession();
ILogger Logger { get; }
ObservableHashset<object> Instances { get; }

public Task Init(IPeerSession session);
public void OnPropertyChanged(object instance, string propertyName, object oldProperty);
//public void OnInstanceAdded(object instance, bool isInstanceGroup);
//public void OnInstanceGroupAdded(object[] instanceGroup);
public void OnInstanceRemoved(Guid instanceID, object instance);
public void OnAssetAdded(Guid instanceID, byte[] bytes);

public bool TryAddAndTrackObject(object instance, FormatType formatType);
public bool HasAuthority(object instance);
public bool HasAuthority(object instance, string propertyName);
*/
    }
}
