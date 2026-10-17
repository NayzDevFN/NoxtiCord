using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

static class Noxtcord
{
    const string Repo = "https://github.com/NayzDevFN/NoxtiCord/raw/main/";

    static readonly Color Bg = Color.FromArgb(18, 19, 22);
    static readonly Color Card = Color.FromArgb(30, 31, 34);
    static readonly Color Surface = Color.FromArgb(43, 45, 49);
    static readonly Color Hover = Color.FromArgb(53, 55, 60);
    static readonly Color Blurple = Color.FromArgb(88, 101, 242);
    static readonly Color BlurpleHover = Color.FromArgb(71, 82, 196);
    static readonly Color Green = Color.FromArgb(35, 165, 89);
    static readonly Color Red = Color.FromArgb(218, 55, 60);
    static readonly Color RedDark = Color.FromArgb(120, 40, 42);
    static readonly Color TextMain = Color.FromArgb(219, 222, 225);
    static readonly Color TextMuted = Color.FromArgb(145, 150, 157);

    static CardPanel statusCard;
    static Label statusTitle;
    static Label statusDetail;
    static Label statusDot;
    static ComboBox branchBox;
    static TextBox logBox;
    static ProgressBar bar;
    static Label footerStatus;
    static Button installBtn;
    static Button uninstallBtn;
    static Button repairBtn;
    static Button folderBtn;
    static Button githubBtn;
    static Button closeBtn;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var form = new Form
        {
            Text = "NoxtiCord Installer",
            ClientSize = new Size(620, 540),
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            MaximizeBox = false,
            BackColor = Bg
        };

        var title = new Label
        {
            Text = "NoxtiCord",
            Font = new Font("Segoe UI", 20f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Bg,
            Location = new Point(20, 16)
        };

        var subtitle = new Label
        {
            Text = "Client modifie pour Discord",
            Font = new Font("Segoe UI", 9.5f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Bg,
            Location = new Point(22, 54)
        };

        statusCard = new CardPanel
        {
            Location = new Point(20, 84),
            Size = new Size(580, 92)
        };

        statusDot = new Label
        {
            Text = "",
            BackColor = Color.Transparent,
            ForeColor = Green,
            Font = new Font("Segoe UI", 22f),
            AutoSize = true,
            Location = new Point(18, 28)
        };
        statusDot.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var b = new SolidBrush(statusDot.ForeColor))
                e.Graphics.FillEllipse(b, 4, 10, 16, 16);
        };

        statusTitle = new Label
        {
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Color.Transparent,
            Location = new Point(52, 18)
        };

        statusDetail = new Label
        {
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(52, 50)
        };

        statusCard.Controls.Add(statusDot);
        statusCard.Controls.Add(statusTitle);
        statusCard.Controls.Add(statusDetail);

        var branchLabel = new Label
        {
            Text = "Version de Discord :",
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Bg,
            Location = new Point(20, 194)
        };

        branchBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5f),
            BackColor = Surface,
            ForeColor = TextMain,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(170, 190),
            Size = new Size(200, 28)
        };
        branchBox.Items.AddRange(new object[] { "Toutes les versions", "Discord Stable", "Discord Canary", "Discord PTB" });
        branchBox.SelectedIndex = 0;

        installBtn = MakeButton("Installer", 20, 232, 380, 48, Blurple, BlurpleHover, TextMain, () => RunOp(true));
        uninstallBtn = MakeButton("Desinstaller", 416, 232, 184, 48, RedDark, Red, TextMain, () => RunOp(false));
        repairBtn = MakeButton("Reparer (reinstalle)", 20, 290, 190, 34, Surface, Hover, TextMain, () => RunOp(true));
        folderBtn = MakeButton("Ouvrir le dossier", 220, 290, 190, 34, Surface, Hover, TextMain, OpenFolder);
        githubBtn = MakeButton("GitHub", 420, 290, 180, 34, Surface, Hover, TextMain, () => Process.Start(Repo));

        logBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = false,
            Font = new Font("Consolas", 8.5f),
            BackColor = Card,
            ForeColor = TextMain,
            BorderStyle = BorderStyle.FixedSingle,
            Location = new Point(20, 338),
            Size = new Size(580, 150)
        };

        bar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Location = new Point(20, 496),
            Size = new Size(580, 8),
            Visible = false
        };

        footerStatus = new Label
        {
            Text = "Pret.",
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Bg,
            Location = new Point(20, 512)
        };

        closeBtn = MakeButton("Fermer", 516, 506, 84, 28, Surface, Hover, TextMain, form.Close);

        form.Controls.AddRange(new Control[]
        {
            title, subtitle, statusCard, branchLabel, branchBox,
            installBtn, uninstallBtn, repairBtn, folderBtn, githubBtn,
            logBox, bar, footerStatus, closeBtn
        });

        RefreshStatus();
        Application.Run(form);
    }

    static Button MakeButton(string text, int x, int y, int w, int h, Color back, Color hover, Color fore, Action onClick)
    {
        var b = new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(w, h),
            FlatStyle = FlatStyle.Flat,
            BackColor = back,
            ForeColor = fore,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", h >= 40 ? 10f : 9f, FontStyle.Bold)
        };
        b.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0, 0);
        b.FlatAppearance.BorderSize = 0;
        b.FlatAppearance.MouseOverBackColor = hover;
        b.FlatAppearance.MouseDownBackColor = hover;
        b.Click += (s, e) => onClick();
        return b;
    }

    static string BranchKey()
    {
        switch (branchBox.SelectedIndex)
        {
            case 1: return "stable";
            case 2: return "canary";
            case 3: return "ptb";
            default: return "";
        }
    }

    static void RefreshStatus()
    {
        string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dist = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Noxticord", "dist", "patcher.js");
        bool payload = File.Exists(dist);

        var patched = new List<string>();
        var apps = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("Discord", "Stable"),
            new KeyValuePair<string, string>("DiscordCanary", "Canary"),
            new KeyValuePair<string, string>("DiscordPTB", "PTB"),
            new KeyValuePair<string, string>("DiscordDevelopment", "Dev")
        };

        foreach (var app in apps)
        {
            string root = Path.Combine(local, app.Key);
            if (!Directory.Exists(root)) continue;
            bool found = false;
            foreach (string v in Directory.GetDirectories(root, "app-*"))
            {
                string asar = Path.Combine(v, "resources", "app.asar");
                if (!File.Exists(asar)) continue;
                try
                {
                    using (var fs = File.OpenRead(asar))
                    {
                        byte[] buf = new byte[4096];
                        int n = fs.Read(buf, 0, buf.Length);
                        string head = System.Text.Encoding.UTF8.GetString(buf, 0, n);
                        if (head.Contains("Noxticord") || head.Contains("Noxtcord")) { found = true; break; }
                    }
                }
                catch { }
            }
            if (found) patched.Add(app.Value);
        }

        if (patched.Count > 0)
        {
            statusDot.ForeColor = Green;
            statusTitle.Text = "Noxtcord est installe";
            statusTitle.ForeColor = TextMain;
            statusDetail.Text = "Installe sur : " + string.Join(", ", patched.ToArray());
        }
        else if (payload)
        {
            statusDot.ForeColor = Color.FromArgb(240, 168, 48);
            statusTitle.Text = "Fichiers presents, patch non detecte";
            statusTitle.ForeColor = TextMain;
            statusDetail.Text = "Relancez l'installation, puis redemarrez Discord.";
        }
        else
        {
            statusDot.ForeColor = TextMuted;
            statusTitle.Text = "Noxtcord n'est pas installe";
            statusTitle.ForeColor = TextMuted;
            statusDetail.Text = "Choisissez une version de Discord puis cliquez sur Installer.";
        }
    }

    static void OpenFolder()
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Noxticord");
        Directory.CreateDirectory(dir);
        Process.Start("explorer.exe", dir);
    }

    static void Log(string msg)
    {
        if (logBox.InvokeRequired) { logBox.Invoke((Action)(() => Log(msg))); return; }
        logBox.AppendText(msg + Environment.NewLine);
    }

    static void SetFooter(string text)
    {
        if (footerStatus.InvokeRequired) { footerStatus.Invoke((Action)(() => SetFooter(text))); return; }
        footerStatus.Text = text;
    }

    static void SetBusy(bool busy)
    {
        if (logBox.InvokeRequired) { logBox.Invoke((Action)(() => SetBusy(busy))); return; }
        installBtn.Enabled = !busy;
        uninstallBtn.Enabled = !busy;
        repairBtn.Enabled = !busy;
        branchBox.Enabled = !busy;
        bar.Visible = busy;
    }

    static void RunOp(bool install)
    {
        string branch = BranchKey();
        string script = install ? "install.ps1" : "uninstall.ps1";
        string label = install
            ? (branch.Length > 0 ? "Installation (" + branch + ")" : "Installation (toutes versions)")
            : "Desinstallation";

        SetBusy(true);
        logBox.Clear();
        SetFooter(label + " en cours...");

        Task.Run(() =>
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                string local = Path.Combine(Path.GetTempPath(), "noxtcord-" + script);

                Log("Telechargement de " + script + " depuis GitHub...");
                using (var c = new WebClient())
                    c.DownloadFile(Repo + script, local);
                Log("OK. Lancement...");
                Log("");

                string args = "-Relaunch";
                if (install && branch.Length > 0) args = "-Branch " + branch + " " + args;

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" + local + "\" " + args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var p = Process.Start(psi))
                {
                    p.OutputDataReceived += (s, e) => { if (e.Data != null) Log(e.Data); };
                    p.ErrorDataReceived += (s, e) => { if (e.Data != null) Log("[erreur] " + e.Data); };
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();

                    Log("");
                    if (p.ExitCode == 0)
                    {
                        SetFooter(label + " terminee avec succes.");
                        Log("Termine avec succes.");
                    }
                    else
                    {
                        SetFooter(label + " terminee avec des erreurs (code " + p.ExitCode + ").");
                        Log("Code de sortie : " + p.ExitCode + ". Verifiez que Discord est ferme, puis reessayez.");
                    }
                }
            }
            catch (Exception ex)
            {
                SetFooter("Erreur.");
                Log("Erreur : " + ex.Message);
            }
            finally
            {
                if (statusCard.InvokeRequired) statusCard.Invoke((Action)RefreshStatus);
                else RefreshStatus();
                SetBusy(false);
            }
        });
    }

    class CardPanel : Panel
    {
        public CardPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int r = 12;
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = new GraphicsPath())
            {
                int d = r * 2;
                path.AddArc(rc.X, rc.Y, d, d, 180, 90);
                path.AddArc(rc.Right - d, rc.Y, d, d, 270, 90);
                path.AddArc(rc.Right - d, rc.Bottom - d, d, d, 0, 90);
                path.AddArc(rc.X, rc.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                using (var b = new SolidBrush(Card))
                    e.Graphics.FillPath(b, path);
            }
        }
    }
}
