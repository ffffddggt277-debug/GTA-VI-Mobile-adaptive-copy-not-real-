#!/usr/bin/env node
/* Проверяет GLB через glTF-Validator, подгружая внешние текстуры с диска.
 *
 * Запуск: node game/tools/validate_glb.cjs models/generated/houses/house_00.glb
 * Зависимость: npm i gltf-validator
 */
const fs = require("fs");
const path = require("path");

let validator;
try {
  validator = require("gltf-validator");
} catch (e) {
  validator = require("/tmp/tools/node_modules/gltf-validator");
}

const files = process.argv.slice(2);
(async () => {
  let bad = 0;
  for (const file of files) {
    const base = path.dirname(path.resolve(file));
    const data = new Uint8Array(fs.readFileSync(file));
    const report = await validator.validateBytes(data, {
      uri: path.basename(file),
      maxIssues: 50,
      externalResourceFunction: (uri) =>
        Promise.resolve(fs.readFileSync(path.join(base, decodeURIComponent(uri)))),
    });
    const s = report.issues;
    console.log(`${path.basename(file)}  errs ${s.numErrors}  warns ${s.numWarnings}`);
    if (s.numErrors) {
      bad++;
      for (const m of s.messages.filter((m) => m.severity === 0).slice(0, 8)) {
        console.log(`  E ${m.code} ${m.pointer} ${m.message}`);
      }
    }
  }
  process.exit(bad ? 1 : 0);
})();
