# Noxtcord Installer

Support d'installation de **Noxtcord** (fork de Noxticord) pour Discord.

-   Patch de `resources\app.asar` (l'original est sauvegardé en `_app.asar`)
-   Installe le payload dans `%APPDATA%\Noxticord\dist`
-   Détection automatique de **Discord Stable, Canary, PTB et Development**
-   Interface **FR/EN** avec sélecteur de langue dans les réglages
-   **Build hébergé sur GitHub** : tout le build est publié sur ce dépôt GitHub, puis installé localement dans `%APPDATA%\Noxticord\dist` ; un toast « Mettre à jour et redémarrer » apparaît dans Discord quand les fichiers du `dist` changent
-   Install / Uninstall / Status / Repair en CLI Node.js **ou** en PowerShell (sans Node)

## Page d'installation (GitHub Pages)

Le dossier [`docs/`](docs/) contient une page web d'installation auto-hébergée :

> **<https://nayzdevfn.github.io/NoxtiCord/>**

Elle propose le téléchargement direct de `noxtcord.exe`, la commande PowerShell
one-liner (bouton « copier »), le guide SmartScreen, le dépannage et l'état du
dernier build CI (via l'API publique GitHub).

Activation (une seule fois) : **Settings → Pages → Source : Deploy from a branch →
`main` / `docs` / (root)**. Le site se redéploie automatiquement à chaque push sur `main`.

## Comment ça marche

Discord charge `resources\app.asar` au démarrage. L'installateur :

1. renomme l'`app.asar` d'origine en `_app.asar` (backup) ;
2. écrit un mini `app.asar` dont le `index.js` contient `require("<%APPDATA%>\Noxticord\dist\patcher.js")` ;
3. copie le `payload/` du dépôt dans `%APPDATA%\Noxticord\dist` ;
4. relance Discord — le build Noxtcord prend le relais.

`uninstall` fait l'inverse : `_app.asar` redevient `app.asar`, Discord redevient d'origine.

## Installation

### A : Exécutable `noxtcord.exe` (double-clic)

1. Téléchargez [`noxtcord.exe`](https://github.com/NayzDevFN/NoxtiCord/raw/main/noxtcord.exe) depuis ce dépôt ;
2. double-cliquez dessus — la **fenêtre « NoxtiCord Installer »** (style Vencord) s'ouvre : carte d'état (installé / non installé), choix de la version (Stable, Canary, PTB ou toutes), boutons **Installer** / **Desinstaller** / **Reparer**, journal en direct ;
3. l'installation télécharge tout depuis GitHub et relance Discord.

> C'est la méthode recommandée sur un PC tiers : aucun clone, aucune dépendance.

> **⚠ Si Windows affiche « Microsoft Defender SmartScreen a empêché le démarrage d'une
> application »** : c'est normal tant que le fichier n'est pas signé / pas encore connu.
> Rien n'est supprimé — il suffit de cliquer sur **Plus d'informations → Exécuter quand
> même**, ou de débloquer le fichier. Tout est expliqué dans
> [Windows bloque noxtcord.exe](#windows-bloque-noxtcordexe-microsoft-defender-smartscreen).

### B : One-liner PowerShell (sans clone, sans Node)

```powershell
irm https://github.com/NayzDevFN/NoxtiCord/raw/main/install.ps1 | iex
```

> Fonctionne sur n'importe quel PC : le script télécharge tout depuis GitHub
> (archive du dépôt, avec repli fichier par fichier si l'échec), puis patche Discord.
> Pour relancer Discord directement : téléchargez le dépôt et lancez `.\install.ps1 -Relaunch`.

### PowerShell (dépôt cloné)

```powershell
.\install.ps1            # installe Stable + Canary + PTB détectés
.\install.ps1 -Branch canary
.\install.ps1 -Relaunch  # et relance Discord
.\uninstall.ps1          # restaure Discord
```

### Node.js (CLI)

```powershell
node installer.mjs install           # installe sur toutes les versions trouvées
node installer.mjs install canary    # une version précise
node installer.mjs status            # état des installations
node installer.mjs uninstall         # restaure Discord
node installer.mjs repair            # après une mise à jour Discord
```

Options : `--source <dossier>` (payload alternatif), `--no-kill`, `--no-relaunch`.

## Mise à jour après publication sur GitHub

Le build est hébergé sur ce dépôt GitHub. Après avoir publié une nouvelle version (voir plus bas) :

1. relancez l'installateur (`.\install.ps1` ou `node installer.mjs install`) — il récupère le dernier `payload/` depuis GitHub ;
2. un toast **« Une mise à jour de Noxtcord est disponible »** apparaît dans Discord (sous 3 s, sinon vérifié toutes les 20 s) ;
3. cliquez sur **« Mettre à jour et redémarrer »** → Discord se relance avec les nouveaux fichiers.

## Après une mise à jour Discord

Discord recrée son dossier `app-<version>` → le patch disparaît. Rouvrez ce dépôt et :

```powershell
node installer.mjs repair
```

## Publier une nouvelle version du build

```powershell
node scripts/makePayload.mjs   # prépare le payload publié sur GitHub (sans .bak ni .map)
git add payload && git commit -m "Update payload" && git push
```

Si vous avez modifié le lanceur :

```powershell
.\scripts\build-launcher.ps1   # recompile noxtcord.exe (métadonnées + version incluses)
git add noxtcord.cs noxtcord.exe && git commit -m "Update launcher" && git push
```

> Pousser une modification de `noxtcord.cs` déclenche le workflow **Build + Sign
> noxtcord.exe**, qui compile et republie un `noxtcord.exe` **signé** (dès que les
> secrets SignPath sont configurés).

## Windows bloque noxtcord.exe (Microsoft Defender SmartScreen)

Sur un autre PC, Windows affiche :

> **Windows a protégé votre ordinateur**
> Microsoft Defender SmartScreen a empêché le démarrage d'une application non reconnue.
> L'exécution de cette application peut mettre l'ordinateur en danger.

**Cause** : `noxtcord.exe` est un exécutable **non signé numériquement** *et* téléchargé
depuis Internet. Windows ajoute alors la marque « Mark of the Web » au fichier, et
SmartScreen bloque tout exécutable non signé / sans réputation. Ce n'est pas un virus :
c'est le comportement par défaut de Windows pour tout binaire inconnu.

### Solution immédiate (sur le PC qui bloque)

Méthode 1 — depuis la fenêtre d'avertissement :

1. cliquez sur **Plus d'informations** ;
2. cliquez sur **Exécuter quand même**.

Méthode 2 — débloquer le fichier une bonne fois pour toutes (double-clic bloqué) :

```powershell
Unblock-File -LiteralPath "C:\chemin\vers\noxtcord.exe"
```

ou : clic droit sur le fichier → **Propriétés** → cocher **Débloquer** → OK.

> Même réflexe pour `Noxticord Installer.bat` ou une archive `zip` extraite :
> clic droit → Propriétés → **Débloquer**. Le script `install.ps1` fait désormais cet
> `Unblock-File` automatiquement sur les fichiers qu'il télécharge.

### Solution définitive

Deux actions, indépendantes l'une de l'autre (les deux cumulent leur effet) :

1. **Signer le code** — voir [Signature du code (SignPath)](#signature-du-code-signpath) :
   un exécutable signé n'affiche plus « éditeur inconnu » et acquiert une réputation.
2. **Soumettre le fichier à Microsoft** pour analyse (gratuit) :
   <https://www.microsoft.com/wdsi/filesubmission> → onglet *Software* →
   déposez `noxtcord.exe`. Microsoft vérifie le fichier et met à jour SmartScreen
   (délai de quelques jours à quelques semaines). À refaire à chaque nouvelle version
   majeure tant que le fichier n'est pas signé.

Aucune de ces deux actions n'est « magique » : il n'existe pas de réglage Windows qui
force SmartScreen à ignorer un exécutable inconnu.

## Signature du code (SignPath)

Le dépôt est prêt pour la **signature gratuite** proposée par la
[SignPath Foundation](https://signpath.org/apply) aux projets open source
(signature AuthentiCode effectuée dans un HSM, sans jamais manipuler la clé privée).

### Mise en route (une seule fois)

1. Déposez votre projet : <https://signpath.org/apply> (dépôt public GitHub requis) ;
2. dans SignPath : créez un *Project* + une *Signing Policy* (ex. `release-signing`) ;
3. installez l'[application SignPath GitHub](https://github.com/apps/signpath) et
   autorisez l'accès à `NayzDevFN/NoxtiCord` ;
4. dans GitHub : **Settings → Secrets and variables → Actions → New repository secret**
   et créez les 4 secrets :
   `SIGNPATH_API_TOKEN`, `SIGNPATH_ORGANIZATION_ID`, `SIGNPATH_PROJECT_SLUG`,
   `SIGNPATH_SIGNING_POLICY_SLUG` ;
5. lancez le workflow **Actions → Build + Sign noxtcord.exe → Run workflow**
   (ou poussez une modification de `noxtcord.cs`).

Le workflow compile `noxtcord.cs`, fait signer l'artefact par SignPath, puis **commit
`noxtcord.exe` signé dans le dépôt** : les utilisateurs téléchargent ensuite un
exécutable dont l'éditeur est reconnu.

> Un certificat OV/EV acheté (Certum, SSL.com, DigiCert…) donne le même résultat en
> dehors de GitHub : voir `-CertificatePath` / `-CertificateThumbprint` de
> `scripts/build-launcher.ps1`.

### Compiler localement

```powershell
.\scripts\build-launcher.ps1                 # noxtcord.cs -> noxtcord.exe
.\scripts\build-launcher.ps1 -Version 1.2.0  # avec numéro de version
```

Aucune dépendance : le script utilise le `csc.exe` déjà présent sur tout Windows.
Ajoute les métadonnées Win32 (Company / Product / version) et le manifeste embarqué,
deux signaux pris en compte par SmartScreen. Avec un certificat local :

```powershell
.\scripts\build-launcher.ps1 -CertificateThumbprint <SHA1-du-certificat>
```

## Dépannage

| Symptôme                                                      | Solution                                                                                       |
| ------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| « Microsoft Defender SmartScreen a empêché le démarrage… »     | Voir [Windows bloque noxtcord.exe](#windows-bloque-noxtcordexe-microsoft-defender-smartscreen)  |
| La GUI ne s'ouvre pas (erreur « Jeton inattendu »)             | `installer-gui.ps1` doit être **UTF-8 avec BOM** (PowerShell 5.1 le lit en ANSI sinon)          |
| La catégorie Noxtcord n'apparaît pas                          | `node installer.mjs status` puis relance Discord                                                |
| « Aucune installation trouvée »                               | Discord Stable/PTB/Canary est-il installé ?                                                    |
| Installation refusée (fichier occupé)                         | Fermez Discord (ou ajoutez `--no-kill`)                                                        |
| Après mise à jour Discord                                     | `node installer.mjs repair`                                                                    |
| Tout remettre d'origine                                       | `node installer.mjs uninstall`                                                                 |

## Contenu du dépôt

| Fichier                                       | Rôle                                                |
| --------------------------------------------- | --------------------------------------------------- |
| `payload/`                                    | Build Noxtcord complet (patcher, bundles…)          |
| `installer.mjs`                               | CLI Node.js (install/status/uninstall/repair)       |
| `install.ps1` / `uninstall.ps1`               | Installeur / désinstalleur PowerShell               |
| `installer-gui.ps1` / `Noxticord Installer.bat` | Interface graphique portable (double-clic)        |
| `scripts/makePayload.mjs`                     | Prépare le payload publié sur GitHub               |
| `scripts/build-launcher.ps1`                  | Compile (et signe) `noxtcord.exe`, sans dépendance  |
| `.github/workflows/build-sign-launcher.yml`   | CI : compile, fait signer via SignPath, republie    |
| `noxtcord.cs` / `noxtcord.exe`                | Lanceur graphique : double-clic = install           |

## Prérequis

-   Discord installé (Stable, Canary, PTB ou Development)
-   Node.js ≥ 18 (CLI) **ou** PowerShell ≥ 5.1 **ou** `noxtcord.exe` (rien d'autre)

## Avertissement

Modifier Discord viole les conditions d'utilisation de Discord. Usage à vos risques et périls.
