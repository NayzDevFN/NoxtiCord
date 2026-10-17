using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

static class Noxtcord
{
    const string Repo = "https://github.com/NayzDevFN/NoxtiCord/raw/main/";

    // Logo Discord (Clyde) — simple-icons, viewBox 24x24
    const string ClydePath = "M20.317 4.3698a19.7913 19.7913 0 00-4.8851-1.5152.0741.0741 0 00-.0785.0371c-.211.3753-.4447.8648-.6083 1.2495-1.8447-.2762-3.68-.2762-5.4868 0-.1636-.3933-.4058-.8742-.6177-1.2495a.077.077 0 00-.0785-.037 19.7363 19.7363 0 00-4.8852 1.515.0699.0699 0 00-.0321.0277C.5334 9.0458-.319 13.5799.0992 18.0578a.0824.0824 0 00.0312.0561c2.0528 1.5076 4.0413 2.4228 5.9929 3.0294a.0777.0777 0 00.0842-.0276c.4616-.6304.8731-1.2952 1.226-1.9942a.076.076 0 00-.0416-.1057c-.6528-.2476-1.2743-.5495-1.8722-.8923a.077.077 0 01-.0076-.1277c.1258-.0943.2517-.1923.3718-.2914a.0743.0743 0 01.0776-.0105c3.9278 1.7933 8.18 1.7933 12.0614 0a.0739.0739 0 01.0785.0095c.1202.099.246.1981.3728.2924a.077.077 0 01-.0066.1276 12.2986 12.2986 0 01-1.873.8914.0766.0766 0 00-.0407.1067c.3604.698.7719 1.3628 1.225 1.9932a.076.076 0 00.0842.0286c1.961-.6067 3.9495-1.5219 6.0023-3.0294a.077.077 0 00.0313-.0552c.5004-5.177-.8382-9.6739-3.5485-13.6604a.061.061 0 00-.0312-.0286zM8.02 15.3312c-1.1825 0-2.1569-1.0857-2.1569-2.419 0-1.3332.9555-2.4189 2.157-2.4189 1.2108 0 2.1757 1.0952 2.1568 2.419 0 1.3332-.9555 2.4189-2.1569 2.4189zm7.9748 0c-1.1825 0-2.1569-1.0857-2.1569-2.419 0-1.3332.9554-2.4189 2.1569-2.4189 1.2108 0 2.1757 1.0952 2.1568 2.419 0 1.3332-.946 2.4189-2.1568 2.4189Z";

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

        logoStable = RenderClyde(Accent1, 256);
        logoCanary = RenderClyde(Canary, 256);
        logoMuted = RenderClyde(Color.FromArgb(110, 116, 125), 256);

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
            Size = new Size(48, 48),
            Location = new Point(24, 26),
            BackColor = Color.Transparent
        };

        statusTitle = new Label
        {
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Color.Transparent,
            Location = new Point(88, 24)
        };

        statusDetail = new Label
        {
            Font = new Font("Segoe UI", 9.5f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(88, 58)
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

    static Image RenderClyde(Color color, int size)
    {
        GraphicsPath gp = ParseSvgPath(ClydePath, size / 24f);
        var bmp = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Clear(Color.Transparent);
            using (var b = new SolidBrush(color))
                g.FillPath(b, gp);
        }
        gp.Dispose();
        return bmp;
    }

    class SvgScanner
    {
        readonly string s;
        int i;

        public SvgScanner(string text) { s = text; }

        public bool HasNumber()
        {
            int j = i;
            while (j < s.Length && (s[j] == ' ' || s[j] == ',')) j++;
            if (j >= s.Length) return false;
            char c = s[j];
            return c == '-' || c == '+' || c == '.' || char.IsDigit(c);
        }

        public float NextFloat()
        {
            while (i < s.Length && (s[i] == ' ' || s[i] == ',')) i++;
            int start = i;
            if (i < s.Length && (s[i] == '-' || s[i] == '+')) i++;
            bool dot = false;
            while (i < s.Length && (char.IsDigit(s[i]) || (s[i] == '.' && !dot)))
            {
                if (s[i] == '.') dot = true;
                i++;
            }
            return float.Parse(s.Substring(start, i - start), CultureInfo.InvariantCulture);
        }

        public int NextFlag()
        {
            while (i < s.Length && (s[i] == ' ' || s[i] == ',')) i++;
            char c = s[i];
            i++;
            return c == '1' ? 1 : 0;
        }

        public bool HasCommand()
        {
            int j = i;
            while (j < s.Length && (s[j] == ' ' || s[j] == ',')) j++;
            return j < s.Length && char.IsLetter(s[j]);
        }

        public char NextCommand()
        {
            while (i < s.Length && (s[i] == ' ' || s[i] == ',')) i++;
            char c = s[i];
            i++;
            return c;
        }
    }

    static GraphicsPath ParseSvgPath(string d, float scale)
    {
        var gp = new GraphicsPath();
        gp.FillMode = FillMode.Winding;
        var sc = new SvgScanner(d);

        float cx = 0, cy = 0, sx = 0, sy = 0;
        char cmd = ' ';

        while (true)
        {
            if (sc.HasNumber())
            {
                if (cmd == ' ') break;
            }
            else
            {
                if (!sc.HasCommand()) break;
                cmd = sc.NextCommand();
            }

            bool rel = char.IsLower(cmd);
            char u = char.ToUpperInvariant(cmd);

            if (u == 'M')
            {
                float x = sc.NextFloat() * scale, y = sc.NextFloat() * scale;
                if (rel) { x += cx; y += cy; }
                cx = x; cy = y; sx = x; sy = y;
                gp.StartFigure();
                cmd = rel ? 'l' : 'L';
            }
            else if (u == 'L')
            {
                while (sc.HasNumber())
                {
                    float x = sc.NextFloat() * scale, y = sc.NextFloat() * scale;
                    if (rel) { x += cx; y += cy; }
                    gp.AddLine(cx, cy, x, y);
                    cx = x; cy = y;
                }
            }
            else if (u == 'H')
            {
                while (sc.HasNumber())
                {
                    float x = sc.NextFloat() * scale;
                    if (rel) x += cx;
                    gp.AddLine(cx, cy, x, cy);
                    cx = x;
                }
            }
            else if (u == 'V')
            {
                while (sc.HasNumber())
                {
                    float y = sc.NextFloat() * scale;
                    if (rel) y += cy;
                    gp.AddLine(cx, cy, cx, y);
                    cy = y;
                }
            }
            else if (u == 'C')
            {
                while (sc.HasNumber())
                {
                    float x1 = sc.NextFloat() * scale, y1 = sc.NextFloat() * scale;
                    float x2 = sc.NextFloat() * scale, y2 = sc.NextFloat() * scale;
                    float x = sc.NextFloat() * scale, y = sc.NextFloat() * scale;
                    if (rel) { x1 += cx; y1 += cy; x2 += cx; y2 += cy; x += cx; y += cy; }
                    gp.AddBezier(cx, cy, x1, y1, x2, y2, x, y);
                    cx = x; cy = y;
                }
            }
            else if (u == 'A')
            {
                while (sc.HasNumber())
                {
                    float rx = sc.NextFloat(), ry = sc.NextFloat(), rot = sc.NextFloat();
                    int laf = sc.NextFlag(), sf = sc.NextFlag();
                    float x = sc.NextFloat(), y = sc.NextFloat();
                    if (rel) { x += cx / scale; y += cy / scale; }
                    AddSvgArc(gp, cx / scale, cy / scale, rx, ry, rot, laf == 1, sf == 1, x, y, scale);
                    cx = x * scale; cy = y * scale;
                }
            }
            else if (u == 'Z')
            {
                gp.CloseFigure();
                cx = sx; cy = sy;
            }
            else
            {
                break;
            }
        }
        return gp;
    }

    static void AddSvgArc(GraphicsPath gp, float x1, float y1, float rx, float ry, float rotDeg, bool large, bool sweep, float x2, float y2, float scale)
    {
        x1 *= scale; y1 *= scale; x2 *= scale; y2 *= scale;
        rx *= scale; ry *= scale;
        if (rx <= 0 || ry <= 0) { gp.AddLine(x1, y1, x2, y2); return; }

        double phi = rotDeg * Math.PI / 180.0;
        double cosP = Math.Cos(phi), sinP = Math.Sin(phi);
        double dx2 = (x1 - x2) / 2.0, dy2 = (y1 - y2) / 2.0;
        double x1p = cosP * dx2 + sinP * dy2;
        double y1p = -sinP * dx2 + cosP * dy2;
        double rx2 = rx * rx, ry2 = ry * ry;
        double x1p2 = x1p * x1p, y1p2 = y1p * y1p;
        double lambda = x1p2 / rx2 + y1p2 / ry2;
        if (lambda > 1) { double s = Math.Sqrt(lambda); rx = (float)(rx * s); ry = (float)(ry * s); rx2 = rx * rx; ry2 = ry * ry; }
        double num = rx2 * ry2 - rx2 * y1p2 - ry2 * x1p2;
        double den = rx2 * y1p2 + ry2 * x1p2;
        double co = den == 0 ? 0 : Math.Sqrt(Math.Max(0, num / den));
        if (large == sweep) co = -co;
        double cxp = co * rx * y1p / ry;
        double cyp = -co * ry * x1p / rx;
        double cx = cosP * cxp - sinP * cyp + (x1 + x2) / 2.0;
        double cy = sinP * cxp + cosP * cyp + (y1 + y2) / 2.0;

        double ux = (x1p - cxp) / rx, uy = (y1p - cyp) / ry;
        double vx = (-x1p - cxp) / rx, vy = (-y1p - cyp) / ry;
        double theta1 = SvgAngle(1, 0, ux, uy);
        double dtheta = SvgAngle(ux, uy, vx, vy);
        if (!sweep && dtheta > 0) dtheta -= 2 * Math.PI;
        else if (sweep && dtheta < 0) dtheta += 2 * Math.PI;

        int segs = (int)Math.Ceiling(Math.Abs(dtheta / (Math.PI / 2.0)));
        if (segs < 1) segs = 1;
        double delta = dtheta / segs;
        double k = 4.0 / 3.0 * Math.Tan(delta / 4.0);
        double theta = theta1;

        for (int s = 0; s < segs; s++)
        {
            double th2 = theta + delta;
            double cos1 = Math.Cos(theta), sin1 = Math.Sin(theta);
            double cos2 = Math.Cos(th2), sin2 = Math.Sin(th2);
            double e1x = cosP * rx * cos1 - sinP * ry * sin1 + cx;
            double e1y = sinP * rx * cos1 + cosP * ry * sin1 + cy;
            double e2x = cosP * rx * cos2 - sinP * ry * sin2 + cx;
            double e2y = sinP * rx * cos2 + cosP * ry * sin2 + cy;
            double d1x = -cosP * rx * sin1 - sinP * ry * cos1;
            double d1y = -sinP * rx * sin1 + cosP * ry * cos1;
            double d2x = -cosP * rx * sin2 - sinP * ry * cos2;
            double d2y = -sinP * rx * sin2 + cosP * ry * cos2;
            gp.AddBezier(
                (float)e1x, (float)e1y,
                (float)(e1x + k * d1x), (float)(e1y + k * d1y),
                (float)(e2x - k * d2x), (float)(e2y - k * d2y),
                (float)e2x, (float)e2y);
            theta = th2;
        }
    }

    static double SvgAngle(double ux, double uy, double vx, double vy)
    {
        double dot = ux * vx + uy * vy;
        double len = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
        double a = Math.Acos(Math.Max(-1.0, Math.Min(1.0, len == 0 ? 0 : dot / len)));
        if (ux * vy - uy * vx < 0) a = -a;
        return a;
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

            e.Graphics.DrawImage(icon, 20, 18, 48, 48);

            using (var fTitle = new Font("Segoe UI", 11.5f, FontStyle.Bold))
            using (var fSub = new Font("Segoe UI", 8.5f))
            {
                e.Graphics.DrawString(label, fTitle, new SolidBrush(selected ? TextMain : TextMuted), 82, 20);
                string sub = Branch == "canary" ? "Version bêta quotidienne" : "Version principale";
                e.Graphics.DrawString(sub, fSub, new SolidBrush(TextMuted), 82, 46);
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
