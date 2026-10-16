// Copie le build dist/ (ou une source donnée) → payload/ pour publication GitHub
// Usage: node scripts/makePayload.mjs [--source <dossier>]
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const ROOT = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
const args = process.argv.slice(2);
const srcArg = args.includes("--source") ? args[args.indexOf("--source") + 1] : undefined;
const src = srcArg ?? path.join(process.env.APPDATA, "Noxticord", "dist");
const dst = path.join(ROOT, "payload");

if (!fs.existsSync(src)) { console.error("source introuvable: " + src); process.exit(1); }
fs.rmSync(dst, { recursive: true, force: true });
fs.cpSync(src, dst, {
  recursive: true,
  filter: (p) => !/\.(bak2?|map|log)$/i.test(path.basename(p)),
});
const n = [...(function* walk(d) {
  for (const e of fs.readdirSync(d, { withFileTypes: true })) {
    const f = path.join(d, e.name);
    if (e.isDirectory()) yield* walk(f); else yield f;
  }
})(dst)].length;
console.log(`payload → ${dst} (${n} fichiers)`);
