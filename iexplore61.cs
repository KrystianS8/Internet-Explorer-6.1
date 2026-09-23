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
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string initialTarget = null;
            if (args != null && args.Length > 0)
            {
                initialTarget = args[0];
            }

            Application.Run(new BrowserForm(initialTarget));
        }
    }

    public class BrowserForm : Form
    {
        private WebBrowser webBrowser;
        private TextBox txtUrl;
        
        private ToolStrip toolStrip;
        private ToolStripButton btnBack;
        private ToolStripButton btnForward;
        private ToolStripButton btnRefresh;
        private ToolStripButton btnStop;
        private ToolStripButton btnHome;
        private ToolStripControlHost addressBarHost;
        private ToolStripButton btnGo;
        private ToolTip toolTip;
        
        private MenuStrip menuStrip;
        private ToolStripMenuItem menuBrowser;
        private ToolStripMenuItem menuHistory;
        
        private ToolStripMenuItem menuWebApps;
        private ToolStripMenuItem menuTurtle;
        private ToolStripMenuItem menuHtmlGameMaker;

        private ToolStripMenuItem menuOptions;
        private ToolStripMenuItem menuInternetOptions;
        private ToolStripMenuItem menuSetHomepage;
        private ToolStripMenuItem menuSetDefaultBrowser;
        private ToolStripMenuItem menuCheckForUpdates;

        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private ToolStripProgressBar progressBar;

        private List<string> historyList = new List<string>();
        private readonly string historyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.txt");
        private readonly string homepageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "homepage.txt");
        private readonly string defaultCheckFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "default_check.txt");
        private readonly string versionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.txt");
        private readonly string sourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "source");
        private string currentHomepage = "http://welcome.com";

        public BrowserForm(string startupUrl = null)
        {
            this.Text = "Internet Explorer 6.1";
            this.Width = 1024;
            this.Height = 720;
            this.MinimumSize = new Size(600, 400);

            try
            {
                string iconPath = Path.Combine(sourceDir, "ieicon.ico");
                if (!File.Exists(iconPath)) iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ieicon.ico");

                if (File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch
            {
                // Ignore icon failures
            }

            LoadSettings();
            LoadHistoryFromFile();
            CheckForUpdates(false);

            this.Shown += (s, e) => {
                CheckAndPromptDefaultBrowser();
            };

            // --- Menu Strip ---
            menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(235, 243, 253);
            
            menuBrowser = new ToolStripMenuItem("Browser");
            
            menuHistory = new ToolStripMenuItem("History");
            menuHistory.DropDownOpening += (s, e) => UpdateHistoryMenu();
            menuBrowser.DropDownItems.Add(menuHistory);
            menuBrowser.DropDownItems.Add(new ToolStripSeparator());

            menuWebApps = new ToolStripMenuItem("Web Apps");
            menuTurtle = new ToolStripMenuItem("Turtle");
            menuHtmlGameMaker = new ToolStripMenuItem("HTML Game Maker");

            menuTurtle.Click += (s, e) => NavigateToWebApp("http://turtle.chatbot/", "turtle.html");
            menuHtmlGameMaker.Click += (s, e) => NavigateToWebApp("http://htmlgamemaker.org/", "maker.html");

            menuWebApps.DropDownItems.Add(menuTurtle);
            menuWebApps.DropDownItems.Add(menuHtmlGameMaker);
            menuBrowser.DropDownItems.Add(menuWebApps);

            menuOptions = new ToolStripMenuItem("Options");
            menuInternetOptions = new ToolStripMenuItem("Internet Options...");
            menuSetHomepage = new ToolStripMenuItem("Set Current Page as Homepage");
            menuSetDefaultBrowser = new ToolStripMenuItem("Set Default Browser");
            menuCheckForUpdates = new ToolStripMenuItem("Check For Updates");

            menuInternetOptions.Click += (s, e) => {
                try { Process.Start(new ProcessStartInfo("inetcpl.cpl") { UseShellExecute = true }); }
                catch { MessageBox.Show("Unable to open Windows Internet Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            menuSetHomepage.Click += (s, e) => {
                if (webBrowser.Url != null)
                {
                    currentHomepage = txtUrl.Text;
                    SaveSettings();
                    MessageBox.Show("Homepage updated successfully.", "Homepage Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            menuSetDefaultBrowser.Click += (s, e) => {
                SetAsDefaultBrowser();
                MessageBox.Show("Internet Explorer has been set as your default browser.", "Internet Explorer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            menuCheckForUpdates.Click += (s, e) => CheckForUpdates(true);

            menuOptions.DropDownItems.Add(menuInternetOptions);
            menuOptions.DropDownItems.Add(menuSetHomepage);
            menuOptions.DropDownItems.Add(menuSetDefaultBrowser);
            menuOptions.DropDownItems.Add(new ToolStripSeparator());
            menuOptions.DropDownItems.Add(menuCheckForUpdates);

            menuStrip.Items.Add(menuBrowser);
            menuStrip.Items.Add(menuOptions);
            this.Controls.Add(menuStrip);

            // --- Status Strip ---
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(235, 243, 253);
            lblStatus = new ToolStripStatusLabel("Ready");
            lblStatus.Spring = true;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            progressBar = new ToolStripProgressBar();
            progressBar.Size = new Size(120, 14);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = false;

            statusStrip.Items.Add(lblStatus);
            statusStrip.Items.Add(progressBar);
            this.Controls.Add(statusStrip);

            // --- Native ToolStrip for Toolbar ---
            toolStrip = new ToolStrip();
            toolStrip.Dock = DockStyle.Top;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.BackColor = Color.FromArgb(220, 235, 252);
            toolStrip.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            toolStrip.Padding = new Padding(4, 4, 4, 4);

            // Back Button (<)
            btnBack = new ToolStripButton("<");
            btnBack.ForeColor = Color.FromArgb(15, 65, 130);
            btnBack.ToolTipText = "Back";
            btnBack.Click += (s, e) => { if (webBrowser.CanGoBack) webBrowser.GoBack(); };

            // Forward Button (>)
            btnForward = new ToolStripButton(">");
            btnForward.ForeColor = Color.FromArgb(15, 65, 130);
            btnForward.ToolTipText = "Forward";
            btnForward.Click += (s, e) => { if (webBrowser.CanGoForward) webBrowser.GoForward(); };

            // Refresh Button (↻)
            btnRefresh = new ToolStripButton("\u21BB");
            btnRefresh.ForeColor = Color.FromArgb(15, 65, 130);
            btnRefresh.ToolTipText = "Refresh";
            btnRefresh.Click += (s, e) => webBrowser.Refresh();

            // Stop Button (x)
            btnStop = new ToolStripButton("x");
            btnStop.ForeColor = Color.FromArgb(15, 65, 130);
            btnStop.ToolTipText = "Stop";
            btnStop.Click += (s, e) => webBrowser.Stop();

            // Home Button
            btnHome = new ToolStripButton("Home");
            btnHome.ForeColor = Color.FromArgb(15, 65, 130);
            btnHome.ToolTipText = "Home";
            btnHome.Click += (s, e) => NavigateToUrlString(currentHomepage);

            // Address Bar TextBox
            txtUrl = new TextBox();
            txtUrl.BorderStyle = BorderStyle.Fixed3D;
            txtUrl.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            txtUrl.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    NavigateToUrl();
                    e.SuppressKeyPress = true;
                }
            };
            
            // Tooltip for the address bar text box
            toolTip = new ToolTip();
            toolTip.SetToolTip(txtUrl, "Address");

            // Host the TextBox inside the ToolStrip
            addressBarHost = new ToolStripControlHost(txtUrl);
            addressBarHost.AutoSize = false;
            addressBarHost.Width = 400; // Initial width
            addressBarHost.Height = 26;

            // Go Button (>>>)
            btnGo = new ToolStripButton(">>>");
            btnGo.ForeColor = Color.FromArgb(15, 65, 130);
            btnGo.ToolTipText = "Go";
            btnGo.Click += (s, e) => NavigateToUrl();

            toolStrip.Items.Add(btnBack);
            toolStrip.Items.Add(btnForward);
            toolStrip.Items.Add(btnRefresh);
            toolStrip.Items.Add(btnStop);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnHome);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(addressBarHost);
            toolStrip.Items.Add(btnGo);

            // Dynamically stretch address bar when toolbar resizes
            toolStrip.Resize += (s, e) => {
                int fixedWidths = 0;
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    if (item != addressBarHost)
                    {
                        fixedWidths += item.Width + item.Margin.Horizontal;
                    }
                }
                int availableWidth = toolStrip.ClientRectangle.Width - fixedWidths - toolStrip.Padding.Horizontal - 25;
                if (availableWidth > 100)
                {
                    addressBarHost.Width = availableWidth;
                }
            };

            this.Controls.Add(toolStrip);

            // --- WebBrowser Control ---
            webBrowser = new WebBrowser();
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.ScriptErrorsSuppressed = true;
            
            webBrowser.Navigating += (s, e) => {
                if (e.Url != null)
                {
                    lblStatus.Text = "Loading: " + e.Url.ToString();
                    progressBar.Visible = true;
                }
            };
            
            webBrowser.Navigated += (s, e) => {
                if (webBrowser.Url != null)
                {
                    string currentUrl = webBrowser.Url.ToString();
                    if (currentUrl.EndsWith("welcome.html", StringComparison.OrdinalIgnoreCase))
                    {
                        txtUrl.Text = "http://welcome.com";
                        AddUrlToHistory("http://welcome.com");
                    }
                    else if (currentUrl.EndsWith("turtle.html", StringComparison.OrdinalIgnoreCase))
                    {
                        txtUrl.Text = "http://turtle.chatbot/";
                        AddUrlToHistory("http://turtle.chatbot/");
                    }
                    else if (currentUrl.EndsWith("maker.html", StringComparison.OrdinalIgnoreCase))
                    {
                        txtUrl.Text = "http://htmlgamemaker.org/";
                        AddUrlToHistory("http://htmlgamemaker.org/");
                    }
                    else
                    {
                        txtUrl.Text = currentUrl;
                        AddUrlToHistory(currentUrl);
                    }
                }
            };

            webBrowser.DocumentCompleted += (s, e) => {
                lblStatus.Text = "Ready";
                progressBar.Visible = false;
                if (webBrowser.Document != null && !string.IsNullOrEmpty(webBrowser.Document.Title))
                {
                    this.Text = webBrowser.Document.Title + " - Internet Explorer 6.1";
                }
                else
                {
                    this.Text = "Internet Explorer 6.1";
                }
            };

            this.Controls.Add(webBrowser);
            this.Controls.Add(statusStrip);
            this.Controls.Add(toolStrip);
            this.Controls.Add(menuStrip);

            if (!string.IsNullOrEmpty(startupUrl))
            {
                NavigateToUrlString(startupUrl);
            }
            else
            {
                NavigateToUrlString(currentHomepage);
            }
        }

        private string GetCurrentVersion()
        {
            try
            {
                if (File.Exists(versionFilePath))
                {
                    string savedVersion = File.ReadAllText(versionFilePath).Trim();
                    if (!string.IsNullOrEmpty(savedVersion)) return savedVersion;
                }
                else
                {
                    File.WriteAllText(versionFilePath, "v1.0");
                }
            }
            catch { }
            return "v1.0";
        }

        private async void CheckForUpdates(bool manualCheck)
        {
            try
            {
                if (manualCheck)
                {
                    lblStatus.Text = "Checking for updates...";
                    progressBar.Visible = true;
                }

                string repoOwner = "KrystianS8";
                string repoName = "Internet-Explorer-6.1";
                string apiUrl = string.Format("https://api.github.com/repos/{0}/{1}/releases/latest", repoOwner, repoName);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Internet-Explorer-6.1");
                    string json = await client.GetStringAsync(apiUrl);

                    Match tagMatch = Regex.Match(json, "\"tag_name\"\\s*:\\s*\"([^\"]+)\"");
                    Match zipMatch = Regex.Match(json, "\"zipball_url\"\\s*:\\s*\"([^\"]+)\"");

                    if (tagMatch.Success)
                    {
                        string latestVersion = tagMatch.Groups[1].Value;
                        string currentVersion = GetCurrentVersion();

                        if (latestVersion != currentVersion)
                        {
                            DialogResult result = MessageBox.Show(
                                string.Format("A new source update ({0}) is available! Would you like to download and update now?", latestVersion),
                                "Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                            if (result == DialogResult.Yes && zipMatch.Success)
                            {
                                string zipUrl = zipMatch.Groups[1].Value;
                                lblStatus.Text = "Downloading update...";
                                progressBar.Visible = true;

                                byte[] zipBytes = await client.GetByteArrayAsync(zipUrl);
                                string tempZipPath = Path.Combine(Path.GetTempPath(), "ie_update.zip");
                                File.WriteAllBytes(tempZipPath, zipBytes);

                                try { File.WriteAllText(versionFilePath, latestVersion); } catch { }

                                string targetDirectory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
                                string tempScriptPath = Path.Combine(Path.GetTempPath(), "ie_update.ps1");

                                string psScriptContent = 
                                    "Start-Sleep -Seconds 2\n" +
                                    "Expand-Archive -Path '" + tempZipPath + "' -DestinationPath '" + Path.GetTempPath() + "ie_extract' -Force\n" +
                                    "$innerFolder = Get-ChildItem '" + Path.GetTempPath() + "ie_extract' | Select-Object -First 1\n" +
                                    "Copy-Item -Path \"$($innerFolder.FullName)\\*\" -Destination '" + targetDirectory + "' -Recurse -Force\n" +
                                    "Remove-Item '" + tempZipPath + "' -Force\n" +
                                    "Remove-Item '" + Path.GetTempPath() + "ie_extract' -Recurse -Force\n" +
                                    "Start-Process '" + Path.Combine(targetDirectory, "iexplore61.exe") + "'\n" +
                                    "Remove-Item $MyInvocation.MyCommand.Path -Force";

                                File.WriteAllText(tempScriptPath, psScriptContent);

                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = "powershell.exe",
                                    Arguments = string.Format("-ExecutionPolicy Bypass -File \"{0}\"", tempScriptPath),
                                    CreateNoWindow = true,
                                    UseShellExecute = false
                                });

                                Application.Exit();
                            }
                        }
                        else if (manualCheck)
                        {
                            MessageBox.Show(string.Format("You are running the latest version ({0}).", currentVersion), "No Updates Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch
            {
                if (manualCheck)
                {
                    MessageBox.Show("Unable to check for updates. Check your internet connection.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                lblStatus.Text = "Ready";
                progressBar.Visible = false;
            }
        }

        private void CheckAndPromptDefaultBrowser()
        {
            try
            {
                if (File.Exists(defaultCheckFilePath))
                {
                    string setting = File.ReadAllText(defaultCheckFilePath).Trim();
                    if (setting.Equals("skip", StringComparison.OrdinalIgnoreCase)) return;
                }

                bool isDefault = true;
                string exeName = Path.GetFileName(Application.ExecutablePath);

                using (var keyHttp = Registry.CurrentUser.OpenSubKey(@"Software\Classes\http\shell\open\command", false))
                {
                    string cmd = (keyHttp != null) ? keyHttp.GetValue(null) as string : null;
                    if (string.IsNullOrEmpty(cmd) || cmd.IndexOf(exeName, StringComparison.OrdinalIgnoreCase) < 0) isDefault = false;
                }

                if (!isDefault)
                {
                    DialogResult res = MessageBox.Show(
                        "Internet Explorer is not your default browser.\n\nWould you like to make it your default browser?",
                        "Internet Explorer",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (res == DialogResult.Yes)
                    {
                        SetAsDefaultBrowser();
                        MessageBox.Show("Internet Explorer is now your default browser.", "Internet Explorer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        File.WriteAllText(defaultCheckFilePath, "skip");
                    }
                }
            }
            catch { }
        }

        private void SetAsDefaultBrowser()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                string command = "\"" + exePath + "\" \"%1\"";

                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\http\shell\open\command")) key.SetValue("", command);
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\https\shell\open\command")) key.SetValue("", command);

                foreach (string ext in new[] { ".html", ".htm", ".shtml", ".xhtml", ".xml" })
                {
                    using (var key = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + ext + "\\shell\\open\\command")) key.SetValue("", command);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not set default browser extensions:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(homepageFilePath))
                {
                    string saved = File.ReadAllText(homepageFilePath).Trim();
                    if (!string.IsNullOrEmpty(saved)) currentHomepage = saved;
                }
            }
            catch { }
        }

        private void SaveSettings()
        {
            try { File.WriteAllText(homepageFilePath, currentHomepage); } catch { }
        }

        private void NavigateToWebApp(string customUrl, string fileName)
        {
            string fullPath = Path.Combine(sourceDir, fileName);
            if (!File.Exists(fullPath)) fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (File.Exists(fullPath))
            {
                webBrowser.Navigate(new Uri(fullPath).AbsoluteUri);
                txtUrl.Text = customUrl;
                AddUrlToHistory(customUrl);
            }
            else
            {
                MessageBox.Show("Could not find file: " + fileName, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void NavigateToUrlString(string targetUrl)
        {
            if (targetUrl.Equals("http://welcome.com", StringComparison.OrdinalIgnoreCase)) NavigateToWebApp("http://welcome.com", "welcome.html");
            else if (targetUrl.Equals("http://turtle.chatbot/", StringComparison.OrdinalIgnoreCase)) NavigateToWebApp("http://turtle.chatbot/", "turtle.html");
            else if (targetUrl.Equals("http://htmlgamemaker.org/", StringComparison.OrdinalIgnoreCase)) NavigateToWebApp("http://htmlgamemaker.org/", "maker.html");
            else
            {
                if (File.Exists(targetUrl) || Path.IsPathRooted(targetUrl))
                {
                    try { webBrowser.Navigate(new Uri(Path.GetFullPath(targetUrl)).AbsoluteUri); return; } catch { }
                }
                webBrowser.Navigate(targetUrl);
            }
        }

        private void LoadHistoryFromFile()
        {
            try
            {
                if (File.Exists(historyFilePath)) historyList = new List<string>(File.ReadAllLines(historyFilePath));
            }
            catch { historyList = new List<string>(); }
        }

        private void SaveHistoryToFile()
        {
            try { File.WriteAllLines(historyFilePath, historyList); } catch { }
        }

        private void AddUrlToHistory(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || url == "about:blank") return;
            historyList.RemoveAll(u => u.Equals(url, StringComparison.OrdinalIgnoreCase));
            historyList.Add(url);
            if (historyList.Count > 15) historyList.RemoveAt(0);
            SaveHistoryToFile();
        }

        private void UpdateHistoryMenu()
        {
            menuHistory.DropDownItems.Clear();
            if (historyList.Count == 0)
            {
                ToolStripMenuItem emptyItem = new ToolStripMenuItem("(No history yet)") { Enabled = false };
                menuHistory.DropDownItems.Add(emptyItem);
                return;
            }

            foreach (string url in historyList.AsEnumerable().Reverse())
            {
                string displayLabel = url.Length > 55 ? url.Substring(0, 52) + "..." : url;
                ToolStripMenuItem item = new ToolStripMenuItem(displayLabel);
                string targetUrl = url;
                item.Click += (s, e) => NavigateToUrlString(targetUrl);
                menuHistory.DropDownItems.Add(item);
            }
        }

        private void NavigateToUrl()
        {
            string input = txtUrl.Text.Trim();
            if (input.Equals("http://welcome.com", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("http://turtle.chatbot/", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("http://htmlgamemaker.org/", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToUrlString(input);
                return;
            }

            if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                webBrowser.Navigate(input);
                return;
            }

            if (File.Exists(input) || Path.IsPathRooted(input))
            {
                try { webBrowser.Navigate(new Uri(Path.GetFullPath(input)).AbsoluteUri); return; } catch { }
            }

            if (!input.StartsWith("http://") && !input.StartsWith("https://") && !input.StartsWith("file://"))
            {
                input = "http://" + input;
            }

            webBrowser.Navigate(input);
        }
    }
}