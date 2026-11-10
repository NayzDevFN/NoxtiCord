# Journal des modifications — 10 novembre 2026

Opération : **vérifier et relancer Discord Stable en conservant les mods actuels**.

## État constaté avant l'opération

| Élément | État |
| --- | --- |
| Discord Stable 1.0.9267 | Lancé (6 processus), **déjà patché** (`app.asar` → `%APPDATA%\Noxticord\dist\patcher.js`) |
| Discord Stable 1.0.9188 (ancien) | Patché, backup `_app.asar` présent |
| Discord Canary 1.0.1319 | **Vanilla** (non patché — non touché lors de cette opération) |
| Mods locaux (`%APPDATA%\Noxticord\dist`) | 5 bundles JS moddés par rapport au `payload/` du repo : `browser.js`, `renderer.js`, `extension.js`, `Noxticord.user.js`, `noxticordDesktopRenderer.js` (~4 000 caractères de code ajoutés dans chacun) |
| Réglages / plugins | `settings.json` intact (plugins actifs : CommandsAPI, MessageAccessoriesAPI, MessageUpdaterAPI, UserSettingsAPI, FakeVoice, MessageLogger, FollowUser, BadgeAPI…) |
| Dépôt git | Propre (`git status` sans modification) |

## Modifications effectuées

1. **Création d'une sauvegarde complète du dist moddé**
   - Chemin : `C:\Users\offpa\AppData\Roaming\Noxticord\dist_backup_1011`
   - Contenu : les 14 fichiers du `dist` + le dossier `vendor` (19 fichiers, ~10,9 Mo)
   - But : pouvoir restaurer les mods à l'identique si jamais ils sont écrasés.

2. **Arrêt de Discord** (6 processus `Discord.exe` de l'app 1.0.9267, arrêt forcé).

3. **Relance de Discord** via `C:\Users\offpa\AppData\Local\Discord\app-1.0.9267\Discord.exe`
   - Nouveaux processus démarrés à 19:00:33.

4. **Rédaction de ce fichier** `MODIFS.md` (nouveau fichier dans le dépôt, non commité).

## Ce qui n'a PAS été modifié (vérifié après relance)

- ✅ Les 6 bundles moddés : **empreintes SHA-256 identiques** avant/après → mods intacts.
- ✅ `patcher.js`, les CSS et les autres fichiers du `dist` : inchangés (dates du 21/10 et 10/11 conservées).
- ✅ `app.asar` / `_app.asar` des deux Stable : non touchés (toujours patchés, backups intacts).
- ✅ `settings.json`, `quickCss.css`, `themes/` : aucun contenu modifié par moi.
- ✅ Dépôt git : `git status` toujours propre (hors ce nouveau `MODIFS.md`).
- ℹ️ `settings.json` a été **réécrit automatiquement par Discord lui-même** à 19:01:43 au démarrage — comportement normal, mêmes valeurs.

## Preuves que les mods sont chargés

- `%TEMP%\noxtcord-updater.log` : `watchers installed` à 18:00:34 UTC (= 19:00:34 locale), puis `status check -> false` en continu → le patcher moddé s'exécute et aucune mise à jour n'est en attente.
- `app.asar` de l'app 1.0.9267 : pointe toujours vers `%APPDATA%\Noxticord\dist\patcher.js`.

## ⚠️ Comment ne PAS perdre les mods

- **Ne pas relancer `install.ps1` / `node installer.mjs install` / `noxtcord.exe`** : l'installateur **écrase** `%APPDATA%\Noxticord\dist` avec le `payload/` du dépôt → les 5 bundles moddés seraient perdus.
- En cas de mistake : restaurer avec
  ```powershell
  Remove-Item "$env:APPDATA\Noxticord\dist" -Recurse -Force
  Copy-Item "$env:APPDATA\Noxticord\dist_backup_1011" "$env:APPDATA\Noxticord\dist" -Recurse
  ```
- Si tu veux rendre les mods « officiels » (plus jamais écrasables), il faudrait les **copier dans le `payload/` du repo** et pousser — possible sur demande.

## Pour aller plus loin (non fait — à demander)

- Patcher **Discord Canary** (actuellement vanilla).
- Synchroniser les 5 bundles moddés vers le `payload/` du dépôt.
