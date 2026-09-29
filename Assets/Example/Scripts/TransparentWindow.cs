// ***********************************************************************************
// FileName: TransparentWindow.cs
// Description:
// 
// Version: v1.0.0
// Creator: Jacky(jackylvm@foxmail.com)
// CreationTime: 2026-09-29 22:26:54
// ==============================================
// History update record:
// 
// ==============================================
// *************************************************************************************

using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

// ReSharper disable CheckNamespace
namespace Example
{
    public class TransparentWindow : MonoBehaviour
    {
        [DllImport("User32.dll")]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);

        [DllImport("User32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("User32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int index, int value);

        [DllImport("User32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("User32.dll")]
        private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        // --- COM 接口：用于直接操作任务栏 (重点在这里) ---
        [ComImport]
        [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
        public class TaskbarList
        {
        }

        [ComImport]
        [Guid("56FDF342-FD6D-11d0-958A-006097C9A090")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITaskbarList
        {
            void HrInit();
            void AddTab(IntPtr hwnd);
            void DeleteTab(IntPtr hwnd);
            void ActivateTab(IntPtr hwnd);
            void SetActiveAlt(IntPtr hwnd);
        }

        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("Dwmapi.dll")]
        private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        const int GWL_EXSTYLE = -20;

        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_LAYERED = 0x00080000;

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        const uint LWA_COLORKEY = 0x00000001;

        // Start is called before the first frame update
        private void Start()
        {
            if (!Application.isEditor)
            {
                Application.runInBackground = true;
                // 依然需要等待 Logo 播放完毕
                StartCoroutine(WaitSplashScreenAndApply());
            }
        }

        IEnumerator WaitSplashScreenAndApply()
        {
            // 1. 等待 Unity 启动动画结束
            while (!SplashScreen.isFinished)
            {
                yield return null;
            }

            // 2. 额外缓冲 0.5 秒，确保渲染管线稳定
            yield return new WaitForSeconds(0.5f);

            // 3. 执行透明化和隐藏任务栏图标
            ApplyTransparentAndHideTaskbar();
        }

        void ApplyTransparentAndHideTaskbar()
        {
            var hWnd = GetActiveWindow();
            var margins = new MARGINS { cxLeftWidth = -1 };
            DwmExtendFrameIntoClientArea(hWnd, ref margins);

            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
            SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);

            try
            {
                ITaskbarList taskbar = (ITaskbarList)new TaskbarList();
                taskbar.HrInit();
                taskbar.DeleteTab(hWnd); // 任务栏图标消失术
            }
            catch (Exception e)
            {
                Debug.LogError("隐藏任务栏图标失败: " + e.Message);
            }
        }
    }
}