# Noxtcord installer (PowerShell) — installe le payload et patche Discord
# Usage: .\install.ps1 [-Relaunch] [-Branch stable|canary|ptb|dev] [-NoKill]
param(
    [switch]$Relaunch,
    [string]$Branch = "",
    [switch]$NoKill
)
$ErrorActionPreference = "Stop"

$Dist = Join-Path $env:APPDATA "Noxticord\dist"
$Patcher = Join-Path $Dist "patcher.js"
$Apps = @(
    @{ key = "stable"; dir = "Discord";       exe = "Discord.exe" },
    @{ key = "canary"; dir = "DiscordCanary"; exe = "DiscordCanary.exe" },
    @{ key = "ptb";    dir = "DiscordPTB";    exe = "DiscordPTB.exe" },
    @{ key = "dev";    dir = "DiscordDevelopment"; exe = "DiscordDevelopment.exe" }
)

function Get-PayloadDir {
    if ($PSScriptRoot -and (Test-Path (Join-Path $PSScriptRoot "payload"))) { return (Join-Path $PSScriptRoot "payload") }
    Write-Host "Téléchargement du payload depuis GitHub..."
    $zip = Join-Path $env:TEMP "noxtcord-payload.zip"
    $tmp = Join-Path $env:TEMP "noxtcord-repo"
    Invoke-WebRequest -Uri "https://github.com/NayzDevFN/NoxtiCord/archive/refs/heads/main.zip" -OutFile $zip
    if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
    Expand-Archive -Path $zip -DestinationPath $tmp -Force
    $p = Get-ChildItem $tmp -Directory | ForEach-Object { Join-Path $_.FullName "payload" } | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $p) { throw "payload introuvable dans l'archive téléchargée" }
    return $p
}

function New-MiniAsar([string]$PatcherPath) {
    $idx = 'require("' + ($PatcherPath -replace '\\', '\\') + '")'
    $pkg = "{`n`t`"name`": `"discord`",`n`t`"main`": `"index.js`"`n}"
    $idxB = [Text.Encoding]::UTF8.GetBytes($idx)
    $pkgB = [Text.Encoding]::UTF8.GetBytes($pkg)
    $json = '{"files":{"index.js":{"size":' + $idxB.Length + ',"offset":"0"},"package.json":{"size":' + $pkgB.Length + ',"offset":"' + $idxB.Length + '"}}}'
    $jsonB = [Text.Encoding]::UTF8.GetBytes($json)
    $L = $jsonB.Length
    $head = New-Object byte[] 16
    [BitConverter]::GetBytes([uint32]4).CopyTo($head, 0)
    [BitConverter]::GetBytes([uint32]($L + 8)).CopyTo($head, 4)
    [BitConverter]::GetBytes([uint32]($L + 4)).CopyTo($head, 8)
    [BitConverter]::GetBytes([uint32]$L).CopyTo($head, 12)
    $out = New-Object IO.MemoryStream
    $out.Write($head, 0, 16); $out.Write($jsonB, 0, $jsonB.Length)
    $out.Write($idxB, 0, $idxB.Length); $out.Write($pkgB, 0, $pkgB.Length)
    return $out.ToArray()
}

function Stop-Discord {
    foreach ($exe in "Discord.exe", "DiscordCanary.exe", "DiscordPTB.exe", "DiscordDevelopment.exe", "Update.exe") {
        Stop-Process -Name ($exe -replace "\.exe$", "") -Force -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 2
}

$targets = @()
foreach ($a in $Apps) {
    if ($Branch -and $Branch -ne $a.key) { continue }
    $root = Join-Path $env:LOCALAPPDATA $a.dir
    if (-not (Test-Path $root)) { continue }
    Get-ChildItem $root -Directory -Filter "app-*" | ForEach-Object {
        $asar = Join-Path $_.FullName "resources\app.asar"
        if (Test-Path $asar) {
            $targets += [pscustomobject]@{ app = $a; ver = $_.Name; asar = $asar; backup = ($asar -replace "app\.asar$", "_app.asar") }
        }
    }
}
if (-not $targets) { Write-Error "Aucune installation Discord trouvée dans $env:LOCALAPPDATA" }

$payload = Get-PayloadDir
if (-not $NoKill) { Stop-Discord }

if (Test-Path $Dist) { Remove-Item $Dist -Recurse -Force }
New-Item -ItemType Directory -Path $Dist -Force | Out-Null
robocopy $payload $Dist /E /NFL /NDL /NJH /NJS /NJS | Out-Null
Write-Host "payload copié → $Dist"

$asarBytes = New-MiniAsar $Patcher
foreach ($t in $targets) {
    $isPatched = $false
    try { $headTxt = [Text.Encoding]::UTF8.GetString([IO.File]::ReadAllBytes($t.asar)[0..([Math]::Min(4095, (Get-Item $t.asar).Length - 1))]); $isPatched = $headTxt -match "Noxti?cord" } catch {}
    if (-not $isPatched -and -not (Test-Path $t.backup)) { Copy-Item $t.asar $t.backup }
    [IO.File]::WriteAllBytes($t.asar, $asarBytes)
    Write-Host "installé: $($t.app.key) $($t.ver)"
}

if ($Relaunch) {
    $a = $targets[0].app
    $updater = Join-Path $env:LOCALAPPDATA "$($a.dir)\Update.exe"
    if (Test-Path $updater) { Start-Process $updater -ArgumentList "--processStart", $a.exe; Write-Host "Discord relancé" }
    else { Write-Host "Relance manuelle: $($a.exe)" }
} else {
    Write-Host "Fini. Relance Discord pour appliquer (ou relancez avec -Relaunch)."
}
