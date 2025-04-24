using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace WindowsInputHelper
{
    public partial class MainForm : Form
    {
        private NotifyIcon _notifyIcon = null!;
        private KeyboardHook _keyboardHook = null!;
        private bool _isF8Enabled = true;

        public MainForm()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            InitializeKeyboardHook();
        }

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = SystemIcons.Application;
            UpdateNotifyIconText();
            _notifyIcon.Visible = true;

            var contextMenu = new ContextMenuStrip();
            
            var toggleF8MenuItem = new ToolStripMenuItem("禁用F8功能");
            toggleF8MenuItem.CheckOnClick = true;
            toggleF8MenuItem.Checked = !_isF8Enabled;
            toggleF8MenuItem.Click += (s, e) =>
            {
                _isF8Enabled = !toggleF8MenuItem.Checked;
                UpdateNotifyIconText();
            };
            contextMenu.Items.Add(toggleF8MenuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Click += (s, e) => Application.Exit();
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void InitializeKeyboardHook()
        {
            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyPressed += (s, e) =>
            {
                if (e == Keys.F11)
                {
                    Application.Exit();
                }
                else if (e == Keys.F10)
                {
                    _isF8Enabled = !_isF8Enabled;
                    UpdateNotifyIconText();
                    if (_notifyIcon.ContextMenuStrip?.Items[0] is ToolStripMenuItem toggleF8MenuItem)
                    {
                        toggleF8MenuItem.Checked = !_isF8Enabled;
                    }
                }
                else if (e == Keys.F8 && _isF8Enabled)
                {
                    string clipboardText = Clipboard.GetText();
                    if (!string.IsNullOrEmpty(clipboardText))
                    {
                        //Thread.Sleep(500); // 给用户一点时间准备
                        SendKeys.SendWait(clipboardText);
                    }
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Hide();
            ShowInTaskbar = false;
        }

        private void UpdateNotifyIconText()
        {
            _notifyIcon.Text = $"Windows Input Helper - F8功能已{(_isF8Enabled ? "启用" : "禁用")}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                _keyboardHook?.Dispose();
                _notifyIcon?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}