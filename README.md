# Noxtcord Installer

Support d'installation de **Noxtcord** (fork de Noxticord) pour Discord.

-   Patch de `resources\app.asar` (l'original est sauvegardé en `_app.asar`)
-   Installe le payload dans `%APPDATA%\Noxticord\dist`
-   Détection automatique de **Discord Stable, Canary, PTB et Development**
-   Interface **FR/EN** avec sélecteur de langue dans les réglages
-   **Build hébergé sur GitHub** : tout le build est publié sur ce dépôt GitHub, puis installé localement dans `%APPDATA%\Noxticord\dist` ; un toast « Mettre à jour et redémarrer » apparaît dans Discord quand les fichiers du `dist` changent
-   Install / Uninstall / Status / Repair en CLI Node.js **ou** en PowerShell (sans Node)

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
2. double-cliquez dessus — il télécharge le dernier `install.ps1` + payload depuis GitHub, installe, et relance Discord ;
3. un message confirme la fin de l'installation.

> C'est la méthode recommandée sur un PC tiers : aucun clone, aucune dépendance.

### B : One-liner PowerShell (sans clone, sans Node)

```powershell
irm https://github.com/NayzDevFN/NoxtiCord/raw/main/install.ps1 | iex
```

> Le script télécharge le payload depuis ce dépôt et patche Discord.
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

## Dépannage

| Symptôme                              | Solution                                        |
| ------------------------------------- | ----------------------------------------------- |
| La catégorie Noxtcord n'apparaît pas  | `node installer.mjs status` puis relance Discord |
| « Aucune installation trouvée »       | Discord Stable/PTB/Canary est-il installé ?     |
| Installation refusée (fichier occupé) | Fermez Discord (ou ajoutez `--no-kill`)         |
| Après mise à jour Discord             | `node installer.mjs repair`                     |
| Tout remettre d'origine               | `node installer.mjs uninstall`                  |

## Contenu du dépôt

| Fichier                      | Rôle                                          |
| ---------------------------- | --------------------------------------------- |
| `payload/`                   | Build Noxtcord complet (patcher, bundles…)    |
| `installer.mjs`              | CLI Node.js (install/status/uninstall/repair) |
| `install.ps1` / `uninstall.ps1` | Installeur / désinstalleur PowerShell      |
| `scripts/makePayload.mjs`    | Prépare le payload publié sur GitHub         |
| `noxtcord.cs` / `noxtcord.exe` | Lanceur graphique : double-clic = install   |

## Prérequis

-   Discord installé (Stable, Canary, PTB ou Development)
-   Node.js ≥ 18 (CLI) **ou** PowerShell ≥ 5.1 **ou** `noxtcord.exe` (rien d'autre)

## Avertissement

Modifier Discord viole les conditions d'utilisation de Discord. Usage à vos risques et périls.
