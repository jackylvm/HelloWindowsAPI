// ***********************************************************************************
// FileName: SystemTrayManager.cs
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Rendering;
using Application = UnityEngine.Application;
using ContextMenu = System.Windows.Forms.ContextMenu;
using Debug = UnityEngine.Debug;

// ReSharper disable CheckNamespace
namespace Example
{
    public class SystemTrayManager : MonoBehaviour
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        /// <summary>
        /// Start 函数会在脚本实例被启用时调用，并且只在第一帧更新之前执行一次。它一般用于：
        /// <para>1. 依赖初始化：当某些初始化操作依赖于其他对象的 Awake 函数完成时，可以在 Start 函数里进行。</para>
        /// <para>2. 数据加载：从文件或者服务器加载数据。</para>
        /// </summary>
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
            AddSystemTrayIcon();
        }

        private void AddSystemTrayIcon()
        {
            // 1. 初始化右键菜单
            trayMenu = new ContextMenu();

            // 添加菜单项和点击事件
            trayMenu.MenuItems.Add("显示游戏", OnShowClicked);
            trayMenu.MenuItems.Add("退出游戏", OnExitClicked);

            // 2. 初始化托盘图标
            trayIcon = new NotifyIcon();
            trayIcon.Text = "系统托盘示例"; // 鼠标悬停时的提示文字

            // 加载 .ico 图标文件
            var iconPath = Path.Combine(Application.streamingAssetsPath, "tray_icon.ico");
            if (File.Exists(iconPath))
            {
                trayIcon.Icon = new Icon(iconPath);
            }
            else
            {
                Debug.LogError("找不到托盘图标文件！请检查 StreamingAssets 目录。");
            }

            // 绑定右键菜单
            trayIcon.ContextMenu = trayMenu;

            // 绑定双击事件
            trayIcon.DoubleClick += OnShowClicked;

            // 显示托盘图标
            trayIcon.Visible = true;
        }

        // 点击“显示游戏”或双击托盘时的逻辑
        private void OnShowClicked(object sender, EventArgs e)
        {
            // 如果你的窗口被隐藏了，这里需要调用 user32.dll 的 ShowWindow 把游戏窗口呼唤出来
            Debug.Log("显示游戏");
        }

        // 点击“退出”时的逻辑
        private void OnExitClicked(object sender, EventArgs e)
        {
            Application.Quit();
        }

        // 必须在程序退出时销毁托盘图标，否则它会一直残留在任务栏直到鼠标划过
        void OnApplicationQuit()
        {
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }
        }
    }
}