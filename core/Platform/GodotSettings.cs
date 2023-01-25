using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Altimit.UI;

namespace Altimit
{
#if GODOT
    public class GodotSettings : PlatformSettings
    {
        // TODO: Change to Godot.OS.GetDataDir() for non-editor apps
        public override string AssetsPath => Godot.ProjectSettings.GlobalizePath("res://");

        [DebuggerHidden]
        [StackTraceIgnore]
        public override ILogger GetLogger(string name)
        {
            return new Logger(name,
                x => Godot.GD.Print(x),//Updater.Instance.OnNextUpdate(() => GD.Print(x)),
                x => Updater.Instance.OnNextUpdate(() => Godot.GD.PushWarning((string)x)),
                // TODO: Add custom Altimit UI console for better error handling
                x =>
                {
                    Godot.GD.PrintErr(x);
                    Godot.GD.PushError(x.InnerException.Message);
                });
        }

        public static Godot.Node SceneRoot;

        public override Node GetRootNode()
        {
            var node = new Node();
            SceneRoot.AddChild(node.GDNode);
            return node;
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
    }
#endif
}