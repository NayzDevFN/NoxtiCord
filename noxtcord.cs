using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

static class Noxtcord
{
    const string Repo = "https://github.com/NayzDevFN/NoxtiCord/raw/main/";

    static readonly Color Bg = Color.FromArgb(30, 31, 34);
    static readonly Color Surface = Color.FromArgb(43, 45, 49);
    static readonly Color Hover = Color.FromArgb(53, 55, 60);
    static readonly Color TextMain = Color.FromArgb(219, 222, 225);
    static readonly Color TextMuted = Color.FromArgb(181, 186, 193);

    static TextBox logBox;
    static ProgressBar bar;
    static Label statusLabel;
    static Button[] actionButtons;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var form = new Form
        {
            Text = "NoxtiCord",
            Size = new Size(600, 480),
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.FixedSingle,
            MaximizeBox = false,
            BackColor = Bg
        };

        var title = new Label
        {
            Text = "NoxtiCord",
            Font = new Font("Segoe UI", 16f, FontStyle.Bold),
            AutoSize = true,
            ForeColor = TextMain,
            BackColor = Bg,
            Location = new Point(16, 12)
        };

        var subtitle = new Label
        {
            Text = "Choisissez une action :",
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Bg,
            Location = new Point(18, 48)
        };

        var btnInstallAll = MakeButton("Installer (toutes les versions detectees)", 16, 74, 280, () => RunScript("install.ps1", "-Relaunch", "Installation de toutes les versions"));
        var btnStable = MakeButton("Installer sur Discord Stable", 16, 108, 280, () => RunScript("install.ps1", "-Branch stable -Relaunch", "Installation de Discord Stable"));
        var btnCanary = MakeButton("Installer sur Discord Canary", 16, 142, 280, () => RunScript("install.ps1", "-Branch canary -Relaunch", "Installation de Discord Canary"));
        var btnPtb = MakeButton("Installer sur Discord PTB", 16, 176, 280, () => RunScript("install.ps1", "-Branch ptb -Relaunch", "Installation de Discord PTB"));
        var btnUninstall = MakeButton("Desinstaller (restaure Discord)", 310, 74, 260, () => RunScript("uninstall.ps1", "-Relaunch", "Desinstallation"));
        actionButtons = new[] { btnInstallAll, btnStable, btnCanary, btnPtb, btnUninstall };

        statusLabel = new Label
        {
            Text = "Pret.",
            Font = new Font("Segoe UI", 9f),
            AutoSize = true,
            ForeColor = TextMuted,
            BackColor = Bg,
            Location = new Point(16, 214)
        };

        logBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = false,
            Location = new Point(16, 236),
            Size = new Size(554, 150),
            Font = new Font("Consolas", 9f),
            BackColor = Surface,
            ForeColor = TextMain,
            BorderStyle = BorderStyle.FixedSingle
        };

        bar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Location = new Point(16, 394),
            Size = new Size(554, 18),
            Visible = false
        };

        var closeBtn = MakeButton("Fermer", 16, 418, 100, form.Close);

        form.Controls.AddRange(new Control[]
        {
            title, subtitle, btnInstallAll, btnStable, btnCanary, btnPtb, btnUninstall,
            statusLabel, logBox, bar, closeBtn
        });

        Application.Run(form);
    }

    static Button MakeButton(string text, int x, int y, int w, Action onClick)
    {
        var b = new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(w, y >= 400 ? 32 : 28),
            FlatStyle = FlatStyle.Flat,
            BackColor = Surface,
            ForeColor = TextMain,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 8.5f)
        };
        b.FlatAppearance.BorderColor = Bg;
        b.FlatAppearance.MouseOverBackColor = Hover;
        b.FlatAppearance.MouseDownBackColor = Hover;
        b.Click += (s, e) => onClick();
        return b;
    }

    static void Log(string msg)
    {
        if (logBox.InvokeRequired) { logBox.Invoke((Action)(() => Log(msg))); return; }
        logBox.AppendText(msg + Environment.NewLine);
    }

    static void SetStatus(string text)
    {
        if (statusLabel.InvokeRequired) { statusLabel.Invoke((Action)(() => SetStatus(text))); return; }
        statusLabel.Text = text;
    }

    static void SetBusy(bool busy)
    {
        if (logBox.InvokeRequired) { logBox.Invoke((Action)(() => SetBusy(busy))); return; }
        foreach (var b in actionButtons) b.Enabled = !busy;
        bar.Visible = busy;
    }

    static void RunScript(string script, string args, string label)
    {
        SetBusy(true);
        logBox.Clear();
        SetStatus(label + " en cours...");

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

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" + local + "\"" + (args.Length > 0 ? " " + args : ""),
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
                        SetStatus(label + " termine avec succes.");
                        Log("Termine avec succes.");
                    }
                    else
                    {
                        SetStatus(label + " terminee avec des erreurs (code " + p.ExitCode + ").");
                        Log("Code de sortie : " + p.ExitCode + ". Verifiez que Discord est ferme, puis reessayez.");
                    }
                }
            }
            catch (Exception ex)
            {
                SetStatus("Erreur.");
                Log("Erreur : " + ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        });
    }
}
