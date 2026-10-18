using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

static class Noxtcord
{
    const string Repo = "https://github.com/NayzDevFN/NoxtiCord/raw/main/";

    // Logo Discord (Clyde) — simple-icons, viewBox 24x24
    const string ClydePath = "M20.317 4.3698a19.7913 19.7913 0 00-4.8851-1.5152.0741.0741 0 00-.0785.0371c-.211.3753-.4447.8648-.6083 1.2495-1.8447-.2762-3.68-.2762-5.4868 0-.1636-.3933-.4058-.8742-.6177-1.2495a.077.077 0 00-.0785-.037 19.7363 19.7363 0 00-4.8852 1.515.0699.0699 0 00-.0321.0277C.5334 9.0458-.319 13.5799.0992 18.0578a.0824.0824 0 00.0312.0561c2.0528 1.5076 4.0413 2.4228 5.9929 3.0294a.0777.0777 0 00.0842-.0276c.4616-.6304.8731-1.2952 1.226-1.9942a.076.076 0 00-.0416-.1057c-.6528-.2476-1.2743-.5495-1.8722-.8923a.077.077 0 01-.0076-.1277c.1258-.0943.2517-.1923.3718-.2914a.0743.0743 0 01.0776-.0105c3.9278 1.7933 8.18 1.7933 12.0614 0a.0739.0739 0 01.0785.0095c.1202.099.246.1981.3728.2924a.077.077 0 01-.0066.1276 12.2986 12.2986 0 01-1.873.8914.0766.0766 0 00-.0407.1067c.3604.698.7719 1.3628 1.225 1.9932a.076.076 0 00.0842.0286c1.961-.6067 3.9495-1.5219 6.0023-3.0294a.077.077 0 00.0313-.0552c.5004-5.177-.8382-9.6739-3.5485-13.6604a.061.061 0 00-.0312-.0286zM8.02 15.3312c-1.1825 0-2.1569-1.0857-2.1569-2.419 0-1.3332.9555-2.4189 2.157-2.4189 1.2108 0 2.1757 1.0952 2.1568 2.419 0 1.3332-.9555 2.4189-2.1569 2.4189zm7.9748 0c-1.1825 0-2.1569-1.0857-2.1569-2.419 0-1.3332.9554-2.4189 2.1569-2.4189 1.2108 0 2.1757 1.0952 2.1568 2.419 0 1.3332-.946 2.4189-2.1568 2.4189Z";

    // Lettre « N » — logo du site (viewBox 64x64)
    const string NPath = "M19 47V17h9l9 14 9-14h9v30h-9V31l-9 13-9-13v16z";

    // ============================================================
    //  Palette IDENTIQUE au site nayzdevfn.github.io/NoxtiCord
    // ============================================================
    static readonly Color BgSite = Color.FromArgb(23, 24, 28);        // #17181c
    static readonly Color Accent1 = Color.FromArgb(88, 101, 242);     // #5865f2 indigo
    static readonly Color Accent2 = Color.FromArgb(0, 168, 252);      // #00a8fc cyan
    static readonly Color Purple = Color.FromArgb(124, 92, 255);      // #7c5cff (btn principal)
    static readonly Color Canary = Color.FromArgb(250, 166, 26);      // jaune Canary
    static readonly Color CardTop = Color.FromArgb(35, 36, 41);       // #232429 (haut de carte)
    static readonly Color Card = Color.FromArgb(27, 28, 33);          // #1b1c21 (bas de carte)
    static readonly Color CardBorder = Color.FromArgb(42, 44, 51);    // #2a2c33
    static readonly Color Surface = Color.FromArgb(30, 31, 36);       // #1e1f24
    static readonly Color Hover = Color.FromArgb(43, 45, 54);         // #2b2d36
    static readonly Color CodeBg = Color.FromArgb(16, 17, 21);        // #101115
    static readonly Color Mint = Color.FromArgb(185, 245, 233);       // #b9f5e9 (code)
    static readonly Color Green = Color.FromArgb(35, 165, 89);        // #23a559
    static readonly Color GreenTxt = Color.FromArgb(126, 224, 168);   // #7ee0a8
    static readonly Color Amber = Color.FromArgb(240, 168, 48);
    static readonly Color AmberTxt = Color.FromArgb(255, 210, 138);   // #ffd28a
    static readonly Color RedDark = Color.FromArgb(94, 32, 36);
    static readonly Color Red = Color.FromArgb(218, 55, 60);
    static readonly Color TextMain = Color.FromArgb(230, 232, 236);   // #e6e8ec
    static readonly Color TextMuted = Color.FromArgb(139, 145, 155);  // #8b919b
    static readonly Color BadgeTxt = Color.FromArgb(200, 205, 251);   // #c8cdfb
    static readonly Color NeonCyan = Color.FromArgb(0, 255, 220);     // #00ffdc
    static readonly Color NeonPurple = Color.FromArgb(170, 90, 255);  // #aa5aff

    static Image logoSite;     // logo du site (N dégradé)
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
    static NeonButton installBtn;
    static NeonButton uninstallBtn;
    static NeonButton repairBtn;
    static NeonButton folderBtn;
    static NeonButton githubBtn;
    static NeonButton closeBtn;
    static NeonButton minBtn;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        logoSite = RenderSiteLogo(64, 17);
        logoStable = RenderClyde(Accent1, 256);
        logoCanary = RenderClyde(Canary, 256);
        logoMuted = RenderClyde(Color.FromArgb(110, 116, 125), 256);

        form = new FuturisticForm
        {
            Text = "NoxtiCord Installer",
            ClientSize = new Size(880, 740),
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.None,
            MaximizeBox = false,
            BackColor = Color.FromArgb(10, 11, 14),
            Padding = new Padding(0)
        };

        // Barre de titre personnalisée (vitrée) + drag
        var titleBar = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(880, 44),
            BackColor = Color.Transparent,
            Cursor = Cursors.SizeAll
        };
        var iconPic = new PictureBox
        {
            Image = logoSite,                       // logo identique à la navbar du site
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(28, 28),
            Location = new Point(14, 8),
            BackColor = Color.Transparent,
            Cursor = Cursors.SizeAll
        };
        var titleLbl = new Label
        {
            Text = "NOXTICORD",
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Color.Transparent,
            Location = new Point(50, 6)
        };
        var tagLbl = new Label
        {
            Text = "// nayzdevfn.github.io/NoxtiCord  ·  lanceur officiel",
            Font = new Font("Consolas", 7f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(52, 27)
        };

        minBtn = MakeNeonButton("–", new Point(798, 6), new Size(30, 30),
            Surface, Hover, TextMuted, null, () => form.WindowState = FormWindowState.Minimized,
            NeonPurple, Color.FromArgb(46, 255, 255, 255));
        closeBtn = MakeNeonButton("×", new Point(838, 6), new Size(38, 32),
            RedDark, Red, TextMain, null, form.Close,
            Color.FromArgb(255, 60, 60), Color.FromArgb(120, 255, 90, 90));

        titleBar.Controls.AddRange(new Control[] { iconPic, titleLbl, tagLbl, minBtn, closeBtn });

        // Pastille « badge » du site (point vert + texte mono sur pastille indigo)
        var heroBadge = new BadgePanel
        {
            Value = "0 avertissement Windows · PowerShell recommandé",
            Location = new Point(28, 50)
        };
        heroBadge.UpdateSize();

        // Attacher le déplacement de fenêtre à la barre de titre et ses enfants textes
        Action<Control> attachDrag = c =>
        {
            c.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(form.Handle, 0xA1, 2, 0); // WM_NCLBUTTONDOWN, HTCAPTION
                }
            };
        };
        attachDrag(titleBar);
        attachDrag(iconPic);
        attachDrag(titleLbl);
        attachDrag(tagLbl);

        var neonBar = new GradientBar { Location = new Point(0, 44), Size = new Size(880, 2) };

        // Titre en dégradé comme le <h1> du site (#fff → #b9c4ff → #00ffdc)
        var title = new GradientLabel
        {
            Text = "NOXTICORD",
            Font = new Font("Segoe UI", 26f, FontStyle.Bold),
            AutoSize = true,
            BackColor = Color.Transparent,
            Location = new Point(28, 76)
        };

        // Sur-titre façon .eyebrow du site : mono, majuscules, néon
        var tag = new Label
        {
            Text = "// installation locale  ·  stable / canary / ptb  ·  100 % réversible",
            Font = new Font("Consolas", 8.5f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = NeonCyan,
            BackColor = Color.Transparent,
            Location = new Point(32, 118)
        };

        statusCard = new CardPanel { Location = new Point(24, 148), Size = new Size(832, 104) };

        var statusLogo = new PictureBox
        {
            Image = logoStable,
            SizeMode = PictureBoxSizeMode.Zoom,
            Size = new Size(52, 52),
            Location = new Point(28, 26),
            BackColor = Color.Transparent
        };

        statusTitle = new Label
        {
            Font = new Font("Segoe UI", 15f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Color.Transparent,
            Location = new Point(100, 24)
        };

        statusDetail = new Label
        {
            Font = new Font("Segoe UI", 10f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(100, 58)
        };

        statusCard.Controls.Add(statusLogo);
        statusCard.Controls.Add(statusTitle);
        statusCard.Controls.Add(statusDetail);
        statusCard.StatusLogo = statusLogo;

        var pickLabel = new Label
        {
            Text = "CHOISISSEZ VOTRE VERSION DE DISCORD",
            Font = new Font("Consolas", 8f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Color.Transparent,
            Location = new Point(28, 276)
        };

        optStable = new LogoOption("stable", "Discord", logoStable, new Point(24, 302), new Size(408, 88));
        optCanary = new LogoOption("canary", "Discord Canary", logoCanary, new Point(448, 302), new Size(408, 88));
        optStable.Picked += () => PickOption(optStable);
        optCanary.Picked += () => PickOption(optCanary);
        optStable.SetSelected(true);

        // Bouton principal : dégradé identique au .btn-main du site (#5865f2 → #7c5cff)
        installBtn = MakeNeonButton("INSTALLER", new Point(24, 412), new Size(520, 60),
            Accent1, Purple, Color.White, new Font("Segoe UI", 13f, FontStyle.Bold), RunInstall,
            NeonCyan, Color.Transparent);
        uninstallBtn = MakeNeonButton("DÉSINSTALLER", new Point(568, 412), new Size(288, 60),
            RedDark, Red, TextMain, new Font("Segoe UI", 11f, FontStyle.Bold), RunUninstall,
            Color.FromArgb(255, 60, 60), Color.FromArgb(120, 255, 90, 90));

        // Boutons secondaires : style .btn-outline du site (fond #1e1f24, bordure #2a2c33)
        repairBtn = MakeNeonButton("RÉPARER", new Point(24, 492), new Size(260, 44),
            Surface, Hover, TextMain, new Font("Segoe UI", 10f, FontStyle.Bold), RunRepair,
            NeonPurple, CardBorder);
        folderBtn = MakeNeonButton("DOSSIER", new Point(308, 492), new Size(260, 44),
            Surface, Hover, TextMain, new Font("Segoe UI", 10f, FontStyle.Bold), OpenFolder,
            NeonPurple, CardBorder);
        githubBtn = MakeNeonButton("GITHUB", new Point(596, 492), new Size(260, 44),
            Surface, Hover, TextMain, new Font("Segoe UI", 10f, FontStyle.Bold), () => Process.Start(Repo),
            NeonPurple, CardBorder);

        // Journal : bloc de code du site (fond #101115, texte #b9f5e9, bordure #2a2c33)
        var logCard = new CardPanel
        {
            Location = new Point(22, 556),
            Size = new Size(836, 132),
            Radius = 12
        };

        logBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = false,
            Font = new Font("Consolas", 9f),
            BackColor = CodeBg,
            ForeColor = Mint,
            BorderStyle = BorderStyle.None,
            Location = new Point(24, 558),
            Size = new Size(832, 128)
        };

        bar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Location = new Point(24, 696),
            Size = new Size(832, 6),
            Visible = false
        };

        footerStatus = new Label
        {
            Text = "✔ Prêt. Aucun blocage, aucun avertissement Windows.",
            Font = new Font("Consolas", 8.5f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = GreenTxt,
            BackColor = Color.Transparent,
            Location = new Point(24, 710)
        };

        form.Controls.AddRange(new Control[]
        {
            titleBar, neonBar, heroBadge, title, tag, statusCard, pickLabel, optStable, optCanary,
            installBtn, uninstallBtn, repairBtn, folderBtn, githubBtn,
            logCard, logBox, bar, footerStatus
        });

        // Rayons identiques à ceux du site (cartes 22/18, boutons 13, code 12)
        RoundControl(statusCard, 18);
        RoundControl(optStable, 16);
        RoundControl(optCanary, 16);
        RoundControl(minBtn, minBtn.Radius = 10);
        RoundControl(closeBtn, closeBtn.Radius = 10);
        RoundControl(installBtn, installBtn.Radius = 13);
        RoundControl(uninstallBtn, uninstallBtn.Radius = 13);
        RoundControl(repairBtn, repairBtn.Radius = 13);
        RoundControl(folderBtn, folderBtn.Radius = 13);
        RoundControl(githubBtn, githubBtn.Radius = 13);
        RoundControl(logCard, 12);
        RoundControl(logBox, 12);
        RoundControl(heroBadge, 10);

        RefreshStatus();
        Application.Run(form);
    }

    static void PickOption(LogoOption chosen)
    {
        selectedBranch = chosen.Branch;
        optStable.SetSelected(chosen == optStable);
        optCanary.SetSelected(chosen == optCanary);
    }

    static void RunInstall() { RunOp(true); }
    static void RunUninstall() { RunOp(false); }
    static void RunRepair() { RunOp(true); }

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

    // Logo du site : tuile arrondie dégradée #5865f2 → #00ffdc avec le « N » creusé
    static Image RenderSiteLogo(int size, int radius)
    {
        var bmp = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Clear(Color.Transparent);

            var rc = new Rectangle(0, 0, size - 1, size - 1);
            using (var tile = MakeRoundRect(rc, radius))
            {
                using (var lg = new LinearGradientBrush(rc, Accent1, NeonCyan, 45f))
                    g.FillPath(lg, tile);
            }
            float scale = size * 0.44f / 30f;
            using (var n = ParseSvgPath(NPath, scale))
            {
                var b = n.GetBounds();
                using (var mx = new Matrix())
                {
                    mx.Translate((size - b.Width) / 2f - b.X, (size - b.Height) / 2f - b.Y);
                    n.Transform(mx);
                }
                using (var fill = new SolidBrush(BgSite))
                    g.FillPath(fill, n);
            }
        }
        return bmp;
    }

    // Rectangle arrondi générique (coins 90°)
    static GraphicsPath MakeRoundRect(Rectangle rc, int r)
    {
        var path = new GraphicsPath();
        if (rc.Width <= 1 || rc.Height <= 1) { path.AddRectangle(rc); return path; }
        int d = r * 2;
        if (d > rc.Width) d = rc.Width;
        if (d > rc.Height) d = rc.Height;
        path.AddArc(rc.X, rc.Y, d, d, 180, 90);
        path.AddArc(rc.Right - d, rc.Y, d, d, 270, 90);
        path.AddArc(rc.Right - d, rc.Bottom - d, d, d, 0, 90);
        path.AddArc(rc.X, rc.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
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
        double vx = (-x1p - cxp) / rx, vy = (-x1p - cyp) / ry;
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

    static NeonButton MakeNeonButton(string text, Point loc, Size size, Color back, Color hover, Color fore, Font font, Action onClick, Color glow, Color border)
    {
        var b = new NeonButton
        {
            Text = text,
            Location = loc,
            Size = size,
            ColorA = back,
            ColorB = hover,
            Fore = fore,
            Font = font ?? new Font("Segoe UI", 10f, FontStyle.Bold),
            Glow = glow,
            Border = border,
            Cursor = Cursors.Hand,
            ClickHandler = onClick
        };
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
            statusCard.Success = true;
            statusTitle.Text = "Noxtcord est installé";
            statusTitle.ForeColor = GreenTxt;
            statusDetail.Text = "Actif sur : " + string.Join(", ", patched.ToArray()) + " · Discord a été relancé.";
        }
        else if (payload)
        {
            statusCard.StatusLogo.Image = logoMuted;
            statusCard.Success = false;
            statusTitle.Text = "Fichiers présents, patch non détecté";
            statusTitle.ForeColor = AmberTxt;
            statusDetail.Text = "Relancez l'installation puis redémarrez Discord.";
        }
        else
        {
            statusCard.StatusLogo.Image = logoMuted;
            statusCard.Success = false;
            statusTitle.Text = "Noxtcord n'est pas installé";
            statusTitle.ForeColor = TextMuted;
            statusDetail.Text = "Choisissez une version puis cliquez sur INSTALLER · lancement direct.";
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
        // Aucun MessageBox ni avertissement : lancement direct, comme demandé.
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

    // Titre en dégradé (équivalent WinForms du <h1> du site)
    class GradientLabel : Label
    {
        public GradientLabel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(Text)) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            var sz = g.MeasureString(Text, Font);
            var rc = new RectangleF(0f, 0f, sz.Width + 2f, sz.Height);
            using (var lg = new LinearGradientBrush(rc, Color.White, Color.White, 96f))
            {
                lg.InterpolationColors = new ColorBlend(3)
                {
                    Positions = new[] { 0f, 0.45f, 1f },
                    Colors = new[] { Color.White, Color.FromArgb(185, 196, 255), NeonCyan }
                };
                g.DrawString(Text, Font, lg, 0f, 0f);
            }
        }
    }

    // Pastille .badge du site : point vert + texte mono sur pastille indigo
    class BadgePanel : Control
    {
        public string Value = "";
        readonly Font f = new Font("Consolas", 8f);

        public BadgePanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(280, 20);
        }

        public void UpdateSize()
        {
            int w;
            using (var g = CreateGraphics())
                w = (int)Math.Ceiling(g.MeasureString(Value, f).Width);
            Size = new Size(w + 36, 20);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = MakeRoundRect(rc, rc.Height / 2))
            {
                using (var b = new SolidBrush(Color.FromArgb(33, 88, 101, 242)))   // rgba(88,101,242,.13)
                    g.FillPath(b, path);
                using (var p = new Pen(Color.FromArgb(102, 88, 101, 242), 1f))      // rgba(88,101,242,.4)
                    g.DrawPath(p, path);
            }
            using (var b = new SolidBrush(Green))
                g.FillEllipse(b, 11, (Height - 7) / 2, 7, 7);
            using (var b = new SolidBrush(BadgeTxt))
                g.DrawString(Value, f, b, 24f, (Height - f.GetHeight(g)) / 2f);
        }
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
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                cp.ClassStyle |= 0x00020000; // CS_DROPSHADOW
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            int w = ClientSize.Width, h = ClientSize.Height;

            // Fond #17181c (couleur de page du site)
            using (var b = new SolidBrush(BgSite))
                g.FillRectangle(b, 0, 0, w, h);

            // Grille 64 px du site (blanc 3 %) estompée par un masque radial
            g.SmoothingMode = SmoothingMode.None;
            int step = 64;
            for (int x = 0; x <= w; x += step)
            {
                for (int seg = 0; seg < h; seg += step)
                {
                    int a = GridAlpha(x, seg + step / 2f, w, h);
                    if (a <= 0) continue;
                    using (var pen = new Pen(Color.FromArgb(a, 255, 255, 255), 1f))
                        g.DrawLine(pen, x, seg, x, Math.Min(seg + step, h));
                }
            }
            for (int y = 0; y <= h; y += step)
            {
                for (int seg = 0; seg < w; seg += step)
                {
                    int a = GridAlpha(seg + step / 2f, y, w, h);
                    if (a <= 0) continue;
                    using (var pen = new Pen(Color.FromArgb(a, 255, 255, 255), 1f))
                        g.DrawLine(pen, seg, y, Math.Min(seg + step, w), y);
                }
            }

            // Halos du site : blurple en haut, violets à gauche, cyan à droite, néon en bas
            g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawAmbientGlow(g, new PointF(w * 0.50f, -h * 0.06f), 470f, Color.FromArgb(82, 88, 101, 242));
            DrawAmbientGlow(g, new PointF(w * 0.04f, h * 0.32f), 300f, Color.FromArgb(46, 170, 90, 255));
            DrawAmbientGlow(g, new PointF(w * 0.97f, h * 0.30f), 300f, Color.FromArgb(38, 0, 168, 252));
            DrawAmbientGlow(g, new PointF(w * 0.50f, h * 1.08f), 520f, Color.FromArgb(26, 0, 255, 220));
        }

        // Opacité de la grille : radial centré en haut (équivalent du mask-image du site)
        static int GridAlpha(float x, float y, int w, int h)
        {
            float dx = (x - w * 0.5f) / (w * 0.62f);
            float dy = (y - h * 0.05f) / (h * 0.78f);
            float d = (float)Math.Sqrt(dx * dx + dy * dy);
            if (d >= 1f) return 0;
            return (int)(9f * (1f - d));
        }

        void DrawAmbientGlow(Graphics g, PointF center, float radius, Color color)
        {
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = color;
                    brush.SurroundColors = new Color[] { Color.FromArgb(0, color.R, color.G, color.B) };
                    g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
                }
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
            var rc = new Rectangle(0, 0, Width, Height);
            using (var lg = new LinearGradientBrush(rc, Color.Transparent, Color.Transparent, LinearGradientMode.Horizontal))
            {
                // liseré cyan du site (transparent → #00ffdc → transparent)
                lg.InterpolationColors = new ColorBlend(3)
                {
                    Positions = new[] { 0f, 0.5f, 1f },
                    Colors = new[] { Color.FromArgb(0, 0, 255, 220), Color.FromArgb(150, 0, 255, 220), Color.FromArgb(0, 0, 255, 220) }
                };
                e.Graphics.FillRectangle(lg, rc);
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

            Color glow = selected ? Accent1 : Color.Transparent;

            int r = 16;   // doit correspondre au RoundControl()
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = MakeRoundRect(rc, r))
            {
                // fond dégradé des cartes du site : #232429 → #1b1c21
                using (var lg = new LinearGradientBrush(rc, CardTop, Card, LinearGradientMode.Vertical))
                    e.Graphics.FillPath(lg, path);

                // halo bleu quand sélectionné
                if (selected)
                {
                    using (var gb = new PathGradientBrush(path))
                    {
                        gb.CenterColor = Color.FromArgb(48, glow.R, glow.G, glow.B);
                        gb.SurroundColors = new[] { Color.FromArgb(0, glow.R, glow.G, glow.B) };
                        e.Graphics.FillPath(gb, path);
                    }
                }

                // liseré cyan du haut (.card::before)
                if (Width - 1 > 2 * r + 20)
                {
                    var lr = new Rectangle(r, 0, Width - 1 - 2 * r, 1);
                    using (var lgr = new LinearGradientBrush(lr, Color.Transparent, Color.Transparent, LinearGradientMode.Horizontal))
                    {
                        lgr.InterpolationColors = new ColorBlend(3)
                        {
                            Positions = new[] { 0f, 0.5f, 1f },
                            Colors = new[] { Color.FromArgb(0, 0, 255, 220), Color.FromArgb(175, 0, 255, 220), Color.FromArgb(0, 0, 255, 220) }
                        };
                        e.Graphics.FillRectangle(lgr, lr);
                    }
                }

                using (var pen = new Pen(selected ? Accent1 : CardBorder, selected ? 1.6f : 1.2f))
                    e.Graphics.DrawPath(pen, path);
            }

            int iconeSize = selected ? 52 : 44;
            int ix = (Width - iconeSize) / 2 - 50;
            int iy = (Height - iconeSize) / 2;
            e.Graphics.DrawImage(icon, ix, iy, iconeSize, iconeSize);

            using (var fTitle = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (var fSub = new Font("Segoe UI", 8.5f))
            using (var bTitle = new SolidBrush(selected ? TextMain : TextMuted))
            using (var bSub = new SolidBrush(TextMuted))
            {
                int tx = ix + iconeSize + 16;
                e.Graphics.DrawString(label, fTitle, bTitle, tx, iy + 4);
                string sub = Branch == "canary" ? "Version bêta quotidienne (néon)" : "Version stable principale";
                e.Graphics.DrawString(sub, fSub, bSub, tx, iy + iconeSize - 16);
            }

            if (selected)
            {
                using (var b = new SolidBrush(Accent2))
                    e.Graphics.FillEllipse(b, Width - 22, 14, 12, 12);
                using (var p = new Pen(Color.White, 1.5f))
                    e.Graphics.DrawEllipse(p, Width - 22, 14, 12, 12);
            }
        }
    }

    class CardPanel : Panel
    {
        public PictureBox StatusLogo;
        public int Radius = 18;      // doit correspondre au RoundControl()
        public bool Success;         // carte « installé » (.ok-box du site)

        public CardPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int r = Radius;
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = MakeRoundRect(rc, r))
            {
                // Fond dégradé des cartes du site : #232429 → #1b1c21
                using (var lg = new LinearGradientBrush(rc, CardTop, Card, LinearGradientMode.Vertical))
                    e.Graphics.FillPath(lg, path);

                // Halo vert quand c'est installé (carte .ok-box du site)
                if (Success)
                {
                    using (var gb = new PathGradientBrush(path))
                    {
                        gb.CenterColor = Color.FromArgb(40, 35, 165, 89);
                        gb.SurroundColors = new[] { Color.FromArgb(0, 35, 165, 89) };
                        e.Graphics.FillPath(gb, path);
                    }
                }

                // Liseré cyan du haut (.card::before)
                if (Width - 1 > 2 * r + 20)
                {
                    var lr = new Rectangle(r, 0, Width - 1 - 2 * r, 1);
                    using (var lg = new LinearGradientBrush(lr, Color.Transparent, Color.Transparent, LinearGradientMode.Horizontal))
                    {
                        lg.InterpolationColors = new ColorBlend(3)
                        {
                            Positions = new[] { 0f, 0.5f, 1f },
                            Colors = new[] { Color.FromArgb(0, 0, 255, 220), Color.FromArgb(175, 0, 255, 220), Color.FromArgb(0, 0, 255, 220) }
                        };
                        e.Graphics.FillRectangle(lg, lr);
                    }
                }

                // Bordure #2a2c33 (verte quand c'est installé)
                Color bc = Success ? Color.FromArgb(96, 35, 165, 89) : CardBorder;
                using (var pen = new Pen(bc, Success ? 1.4f : 1.2f))
                    e.Graphics.DrawPath(pen, path);
            }
        }
    }

    // ---------- Bouton néon avec effet de verre et shimmer animé ----------
    class NeonButton : Control
    {
        static readonly Timer shimmerTimer = new Timer { Interval = 22 };
        static float shimmerPos = -120f;
        static readonly List<NeonButton> instances = new List<NeonButton>();

        public string Text;
        public Color ColorA;
        public Color ColorB;
        public Color Fore = TextMain;
        public Font Font;
        public Color Glow = NeonCyan;
        public Color Border = Color.Transparent;   // bordure du .btn du site
        public int Radius = 10;                    // doit correspondre au RoundControl()
        public Action ClickHandler;

        bool hovered;
        bool pressed;

        public NeonButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            instances.Add(this);
            if (instances.Count == 1)
            {
                shimmerTimer.Tick += (s, e) =>
                {
                    shimmerPos += 2.6f;
                    if (shimmerPos > 520f) shimmerPos = -160f;
                    foreach (var inst in instances)
                        if (inst.hovered) inst.Invalidate();
                };
                shimmerTimer.Start();
            }
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); hovered = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); hovered = false; pressed = false; Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) { pressed = true; Invalidate(); } }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); pressed = false; Invalidate(); }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Action handler = ClickHandler;
            if (handler != null) handler();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int r = Radius;
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);

            // Halo néon quand survolé
            if (hovered)
            {
                using (var glowPath = new GraphicsPath())
                {
                    int gd = (r + 14) * 2;
                    var grc = new Rectangle(-14, -14, Width + 26, Height + 26);
                    glowPath.AddArc(grc.X, grc.Y, gd, gd, 180, 90);
                    glowPath.AddArc(grc.Right - gd, grc.Y, gd, gd, 270, 90);
                    glowPath.AddArc(grc.Right - gd, grc.Bottom - gd, gd, gd, 0, 90);
                    glowPath.AddArc(grc.X, grc.Bottom - gd, gd, gd, 90, 90);
                    glowPath.CloseFigure();
                    using (var gb = new PathGradientBrush(glowPath))
                    {
                        byte a = pressed ? (byte)45 : (byte)70;
                        gb.CenterColor = Color.FromArgb(a, Glow.R, Glow.G, Glow.B);
                        gb.SurroundColors = new Color[] { Color.FromArgb(0, Glow.R, Glow.G, Glow.B) };
                        g.FillPath(gb, glowPath);
                    }
                }
            }

            // Dégradé du bouton
            Color start = ColorA, end = ColorB;
            if (pressed) { start = Darken(start, 0.35f); end = Darken(end, 0.35f); }
            using (var lg = new LinearGradientBrush(rc, start, end, LinearGradientMode.Vertical))
            {
                lg.WrapMode = WrapMode.TileFlipXY;
                using (var path = RoundPath(rc, r))
                    g.FillPath(lg, path);
            }

            // Shimmer animé (bande lumineuse diagonale) quand survolé
            if (hovered)
            {
                using (var shimmerPath = new GraphicsPath())
                {
                    float sx = shimmerPos;
                    shimmerPath.AddLine(sx, -60, sx + 260, Height + 60);
                    shimmerPath.AddLine(sx + 260 + 40, Height + 60, sx + 40, -60);
                    shimmerPath.CloseFigure();
                    using (var sb = new SolidBrush(Color.FromArgb(70, 255, 255, 255)))
                        g.FillPath(sb, shimmerPath);
                }
            }

            // Bordure (transparente sur le bouton principal, comme le .btn-main du site)
            if (Border.A > 0)
            {
                using (var pen = new Pen(Border, pressed ? 1.6f : 1.1f))
                using (var path = RoundPath(rc, r))
                    g.DrawPath(pen, path);
            }

            // Texte centré
            // PIÈGE : « Font » est un champ partagé par TOUTES les peintures du bouton.
            // L'envelopper dans un using le détruisait à la fin de la première peinture :
            // toutes les suivantes levaient System.ArgumentException « Le paramètre n'est
            // pas valide. » dans Graphics.MeasureString (boutons restés blancs).
            using (var brush = new SolidBrush(Fore))
            {
                System.Drawing.Font f = Font ?? new Font("Segoe UI", 10f, FontStyle.Bold);
                string label = Text ?? "";
                if (label.Length > 0)
                {
                    var size = g.MeasureString(label, f);
                    float x = (Width - size.Width) / 2f;
                    float y = (Height - size.Height) / 2f;
                    g.DrawString(label, f, brush, x, y);
                }
            }
        }

        static GraphicsPath RoundPath(Rectangle rc, int r)
        {
            var path = new GraphicsPath();
            int d = r * 2;
            path.AddArc(rc.X, rc.Y, d, d, 180, 90);
            path.AddArc(rc.Right - d, rc.Y, d, d, 270, 90);
            path.AddArc(rc.Right - d, rc.Bottom - d, d, d, 0, 90);
            path.AddArc(rc.X, rc.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        static Color Darken(Color c, float factor)
        {
            return Color.FromArgb(c.A, (int)(c.R * factor), (int)(c.G * factor), (int)(c.B * factor));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                instances.Remove(this);
                if (instances.Count == 0) shimmerTimer.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Déplacement de fenêtre sans bordure (P/Invoke user32)
    [DllImport("user32.dll")]
    static extern bool ReleaseCapture();
    [DllImport("user32.dll")]
    static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
}
