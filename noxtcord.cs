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

    static readonly Color Accent1 = Color.FromArgb(88, 101, 242);
    static readonly Color Accent2 = Color.FromArgb(0, 168, 252);
    static readonly Color Card = Color.FromArgb(23, 24, 28);
    static readonly Color CardBorder = Color.FromArgb(42, 44, 51);
    static readonly Color Surface = Color.FromArgb(30, 31, 36);
    static readonly Color Hover = Color.FromArgb(43, 45, 54);
    static readonly Color Green = Color.FromArgb(35, 165, 89);
    static readonly Color Amber = Color.FromArgb(240, 168, 48);
    static readonly Color RedDark = Color.FromArgb(94, 32, 36);
    static readonly Color Red = Color.FromArgb(218, 55, 60);
    static readonly Color TextMain = Color.FromArgb(230, 232, 236);
    static readonly Color TextMuted = Color.FromArgb(128, 134, 143);

    static FuturisticForm form;
    static CardPanel statusCard;
    static Label statusTitle;
    static Label statusDetail;
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

        form = new FuturisticForm
        {
            Text = "NoxtiCord Installer",
            ClientSize = new Size(720, 600),
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            MaximizeBox = false
        };

        var accentBar = new GradientBar
        {
            Location = new Point(0, 0),
            Size = new Size(720, 3)
        };

        var title = new Label
        {
            Text = "NOXTICORD",
            Font = new Font("Segoe UI", 22f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Color.Transparent,
            Location = new Point(24, 18)
        };

        var tag = new Label
        {
            Text = "// LANCEUR OFFICIEL POUR DISCORD",
            Font = new Font("Consolas", 9f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = Accent2,
            BackColor = Color.Transparent,
            Location = new Point(26, 58)
        };

        statusCard = new CardPanel
        {
            Location = new Point(20, 88),
            Size = new Size(680, 100)
        };

        var dotPanel = new DotPanel { Location = new Point(22, 40), Size = new Size(20, 20) };
        dotPanel.SetColor(Green);

        statusTitle = new Label
        {
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Color.Transparent,
            Location = new Point(58, 22)
        };

        statusDetail = new Label
        {
            Font = new Font("Segoe UI", 9.5f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(58, 58)
        };

        statusCard.Controls.Add(dotPanel);
        statusCard.Controls.Add(statusTitle);
        statusCard.Controls.Add(statusDetail);
        statusCard.DotPanel = dotPanel;

        var branchLabel = new Label
        {
            Text = "VERSION DE DISCORD",
            Font = new Font("Consolas", 8.5f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(22, 208)
        };

        branchBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10f),
            BackColor = Surface,
            ForeColor = TextMain,
            FlatStyle = FlatStyle.Flat,
            Location = new Point(200, 202),
            Size = new Size(220, 30)
        };
        branchBox.Items.AddRange(new object[] { "Toutes les versions", "Discord Stable", "Discord Canary", "Discord PTB" });
        branchBox.SelectedIndex = 0;

        installBtn = MakeButton("INSTALLER", 20, 248, 444, 52, Accent1, Accent2, TextMain, true, () => RunOp(true));
        uninstallBtn = MakeButton("DESINSTALLER", 480, 248, 220, 52, RedDark, Red, TextMain, false, () => RunOp(false));

        repairBtn = MakeButton("REPARER", 20, 316, 210, 38, Surface, Hover, TextMain, false, () => RunOp(true));
        folderBtn = MakeButton("DOSSIER", 246, 316, 210, 38, Surface, Hover, TextMain, false, OpenFolder);
        githubBtn = MakeButton("GITHUB", 472, 316, 228, 38, Surface, Hover, TextMain, false, () => Process.Start(Repo));

        logBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = false,
            Font = new Font("Consolas", 9f),
            BackColor = Color.FromArgb(12, 13, 16),
            ForeColor = Color.FromArgb(120, 220, 160),
            BorderStyle = BorderStyle.FixedSingle,
            Location = new Point(20, 370),
            Size = new Size(680, 168)
        };

        bar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Location = new Point(20, 548),
            Size = new Size(680, 6),
            Visible = false
        };

        footerStatus = new Label
        {
            Text = "PRET.",
            Font = new Font("Consolas", 8.5f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(20, 564)
        };

        closeBtn = MakeButton("X", 660, 556, 40, 32, Surface, Hover, TextMuted, false, form.Close);

        form.Controls.AddRange(new Control[]
        {
            accentBar, title, tag, statusCard, branchLabel, branchBox,
            installBtn, uninstallBtn, repairBtn, folderBtn, githubBtn,
            logBox, bar, footerStatus, closeBtn
        });

        RoundControl(installBtn, 10);
        RoundControl(uninstallBtn, 10);
        RoundControl(repairBtn, 8);
        RoundControl(folderBtn, 8);
        RoundControl(githubBtn, 8);
        RoundControl(closeBtn, 8);

        RefreshStatus();
        Application.Run(form);
    }

    static Button MakeButton(string text, int x, int y, int w, int h, Color back, Color hover, Color fore, bool primary, Action onClick)
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
            Font = new Font("Segoe UI", primary ? 12f : 9.5f, FontStyle.Bold)
        };
        b.FlatAppearance.BorderColor = primary ? Color.FromArgb(120, 130, 255) : CardBorder;
        b.FlatAppearance.BorderSize = primary ? 1 : 1;
        b.FlatAppearance.MouseOverBackColor = hover;
        b.FlatAppearance.MouseDownBackColor = hover;
        b.Click += (s, e) => onClick();
        return b;
    }

    static void RoundControl(Control c, int r)
    {
        using (var path = new GraphicsPath())
        {
            int d = r * 2;
            var rc = new Rectangle(0, 0, c.Width, c.Height);
            path.AddArc(rc.X, rc.Y, d, d, 180, 90);
            path.AddArc(rc.Right - d, rc.Y, d, d, 270, 90);
            path.AddArc(rc.Right - d, rc.Bottom - d, d, d, 0, 90);
            path.AddArc(rc.X, rc.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            c.Region = new Region(path);
        }
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
            statusCard.DotPanel.SetColor(Green);
            statusTitle.Text = "NOXTCORD EST INSTALLE";
            statusTitle.ForeColor = TextMain;
            statusDetail.Text = "Actif sur : " + string.Join(", ", patched.ToArray());
        }
        else if (payload)
        {
            statusCard.DotPanel.SetColor(Amber);
            statusTitle.Text = "FICHIERS PRESENTS, PATCH NON DETECTE";
            statusTitle.ForeColor = Amber;
            statusDetail.Text = "Relancez l'installation puis redemarrez Discord.";
        }
        else
        {
            statusCard.DotPanel.SetColor(TextMuted);
            statusTitle.Text = "NOXTCORD N'EST PAS INSTALLE";
            statusTitle.ForeColor = TextMuted;
            statusDetail.Text = "Choisissez une version de Discord puis cliquez sur INSTALLER.";
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
        footerStatus.Text = text.ToUpperInvariant();
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

                Log("> Telechargement de " + script + " depuis GitHub...");
                using (var c = new WebClient())
                    c.DownloadFile(Repo + script, local);
                Log("> OK, lancement...");
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
                    p.ErrorDataReceived += (s, e) => { if (e.Data != null) Log("[!] " + e.Data); };
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();

                    Log("");
                    if (p.ExitCode == 0)
                    {
                        SetFooter(label + " terminee avec succes");
                        Log("> Termine avec succes.");
                    }
                    else
                    {
                        SetFooter(label + " echec (code " + p.ExitCode + ")");
                        Log("> Code de sortie : " + p.ExitCode + ". Verifiez que Discord est ferme, puis reessayez.");
                    }
                }
            }
            catch (Exception ex)
            {
                SetFooter("Erreur");
                Log("[!] Erreur : " + ex.Message);
            }
            finally
            {
                if (form.InvokeRequired) form.Invoke((Action)RefreshStatus);
                else RefreshStatus();
                SetBusy(false);
            }
        });
    }

    class FuturisticForm : Form
    {
        public FuturisticForm()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var lg = new LinearGradientBrush(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height),
                Color.FromArgb(10, 11, 14), Color.FromArgb(18, 20, 26), LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(lg, 0, 0, ClientSize.Width, ClientSize.Height);
            }
        }
    }

    class GradientBar : Control
    {
        public GradientBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var lg = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Accent1, Accent2, LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(lg, 0, 0, Width, Height);
            }
        }
    }

    class DotPanel : Control
    {
        Color dotColor = Green;

        public DotPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void SetColor(Color c)
        {
            dotColor = c;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var glow = new SolidBrush(Color.FromArgb(40, dotColor)))
                e.Graphics.FillEllipse(glow, 0, 0, 20, 20);
            using (var b = new SolidBrush(dotColor))
                e.Graphics.FillEllipse(b, 4, 4, 12, 12);
        }
    }

    class CardPanel : Panel
    {
        public DotPanel DotPanel;

        public CardPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int r = 14;
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
                using (var pen = new Pen(CardBorder, 1f))
                    e.Graphics.DrawPath(pen, path);
            }
        }
    }
}
