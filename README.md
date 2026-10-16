# NoxtiCord Installer

Support d'installation de **NoxtiCord** pour Discord.

-   Détection automatique de **Discord Normal, Canary, PTB et Development**
-   Patch de `resources\app.asar` (l'original est sauvegardé en `_app.asar`)
-   Install / Uninstall / Status / Repair en CLI Node.js **ou** en PowerShell (sans Node)
-   Après installation, la catégorie **« Noxticord »** apparaît dans les Réglages utilisateur de Discord (options, plugins, thèmes, updater…)

## Comment ça marche

Discord charge `resources\app.asar` au démarrage. L'installateur :

1. renomme l'`app.asar` d'origine en `_app.asar` (backup) ;
2. écrit un mini `app.asar` dont le `index.js` contient `require("<path>\patcher.js")` ;
3. `patcher.js` (le build NoxtiCord) prend le relais → Discord démarre avec NoxtiCord.

`uninstall` fait l'inverse : `_app.asar` redevient `app.asar`, Discord redevient d'origine.

## Installation

### Avec l'exécutable (NoxtiCord.exe)

Installateur graphique, ouverture immédiate :

```powershell
.\release\NoxtiCord.exe
```

> Ce binaire s'exécute directement depuis `release\` (les dossiers `resources\`, `locales\` et les `.dll` doivent rester à côté).
> `release\NoxtiCord-portable.exe` est l'ancienne version auto-extractante : elle ré-extrait l'app à chaque lancement (~10 s).

-   détection automatique des versions installées (Stable, Canary, PTB, Development) ;
-   case à cocher par version → installer / désinstaller / relancer Discord ;
-   options « fermer Discord avant le patch » et « relancer après l'installation » ;
-   journal des opérations en direct.

Pour le (re)générer à partir du dépôt :

```powershell
npm install
npm run build:exe   # → release\NoxtiCord.exe
```

### Avec Node.js (CLI)

```powershell
node installer.mjs install          # installe sur toutes les versions trouvées
node installer.mjs install canary   # une version précise
node installer.mjs status           # état des installations
node installer.mjs uninstall        # restaure Discord
node installer.mjs repair           # après une mise à jour Discord
```

### Sans Node.js (PowerShell)

```powershell
.\install.ps1           # installe partout
.\install.ps1 -Relaunch # et relance Discord
.\uninstall.ps1         # restaure Discord
```

### One-liner depuis GitHub

```powershell
irm https://github.com/NayzDevFN/NoxtiCord/raw/main/install.ps1 | iex
```

> Sans Node et sans clone : le script télécharge le payload (`payload/` du dépôt) et patche Discord.

## Modes d'installation

| Mode            | Commande                                             | Payload                        |
| --------------- | ---------------------------------------------------- | ------------------------------ |
| **Copie** (défaut) | `node installer.mjs install`                       | copié dans `%APPDATA%\Noxticord\dist` |
| **Dev**         | `node installer.mjs install --dev --source <dist>`   | lien direct vers votre build   |
| **Téléchargé**  | `.\install.ps1` (sans clone/local)                   | téléchargé depuis GitHub       |

Le mode copie survit à la suppression/déplacement du dépôt (le payload est copié dans `%APPDATA%\Noxticord`).
Le mode `--dev` est pour développer : chaque `pnpm build` est directement utilisé au prochain redémarrage de Discord.

## Interface graphique (`gui/`)

App Electron réutilisant `lib/` (détection, patch, payload) — même moteur que la CLI.

| Commande             | Rôle                                             |
| -------------------- | ------------------------------------------------ |
| `npm run gui`        | Lance l'interface en développement               |
| `npm run test:gui`   | Test de fumée : détection, rendu, boutons, IPC    |
| `npm run makeIcon`   | Régénère `gui/icon.ico`                          |
| `npm run build:exe`  | Construit `release\NoxtiCord.exe` (portable)     |

Le payload est embarqué dans l'exécutable (`resources\payload`) : l'installation
fonctionne sans clone du dépôt ni connexion réseau.

## Publier sur GitHub (payload téléchargeable)

```powershell
node scripts/makePayload.mjs                          # copie dist/ → payload/ (sans les .map)
git add payload && git commit -m "Add payload" && git push
```

-   Le `one-liner` et `install.ps1` utilisent alors `payload/` du dépôt `main`.
-   Optionnel : pousse un tag `v1.0.0` → le workflow **Release** publie une release GitHub avec `noxticord-payload.zip`.

## Après une mise à jour Discord

Discord recrée son dossier `app-<version>` → le patch disparaît. Rouvre ce dépôt et :

```powershell
node installer.mjs repair
```

## Dépannage

| Symptôme                              | Solution                                        |
| ------------------------------------- | ----------------------------------------------- |
| La catégorie Noxticord n'apparaît pas | Vérifie `node installer.mjs status` puis relance Discord |
| « Aucune installation trouvée »       | Discord Stable/PTB/Canary est-il installé ?     |
| Installation refusée (fichier occupé) | Ferme Discord puis relance sans `--no-kill`      |
| Après mise à jour Discord            | `node installer.mjs repair`                     |
| Tout remettre d'origine               | `node installer.mjs uninstall`                  |

## Prérequis

-   Node.js ≥ 18 (CLI) **ou** PowerShell ≥ 5.1 (scripts `.ps1`)
-   Un build NoxtiCord : le dépôt `payload/`, un `dist/`, ou `--source <dossier>`

## Avertissement

Modifier Discord viole les conditions d'utilisation de Discord. Usage à vos risques et périls.
