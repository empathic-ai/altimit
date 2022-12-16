using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Altimit
{
    public abstract class PlatformSettings
    {
        public abstract string AssetsPath { get; }
        public abstract ILogger GetLogger(string name);

        public bool IsDesktop = true;
        // TODO: Set whether is playing or not properly per platform, possibly refactor
        public bool IsPlaying = true;
        public Guid MasterServerAppID = new Guid("cd366557-e227-4c2f-8479-c418da933ad0");
        public Guid P2PServerAppID = new Guid("cd366557-e227-4c2f-8479-c418da933ad0");
        public int P2PServerPort = 8080;//8080;// 8080; //1776;
        public string FullURL
        {
            get
            {
                return "https://" + MasterURL + "/";
            }
        }

        public string MasterURL = "localhost";//"meridianvr.ddns.net";// "meridianvr.ddns.net";//"meridianvr.com"; //"216.99.113.162";

        public virtual void Init()
        {
            // Define whether the current target device is desktop or VR
            //if (!Application.isEditor)
            //{
            IsDesktop = !Environment.GetCommandLineArgs().Contains("-vr");
            //}
        }
    }
}