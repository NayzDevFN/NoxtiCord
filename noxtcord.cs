using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

static class Noxtcord
{
    const string Repo = "https://github.com/NayzDevFN/NoxtiCord/raw/main/";

    // Logo Discord "D" (Wikimedia Commons, Discord color D.svg) — remplacé à la compilation
    const string LogoBase64 = "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";

    static readonly Color Accent1 = Color.FromArgb(88, 101, 242);
    static readonly Color Accent2 = Color.FromArgb(0, 168, 252);
    static readonly Color Canary = Color.FromArgb(250, 166, 26);
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

    static Image logoStable;
    static Image logoCanary;
    static Image logoMuted;

    static FuturisticForm form;
    static CardPanel statusCard;
    static Label statusTitle;
    static Label statusDetail;
    static LogoOption optStable;
    static LogoOption optCanary;
    static string selectedBranch = "stable";
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

        using (var ms = new MemoryStream(Convert.FromBase64String(LogoBase64)))
        {
            var src = Image.FromStream(ms);
            logoStable = new Bitmap(src);
            logoCanary = Tint((Bitmap)logoStable, Canary);
            logoMuted = Grayscale((Bitmap)logoStable);
            src.Dispose();
        }

        form = new FuturisticForm
        {
            Text = "NoxtiCord Installer",
            ClientSize = new Size(720, 640),
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            MaximizeBox = false
        };

        var accentBar = new GradientBar { Location = new Point(0, 0), Size = new Size(720, 3) };

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

        statusCard = new CardPanel { Location = new Point(20, 88), Size = new Size(680, 100) };

        var statusLogo = new PictureBox
        {
            Image = logoStable,
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(40, 56),
            Location = new Point(24, 22),
            BackColor = Color.Transparent
        };

        statusTitle = new Label
        {
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Color.Transparent,
            Location = new Point(84, 24)
        };

        statusDetail = new Label
        {
            Font = new Font("Segoe UI", 9.5f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(84, 58)
        };

        statusCard.Controls.Add(statusLogo);
        statusCard.Controls.Add(statusTitle);
        statusCard.Controls.Add(statusDetail);
        statusCard.StatusLogo = statusLogo;

        var pickLabel = new Label
        {
            Text = "CHOISISSEZ VOTRE VERSION DE DISCORD",
            Font = new Font("Consolas", 8.5f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(22, 206)
        };

        optStable = new LogoOption("stable", "Discord", logoStable, new Point(20, 232), new Size(332, 84));
        optCanary = new LogoOption("canary", "Discord Canary", logoCanary, new Point(368, 232), new Size(332, 84));
        optStable.Picked += () => PickOption(optStable);
        optCanary.Picked += () => PickOption(optCanary);
        optStable.SetSelected(true);

        installBtn = MakeButton("INSTALLER", 20, 340, 444, 52, Accent1, Accent2, TextMain, true, () => RunOp(true));
        uninstallBtn = MakeButton("DÉSINSTALLER", 480, 340, 220, 52, RedDark, Red, TextMain, false, () => RunOp(false));

        repairBtn = MakeButton("RÉPARER", 20, 408, 210, 38, Surface, Hover, TextMain, false, () => RunOp(true));
        folderBtn = MakeButton("DOSSIER", 246, 408, 210, 38, Surface, Hover, TextMain, false, OpenFolder);
        githubBtn = MakeButton("GITHUB", 472, 408, 228, 38, Surface, Hover, TextMain, false, () => Process.Start(Repo));

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
            Location = new Point(20, 462),
            Size = new Size(680, 132)
        };

        bar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Location = new Point(20, 604),
            Size = new Size(680, 6),
            Visible = false
        };

        footerStatus = new Label
        {
            Text = "Prêt.",
            Font = new Font("Consolas", 8.5f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(20, 618)
        };

        closeBtn = MakeButton("X", 660, 612, 40, 28, Surface, Hover, TextMuted, false, form.Close);

        form.Controls.AddRange(new Control[]
        {
            accentBar, title, tag, statusCard, pickLabel, optStable, optCanary,
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

    static void PickOption(LogoOption chosen)
    {
        selectedBranch = chosen.Branch;
        optStable.SetSelected(chosen == optStable);
        optCanary.SetSelected(chosen == optCanary);
    }

    static Image Tint(Bitmap src, Color c)
    {
        var bmp = new Bitmap(src.Width, src.Height);
        using (var g = Graphics.FromImage(bmp))
        {
            float r = c.R / 255f, gr = c.G / 255f, b = c.B / 255f;
            var m = new ColorMatrix(new float[][]
            {
                new float[] { r, 0, 0, 0, 0 },
                new float[] { 0, gr, 0, 0, 0 },
                new float[] { 0, 0, b, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            });
            var ia = new ImageAttributes();
            ia.SetColorMatrix(m);
            g.DrawImage(src, new Rectangle(0, 0, src.Width, src.Height), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, ia);
        }
        return bmp;
    }

    static Image Grayscale(Bitmap src)
    {
        var bmp = new Bitmap(src.Width, src.Height);
        using (var g = Graphics.FromImage(bmp))
        {
            var m = new ColorMatrix(new float[][]
            {
                new float[] { 0.33f, 0.33f, 0.33f, 0, 0 },
                new float[] { 0.33f, 0.33f, 0.33f, 0, 0 },
                new float[] { 0.33f, 0.33f, 0.33f, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            });
            var ia = new ImageAttributes();
            ia.SetColorMatrix(m);
            g.DrawImage(src, new Rectangle(0, 0, src.Width, src.Height), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, ia);
        }
        return bmp;
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
        b.FlatAppearance.BorderSize = 1;
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

    static void RefreshStatus()
    {
        string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dist = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Noxticord", "dist", "patcher.js");
        bool payload = File.Exists(dist);

        var patched = new List<string>();
        var apps = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("Discord", "Discord"),
            new KeyValuePair<string, string>("DiscordCanary", "Discord Canary"),
            new KeyValuePair<string, string>("DiscordPTB", "Discord PTB"),
            new KeyValuePair<string, string>("DiscordDevelopment", "Discord Dev")
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
            bool onlyCanary = true;
            foreach (string p in patched) if (p != "Discord Canary") { onlyCanary = false; break; }
            statusCard.StatusLogo.Image = onlyCanary ? logoCanary : logoStable;
            statusTitle.Text = "Noxtcord est installé";
            statusTitle.ForeColor = Green;
            statusDetail.Text = "Actif sur : " + string.Join(", ", patched.ToArray()) + ". Discord a été relancé.";
        }
        else if (payload)
        {
            statusCard.StatusLogo.Image = logoMuted;
            statusTitle.Text = "Fichiers présents, patch non détecté";
            statusTitle.ForeColor = Amber;
            statusDetail.Text = "Relancez l'installation puis redémarrez Discord.";
        }
        else
        {
            statusCard.StatusLogo.Image = logoMuted;
            statusTitle.Text = "Noxtcord n'est pas installé";
            statusTitle.ForeColor = TextMuted;
            statusDetail.Text = "Choisissez une version puis cliquez sur INSTALLER.";
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
        optStable.Enabled = !busy;
        optCanary.Enabled = !busy;
        bar.Visible = busy;
    }

    static void RunOp(bool install)
    {
        string branch = selectedBranch;
        string script = install ? "install.ps1" : "uninstall.ps1";
        string label = install
            ? "Installation (" + (branch == "canary" ? "Discord Canary" : "Discord") + ")"
            : "Désinstallation";

        SetBusy(true);
        logBox.Clear();
        SetFooter(label + " en cours...");

        Task.Run(() =>
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                string local = Path.Combine(Path.GetTempPath(), "noxtcord-" + script);

                Log("> Téléchargement de " + script + " depuis GitHub...");
                using (var c = new WebClient())
                    c.DownloadFile(Repo + script, local);
                Log("> OK, lancement...");
                Log("");

                string args = "-Relaunch";
                if (install) args = "-Branch " + branch + " " + args;

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
                        SetFooter(label + " terminée avec succès");
                        Log("> Terminé avec succès.");
                    }
                    else
                    {
                        SetFooter(label + " échec (code " + p.ExitCode + ")");
                        Log("> Code de sortie : " + p.ExitCode + ". Vérifiez que Discord est fermé, puis réessayez.");
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

    class LogoOption : Panel
    {
        public string Branch;
        public event Action Picked;
        readonly Image icon;
        readonly string label;
        bool selected;

        public LogoOption(string branch, string text, Image image, Point loc, Size size)
        {
            Branch = branch;
            icon = image;
            label = text;
            Location = loc;
            Size = size;
            Cursor = Cursors.Hand;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        }

        public void SetSelected(bool sel)
        {
            selected = sel;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (Picked != null) Picked();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color back = selected ? Color.FromArgb(34, 36, 44) : Card;
            Color border = selected ? Accent1 : CardBorder;

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
                using (var b = new SolidBrush(back))
                    e.Graphics.FillPath(b, path);
                using (var pen = new Pen(border, selected ? 2f : 1f))
                    e.Graphics.DrawPath(pen, path);
            }

            e.Graphics.DrawImage(icon, 18, 14, 40, 56);

            using (var fTitle = new Font("Segoe UI", 11.5f, FontStyle.Bold))
            using (var fSub = new Font("Segoe UI", 8.5f))
            {
                e.Graphics.DrawString(label, fTitle, new SolidBrush(selected ? TextMain : TextMuted), 72, 20);
                string sub = Branch == "canary" ? "Version bêta quotidienne" : "Version principale";
                e.Graphics.DrawString(sub, fSub, new SolidBrush(TextMuted), 72, 46);
            }

            if (selected)
            {
                using (var b = new SolidBrush(Accent2))
                    e.Graphics.FillEllipse(b, Width - 26, 12, 10, 10);
            }
        }
    }

    class CardPanel : Panel
    {
        public PictureBox StatusLogo;

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
