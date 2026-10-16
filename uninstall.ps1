# Noxtcord uninstaller — restaure les app.asar d'origine
# Usage: .\uninstall.ps1 [-Relaunch] [-Branch stable|canary|ptb|dev] [-NoKill]
param(
    [switch]$Relaunch,
    [string]$Branch = "",
    [switch]$NoKill
)
$ErrorActionPreference = "Stop"

$Apps = @(
    @{ key = "stable"; dir = "Discord";       exe = "Discord.exe" },
    @{ key = "canary"; dir = "DiscordCanary"; exe = "DiscordCanary.exe" },
    @{ key = "ptb";    dir = "DiscordPTB";    exe = "DiscordPTB.exe" },
    @{ key = "dev";    dir = "DiscordDevelopment"; exe = "DiscordDevelopment.exe" }
)

$restored = @()
foreach ($a in $Apps) {
    if ($Branch -and $Branch -ne $a.key) { continue }
    $root = Join-Path $env:LOCALAPPDATA $a.dir
    if (-not (Test-Path $root)) { continue }
    Get-ChildItem $root -Directory -Filter "app-*" | ForEach-Object {
        $asar = Join-Path $_.FullName "resources\app.asar"
        $backup = Join-Path $_.FullName "resources\_app.asar"
        if ((Test-Path $asar) -and (Test-Path $backup)) {
            $headTxt = ""
            try { $headTxt = [Text.Encoding]::UTF8.GetString([IO.File]::ReadAllBytes($asar)[0..([Math]::Min(4095, (Get-Item $asar).Length - 1))]) } catch {}
            if ($headTxt -match "Noxti?cord") {
                $restored += [pscustomobject]@{ app = $a; ver = $_.Name; asar = $asar; backup = $backup }
            }
        }
    }
}

if (-not $restored) { Write-Host "Rien à désinstaller (aucun patch détecté)."; exit 0 }

if (-not $NoKill) {
    foreach ($exe in "Discord.exe", "DiscordCanary.exe", "DiscordPTB.exe", "DiscordDevelopment.exe", "Update.exe") {
        Stop-Process -Name ($exe -replace "\.exe$", "") -Force -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 2
}

foreach ($r in $restored) {
    Copy-Item $r.backup $r.asar -Force
    Write-Host "restauré: $($r.app.key) $($r.ver)"
}

if ($Relaunch) {
    $a = $restored[0].app
    $updater = Join-Path $env:LOCALAPPDATA "$($a.dir)\Update.exe"
    if (Test-Path $updater) { Start-Process $updater -ArgumentList "--processStart", $a.exe; Write-Host "Discord relancé" }
} else {
    Write-Host "Fini. Relance Discord pour revenir à la version d'origine."
}
