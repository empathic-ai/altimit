using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#if !UNITY_64
using TextCopy;
#endif

namespace Altimit.UI
{
    public class Input : Singleton<Input>
    {
#if GODOT
        public static Vector2 ScreenSize => (Vector2)Godot.DisplayServer.ScreenGetSize(0) + new Vector2(Godot.DisplayServer.ScreenGetSize(1).x, 0);
#elif WEB
        public static Vector2 ScreenSize { get; set; }
#else
        public static Vector2 ScreenSize { get; set; }
#endif
        public static bool IsOutOfBounds(Vector2 screenPos)
        {
            if (screenPos.x < 0 || screenPos.y < 0 || screenPos.x > Input.ScreenSize.x || screenPos.y > Input.ScreenSize.y)
                return true;

            return false;
        }

        public Input() : base()
        {
            AutoUpdate();
        }

        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(Vector2 position)
        {
            SetCursorPos((int)position.x, (int)position.y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static Vector2 GetMousePosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return new Vector2(currentMousePoint.X, currentMousePoint.Y); ;
        }

        public static void SetMouseEvent(MouseEventFlags value)
        {
            Vector2 position = GetMousePosition();

            mouse_event
                ((int)value,
                 (int)position.x,
                 (int)position.y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(int button);

        public static Action OnMouseDown;
        public static Action OnMouseHeld;
        public static Action OnMouseUp;

        public static bool IsMouseButtonPressed(MouseButton button)
        {
            return GetAsyncKeyState((int)button);
        }
        bool wasMousePressed = false;

        public async void AutoUpdate()
        {
            while (true)
            {
                await Task.Delay(1);

                if (Input.IsMouseButtonPressed(Input.MouseButton.LeftMouseButton))//Input.IsActionPressed("Mouse"))
                {
                    if (!wasMousePressed)
                        OnMouseDown?.Invoke();

                    wasMousePressed = true;
                    OnMouseHeld?.Invoke();
                }
                else
                {
                    if (wasMousePressed)
                        OnMouseUp?.Invoke();

                    wasMousePressed = false;
                }
            }
        }

        public enum MouseButton
        {
            LeftMouseButton = 0x01,
            RightMouseButton = 0x02,
            MiddleMouseButton = 0x04,
        }

        public static void SetTextInClipboard(string text)
        {
#if !UNITY_64
            ClipboardService.SetText(text);
#endif
        }
    }
}