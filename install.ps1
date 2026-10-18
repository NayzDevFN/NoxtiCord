# Noxtcord installer (PowerShell) — installe le payload et patche Discord
# Usage: .\install.ps1 [-Relaunch] [-Branch stable|canary|ptb|dev] [-NoKill]
param(
    [switch]$Relaunch,
    [string]$Branch = "",
    [switch]$NoKill
)
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"
try { [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12 } catch {}

$Repo = "NayzDevFN/NoxtiCord"
$ZipUrl = "https://codeload.github.com/$Repo/zip/refs/heads/main"
$RawUrl = "https://raw.githubusercontent.com/$Repo/main/payload/"

$PayloadFiles = @(
    "patcher.js", "preload.js", "renderer.js", "renderer.css",
    "browser.js", "browser.css", "extension.js", "extension.css",
    "Noxticord.user.js", "Noxticord.user.css",
    "noxticordDesktopMain.js", "noxticordDesktopPreload.js",
    "noxticordDesktopRenderer.js", "noxticordDesktopRenderer.css",
    "vendor/monaco/index.js", "vendor/monaco/index.css",
    "vendor/monaco/codicon-LN6W7LCM.ttf",
    "vendor/monaco/vs/editor/editor.worker.js",
    "vendor/monaco/vs/language/css/css.worker.js"
)

$Dist = Join-Path $env:APPDATA "Noxticord\dist"
$Patcher = Join-Path $Dist "patcher.js"
$Apps = @(
    @{ key = "stable"; dir = "Discord";       exe = "Discord.exe" },
    @{ key = "canary"; dir = "DiscordCanary"; exe = "DiscordCanary.exe" },
    @{ key = "ptb";    dir = "DiscordPTB";    exe = "DiscordPTB.exe" },
    @{ key = "dev";    dir = "DiscordDevelopment"; exe = "DiscordDevelopment.exe" }
)

function Invoke-Download([string]$Uri, [string]$OutFile) {
    Invoke-WebRequest -Uri $Uri -OutFile $OutFile -UseBasicParsing -TimeoutSec 180
    # Fichier venu d'Internet : on retire le Mark of the Web, sinon Windows
    # bloque les .ps1 / .bat / .exe qui en heritent.
    Unblock-File -LiteralPath $OutFile -ErrorAction SilentlyContinue
}

function Clear-InternetMark([string]$Path) {
    try {
        if (-not (Test-Path -LiteralPath $Path)) { return }
        if (Test-Path -LiteralPath $Path -PathType Container) {
            Get-ChildItem -LiteralPath $Path -Recurse -Force -File -ErrorAction SilentlyContinue |
                ForEach-Object { Unblock-File -LiteralPath $_.FullName -ErrorAction SilentlyContinue }
        } else {
            Unblock-File -LiteralPath $Path -ErrorAction SilentlyContinue
        }
    } catch {}
}

function Get-PayloadDir {
    if ($PSScriptRoot -and (Test-Path (Join-Path $PSScriptRoot "payload\patcher.js"))) {
        return (Join-Path $PSScriptRoot "payload")
    }

    Write-Host "Telechargement du payload depuis GitHub..."
    $zip = Join-Path $env:TEMP "noxtcord-payload.zip"
    $tmp = Join-Path $env:TEMP "noxtcord-repo"

    try {
        Invoke-Download $ZipUrl $zip
        $len = (Get-Item $zip).Length
        if ($len -lt 1024) { throw "archive anormalement petite ($len octets)" }
        if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
        Expand-Archive -Path $zip -DestinationPath $tmp -Force
        Clear-InternetMark $tmp
        $p = Get-ChildItem $tmp -Directory |
            ForEach-Object { Join-Path $_.FullName "payload" } |
            Where-Object { Test-Path (Join-Path $_ "patcher.js") } |
            Select-Object -First 1
        if ($p) { Write-Host "payload telecharge -> $p"; return $p }
        throw "l'archive ne contient pas de dossier payload/"
    } catch {
        Write-Host "Echec du telechargement de l'archive ($($_.Exception.Message))"
        Write-Host "Nouvelle tentative : telechargement fichier par fichier..."
    }

    $dest = Join-Path $env:TEMP "noxtcord-payload"
    if (Test-Path $dest) { Remove-Item $dest -Recurse -Force }
    New-Item -ItemType Directory -Path $dest -Force | Out-Null
    foreach ($f in $PayloadFiles) {
        $out = Join-Path $dest ($f -replace "/", [IO.Path]::DirectorySeparatorChar)
        $dir = Split-Path $out -Parent
        if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
        Invoke-Download ($RawUrl + $f) $out
        Write-Host "  ok: $f"
    }
    if (-not (Test-Path (Join-Path $dest "patcher.js"))) { throw "echec du telechargement du payload" }
    Clear-InternetMark $dest
    Write-Host "payload telecharge -> $dest"
    return $dest
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
if (-not $targets) { Write-Error "Aucune installation Discord trouvee dans $env:LOCALAPPDATA" }

try {
    $payload = Get-PayloadDir
} catch {
    Write-Error "Impossible d'obtenir le payload: $($_.Exception.Message)"
    exit 1
}

if (-not $NoKill) { Stop-Discord }

if (Test-Path $Dist) { Remove-Item $Dist -Recurse -Force }
New-Item -ItemType Directory -Path $Dist -Force | Out-Null
robocopy $payload $Dist /E /NFL /NDL /NJH /NJS /NJS | Out-Null
if (-not (Test-Path $Patcher)) { Write-Error "echec de la copie du payload vers $Dist"; exit 1 }
Write-Host "payload copie -> $Dist"

$asarBytes = New-MiniAsar $Patcher
foreach ($t in $targets) {
    $isPatched = $false
    try { $headTxt = [Text.Encoding]::UTF8.GetString([IO.File]::ReadAllBytes($t.asar)[0..([Math]::Min(4095, (Get-Item $t.asar).Length - 1))]); $isPatched = $headTxt -match "Noxti?cord" } catch {}
    if (-not $isPatched -and -not (Test-Path $t.backup)) { Copy-Item $t.asar $t.backup }
    [IO.File]::WriteAllBytes($t.asar, $asarBytes)
    Write-Host "installe: $($t.app.key) $($t.ver)"
}

if ($Relaunch) {
    $a = $targets[0].app
    $updater = Join-Path $env:LOCALAPPDATA "$($a.dir)\Update.exe"
    if (Test-Path $updater) { Start-Process $updater -ArgumentList "--processStart", $a.exe; Write-Host "Discord relance" }
    else { Write-Host "Relance manuelle: $($a.exe)" }
} else {
    Write-Host "Fini. Relance Discord pour appliquer (ou relancez avec -Relaunch)."
}
