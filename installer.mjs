#!/usr/bin/env node
// Noxtcord installer CLI — install / status / uninstall / repair
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { spawnSync } from "node:child_process";

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const APPDATA = process.env.APPDATA;
const LOCALAPPDATA = process.env.LOCALAPPDATA;
const DIST = path.join(APPDATA, "Noxticord", "dist");
const PATCHER = path.join(DIST, "patcher.js");

const APPS = [
  { key: "stable", dir: "Discord", exe: "Discord.exe" },
  { key: "canary", dir: "DiscordCanary", exe: "DiscordCanary.exe" },
  { key: "ptb", dir: "DiscordPTB", exe: "DiscordPTB.exe" },
  { key: "dev", dir: "DiscordDevelopment", exe: "DiscordDevelopment.exe" },
];

function detect(branch) {
  const out = [];
  for (const app of APPS) {
    if (branch && branch !== app.key) continue;
    const root = path.join(LOCALAPPDATA, app.dir);
    if (!fs.existsSync(root)) continue;
    for (const v of fs.readdirSync(root)) {
      if (!v.startsWith("app-")) continue;
      const res = path.join(root, v, "resources");
      const asar = path.join(res, "app.asar");
      if (!fs.existsSync(asar)) continue;
      let patched = false;
      try {
        const head = fs.readFileSync(asar).subarray(0, 4096).toString("utf8");
        patched = head.includes("Noxticord") || head.includes("Noxtcord");
      } catch {}
      out.push({ app, ver: v, res, asar, backup: path.join(res, "_app.asar"), patched });
    }
  }
  return out;
}

function miniAsar(patcherPath) {
  const idx = `require(${JSON.stringify(patcherPath)})`;
  const pkg = '{\n\t"name": "discord",\n\t"main": "index.js"\n}';
  const header = JSON.stringify({
    files: {
      "index.js": { size: Buffer.byteLength(idx), offset: "0" },
      "package.json": { size: Buffer.byteLength(pkg), offset: String(Buffer.byteLength(idx)) },
    },
  });
  const L = Buffer.byteLength(header);
  const head = Buffer.alloc(16);
  head.writeUInt32LE(4, 0);
  head.writeUInt32LE(L + 8, 4);
  head.writeUInt32LE(L + 4, 8);
  head.writeUInt32LE(L, 12);
  return Buffer.concat([head, Buffer.from(header, "utf8"), Buffer.from(idx, "utf8"), Buffer.from(pkg, "utf8")]);
}

function killDiscord() {
  for (const exe of ["Discord.exe", "DiscordCanary.exe", "DiscordPTB.exe", "DiscordDevelopment.exe", "Update.exe"])
    spawnSync("taskkill", ["/F", "/IM", exe], { stdio: "ignore" });
}

function relaunch(app) {
  const updater = path.join(LOCALAPPDATA, app.dir, "Update.exe");
  if (!fs.existsSync(updater)) return false;
  spawnSync(updater, ["--processStart", app.exe], { detached: true, stdio: "ignore" }).unref?.();
  return true;
}

function copyPayload(source) {
  const src = source ?? path.join(ROOT, "payload");
  if (!fs.existsSync(src) || !fs.existsSync(path.join(src, "patcher.js"))) {
    console.error("payload introuvable: " + src);
    console.error("Clonez le depôt complet (git clone https://github.com/NayzDevFN/NoxtiCord) ou utilisez install.ps1 qui telecharge le payload automatiquement.");
    process.exit(1);
  }
  fs.rmSync(DIST, { recursive: true, force: true });
  fs.cpSync(src, DIST, { recursive: true });
  console.log("payload copié → " + DIST);
}

function cmdStatus() {
  const list = detect();
  if (!list.length) { console.log("Aucune installation Discord trouvée dans " + LOCALAPPDATA); return; }
  for (const d of list)
    console.log(`${d.app.key.padEnd(7)} ${d.ver.padEnd(14)} ${d.patched ? "PATCHÉ" : "vanilla"}  ${d.patched ? (fs.existsSync(d.backup) ? "(backup ok)" : "(SANS BACKUP)") : ""}`);
  console.log("dist: " + (fs.existsSync(PATCHER) ? "présent" : "ABSENT") + "  payload: " + (fs.existsSync(path.join(ROOT, "payload")) ? "présent" : "absent"));
}

function cmdInstall(opts) {
  const list = detect(opts.branch);
  if (!list.length) { console.error("Aucune installation Discord trouvée."); process.exit(1); }
  copyPayload(opts.source);
  if (!opts.noKill) killDiscord();
  for (const d of list) {
    if (!d.patched && fs.existsSync(d.asar) && !fs.existsSync(d.backup)) fs.copyFileSync(d.asar, d.backup);
    fs.writeFileSync(d.asar, miniAsar(PATCHER));
    console.log(`installé: ${d.app.key} ${d.ver}`);
  }
  if (!opts.noRelaunch) {
    const first = list[0];
    setTimeout(() => { if (!relaunch(first.app)) console.log("Relance manuelle: " + first.app.exe); }, 800);
    console.log("Relance de Discord…");
  }
}

function cmdUninstall(opts) {
  const list = detect(opts.branch);
  const patched = list.filter((d) => d.patched && fs.existsSync(d.backup));
  if (!patched.length) { console.log("Rien à désinstaller (pas de backup)."); return; }
  if (!opts.noKill) killDiscord();
  for (const d of patched) {
    fs.copyFileSync(d.backup, d.asar);
    console.log(`restauré: ${d.app.key} ${d.ver}`);
  }
  if (!opts.noRelaunch) {
    setTimeout(() => relaunch(patched[0].app), 800);
    console.log("Relance de Discord…");
  }
}

export { miniAsar };

const invokedDirectly = process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url);
if (invokedDirectly) {
  const [, , cmd, ...rest] = process.argv;
  const opts = { branch: undefined, source: undefined, noKill: false, noRelaunch: false };
  for (let i = 0; i < rest.length; i++) {
    const a = rest[i];
    if (a === "--source") opts.source = rest[++i];
    else if (a === "--no-kill") opts.noKill = true;
    else if (a === "--no-relaunch") opts.noRelaunch = true;
    else if (["stable", "canary", "ptb", "dev"].includes(a)) opts.branch = a;
  }
  switch (cmd) {
    case "install": cmdInstall(opts); break;
    case "status": cmdStatus(); break;
    case "uninstall": cmdUninstall(opts); break;
    case "repair": cmdInstall({ ...opts, noRelaunch: false }); break;
    default:
      console.log("Usage: node installer.mjs <install|status|uninstall|repair> [stable|canary|ptb|dev] [--source <dossier>] [--no-kill] [--no-relaunch]");
  }
}
