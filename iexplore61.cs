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

// Assembly branding metadata
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
            // Force TLS 1.2 so HttpClient can communicate securely with GitHub API
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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
        private Button btnBack;
        private Button btnForward;
        private Button btnRefresh;
        private Button btnStop;
        private Button btnHome;
        private Button btnGo;
        
        // MenuStrip components
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

        // StatusStrip components
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private ToolStripProgressBar progressBar;

        // History, Homepage, Settings paths & files
        private List<string> historyList = new List<string>();
        private readonly string historyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.txt");
        private readonly string homepageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "homepage.txt");
        private readonly string defaultCheckFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "default_check.txt");
        private readonly string versionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.txt");
        private readonly string sourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "source");
        private string currentHomepage = "http://welcome.com";

        public BrowserForm(string startupUrl = null)
        {
            // Window settings
            this.Text = "Internet Explorer 6.1";
            this.Width = 1000;
            this.Height = 700;

            // Load custom icon from the 'source' folder or working directory
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
                // Ignore if icon fails to load
            }

            // Load saved settings & history on startup
            LoadSettings();
            LoadHistoryFromFile();

            // Trigger background update check automatically on startup
            CheckForUpdates(false);

            // Check default browser status once window is shown
            this.Shown += (s, e) => {
                CheckAndPromptDefaultBrowser();
            };

            // Main MenuStrip
            menuStrip = new MenuStrip();
            
            // Browser -> History menu
            menuBrowser = new ToolStripMenuItem("Browser");
            menuHistory = new ToolStripMenuItem("History");
            menuHistory.DropDownOpening += (s, e) => UpdateHistoryMenu();
            menuBrowser.DropDownItems.Add(menuHistory);

            // Web Apps menu -> Turtle & HTML Game Maker
            menuWebApps = new ToolStripMenuItem("Web Apps");
            menuTurtle = new ToolStripMenuItem("Turtle");
            menuHtmlGameMaker = new ToolStripMenuItem("HTML Game Maker");

            menuTurtle.Click += (s, e) => NavigateToWebApp("http://turtle.chatbot/", "turtle.html");
            menuHtmlGameMaker.Click += (s, e) => NavigateToWebApp("http://htmlgamemaker.org/", "maker.html");

            menuWebApps.DropDownItems.Add(menuTurtle);
            menuWebApps.DropDownItems.Add(menuHtmlGameMaker);

            // Options menu -> Internet Options, Set Homepage, Set Default Browser & Check For Updates
            menuOptions = new ToolStripMenuItem("Options");
            menuInternetOptions = new ToolStripMenuItem("Internet Options...");
            menuSetHomepage = new ToolStripMenuItem("Set Current Page as Homepage");
            menuSetDefaultBrowser = new ToolStripMenuItem("Set Default Browser");
            menuCheckForUpdates = new ToolStripMenuItem("Check For Updates");

            menuInternetOptions.Click += (s, e) => {
                try
                {
                    Process.Start("inetcpl.cpl");
                }
                catch
                {
                    MessageBox.Show("Unable to open Windows Internet Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            menuSetHomepage.Click += (s, e) => {
                if (webBrowser.Url != null)
                {
                    string target = txtUrl.Text;
                    if (target.Equals("http://welcome.com", StringComparison.OrdinalIgnoreCase))
                        currentHomepage = "http://welcome.com";
                    else if (target.Equals("http://turtle.chatbot/", StringComparison.OrdinalIgnoreCase))
                        currentHomepage = "http://turtle.chatbot/";
                    else if (target.Equals("http://htmlgamemaker.org/", StringComparison.OrdinalIgnoreCase))
                        currentHomepage = "http://htmlgamemaker.org/";
                    else
                        currentHomepage = webBrowser.Url.ToString();

                    SaveSettings();
                    MessageBox.Show("Homepage updated to:\n" + currentHomepage, "Homepage Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            menuSetDefaultBrowser.Click += (s, e) => {
                SetAsDefaultBrowser();
                MessageBox.Show("Internet Explorer has been set as your default browser for all supported web extensions.", "Internet Explorer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            menuCheckForUpdates.Click += (s, e) => {
                // Manual check triggered by user (shows up-to-date dialog if no updates found)
                CheckForUpdates(true);
            };

            menuOptions.DropDownItems.Add(menuInternetOptions);
            menuOptions.DropDownItems.Add(menuSetHomepage);
            menuOptions.DropDownItems.Add(menuSetDefaultBrowser);
            menuOptions.DropDownItems.Add(new ToolStripSeparator());
            menuOptions.DropDownItems.Add(menuCheckForUpdates);

            // Add menus to Strip
            menuStrip.Items.Add(menuBrowser);
            menuStrip.Items.Add(menuWebApps);
            menuStrip.Items.Add(menuOptions);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // StatusStrip at the bottom with a progress bar on the right
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel("Ready");
            lblStatus.Spring = true;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            progressBar = new ToolStripProgressBar();
            progressBar.Size = new Size(120, 16);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = false;

            statusStrip.Items.Add(lblStatus);
            statusStrip.Items.Add(progressBar);
            this.Controls.Add(statusStrip);

            // Top container panel for navigation controls
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 45;
            topPanel.Padding = new Padding(5);

            // Back Button
            btnBack = new Button();
            btnBack.Text = "◀";
            btnBack.Width = 35;
            btnBack.Location = new Point(8, 8);
            btnBack.Click += (s, e) => { if (webBrowser.CanGoBack) webBrowser.GoBack(); };

            // Forward Button
            btnForward = new Button();
            btnForward.Text = "▶";
            btnForward.Width = 35;
            btnForward.Location = new Point(47, 8);
            btnForward.Click += (s, e) => { if (webBrowser.CanGoForward) webBrowser.GoForward(); };

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "↻";
            btnRefresh.Width = 35;
            btnRefresh.Location = new Point(86, 8);
            btnRefresh.Click += (s, e) => webBrowser.Refresh();

            // Stop Button
            btnStop = new Button();
            btnStop.Text = "✕";
            btnStop.Width = 35;
            btnStop.Location = new Point(125, 8);
            btnStop.Click += (s, e) => webBrowser.Stop();

            // Home Button
            btnHome = new Button();
            btnHome.Text = "⌂";
            btnHome.Width = 35;
            btnHome.Location = new Point(164, 8);
            btnHome.Click += (s, e) => NavigateToUrlString(currentHomepage);

            // Go Button
            btnGo = new Button();
            btnGo.Text = "Go";
            btnGo.Width = 55;
            btnGo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGo.Location = new Point(this.ClientSize.Width - 63, 8);
            btnGo.Click += (s, e) => NavigateToUrl();

            // Address Bar TextBox
            txtUrl = new TextBox();
            txtUrl.Location = new Point(204, 10);
            txtUrl.Width = this.ClientSize.Width - 274;
            txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrl.Font = new Font("Segoe UI", 10F);
            txtUrl.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    NavigateToUrl();
                    e.SuppressKeyPress = true;
                }
            };

            topPanel.Controls.Add(btnBack);
            topPanel.Controls.Add(btnForward);
            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(btnStop);
            topPanel.Controls.Add(btnHome);
            topPanel.Controls.Add(txtUrl);
            topPanel.Controls.Add(btnGo);

            // Initialize the WebBrowser
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
            this.Controls.Add(topPanel);
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
                    if (!string.IsNullOrEmpty(savedVersion))
                    {
                        return savedVersion;
                    }
                }
                else
                {
                    // Automatically create version.txt on first launch with initial v1.0
                    File.WriteAllText(versionFilePath, "v1.0");
                }
            }
            catch
            {
                // Fallback
            }
            return "v1.0"; // Default initial version
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
                string apiUrl = "https://api.github.com/repos/" + repoOwner + "/" + repoName + "/releases/latest";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Internet-Explorer-6.1");
                    
                    string json = await client.GetStringAsync(apiUrl);

                    // 1. Extract the tag name (version)
                    int tagIndex = json.IndexOf("\"tag_name\":\"");
                    if (tagIndex != -1)
                    {
                        int startIndex = tagIndex + 12;
                        int endIndex = json.IndexOf("\"", startIndex);
                        string latestVersion = json.Substring(startIndex, endIndex - startIndex);

                        string currentVersion = GetCurrentVersion();

                        if (latestVersion != currentVersion)
                        {
                            DialogResult result = MessageBox.Show(
                                "A new source update (" + latestVersion + ") is available! Would you like to download and update the source files now?\n\n- Click Yes to update, replace files, and restart.\n- Click No to skip.",
                                "Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                            if (result == DialogResult.Yes)
                            {
                                // 2. Extract the zipball_url for the repository source code at this release tag
                                int zipIndex = json.IndexOf("\"zipball_url\":\"");
                                if (zipIndex != -1)
                                {
                                    int zipStart = zipIndex + 15;
                                    int zipEnd = json.IndexOf("\"", zipStart);
                                    string zipUrl = json.Substring(zipStart, zipEnd - zipStart);

                                    lblStatus.Text = "Downloading repository source...";
                                    progressBar.Visible = true;

                                    // 3. Download zip bytes to Temp folder
                                    byte[] zipBytes = await client.GetByteArrayAsync(zipUrl);
                                    string tempZipPath = Path.Combine(Path.GetTempPath(), "ie_update.zip");
                                    File.WriteAllBytes(tempZipPath, zipBytes);

                                    // 4. Save the new version locally
                                    try
                                    {
                                        File.WriteAllText(versionFilePath, latestVersion);
                                    }
                                    catch { }

                                    // 5. Create a temporary PowerShell update script
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

                                    MessageBox.Show("Source downloaded successfully! The browser will now close, update its files, and restart.", "Updating", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // 6. Launch PowerShell script in the background and exit application
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "powershell.exe",
                                        Arguments = "-ExecutionPolicy Bypass -File \"" + tempScriptPath + "\"",
                                        CreateNoWindow = true,
                                        UseShellExecute = false
                                    });

                                    Application.Exit();
                                }
                                else
                                {
                                    MessageBox.Show("Could not locate the zipball source URL in the release JSON.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else if (manualCheck)
                        {
                            MessageBox.Show("You are currently running the latest version (" + currentVersion + ").", "No Updates Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch
            {
                if (manualCheck)
                {
                    MessageBox.Show("Unable to check for updates. Please check your internet connection.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (string.IsNullOrEmpty(cmd) || cmd.IndexOf(exeName, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        isDefault = false;
                    }
                }

                using (var keyHtml = Registry.CurrentUser.OpenSubKey(@"Software\Classes\.html\shell\open\command", false))
                {
                    string cmd = (keyHtml != null) ? keyHtml.GetValue(null) as string : null;
                    if (string.IsNullOrEmpty(cmd) || cmd.IndexOf(exeName, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        isDefault = false;
                    }
                }

                if (!isDefault)
                {
                    DialogResult res = MessageBox.Show(
                        "Internet Explorer is not currently your default browser.\n\nWould you like to make it your default browser for all HTML, XML, and web protocols?",
                        "Internet Explorer",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (res == DialogResult.Yes)
                    {
                        SetAsDefaultBrowser();
                        MessageBox.Show("Internet Explorer has been set as your default browser for all supported web extensions.", "Internet Explorer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        File.WriteAllText(defaultCheckFilePath, "skip");
                    }
                }
            }
            catch
            {
                // Silently handle exceptions
            }
        }

        private void SetAsDefaultBrowser()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                string command = "\"" + exePath + "\" \"%1\"";

                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\http\shell\open\command"))
                {
                    key.SetValue("", command);
                }
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\https\shell\open\command"))
                {
                    key.SetValue("", command);
                }

                string[] extensions = { ".html", ".htm", ".shtml", ".xhtml", ".xml" };
                foreach (string ext in extensions)
                {
                    using (var key = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + ext + "\\shell\\open\\command"))
                    {
                        key.SetValue("", command);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not automatically set default browser extensions:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(homepageFilePath))
                {
                    string saved = File.ReadAllText(homepageFilePath).Trim();
                    if (!string.IsNullOrEmpty(saved))
                    {
                        currentHomepage = saved;
                    }
                }
            }
            catch
            {
                // Fallback
            }
        }

        private void SaveSettings()
        {
            try
            {
                File.WriteAllText(homepageFilePath, currentHomepage);
            }
            catch
            {
                // Ignore
            }
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
                MessageBox.Show("Could not find offline file: " + fileName, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void NavigateToUrlString(string targetUrl)
        {
            if (targetUrl.Equals("http://welcome.com", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToWebApp("http://welcome.com", "welcome.html");
            }
            else if (targetUrl.Equals("http://turtle.chatbot/", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToWebApp("http://turtle.chatbot/", "turtle.html");
            }
            else if (targetUrl.Equals("http://htmlgamemaker.org/", StringComparison.OrdinalIgnoreCase))
            {
                NavigateToWebApp("http://htmlgamemaker.org/", "maker.html");
            }
            else
            {
                if (File.Exists(targetUrl) || Path.IsPathRooted(targetUrl))
                {
                    try
                    {
                        string fullPath = Path.GetFullPath(targetUrl);
                        webBrowser.Navigate(new Uri(fullPath).AbsoluteUri);
                        return;
                    }
                    catch
                    {
                        // Fall back
                    }
                }

                webBrowser.Navigate(targetUrl);
            }
        }

        private void LoadHistoryFromFile()
        {
            try
            {
                if (File.Exists(historyFilePath))
                {
                    var lines = File.ReadAllLines(historyFilePath);
                    historyList = new List<string>(lines);
                }
            }
            catch
            {
                historyList = new List<string>();
            }
        }

        private void SaveHistoryToFile()
        {
            try
            {
                File.WriteAllLines(historyFilePath, historyList);
            }
            catch
            {
                // Ignore
            }
        }

        private void AddUrlToHistory(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || url == "about:blank") return;

            historyList.RemoveAll(u => u.Equals(url, StringComparison.OrdinalIgnoreCase));
            historyList.Add(url);

            if (historyList.Count > 10)
            {
                historyList.RemoveAt(0);
            }

            SaveHistoryToFile();
        }

        private void UpdateHistoryMenu()
        {
            menuHistory.DropDownItems.Clear();

            if (historyList.Count == 0)
            {
                ToolStripMenuItem emptyItem = new ToolStripMenuItem("(No history yet)");
                emptyItem.Enabled = false;
                menuHistory.DropDownItems.Add(emptyItem);
                return;
            }

            var recentUrls = historyList.AsEnumerable().Reverse().ToList();

            foreach (string url in recentUrls)
            {
                string displayLabel = url;
                if (displayLabel.Length > 60)
                {
                    displayLabel = displayLabel.Substring(0, 57) + "...";
                }

                ToolStripMenuItem item = new ToolStripMenuItem(displayLabel);
                string targetUrl = url; 
                item.Click += (s, e) => {
                    NavigateToUrlString(targetUrl);
                };
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

            if (File.Exists(input) || Directory.Exists(input) || Path.IsPathRooted(input))
            {
                try
                {
                    string fullPath = Path.GetFullPath(input);
                    webBrowser.Navigate(new Uri(fullPath).AbsoluteUri);
                    return;
                }
                catch
                {
                    // Fall back
                }
            }

            if (!input.StartsWith("http://") && !input.StartsWith("https://") && !input.StartsWith("file://"))
            {
                input = "http://" + input;
            }

            webBrowser.Navigate(input);
        }
    }
}