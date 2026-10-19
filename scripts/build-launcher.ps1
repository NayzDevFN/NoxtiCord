#Requires -Version 5.1
<#
.SYNOPSIS
    Compile (et signe optionnellement) le lanceur graphique noxtcord.exe.

.DESCRIPTION
    Aucune installation requise : utilise le csc.exe deja present sur tout Windows
    (.NET Framework 4.x), donc le build fonctionne sur n'importe quelle machine.

    Le build ajoute les metadonnees Win32 (Company/Product/FileDescription/version)
    et un manifeste embarque (asInvoker, Windows 7 a 11). Ce sont des signaux
    utilises par Microsoft Defender SmartScreen pour evaluer un binaire.

    La signature numerique (AuthentiCode) est optionnelle :
      - soit via un certificat local (-CertificatePath / -CertificateThumbprint) ;
      - soit via SignPath dans CI (voir .github/workflows/build-sign-launcher.yml).

.EXAMPLE
    .\scripts\build-launcher.ps1
.EXAMPLE
    .\scripts\build-launcher.ps1 -Version 1.2.0
.EXAMPLE
    .\scripts\build-launcher.ps1 -CertificateThumbprint <SHA1> -TimestampUrl http://timestamp.digicert.com
#>
[CmdletBinding()]
param(
    [string]$Source,
    [string]$Output,
    [string]$Version = "1.0.0",
    [string]$FileVersion,
    [string]$CertificatePath,
    [string]$CertificatePassword,
    [string]$CertificateThumbprint,
    [string]$TimestampUrl = "http://timestamp.digicert.com",
    [switch]$SkipSign
)

$ErrorActionPreference = "Stop"
# csc.exe renvoie un code de sortie non nul en cas d'erreur de compilation :
# on ne veut pas que PowerShell transforme ce code en exception avant l'affichage.
$PSNativeCommandUseErrorActionPreference = $false

$Root = Split-Path -Parent $PSScriptRoot
if (-not $Source) { $Source = Join-Path $Root "noxtcord.cs" }
if (-not $Output) { $Output = Join-Path $Root "noxtcord.exe" }
if (-not $FileVersion) { $FileVersion = "$Version.0" }

foreach ($v in @($Version, $FileVersion)) {
    if ($v -notmatch '^\d+\.\d+\.\d+(\.\d+)?$') {
        throw "Version invalide '$v' (attendu : 1.2.3 ou 1.2.3.4)"
    }
}

# ---------------------------------------------------------------- compiler ---
$Csc = @(
    (Join-Path $env:WINDIR "Microsoft.NET\Framework64\v4.0.30319\csc.exe"),
    (Join-Path $env:WINDIR "Microsoft.NET\Framework\v4.0.30319\csc.exe")
) | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $Csc) {
    throw "csc.exe introuvable. Le .NET Framework 4.x est present sur tout Windows 10/11 : $env:WINDIR\Microsoft.NET"
}
if (-not (Test-Path $Source)) { throw "Source introuvable : $Source" }

# ------------------------------------------------- metadonnees Win32 (ver) ---
$Work = Join-Path ([IO.Path]::GetTempPath()) ("noxtcord-build-" + [Guid]::NewGuid().ToString("N").Substring(0, 8))
New-Item -ItemType Directory -Path $Work -Force | Out-Null

$AssemblyInfo = Join-Path $Work "AssemblyInfo.cs"
@"
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NoxtiCord Installer")]
[assembly: AssemblyDescription("Installeur officiel NoxtiCord pour Discord (Stable / Canary / PTB)")]
[assembly: AssemblyCompany("NoxtiCord")]
[assembly: AssemblyProduct("NoxtiCord Installer")]
[assembly: AssemblyCopyright("NoxtiCord - https://github.com/NayzDevFN/NoxtiCord")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("7b1f0d6e-3a55-4a5f-9a1c-6e0a9c1f4d2b")]
[assembly: AssemblyVersion("$Version.0")]
[assembly: AssemblyFileVersion("$FileVersion")]
[assembly: AssemblyInformationalVersion("$Version")]
"@ | Set-Content -LiteralPath $AssemblyInfo -Encoding UTF8

# ------------------------------------------------------- manifeste embarque ---
$Manifest = Join-Path $Work "noxtcord.exe.manifest"
@"
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <assemblyIdentity version="$Version.0" name="NoxtiCord.Installer" type="win32" />
  <description>NoxtiCord Installer</description>
  <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
    <security>
      <requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">
        <requestedExecutionLevel level="asInvoker" uiAccess="false" />
      </requestedPrivileges>
    </security>
  </trustInfo>
  <compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
      <!-- Windows 10 / 11 -->
      <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
      <!-- Windows 8.1 -->
      <supportedOS Id="{1f676c76-80e1-4239-95bb-83d0f6d0da78}" />
      <!-- Windows 8 -->
      <supportedOS Id="{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}" />
      <!-- Windows 7 -->
      <supportedOS Id="{35138b9a-5d96-4fbd-8e2d-a2440225f93a}" />
    </application>
  </compatibility>
  <application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <longPathAware xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">true</longPathAware>
    </windowsSettings>
  </application>
</assembly>
"@ | Set-Content -LiteralPath $Manifest -Encoding UTF8

# ------------------------------------------------- bundle embarque (tout) ---
# install.ps1 + uninstall.ps1 + payload/* sont colles dans l'exe : a la
# fin, l'utilisateur n'a plus besoin d'aucun autre fichier (meme pas de
# reseau) — le lanceur extrait ce zip au lancement.
$Payload = Join-Path $Root "payload"
$Bundle = Join-Path $Work "noxticord.bundle.zip"
foreach ($f in @((Join-Path $Root "install.ps1"), (Join-Path $Root "uninstall.ps1"), (Join-Path $Payload "patcher.js"))) {
    if (-not (Test-Path $f)) { throw "fichier manquant pour le bundle : $f" }
}
Compress-Archive -Path (Join-Path $Root "install.ps1"), (Join-Path $Root "uninstall.ps1"), $Payload `
    -DestinationPath $Bundle -CompressionLevel Optimal -Force
Write-Host "Bundle embarque : $([math]::Round((Get-Item $Bundle).Length / 1KB)) Ko (scripts + payload)"

# --------------------------------------------------------------- compilation --
$CscArgs = @(
    "/nologo",
    "/target:winexe",
    "/platform:anycpu",
    "/optimize+",
    "/debug-",
    # Pas de guillemets imbriques : PowerShell quote deja les arguments qui
    # contiennent des espaces (PowerShell 5.1 et 7 se comportent pareil ainsi).
    "/out:$Output",
    "/win32manifest:$Manifest",
    "/r:System.dll",
    "/r:System.Drawing.dll",
    "/r:System.Windows.Forms.dll",
    "/r:System.IO.Compression.dll",
    "/r:System.IO.Compression.FileSystem.dll",
    "/resource:$Bundle,noxticord.bundle.zip",
    $Source,
    $AssemblyInfo
)

Write-Host "Build de $Output (version $Version)..."
Write-Host "Compilateur : $Csc"
$errs = & $Csc @CscArgs 2>&1 | ForEach-Object { $_.ToString() }
if ($LASTEXITCODE -ne 0) {
    $errs | ForEach-Object { Write-Host $_ }
    $detail = ($errs | Select-Object -First 8) -join " | "
    Remove-Item $Work -Recurse -Force -ErrorAction SilentlyContinue
    throw "compilation echouee (code $LASTEXITCODE) : $detail"
}
$errs | Where-Object { $_ -match "warning" } | ForEach-Object { Write-Warning $_ }

# ----------------------------------------------------------------- signature --
$Signed = $false
if (-not $SkipSign -and ($CertificatePath -or $CertificateThumbprint)) {
    $cert = $null
    if ($CertificateThumbprint) {
        foreach ($store in @("Cert:\CurrentUser\My", "Cert:\LocalMachine\My")) {
            $cert = Get-ChildItem -Path $store -CodeSigningCert -ErrorAction SilentlyContinue |
                Where-Object { $_.Thumbprint -eq $CertificateThumbprint } |
                Select-Object -First 1
            if ($cert) { break }
        }
        if (-not $cert) { throw "Certificat de signature non trouve : $CertificateThumbprint" }
    } else {
        $certArgs = @{ FilePath = (Resolve-Path $CertificatePath).Path }
        if ($CertificatePassword) {
            $sec = ConvertTo-SecureString $CertificatePassword -AsPlainText -Force
            $certArgs.Cert = (New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certArgs.FilePath, $sec))
        } else {
            $certArgs.Cert = (New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certArgs.FilePath))
        }
        $cert = $certArgs.Cert
        if (-not ($cert.HasPrivateKey)) { throw "Le certificat ne contient pas de cle privee" }
    }

    Write-Host "Signature de $Output avec $($cert.Subject)..."
    $sig = Set-AuthenticodeSignature -FilePath $Output -Certificate $cert -HashAlgorithm SHA256 -TimestampServer $TimestampUrl
    if ($sig.Status -ne "Valid") {
        Remove-Item $Work -Recurse -Force -ErrorAction SilentlyContinue
        throw "signature invalide : $($sig.Status) - $($sig.StatusMessage)"
    }
    $Signed = $true
}

# ------------------------------------------------------------------ resultat --
$info = [Diagnostics.FileVersionInfo]::GetVersionInfo($Output)
$sigState = (Get-AuthenticodeSignature -FilePath $Output).Status
Remove-Item $Work -Recurse -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "  Fichier      : $Output"
Write-Host "  Taille       : $((Get-Item $Output).Length) octets"
Write-Host "  Version      : $($info.FileVersion)  ($($info.FileDescription) / $($info.CompanyName))"
Write-Host "  Signature    : $sigState"

if ($sigState -ne "Valid") {
    Write-Host ""
    Write-Warning ("non signe : Microsoft Defender SmartScreen bloquera ce fichier sur un autre PC. " +
        "Voir README.md > « Windows / SmartScreen » (SignPath CI ou soumission Microsoft).")
}

if (-not (Test-Path $Output)) { throw "sortie introuvable : $Output" }
return (Resolve-Path $Output).Path
