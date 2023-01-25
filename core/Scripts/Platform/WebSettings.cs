using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Altimit.UI;

namespace Altimit
{
#if WEB
    public class WebSettings : PlatformSettings
    {
        // TODO: Change to Godot.OS.GetDataDir() for non-editor apps
        public override string AssetsPath => throw new NotImplementedException();

        [DebuggerHidden]
        [StackTraceIgnore]
        public override ILogger GetLogger(string name)
        {
            return new Logger(name,
                x => Console.WriteLine(x),//Updater.Instance.OnNextUpdate(() => GD.Print(x)),
                x => Updater.Instance.OnNextUpdate(() => Console.WriteLine((string)x)),
                // TODO: Add custom Altimit UI console for better error handling
                x =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(x.ToString() + "\n");
                    Console.ResetColor();
                });
        }

        public override void Init()
        {
            base.Init();
        }

        public override Vector3 GetMousePosition()
        {
            throw new NotImplementedException();
        }

        public override bool IsMouseDown()
        {
            throw new NotImplementedException();
        }

        public override bool IsKeyDown(string keyName)
        {
            throw new NotImplementedException();
        }

        public override Node GetRootNode()
        {
            throw new NotImplementedException();
        }
    }
#endif
}