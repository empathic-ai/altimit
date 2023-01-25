using Altimit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Altimit
{
    public abstract class PlatformSettings
    {
        public abstract string AssetsPath { get; }
        [DebuggerHidden]
        public abstract ILogger GetLogger(string name);

        public bool IsDesktop = true;
        // TODO: Set whether is playing or not properly per platform, possibly refactor
        public bool IsPlaying = true;
        public AID MasterServerAppID = new AID("27059dac-1b4f-4ff4-bdcd-6fc991c02563");

        public int MasterPort = 8080;//8080;// 8080; //1776;
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

        public abstract Node GetRootNode();

        public abstract Vector3 GetMousePosition();
        public abstract bool IsMouseDown();
        public abstract bool IsKeyDown(string keyName);
    }
}