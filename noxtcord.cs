using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

static class Noxtcord
{
    [STAThread]
    static void Main()
    {
        try
        {
            string ps1 = Path.Combine(Path.GetTempPath(), "noxtcord-install.ps1");
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            using (var c = new WebClient())
                c.DownloadFile("https://github.com/NayzDevFN/NoxtiCord/raw/main/install.ps1", ps1);

            var p = Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" + ps1 + "\" -Relaunch",
                UseShellExecute = false
            });
            p.WaitForExit();

            if (p.ExitCode == 0)
                MessageBox.Show("Noxtcord installe. Discord a ete relance.", "Noxtcord",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Installation terminee avec des erreurs (code " + p.ExitCode + ").\nVérifiez que Discord est ferme.", "Noxtcord",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception e)
        {
            MessageBox.Show("Erreur : " + e.Message, "Noxtcord",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
