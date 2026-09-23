using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

[assembly: AssemblyTitle("Internet Explorer 6.1")]
[assembly: AssemblyDescription("Internet Explorer Web Browser")]
[assembly: AssemblyCompany("Internet Surfers")]
[assembly: AssemblyProduct("Internet Explorer 6.1")]
[assembly: AssemblyCopyright("Copyright © Internet Surfers")]
[assembly: AssemblyFileVersion("6.1.0.0")]
[assembly: AssemblyVersion("6.1.0.0")]

namespace SingleFileTridentBrowser
{
    static class Program
    {
        [STAThread]
        static void Main(string[] _0x1a)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string _0x2b = null;
            if (_0x1a != null && _0x1a.Length > 0)
            {
                _0x2b = _0x1a[0];
            }

            Application.Run(new BrowserForm(_0x2b));
        }
    }

    public class BrowserForm : Form
    {
        private WebBrowser _0x3c;
        private TextBox _0x4d;
        private Button _0x5e;
        private Button _0x6f;
        private Button _0x70;
        private Button _0x81;
        private Button _0x92;
        private Button _0xa3;
        
        private MenuStrip _0xb4;
        private ToolStripMenuItem _0xc5;
        private ToolStripMenuItem _0xd6;
        
        private ToolStripMenuItem _0xe7;
        private ToolStripMenuItem _0xf8;
        private ToolStripMenuItem _0x09;

        private ToolStripMenuItem _0x1a2;
        private ToolStripMenuItem _0x2b3;
        private ToolStripMenuItem _0x3c4;
        private ToolStripMenuItem _0x4d5;
        private ToolStripMenuItem _0x5e6;

        private StatusStrip _0x6f7;
        private ToolStripStatusLabel _0x708;
        private ToolStripProgressBar _0x819;

        private List<string> _0x92a = new List<string>();
        private readonly string _0xa3b = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\x68\x69\x73\x74\x6f\x72\x79\x2e\x74\x78\x74");
        private readonly string _0xb4c = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\x68\x6f\x6d\x65\x70\x61\x67\x65\x2e\x74\x78\x74");
        private readonly string _0xc5d = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\x64\x65\x66\x61\x75\x6c\x74\x5f\x63\x68\x65\x63\x6b\x2e\x74\x78\x74");
        private readonly string _0xd6e = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\x76\x65\x72\x73\x69\x6f\x6e\x2e\x74\x78\x74");
        private readonly string _0xe7f = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\x73\x6f\x75\x72\x63\x65");
        private string _0xf80 = "\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d";

        public BrowserForm(string _0x09a = null)
        {
            this.Text = "\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72\x20\x36\x2e\x31";
            this.Width = 1000;
            this.Height = 700;

            try
            {
                string _0x1a1 = Path.Combine(_0xe7f, "\x69\x65\x69\x63\x6f\x6e\x2e\x69\x63\x6f");
                if (!File.Exists(_0x1a1)) _0x1a1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\x69\x65\x69\x63\x6f\x6e\x2e\x69\x63\x6f");

                if (File.Exists(_0x1a1))
                {
                    this.Icon = new Icon(_0x1a1);
                }
            }
            catch { }

            LoadSettings();
            LoadHistoryFromFile();
            CheckForUpdates(false);

            this.Shown += (_0x2b2, _0x3c3) => {
                CheckAndPromptDefaultBrowser();
            };

            _0xb4 = new MenuStrip();
            
            _0xc5 = new ToolStripMenuItem("\x42\x72\x6f\x77\x73\x65\x72");
            _0xd6 = new ToolStripMenuItem("\x48\x69\x73\x74\x6f\x72\x79");
            _0xd6.DropDownOpening += (_0x4d4, _0x5e5) => UpdateHistoryMenu();
            _0xc5.DropDownItems.Add(_0xd6);

            _0xe7 = new ToolStripMenuItem("\x57\x65\x62\x20\x41\x70\x70\x73");
            _0xf8 = new ToolStripMenuItem("\x54\x75\x72\x74\x6c\x65");
            _0x09 = new ToolStripMenuItem("\x48\x54\x4d\x4c\x20\x47\x61\x6d\x65\x20\x4d\x61\x6b\x65\x72");

            _0xf8.Click += (_0x6f6, _0x707) => NavigateToWebApp("\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f", "\x74\x75\x72\x74\x6c\x65\x2e\x68\x74\x6d\x6c");
            _0x09.Click += (_0x818, _0x929) => NavigateToWebApp("\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f", "\x6d\x61\x6b\x65\x72\x2e\x68\x74\x6d\x6c");

            _0xe7.DropDownItems.Add(_0xf8);
            _0xe7.DropDownItems.Add(_0x09);

            _0x1a2 = new ToolStripMenuItem("\x4f\x70\x74\x69\x6f\x6e\x73");
            _0x2b3 = new ToolStripMenuItem("\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x4f\x70\x74\x69\x6f\x6e\x73\x2e\x2e\x2e");
            _0x3c4 = new ToolStripMenuItem("\x53\x65\x74\x20\x43\x75\x72\x72\x65\x6e\x74\x20\x50\x61\x67\x65\x20\x61\x73\x20\x48\x6f\x6d\x65\x70\x61\x67\x65");
            _0x4d5 = new ToolStripMenuItem("\x53\x65\x74\x20\x44\x65\x66\x61\x75\x6c\x74\x20\x42\x72\x6f\x77\x73\x65\x72");
            _0x5e6 = new ToolStripMenuItem("\x43\x68\x65\x63\x6b\x20\x46\x6f\x72\x20\x55\x70\x64\x61\x74\x65\x73");

            _0x2b3.Click += (_0xa3a, _0xb4b) => {
                try
                {
                    Process.Start(new ProcessStartInfo("\x69\x6e\x65\x74\x63\x70\x6c\x2e\x63\x70\x6c") { UseShellExecute = true });
                }
                catch
                {
                    MessageBox.Show("\x55\x6e\x61\x62\x6c\x65\x20\x74\x6f\x20\x6f\x70\x65\x6e\x20\x57\x69\x6e\x64\x6f\x77\x73\x20\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x4f\x70\x74\x69\x6f\x6e\x73\x2e", "\x45\x72\x72\x6f\x72", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            _0x3c4.Click += (_0xc5c, _0xd6d) => {
                if (_0x3c.Url != null)
                {
                    string _0xe7e = _0x4d.Text;
                    if (_0xe7e.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d", StringComparison.OrdinalIgnoreCase))
                        _0xf80 = "\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d";
                    else if (_0xe7e.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f", StringComparison.OrdinalIgnoreCase))
                        _0xf80 = "\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f";
                    else if (_0xe7e.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f", StringComparison.OrdinalIgnoreCase))
                        _0xf80 = "\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f";
                    else
                        _0xf80 = _0x3c.Url.ToString();

                    SaveSettings();
                    MessageBox.Show("\x48\x6f\x6d\x65\x70\x61\x67\x65\x20\x75\x70\x64\x61\x74\x65\x64\x20\x74\x6f\x3a\n" + _0xf80, "\x48\x6f\x6d\x65\x70\x61\x67\x65\x20\x53\x61\x76\x65\x64", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            _0x4d5.Click += (_0xf8f, _0x090) => {
                SetAsDefaultBrowser();
                MessageBox.Show("\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72\x20\x68\x61\x73\x20\x62\x65\x65\x6e\x20\x73\x65\x74\x20\x61\x73\x20\x79\x6f\x75\x72\x20\x64\x65\x66\x61\x75\x6c\x74\x20\x62\x72\x6f\x77\x73\x65\x72\x20\x66\x6f\x72\x20\x61\x6c\x6c\x20\x73\x75\x70\x70\x6f\x72\x74\x65\x64\x20\x77\x65\x62\x20\x65\x78\x74\x65\x6e\x73\x69\x6f\x6e\x73\x2e", "\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            _0x5e6.Click += (_0x1a2a, _0x2b3b) => {
                CheckForUpdates(true);
            };

            _0x1a2.DropDownItems.Add(_0x2b3);
            _0x1a2.DropDownItems.Add(_0x3c4);
            _0x1a2.DropDownItems.Add(_0x4d5);
            _0x1a2.DropDownItems.Add(new ToolStripSeparator());
            _0x1a2.DropDownItems.Add(_0x5e6);

            _0xb4.Items.Add(_0xc5);
            _0xb4.Items.Add(_0xe7);
            _0xb4.Items.Add(_0x1a2);
            this.MainMenuStrip = _0xb4;
            this.Controls.Add(_0xb4);

            _0x6f7 = new StatusStrip();
            _0x708 = new ToolStripStatusLabel("\x52\x65\x61\x64\x79");
            _0x708.Spring = true;
            _0x708.TextAlign = ContentAlignment.MiddleLeft;

            _0x819 = new ToolStripProgressBar();
            _0x819.Size = new Size(120, 16);
            _0x819.Style = ProgressBarStyle.Marquee;
            _0x819.Visible = false;

            _0x6f7.Items.Add(_0x708);
            _0x6f7.Items.Add(_0x819);
            this.Controls.Add(_0x6f7);

            Panel _0x3c4c = new Panel();
            _0x3c4c.Dock = DockStyle.Top;
            _0x3c4c.Height = 45;
            _0x3c4c.Padding = new Padding(5);

            _0x5e = new Button();
            _0x5e.Text = "\u25c0";
            _0x5e.Width = 35;
            _0x5e.Location = new Point(8, 8);
            _0x5e.Click += (_0x3c3c, _0x4d4d) => { if (_0x3c.CanGoBack) _0x3c.GoBack(); };

            _0x6f = new Button();
            _0x6f.Text = "\u25b6";
            _0x6f.Width = 35;
            _0x6f.Location = new Point(47, 8);
            _0x6f.Click += (_0x5e5e, _0x6f6f) => { if (_0x3c.CanGoForward) _0x3c.GoForward(); };

            _0x70 = new Button();
            _0x70.Text = "\u21bb";
            _0x70.Width = 35;
            _0x70.Location = new Point(86, 8);
            _0x70.Click += (_0x7070, _0x8181) => _0x3c.Refresh();

            _0x81 = new Button();
            _0x81.Text = "\u2715";
            _0x81.Width = 35;
            _0x81.Location = new Point(125, 8);
            _0x81.Click += (_0x9292, _0xa3a3) => _0x3c.Stop();

            _0x92 = new Button();
            _0x92.Text = "\u2302";
            _0x92.Width = 35;
            _0x92.Location = new Point(164, 8);
            _0x92.Click += (_0xb4b4, _0xc5c5) => NavigateToUrlString(_0xf80);

            _0xa3 = new Button();
            _0xa3.Text = "\x47\x6f";
            _0xa3.Width = 55;
            _0xa3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _0xa3.Location = new Point(this.ClientSize.Width - 63, 8);
            _0xa3.Click += (_0xd6d6, _0xe7e7) => NavigateToUrl();

            _0x4d = new TextBox();
            _0x4d.Location = new Point(204, 10);
            _0x4d.Width = this.ClientSize.Width - 274;
            _0x4d.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _0x4d.Font = new Font("\x53\x65\x67\x6f\x65\x20\x55\x49", 10F);
            _0x4d.KeyDown += (_0xf8f8, _0x0909) => {
                if (_0x0909.KeyCode == Keys.Enter)
                {
                    NavigateToUrl();
                    _0x0909.SuppressKeyPress = true;
                }
            };

            _0x3c4c.Controls.Add(_0x5e);
            _0x3c4c.Controls.Add(_0x6f);
            _0x3c4c.Controls.Add(_0x70);
            _0x3c4c.Controls.Add(_0x81);
            _0x3c4c.Controls.Add(_0x92);
            _0x3c4c.Controls.Add(_0x4d);
            _0x3c4c.Controls.Add(_0xa3);

            _0x3c = new WebBrowser();
            _0x3c.Dock = DockStyle.Fill;
            _0x3c.ScriptErrorsSuppressed = true;
            
            _0x3c.Navigating += (_0x1a1a, _0x2b2b) => {
                if (_0x2b2b.Url != null)
                {
                    _0x708.Text = "\x4c\x6f\x61\x64\x69\x6e\x67\x3a\x20" + _0x2b2b.Url.ToString();
                    _0x819.Visible = true;
                }
            };
            
            _0x3c.Navigated += (_0x3c3c, _0x4d4d) => {
                if (_0x3c.Url != null)
                {
                    string _0x5e5e = _0x3c.Url.ToString();
                    
                    if (_0x5e5e.EndsWith("\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x68\x74\x6d\x6c", StringComparison.OrdinalIgnoreCase))
                    {
                        _0x4d.Text = "\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d";
                        AddUrlToHistory("\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d");
                    }
                    else if (_0x5e5e.EndsWith("\x74\x75\x72\x74\x6c\x65\x2e\x68\x74\x6d\x6c", StringComparison.OrdinalIgnoreCase))
                    {
                        _0x4d.Text = "\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f";
                        AddUrlToHistory("\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f");
                    }
                    else if (_0x5e5e.EndsWith("\x6d\x61\x6b\x65\x72\x2e\x68\x74\x6d\x6c", StringComparison.OrdinalIgnoreCase))
                    {
                        _0x4d.Text = "\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f";
                        AddUrlToHistory("\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f");
                    }
                    else
                    {
                        _0x4d.Text = _0x5e5e;
                        AddUrlToHistory(_0x5e5e);
                    }
                }
            };

            _0x3c.DocumentCompleted += (_0x6f6f, _0x7070) => {
                _0x708.Text = "\x52\x65\x61\x64\x79";
                _0x819.Visible = false;

                if (_0x3c.Document != null && !string.IsNullOrEmpty(_0x3c.Document.Title))
                {
                    this.Text = _0x3c.Document.Title + "\x20\x2d\x20\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72\x20\x36\x2e\x31";
                }
                else
                {
                    this.Text = "\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72\x20\x36\x2e\x31";
                }
            };

            this.Controls.Add(_0x3c);
            this.Controls.Add(_0x6f7);
            this.Controls.Add(_0x3c4c);
            this.Controls.Add(_0xb4);

            if (!string.IsNullOrEmpty(_0x09a))
            {
                NavigateToUrlString(_0x09a);
            }
            else
            {
                NavigateToUrlString(_0xf80);
            }
        }

        private string GetCurrentVersion()
        {
            try
            {
                if (File.Exists(_0xd6e))
                {
                    string _0x8181 = File.ReadAllText(_0xd6e).Trim();
                    if (!string.IsNullOrEmpty(_0x8181)) return _0x8181;
                }
                else
                {
                    File.WriteAllText(_0xd6e, "\x76\x31\x2e\x30");
                }
            }
            catch { }
            return "\x76\x31\x2e\x30";
        }

        private async void CheckForUpdates(bool _0x9292)
        {
            try
            {
                if (_0x9292)
                {
                    _0x708.Text = "\x43\x68\x65\x63\x6b\x69\x6e\x67\x20\x66\x6f\x72\x20\x75\x70\x64\x61\x74\x65\x73\x2e\x2e\x2e";
                    _0x819.Visible = true;
                }

                string _0xa3a3 = "\x4b\x72\x79\x73\x74\x69\x61\x6e\x53\x38";
                string _0xb4b4 = "\x49\x6e\x74\x65\x72\x6e\x65\x74\x2d\x45\x78\x70\x6c\x6f\x72\x65\x72\x2d\x36\x2e\x31";
                string _0xc5c5 = string.Format("\x68\x74\x74\x70\x73\x3a\x2f\x2f\x61\x70\x69\x2e\x67\x69\x74\x68\x75\x62\x2e\x63\x6f\x6d\x2f\x72\x65\x70\x6f\x73\x2f\x7b\x30\x7d\x2f\x7b\x31\x7d\x2f\x72\x65\x6c\x65\x61\x73\x65\x73\x2f\x6c\x61\x74\x65\x73\x74", _0xa3a3, _0xb4b4);

                using (HttpClient _0xd6d6 = new HttpClient())
                {
                    _0xd6d6.DefaultRequestHeaders.Add("\x55\x73\x65\x72\x2d\x41\x67\x65\x6e\x74", "\x49\x6e\x74\x65\x72\x6e\x65\x74\x2d\x45\x78\x70\x6c\x6f\x72\x65\x72\x2d\x36\x2e\x31");
                    string _0xe7e7 = await _0xd6d6.GetStringAsync(_0xc5c5);

                    Match _0xf8f8 = Regex.Match(_0xe7e7, "\x22\x74\x61\x67\x5f\x6e\x61\x6d\x65\x22\x5c\x73\x2a\x3a\x5c\x73\x2a\x22\x28\x5b\x5e\x22\x5d\x2b\x29\x22");
                    Match _0x0909 = Regex.Match(_0xe7e7, "\x22\x7a\x69\x70\x62\x61\x6c\x6c\x5f\x75\x72\x6c\x22\x5c\x73\x2a\x3a\x5c\x73\x2a\x22\x28\x5b\x5e\x22\x5d\x2b\x29\x22");

                    if (_0xf8f8.Success)
                    {
                        string _0x1a1a = _0xf8f8.Groups[1].Value.Trim();
                        string _0x2b2b = GetCurrentVersion().Trim();

                        string _0x3c3c = _0x1a1a.TrimStart('\x76', '\x56');
                        string _0x4d4d = _0x2b2b.TrimStart('\x76', '\x56');

                        if (!string.Equals(_0x3c3c, _0x4d4d, StringComparison.OrdinalIgnoreCase))
                        {
                            DialogResult _0x5e5e = MessageBox.Show(
                                string.Format("\x41\x20\x6e\x65\x77\x20\x73\x6f\x75\x72\x63\x65\x20\x75\x70\x64\x61\x74\x65\x20\x28\x7b\x30\x7d\x29\x20\x69\x73\x20\x61\x76\x61\x69\x6c\x61\x62\x6c\x65\x21\x20\x57\x6f\x75\x6c\x64\x20\x79\x6f\x75\x20\x6c\x69\x6b\x65\x20\x74\x6f\x20\x64\x6f\x77\x6e\x6c\x6f\x61\x64\x20\x61\x6e\x64\x20\x75\x70\x64\x61\x74\x65\x20\x74\x68\x65\x20\x73\x6f\x75\x72\x63\x65\x20\x66\x69\x6c\x65\x73\x20\x6e\x6f\x77\x3f\n\n\x2d\x20\x43\x6c\x69\x63\x6b\x20\x59\x65\x73\x20\x74\x6f\x20\x64\x6f\x77\x6e\x6c\x6f\x61\x64\x20\x61\x6e\x64\x20\x61\x70\x70\x6c\x79\x20\x74\x68\x65\x20\x75\x70\x64\x61\x74\x65\x20\x61\x75\x74\x6f\x6d\x61\x74\x69\x63\x61\x6c\x6c\x79\x2e\n\n\x2d\x20\x43\x6c\x69\x63\x6b\x20\x4e\x6f\x20\x74\x6f\x20\x73\x6b\x69\x70\x2e", _0x1a1a),
                                "\x55\x70\x64\x61\x74\x65\x20\x41\x76\x61\x69\x6c\x61\x62\x6c\x65",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                            if (_0x5e5e == DialogResult.Yes && _0x0909.Success)
                            {
                                string _0x6f6f = _0x0909.Groups[1].Value;

                                _0x708.Text = "\x44\x6f\x77\x6e\x6c\x6f\x61\x64\x69\x6e\x67\x20\x72\x65\x70\x6f\x73\x69\x74\x6f\x72\x79\x20\x73\x6f\x75\x72\x63\x65\x2e\x2e\x2e";
                                _0x819.Visible = true;

                                byte[] _0x7070 = await _0xd6d6.GetByteArrayAsync(_0x6f6f);
                                string _0x8181 = Path.Combine(Path.GetTempPath(), "\x69\x65\x5f\x75\x70\x64\x61\x74\x65\x2e\x7a\x69\x70");
                                File.WriteAllBytes(_0x8181, _0x7070);

                                try { File.WriteAllText(_0xd6e, _0x1a1a); } catch { }

                                string _0x9292a = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
                                string _0xa3a3a = Path.Combine(Path.GetTempPath(), "\x69\x65\x5f\x75\x70\x64\x61\x74\x65\x2e\x70\x73\x31");

                                string _0xb4b4b = string.Format(
                                    "\x53\x74\x61\x72\x74\x2d\x53\x6c\x65\x65\x70\x20\x2d\x53\x65\x63\x6f\x6e\x64\x73\x20\x32\n" +
                                    "\x45\x78\x70\x61\x6e\x64\x2d\x41\x72\x63\x68\x69\x76\x65\x20\x2d\x50\x61\x74\x68\x20\x27\x7b\x30\x7d\x27\x20\x2d\x44\x65\x73\x74\x69\x6e\x61\x74\x69\x6f\x6e\x50\x61\x74\x68\x20\x27\x7b\x31\x7d\x69\x65\x5f\x65\x78\x74\x72\x61\x63\x74\x27\x20\x2d\x46\x6f\x72\x63\x65\n" +
                                    "\x24\x69\x6e\x6e\x65\x72\x46\x6f\x6c\x64\x65\x72\x20\x3d\x20\x47\x65\x74\x2d\x43\x68\x69\x6c\x64\x49\x74\x65\x6d\x20\x27\x7b\x31\x7d\x69\x65\x5f\x65\x78\x74\x72\x61\x63\x74\x27\x20\x7c\x20\x53\x65\x6c\x65\x63\x74\x2d\x4f\x62\x6a\x65\x63\x74\x20\x2d\x46\x69\x72\x73\x74\x20\x31\n" +
                                    "\x43\x6f\x70\x79\x2d\x49\x74\x65\x6d\x20\x2d\x50\x61\x74\x68\x20\x22\x24\x28\x24\x69\x6e\x6e\x65\x72\x46\x6f\x6c\x64\x65\x72\x2e\x46\x75\x6c\x6c\x4e\x61\x6d\x65\x29\x5c\x2a\x22\x20\x2d\x44\x65\x73\x74\x69\x6e\x61\x74\x69\x6f\x6e\x20\x27\x7b\x32\x7d\x27\x20\x2d\x52\x65\x63\x75\x72\x73\x65\x20\x2d\x46\x6f\x72\x63\x65\n" +
                                    "\x52\x65\x6d\x6f\x76\x65\x2d\x49\x74\x65\x6d\x20\x27\x7b\x30\x7d\x27\x20\x2d\x46\x6f\x72\x63\x65\n" +
                                    "\x52\x65\x6d\x6f\x76\x65\x2d\x49\x74\x65\x6d\x20\x27\x7b\x31\x7d\x69\x65\x5f\x65\x78\x74\x72\x61\x63\x74\x27\x20\x2d\x52\x65\x63\x75\x72\x73\x65\x20\x2d\x46\x6f\x72\x63\x65\n" +
                                    "\x53\x74\x61\x72\x74\x2d\x50\x72\x6f\x63\x65\x73\x73\x20\x27\x7b\x33\x7d\x27\n" +
                                    "\x52\x65\x6d\x6f\x76\x65\x2d\x49\x74\x65\x6d\x20\x24\x4d\x79\x49\x76\x6f\x63\x61\x74\x69\x6f\x6e\x2e\x4d\x79\x43\x6f\x6d\x6d\x61\x6e\x64\x2e\x50\x61\x74\x68\x20\x2d\x46\x6f\x72\x63\x65",
                                    _0x8181,
                                    Path.GetTempPath(),
                                    _0x9292a,
                                    Path.Combine(_0x9292a, "\x69\x65\x78\x70\x6c\x6f\x72\x65\x36\x31\x2e\x65\x78\x65"));

                                File.WriteAllText(_0xa3a3a, _0xb4b4b);

                                MessageBox.Show("\x53\x6f\x75\x72\x63\x65\x20\x64\x6f\x77\x6e\x6c\x6f\x61\x64\x65\x64\x20\x73\x75\x63\x63\x65\x73\x73\x66\x75\x6c\x6c\x79\x21\x20\x54\x68\x65\x20\x62\x72\x6f\x77\x73\x65\x72\x20\x77\x69\x6c\x6c\x20\x6e\x6f\x77\x20\x63\x6c\x6f\x73\x65\x2c\x20\x75\x70\x64\x61\x74\x65\x20\x69\x74\x73\x20\x66\x69\x6c\x65\x73\x2c\x20\x61\x6e\x64\x20\x72\x65\x73\x74\x61\x72\x74\x2e", "\x55\x70\x64\x61\x74\x69\x6e\x67", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = "\x70\x6f\x77\x65\x72\x73\x68\x65\x6c\x6c\x2e\x65\x78\x65",
                                    Arguments = string.Format("\x2d\x45\x78\x65\x63\x75\x74\x69\x6f\x6e\x50\x6f\x6c\x69\x63\x79\x20\x42\x79\x70\x61\x73\x73\x20\x2d\x46\x69\x6c\x65\x20\x22\x7b\x30\x7d\x22", _0xa3a3a),
                                    CreateNoWindow = true,
                                    UseShellExecute = false
                                });

                                Application.Exit();
                            }
                        }
                        else if (_0x9292)
                        {
                            MessageBox.Show(string.Format("\x59\x6f\x75\x20\x61\x72\x65\x20\x63\x75\x72\x72\x65\x6e\x74\x6c\x79\x20\x72\x75\x6e\x6e\x69\x6e\x67\x20\x74\x68\x65\x20\x6c\x61\x74\x65\x73\x74\x20\x76\x65\x72\x73\x69\x6f\x6e\x20\x28\x7b\x30\x7d\x29\x2e", _0x2b2b), "\x4e\x6f\x20\x55\x70\x64\x61\x74\x65\x73\x20\x46\x6f\x75\x6e\x64", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch
            {
                if (_0x9292)
                {
                    MessageBox.Show("\x55\x6e\x61\x62\x6c\x65\x20\x74\x6f\x20\x63\x68\x65\x63\x6b\x20\x66\x6f\x72\x20\x75\x70\x64\x61\x74\x65\x73\x2e\x20\x50\x6c\x65\x61\x73\x65\x20\x63\x68\x65\x63\x6b\x20\x79\x6f\x75\x72\x20\x69\x6e\x74\x65\x72\x6e\x65\x74\x20\x63\x6f\x6e\x6e\x65\x63\x74\x69\x6f\x6e\x2e", "\x55\x70\x64\x61\x74\x65\x20\x45\x72\x72\x6f\x72", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                _0x708.Text = "\x52\x65\x61\x64\x79";
                _0x819.Visible = false;
            }
        }

        private void CheckAndPromptDefaultBrowser()
        {
            try
            {
                if (File.Exists(_0xc5d))
                {
                    string _0xc5c = File.ReadAllText(_0xc5d).Trim();
                    if (_0xc5c.Equals("\x73\x6b\x69\x70", StringComparison.OrdinalIgnoreCase)) return;
                }

                bool _0xd6d = true;
                string _0xe7e = Path.GetFileName(Application.ExecutablePath);

                using (var _0xf8f = Registry.CurrentUser.OpenSubKey(@"Software\Classes\http\shell\open\command", false))
                {
                    string _0x090 = (_0xf8f != null) ? _0xf8f.GetValue(null) as string : null;
                    if (string.IsNullOrEmpty(_0x090) || _0x090.IndexOf(_0xe7e, StringComparison.OrdinalIgnoreCase) < 0) _0xd6d = false;
                }

                using (var _0x1a1 = Registry.CurrentUser.OpenSubKey(@"Software\Classes\.html\shell\open\command", false))
                {
                    string _0x2b2 = (_0x1a1 != null) ? _0x1a1.GetValue(null) as string : null;
                    if (string.IsNullOrEmpty(_0x2b2) || _0x2b2.IndexOf(_0xe7e, StringComparison.OrdinalIgnoreCase) < 0) _0xd6d = false;
                }

                if (!_0xd6d)
                {
                    DialogResult _0x3c3 = MessageBox.Show(
                        "\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72\x20\x69\x73\x20\x6e\x6f\x74\x20\x63\x75\x72\x72\x65\x6e\x74\x6c\x79\x20\x79\x6f\x75\x72\x20\x64\x65\x66\x61\x75\x6c\x74\x20\x62\x72\x6f\x77\x73\x65\x72\x2e\n\n\x57\x6f\x75\x6c\x64\x20\x79\x6f\x75\x20\x6c\x69\x6b\x65\x20\x74\x6f\x20\x6d\x61\x6b\x65\x20\x69\x74\x20\x79\x6f\x75\x72\x20\x64\x65\x66\x61\x75\x6c\x74\x20\x62\x72\x6f\x77\x73\x65\x72\x20\x66\x6f\x72\x20\x61\x6c\x6c\x20\x48\x54\x4d\x4c\x2c\x20\x58\x4d\x4c\x2c\x20\x61\x6e\x64\x20\x77\x65\x62\x20\x70\x72\x6f\x74\x6f\x63\x6f\x6c\x73\x3f",
                        "\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (_0x3c3 == DialogResult.Yes)
                    {
                        SetAsDefaultBrowser();
                        MessageBox.Show("\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72\x20\x68\x61\x73\x20\x62\x65\x65\x6e\x20\x73\x65\x74\x20\x61\x73\x20\x79\x6f\x75\x72\x20\x64\x65\x66\x61\x75\x6c\x74\x20\x62\x72\x6f\x77\x73\x65\x72\x20\x66\x6f\x72\x20\x61\x6c\x6c\x20\x73\x75\x70\x70\x6f\x72\x74\x65\x64\x20\x77\x65\x62\x20\x65\x78\x74\x65\x6e\x73\x69\x6f\x6e\x73\x2e", "\x49\x6e\x74\x65\x72\x6e\x65\x74\x20\x45\x78\x70\x6c\x6f\x72\x65\x72", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (_0x3c3 == DialogResult.Cancel)
                    {
                        File.WriteAllText(_0xc5d, "\x73\x6b\x69\x70");
                    }
                }
            }
            catch { }
        }

        private void SetAsDefaultBrowser()
        {
            try
            {
                string _0x4d4 = Application.ExecutablePath;
                string _0x5e5 = "\x22" + _0x4d4 + "\x22\x20\x22\x25\x31\x22";

                using (var _0x6f6 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\http\shell\open\command")) _0x6f6.SetValue("", _0x5e5);
                using (var _0x7f7 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\https\shell\open\command")) _0x7f7.SetValue("", _0x5e5);

                string[] _0x8f8 = { "\x2e\x68\x74\x6d\x6c", "\x2e\x68\x74\x6d", "\x2e\x73\x68\x74\x6d\x6c", "\x2e\x78\x68\x74\x6d\x6c", "\x2e\x78\x6d\x6c" };
                foreach (string _0x9f9 in _0x8f8)
                {
                    using (var _0x0a0 = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + _0x9f9 + "\\shell\\open\\command")) _0x0a0.SetValue("", _0x5e5);
                }
            }
            catch (Exception _0x1b1)
            {
                MessageBox.Show("\x43\x6f\x75\x6c\x64\x20\x6e\x6f\x74\x20\x61\x75\x74\x6f\x6d\x61\x74\x69\x63\x61\x6c\x6c\x79\x20\x73\x65\x74\x20\x64\x65\x66\x61\x75\x6c\x74\x20\x62\x72\x6f\x77\x73\x65\x72\x20\x65\x78\x74\x65\x6e\x73\x69\x6f\x6e\x73\x3a\n" + _0x1b1.Message, "\x45\x72\x72\x6f\x72", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_0xb4c))
                {
                    string _0x2c2 = File.ReadAllText(_0xb4c).Trim();
                    if (!string.IsNullOrEmpty(_0x2c2)) _0xf80 = _0x2c2;
                }
            }
            catch { }
        }

        private void SaveSettings()
        {
            try { File.WriteAllText(_0xb4c, _0xf80); } catch { }
        }

        private void NavigateToWebApp(string _0x3d3, string _0x4e4)
        {
            string _0x5f5 = Path.Combine(_0xe7f, _0x4e4);
            if (!File.Exists(_0x5f5)) _0x5f5 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _0x4e4);

            if (File.Exists(_0x5f5))
            {
                _0x3c.Navigate(new Uri(_0x5f5).AbsoluteUri);
                _0x4d.Text = _0x3d3;
                AddUrlToHistory(_0x3d3);
            }
            else
            {
                MessageBox.Show("\x43\x6f\x75\x6c\x64\x20\x6e\x6f\x74\x20\x66\x69\x6e\x64\x20\x6f\x66\x66\x6c\x69\x6e\x65\x20\x66\x69\x6c\x65\x3a\x20" + _0x4e4, "\x46\x69\x6c\x65\x20\x4e\x6f\x74\x20\x46\x6f\x75\x6e\x64", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void NavigateToUrlString(string _0x6a6)
        {
            if (_0x6a6.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToWebApp("\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d", "\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x68\x74\x6d\x6c");
            }
            else if (_0x6a6.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToWebApp("\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f", "\x74\x75\x72\x74\x6c\x65\x2e\x68\x74\x6d\x6c");
            }
            else if (_0x6a6.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToWebApp("\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f", "\x6d\x61\x6b\x65\x72\x2e\x68\x74\x6d\x6c");
            }
            else
            {
                if (File.Exists(_0x6a6) || Path.IsPathRooted(_0x6a6))
                {
                    try
                    {
                        string _0x7b7 = Path.GetFullPath(_0x6a6);
                        _0x3c.Navigate(new Uri(_0x7b7).AbsoluteUri);
                        return;
                    }
                    catch { }
                }
                _0x3c.Navigate(_0x6a6);
            }
        }

        private void LoadHistoryFromFile()
        {
            try
            {
                if (File.Exists(_0xa3b))
                {
                    var _0x8c8 = File.ReadAllLines(_0xa3b);
                    _0x92a = new List<string>(_0x8c8);
                }
            }
            catch { _0x92a = new List<string>(); }
        }

        private void SaveHistoryToFile()
        {
            try { File.WriteAllLines(_0xa3b, _0x92a); } catch { }
        }

        private void AddUrlToHistory(string _0x9d9)
        {
            if (string.IsNullOrWhiteSpace(_0x9d9) || _0x9d9 == "\x61\x62\x6f\x75\x74\x3a\x62\x6c\x61\x6e\x6b") return;

            _0x92a.RemoveAll(_0x0e0 => _0x0e0.Equals(_0x9d9, StringComparison.OrdinalIgnoreCase));
            _0x92a.Add(_0x9d9);

            if (_0x92a.Count > 10) _0x92a.RemoveAt(0);
            SaveHistoryToFile();
        }

        private void UpdateHistoryMenu()
        {
            _0xd6.DropDownItems.Clear();

            if (_0x92a.Count == 0)
            {
                ToolStripMenuItem _0x1f1 = new ToolStripMenuItem("\x28\x4e\x6f\x20\x68\x69\x73\x74\x6f\x72\x79\x20\x79\x65\x74\x29");
                _0x1f1.Enabled = false;
                _0xd6.DropDownItems.Add(_0x1f1);
                return;
            }

            var _0x2f2 = _0x92a.AsEnumerable().Reverse().ToList();
            foreach (string _0x3f3 in _0x2f2)
            {
                string _0x4g4 = _0x3f3;
                if (_0x4g4.Length > 60) _0x4g4 = _0x4g4.Substring(0, 57) + "\x2e\x2e\x2e";

                ToolStripMenuItem _0x5h5 = new ToolStripMenuItem(_0x4g4);
                string _0x6i6 = _0x3f3;
                _0x5h5.Click += (_0x7j7, _0x8k8) => { NavigateToUrlString(_0x6i6); };
                _0xd6.DropDownItems.Add(_0x5h5);
            }
        }

        private void NavigateToUrl()
        {
            string _0x9l9 = _0x4d.Text.Trim();

            if (_0x9l9.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x77\x65\x6c\x63\x6f\x6d\x65\x2e\x63\x6f\x6d", StringComparison.OrdinalIgnoreCase) ||
                _0x9l9.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x74\x75\x72\x74\x6c\x65\x2e\x63\x68\x61\x74\x62\x6f\x74\x2f", StringComparison.OrdinalIgnoreCase) ||
                _0x9l9.Equals("\x68\x74\x74\x70\x3a\x2f\x2f\x68\x74\x6d\x6c\x67\x61\x6d\x65\x6d\x61\x6b\x65\x72\x2e\x6f\x72\x67\x2f", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToUrlString(_0x9l9);
                return;
            }

            if (Uri.IsWellFormedUriString(_0x9l9, UriKind.Absolute))
            {
                _0x3c.Navigate(_0x9l9);
                return;
            }

            if (File.Exists(_0x9l9) || Directory.Exists(_0x9l9) || Path.IsPathRooted(_0x9l9))
            {
                try
                {
                    string _0x0m0 = Path.GetFullPath(_0x9l9);
                    _0x3c.Navigate(new Uri(_0x0m0).AbsoluteUri);
                    return;
                }
                catch { }
            }

            if (!_0x9l9.StartsWith("\x68\x74\x74\x70\x3a\x2f\x2f") && !_0x9l9.StartsWith("\x68\x74\x74\x70\x73\x3a\x2f\x2f") && !_0x9l9.StartsWith("\x66\x69\x6c\x65\x3a\x2f\x2f"))
            {
                _0x9l9 = "\x68\x74\x74\x70\x3a\x2f\x2f" + _0x9l9;
            }

            _0x3c.Navigate(_0x9l9);
        }
    }
}