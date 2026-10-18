# Noxticord Installer GUI — Portable, aucune installation requise
# Lance avec : powershell.exe -ExecutionPolicy Bypass -File installer-gui.ps1
# ou double-clic sur Noxticord Installer.bat

Add-Type -AssemblyName System.Windows.Forms, System.Drawing

# ---------- Palette futuriste ----------
$Accent1 = [System.Drawing.Color]::FromArgb(88,101,242)   # indigo
$Accent2 = [System.Drawing.Color]::FromArgb(0,168,252)    # cyan
$Canary  = [System.Drawing.Color]::FromArgb(250,166,26)   # jaune
$Card    = [System.Drawing.Color]::FromArgb(23,24,28)
$Surface = [System.Drawing.Color]::FromArgb(30,31,36)
$Hover   = [System.Drawing.Color]::FromArgb(43,45,54)
$Green   = [System.Drawing.Color]::FromArgb(35,165,89)
$Amber   = [System.Drawing.Color]::FromArgb(240,168,48)
$RedDark = [System.Drawing.Color]::FromArgb(94,32,36)
$Red     = [System.Drawing.Color]::FromArgb(218,55,60)
$TextMain= [System.Drawing.Color]::FromArgb(230,232,236)
$TextMuted= [System.Drawing.Color]::FromArgb(128,134,143)
$Glass   = [System.Drawing.Color]::FromArgb(110,16,18,24)

$Repo  = "https://github.com/NayzDevFN/NoxtiCord/raw/main/"
$Script = "install.ps1"
$Branch = "stable"

# ---------- Form ----------
$form = New-Object System.Windows.Forms.Form
$form.Text = "NoxtiCord Installer"
$form.Size = New-Object System.Drawing.Size(820,680)
$form.StartPosition = "CenterScreen"
$form.FormBorderStyle = "None"
$form.BackColor = [System.Drawing.Color]::FromArgb(10,11,14)
$form.DoubleBuffered = $true
$form.Padding = New-Object System.Windows.Forms.Padding(0)

# Ombre portée (light)
$form.Add_Shown({
    $form.Refresh()
})

# ---------- Barre de titre personnalisée ----------
$titleBar = New-Object System.Windows.Forms.Panel
$titleBar.Location = New-Object System.Drawing.Point(0,0)
$titleBar.Size = New-Object System.Drawing.Size(820,44)
$titleBar.BackColor = [System.Drawing.Color]::Transparent
$titleBar.Cursor = [System.Windows.Forms.Cursors]::SizeAll

$iconPic = New-Object System.Windows.Forms.PictureBox
$iconPic.Size = New-Object System.Drawing.Size(24,24)
$iconPic.Location = New-Object System.Drawing.Point(16,10)
$iconPic.BackColor = [System.Drawing.Color]::Transparent
$iconPic.Cursor = [System.Windows.Forms.Cursors]::SizeAll

# Logo Clyde simplifié (cercle indigo) si pas d'image externe
$iconPic.BackColor = $Accent1
# On dessine un petit cercle comme placeholder de logo
$iconPic.Paint.Add({
    param($sender,$e)
    $e.Graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $brush = New-Object System.Drawing.SolidBrush($Accent1)
    $e.Graphics.FillEllipse($brush, 2,2,20,20)
    $brush.Dispose()
})

$titleLbl = New-Object System.Windows.Forms.Label
$titleLbl.Text = "NOXTICORD"
$titleLbl.Font = New-Object System.Drawing.Font("Segoe UI",13,[System.Drawing.FontStyle]::Bold)
$titleLbl.AutoSize = $true
$titleLbl.ForeColor = $TextMain
$titleLbl.BackColor = [System.Drawing.Color]::Transparent
$titleLbl.Location = New-Object System.Drawing.Point(48,12)
$titleLbl.Cursor = [System.Windows.Forms.Cursors]::SizeAll

$tagLbl = New-Object System.Windows.Forms.Label
$tagLbl.Text = "// Installateur portable · Aucun dépendance · Lancement direct"
$tagLbl.Font = New-Object System.Drawing.Font("Consolas",7.5)
$tagLbl.AutoSize = $true
$tagLbl.ForeColor = $TextMuted
$tagLbl.BackColor = [System.Drawing.Color]::Transparent
$tagLbl.Location = New-Object System.Drawing.Point(50,29)

$minBtn = New-Object System.Windows.Forms.Button
$minBtn.Text = "–"
$minBtn.Size = New-Object System.Drawing.Size(30,30)
$minBtn.Location = New-Object System.Drawing.Point(748,6)
$minBtn.BackColor = $Surface
$minBtn.ForeColor = $TextMuted
$minBtn.FlatStyle = "Flat"
$minBtn.FlatAppearance.BorderSize = 0
$minBtn.Cursor = [System.Windows.Forms.Cursors]::Hand
$minBtn.Add_Click({ $form.WindowState = "Minimized" })

$closeBtn = New-Object System.Windows.Forms.Button
$closeBtn.Text = "×"
$closeBtn.Size = New-Object System.Drawing.Size(38,32)
$closeBtn.Location = New-Object System.Drawing.Point(788,6)
$closeBtn.BackColor = $RedDark
$closeBtn.ForeColor = $TextMain
$closeBtn.FlatStyle = "Flat"
$closeBtn.FlatAppearance.BorderSize = 0
$closeBtn.Cursor = [System.Windows.Forms.Cursors]::Hand
$closeBtn.Add_Click({ $form.Close() })

$titleBar.Controls.AddRange(@($iconPic,$titleLbl,$tagLbl,$minBtn,$closeBtn))

# Déplacement fenêtre (drag sur barre de titre)
$drag = $false
$start = $null
$titleBar.Add_MouseDown({
    param($s,$e)
    if ($e.Button -eq "Left") { $drag = $true; $start = $e.Location }
})
$titleBar.Add_MouseMove({
    param($s,$e)
    if ($drag) {
        $new = $form.PointToScreen($e.Location)
        $form.Location = New-Object System.Drawing.Point($new.X-$start.X, $new.Y-$start.Y)
    }
})
$titleBar.Add_MouseUp({ $drag = $false })
$iconPic.Add_MouseDown({
    param($s,$e)
    if ($e.Button -eq "Left") { $drag = $true; $start = $e.Location }
})
$iconPic.Add_MouseMove({
    param($s,$e)
    if ($drag) {
        $new = $form.PointToScreen($e.Location)
        $form.Location = New-Object System.Drawing.Point($new.X-$start.X, $new.Y-$start.Y)
    }
})
$iconPic.Add_MouseUp({ $drag = $false })
$titleLbl.Add_MouseDown({
    param($s,$e)
    if ($e.Button -eq "Left") { $drag = $true; $start = $e.Location }
})
$titleLbl.Add_MouseMove({
    param($s,$e)
    if ($drag) {
        $new = $form.PointToScreen($e.Location)
        $form.Location = New-Object System.Drawing.Point($new.X-$start.X, $new.Y-$start.Y)
    }
})
$titleLbl.Add_MouseUp({ $drag = $false })
$tagLbl.Add_MouseDown({
    param($s,$e)
    if ($e.Button -eq "Left") { $drag = $true; $start = $e.Location }
})
$tagLbl.Add_MouseMove({
    param($s,$e)
    if ($drag) {
        $new = $form.PointToScreen($e.Location)
        $form.Location = New-Object System.Drawing.Point($new.X-$start.X, $new.Y-$start.Y)
    }
})
$tagLbl.Add_MouseUp({ $drag = $false })

$form.Controls.Add($titleBar)

# ---------- Bandeau néon ----------
$neonBar = New-Object System.Windows.Forms.Panel
$neonBar.Location = New-Object System.Drawing.Point(0,44)
$neonBar.Size = New-Object System.Drawing.Size(820,4)
$neonBar.BackColor = $Accent1
$form.Controls.Add($neonBar)

# ---------- Contenu ----------
$title = New-Object System.Windows.Forms.Label
$title.Text = "NOXTICORD"
$title.Font = New-Object System.Drawing.Font("Segoe UI",24,[System.Drawing.FontStyle]::Bold)
$title.AutoSize = $true
$title.ForeColor = $TextMain
$title.BackColor = [System.Drawing.Color]::Transparent
$title.Location = New-Object System.Drawing.Point(28,72)

$tag = New-Object System.Windows.Forms.Label
$tag.Text = "// DISCORD STABLE / CANARY · INSTALLATION DIRECTE · SANS AVERTISSEMENT"
$tag.Font = New-Object System.Drawing.Font("Consolas",8.5,[System.Drawing.FontStyle]::Bold)
$tag.AutoSize = $true
$tag.ForeColor = $Accent2
$tag.BackColor = [System.Drawing.Color]::Transparent
$tag.Location = New-Object System.Drawing.Point(32,118)

$form.Controls.AddRange(@($title,$tag))

# ---------- Carte d'état ----------
$statusCard = New-Object System.Windows.Forms.Panel
$statusCard.Location = New-Object System.Drawing.Point(24,152)
$statusCard.Size = New-Object System.Drawing.Size(772,100)
$statusCard.BackColor = $Glass
$statusCard.Paint.Add({
    param($s,$e)
    $g = $e.Graphics
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $r = 16
    $rc = New-Object System.Drawing.Rectangle(0,0,$statusCard.Width-1,$statusCard.Height-1)
    $d = $r*2
    $path.AddArc($rc.X,$rc.Y,$d,$d,180,90)
    $path.AddArc($rc.Right-$d,$rc.Y,$d,$d,270,90)
    $path.AddArc($rc.Right-$d,$rc.Bottom-$d,$d,$d,0,90)
    $path.AddArc($rc.X,$rc.Bottom-$d,$d,$d,90,90)
    $path.CloseFigure()
    $b = New-Object System.Drawing.SolidBrush($Glass)
    $g.FillPath($b,$path)
    $b.Dispose()
    $pen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(90,255,255,255),1.2)
    $g.DrawPath($pen,$path)
    $pen.Dispose()
})

$statusLogo = New-Object System.Windows.Forms.PictureBox
$statusLogo.Size = New-Object System.Drawing.Size(48,48)
$statusLogo.Location = New-Object System.Drawing.Point(24,26)
$statusLogo.BackColor = [System.Drawing.Color]::Transparent
$statusLogo.Paint.Add({
    param($sender,$e)
    $e.Graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $b = New-Object System.Drawing.SolidBrush($Accent1)
    $e.Graphics.FillEllipse($b,4,4,40,40)
    $b.Dispose()
})

$statusTitle = New-Object System.Windows.Forms.Label
$statusTitle.Text = "Noxtcord n'est pas installé"
$statusTitle.Font = New-Object System.Drawing.Font("Segoe UI",14,[System.Drawing.FontStyle]::Bold)
$statusTitle.AutoSize = $true
$statusTitle.ForeColor = $TextMuted
$statusTitle.BackColor = [System.Drawing.Color]::Transparent
$statusTitle.Location = New-Object System.Drawing.Point(92,22)

$statusDetail = New-Object System.Windows.Forms.Label
$statusDetail.Text = "Choisissez une version puis cliquez sur INSTALLER."
$statusDetail.Font = New-Object System.Drawing.Font("Segoe UI",10)
$statusDetail.AutoSize = $true
$statusDetail.ForeColor = $TextMuted
$statusDetail.BackColor = [System.Drawing.Color]::Transparent
$statusDetail.Location = New-Object System.Drawing.Point(92,52)

$statusCard.Controls.AddRange(@($statusLogo,$statusTitle,$statusDetail))
$form.Controls.Add($statusCard)

# ---------- Sélecteur de version ----------
$pickLabel = New-Object System.Windows.Forms.Label
$pickLabel.Text = "CHOISISSEZ VOTRE VERSION DE DISCORD"
$pickLabel.Font = New-Object System.Drawing.Font("Consolas",8,[System.Drawing.FontStyle]::Bold)
$pickLabel.AutoSize = $true
$pickLabel.ForeColor = $TextMuted
$pickLabel.BackColor = [System.Drawing.Color]::Transparent
$pickLabel.Location = New-Object System.Drawing.Point(28,272)

$optStable = New-Object System.Windows.Forms.Panel
$optStable.Location = New-Object System.Drawing.Point(24,298)
$optStable.Size = New-Object System.Drawing.Size(376,84)
$optStable.BackColor = $Card
$optStable.Cursor = [System.Windows.Forms.Cursors]::Hand
$optStable.Tag = "stable"
$optStable.Paint.Add({
    param($s,$e)
    $g = $e.Graphics
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $back = if ($Branch -eq "stable") { [System.Drawing.Color]::FromArgb(34,36,44) } else { $Card }
    $border = if ($Branch -eq "stable") { $Accent1 } else { [System.Drawing.Color]::FromArgb(42,44,51) }
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $r = 14
    $rc = New-Object System.Drawing.Rectangle(0,0,$optStable.Width-1,$optStable.Height-1)
    $d = $r*2
    $path.AddArc($rc.X,$rc.Y,$d,$d,180,90)
    $path.AddArc($rc.Right-$d,$rc.Y,$d,$d,270,90)
    $path.AddArc($rc.Right-$d,$rc.Bottom-$d,$d,$d,0,90)
    $path.AddArc($rc.X,$rc.Bottom-$d,$d,$d,90,90)
    $path.CloseFigure()
    $b = New-Object System.Drawing.SolidBrush($back)
    $g.FillPath($b,$path)
    $b.Dispose()
    $pen = New-Object System.Drawing.Pen($border,1.4)
    $g.DrawPath($pen,$path)
    $pen.Dispose()
    # logo
    $cb = New-Object System.Drawing.SolidBrush($Accent1)
    $g.FillEllipse($cb,20,18,44,44)
    $cb.Dispose()
    # texte
    $ft = New-Object System.Drawing.Font("Segoe UI",12,[System.Drawing.FontStyle]::Bold)
    $fs = New-Object System.Drawing.Font("Segoe UI",8.5)
    $tb = New-Object System.Drawing.SolidBrush($TextMain)
    $sb = New-Object System.Drawing.SolidBrush($TextMuted)
    $g.DrawString("Discord",$ft,$tb,82,22)
    $g.DrawString("Version principale",$fs,$sb,82,48)
    $tb.Dispose(); $sb.Dispose(); $ft.Dispose(); $fs.Dispose()
})
$optStable.Add_Click({ $Branch = "stable"; $optStable.BackColor = [System.Drawing.Color]::FromArgb(34,36,44); $optCanary.BackColor = $Card; $optStable.Invalidate(); $optCanary.Invalidate() })

$optCanary = New-Object System.Windows.Forms.Panel
$optCanary.Location = New-Object System.Drawing.Point(420,298)
$optCanary.Size = New-Object System.Drawing.Size(376,84)
$optCanary.BackColor = $Card
$optCanary.Cursor = [System.Windows.Forms.Cursors]::Hand
$optCanary.Tag = "canary"
$optCanary.Paint.Add({
    param($s,$e)
    $g = $e.Graphics
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $back = if ($Branch -eq "canary") { [System.Drawing.Color]::FromArgb(34,36,44) } else { $Card }
    $border = if ($Branch -eq "canary") { $Canary } else { [System.Drawing.Color]::FromArgb(42,44,51) }
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $r = 14
    $rc = New-Object System.Drawing.Rectangle(0,0,$optCanary.Width-1,$optCanary.Height-1)
    $d = $r*2
    $path.AddArc($rc.X,$rc.Y,$d,$d,180,90)
    $path.AddArc($rc.Right-$d,$rc.Y,$d,$d,270,90)
    $path.AddArc($rc.Right-$d,$rc.Bottom-$d,$d,$d,0,90)
    $path.AddArc($rc.X,$rc.Bottom-$d,$d,$d,90,90)
    $path.CloseFigure()
    $b = New-Object System.Drawing.SolidBrush($back)
    $g.FillPath($b,$path)
    $b.Dispose()
    $pen = New-Object System.Drawing.Pen($border,1.4)
    $g.DrawPath($pen,$path)
    $pen.Dispose()
    $cb = New-Object System.Drawing.SolidBrush($Canary)
    $g.FillEllipse($cb,20,18,44,44)
    $cb.Dispose()
    $ft = New-Object System.Drawing.Font("Segoe UI",12,[System.Drawing.FontStyle]::Bold)
    $fs = New-Object System.Drawing.Font("Segoe UI",8.5)
    $tb = New-Object System.Drawing.SolidBrush($TextMain)
    $sb = New-Object System.Drawing.SolidBrush($TextMuted)
    $g.DrawString("Discord Canary",$ft,$tb,82,22)
    $g.DrawString("Version bêta quotidienne",$fs,$sb,82,48)
    $tb.Dispose(); $sb.Dispose(); $ft.Dispose(); $fs.Dispose()
})
$optCanary.Add_Click({ $Branch = "canary"; $optCanary.BackColor = [System.Drawing.Color]::FromArgb(34,36,44); $optStable.BackColor = $Card; $optCanary.Invalidate(); $optStable.Invalidate() })

$form.Controls.AddRange(@($pickLabel,$optStable,$optCanary))

# ---------- Boutons ----------
function New-NeoButton {
    param($Text,$Location,$Size,$Back,$Fore,$Glow,$Click)
    $btn = New-Object System.Windows.Forms.Button
    $btn.Text = $Text
    $btn.Location = $Location
    $btn.Size = $Size
    $btn.BackColor = $Back
    $btn.ForeColor = $Fore
    $btn.FlatStyle = "Flat"
    $btn.FlatAppearance.BorderSize = 0
    $btn.FlatAppearance.MouseOverBackColor = $Glow
    $btn.FlatAppearance.MouseDownBackColor = $Glow
    $btn.Cursor = [System.Windows.Forms.Cursors]::Hand
    $btn.Font = New-Object System.Drawing.Font("Segoe UI",11,[System.Drawing.FontStyle]::Bold)
    $btn.Add_Click($Click)
    return $btn
}

$installBtn = New-NeoButton "INSTALLER" (New-Object System.Drawing.Point(24,404)) (New-Object System.Drawing.Size(500,56)) $Accent1 $TextMain $Accent2 {
    $log.Clear()
    $footer.Text = "Installation sur $Branch en cours..."
    $bar.Visible = $true
    $installBtn.Enabled = $false
    $uninstallBtn.Enabled = $false
    # Téléchargement + exécution de install.ps1
    $tmpScript = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(),"noxtcord-install.ps1")
    try {
        [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
        $wc = New-Object System.Net.WebClient
        $wc.DownloadFile("$Repo$Script", $tmpScript)
        Unblock-File -LiteralPath $tmpScript -ErrorAction SilentlyContinue
        $log.AppendText("> Téléchargement de $Script OK`r`n")
        $log.AppendText("> Lancement...`r`n`r`n")
        $args = "-Branch $Branch -Relaunch"
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = "powershell.exe"
        $psi.Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$tmpScript`" $args"
        $psi.UseShellExecute = $false
        $psi.RedirectStandardOutput = $true
        $psi.RedirectStandardError = $true
        $psi.CreateNoWindow = $true
        $p = [System.Diagnostics.Process]::Start($psi)
        $p.OutputDataReceived.Add({ param($s,$e) if ($e.Data) { $log.AppendText("$e.Data`r`n") } })
        $p.ErrorDataReceived.Add({ param($s,$e) if ($e.Data) { $log.AppendText("[!] $e.Data`r`n") } })
        $p.BeginOutputReadLine()
        $p.BeginErrorReadLine()
        $p.WaitForExit()
        $log.AppendText("`r`n> Terminé (code $($p.ExitCode)).`r`n")
        if ($p.ExitCode -eq 0) { $footer.Text = "Installation terminée avec succès"; $statusTitle.Text = "Noxtcord est installé"; $statusTitle.ForeColor = $Green }
        else { $footer.Text = "Installation échec (code $($p.ExitCode))" }
    } catch {
        $footer.Text = "Erreur"
        $log.AppendText("[!] $($_.Exception.Message)`r`n")
    } finally {
        $bar.Visible = $false
        $installBtn.Enabled = $true
        $uninstallBtn.Enabled = $true
    }
}

$uninstallBtn = New-NeoButton "DÉSINSTALLER" (New-Object System.Drawing.Point(544,404)) (New-Object System.Drawing.Size(252,56)) $RedDark $TextMain $Red {
    $log.Clear()
    $footer.Text = "Désinstallation en cours..."
    $bar.Visible = $true
    $installBtn.Enabled = $false
    $uninstallBtn.Enabled = $false
    $tmpScript = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(),"noxtcord-uninstall.ps1")
    try {
        [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
        $wc = New-Object System.Net.WebClient
        $wc.DownloadFile("$Repo/uninstall.ps1", $tmpScript)
        Unblock-File -LiteralPath $tmpScript -ErrorAction SilentlyContinue
        $log.AppendText("> Téléchargement de uninstall.ps1 OK`r`n> Lancement...`r`n`r`n")
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = "powershell.exe"
        $psi.Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$tmpScript`" -Relaunch"
        $psi.UseShellExecute = $false
        $psi.RedirectStandardOutput = $true
        $psi.RedirectStandardError = $true
        $psi.CreateNoWindow = $true
        $p = [System.Diagnostics.Process]::Start($psi)
        $p.OutputDataReceived.Add({ param($s,$e) if ($e.Data) { $log.AppendText("$e.Data`r`n") } })
        $p.ErrorDataReceived.Add({ param($s,$e) if ($e.Data) { $log.AppendText("[!] $e.Data`r`n") } })
        $p.BeginOutputReadLine()
        $p.BeginErrorReadLine()
        $p.WaitForExit()
        $log.AppendText("`r`n> Terminé (code $($p.ExitCode)).`r`n")
        if ($p.ExitCode -eq 0) { $footer.Text = "Désinstallation terminée"; $statusTitle.Text = "Noxtcord n'est pas installé"; $statusTitle.ForeColor = $TextMuted }
        else { $footer.Text = "Désinstallation échec (code $($p.ExitCode))" }
    } catch {
        $footer.Text = "Erreur"
        $log.AppendText("[!] $($_.Exception.Message)`r`n")
    } finally {
        $bar.Visible = $false
        $installBtn.Enabled = $true
        $uninstallBtn.Enabled = $true
    }
}

$repairBtn = New-NeoButton "RÉPARER" (New-Object System.Drawing.Point(24,474)) (New-Object System.Drawing.Size(240,42)) $Surface $TextMain $Accent2 {
    $installBtn.PerformClick()
}

$folderBtn = New-NeoButton "DOSSIER" (New-Object System.Drawing.Point(284,474)) (New-Object System.Drawing.Size(240,42)) $Surface $TextMain $Accent2 {
    $dir = [System.IO.Path]::Combine([System.Environment]::GetFolderPath("ApplicationData"),"Noxticord")
    [System.IO.Directory]::CreateDirectory($dir)
    [System.Diagnostics.Process]::Start("explorer.exe",$dir)
}

$githubBtn = New-NeoButton "GITHUB" (New-Object System.Drawing.Point(544,474)) (New-Object System.Drawing.Size(252,42)) $Surface $TextMain $Accent2 {
    [System.Diagnostics.Process]::Start($Repo)
}

$form.Controls.AddRange(@($installBtn,$uninstallBtn,$repairBtn,$folderBtn,$githubBtn))

# ---------- Log ----------
$log = New-Object System.Windows.Forms.RichTextBox
$log.Multiline = $true
$log.ReadOnly = $true
$log.ScrollBars = "Vertical"
$log.WordWrap = $false
$log.Font = New-Object System.Drawing.Font("Consolas",9)
$log.BackColor = [System.Drawing.Color]::FromArgb(10,11,14)
$log.ForeColor = [System.Drawing.Color]::FromArgb(110,220,150)
$log.BorderStyle = "None"
$log.Location = New-Object System.Drawing.Point(24,534)
$log.Size = New-Object System.Drawing.Size(772,112)
$log.Text = "Prêt. Aucun blocage, aucun avertissement.`r`n"

$bar = New-Object System.Windows.Forms.ProgressBar
$bar.Style = "Marquee"
$bar.MarqueeAnimationSpeed = 30
$bar.Location = New-Object System.Drawing.Point(24,656)
$bar.Size = New-Object System.Drawing.Size(772,6)
$bar.Visible = $false

$footer = New-Object System.Windows.Forms.Label
$footer.Text = "Prêt. Aucun blocage, aucun avertissement."
$footer.Font = New-Object System.Drawing.Font("Consolas",8.5,[System.Drawing.FontStyle]::Bold)
$footer.AutoSize = $true
$footer.ForeColor = $TextMuted
$footer.BackColor = [System.Drawing.Color]::Transparent
$footer.Location = New-Object System.Drawing.Point(24,670)

$form.Controls.AddRange(@($log,$bar,$footer))

# ---------- Rafraîchir l'état ----------
function Refresh-Status {
    $local = [System.Environment]::GetFolderPath("LocalApplicationData")
    $dist = [System.IO.Path]::Combine([System.Environment]::GetFolderPath("ApplicationData"),"Noxticord","dist","patcher.js")
    $payload = [System.IO.File]::Exists($dist)
    $patched = @()
    $apps = @(
        @{key="Discord";name="Discord"},
        @{key="DiscordCanary";name="Discord Canary"},
        @{key="DiscordPTB";name="Discord PTB"},
        @{key="DiscordDevelopment";name="Discord Dev"}
    )
    foreach ($app in $apps) {
        $root = [System.IO.Path]::Combine($local,$app.key)
        if (-not [System.IO.Directory]::Exists($root)) { continue }
        $found = $false
        foreach ($v in [System.IO.Directory]::GetDirectories($root,"app-*")) {
            $asar = [System.IO.Path]::Combine($v,"resources","app.asar")
            if (-not [System.IO.File]::Exists($asar)) { continue }
            try {
                $fs = [System.IO.File]::OpenRead($asar)
                $buf = New-Object byte[] 4096
                $n = $fs.Read($buf,0,$buf.Length)
                $fs.Close()
                $head = [System.Text.Encoding]::UTF8.GetString($buf,0,$n)
                if ($head.Contains("Noxticord") -or $head.Contains("Noxtcord")) { $found = $true; break }
            } catch {}
        }
        if ($found) { $patched += $app.name }
    }
    if ($patched.Count -gt 0) {
        $statusLogo.BackColor = $Green
        $statusTitle.Text = "Noxtcord est installé"
        $statusTitle.ForeColor = $Green
        $statusDetail.Text = "Actif sur : $($patched -join ', ') · Discord a été relancé."
    } elseif ($payload) {
        $statusLogo.BackColor = $Amber
        $statusTitle.Text = "Fichiers présents, patch non détecté"
        $statusTitle.ForeColor = $Amber
        $statusDetail.Text = "Relancez l'installation puis redémarrez Discord."
    } else {
        $statusLogo.BackColor = [System.Drawing.Color]::FromArgb(110,116,125)
        $statusTitle.Text = "Noxtcord n'est pas installé"
        $statusTitle.ForeColor = $TextMuted
        $statusDetail.Text = "Choisissez une version puis cliquez sur INSTALLER · lancement direct."
    }
}

Refresh-Status
$form.Add_Load({ Refresh-Status })

# ---------- Lancer ----------
[void]$form.ShowDialog()
