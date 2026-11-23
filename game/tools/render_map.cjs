#!/usr/bin/env node
/* Рендерит city_map.svg в 4К PNG.
 *
 * Зависимость: @resvg/resvg-js  (npm i @resvg/resvg-js)
 * Шрифты: DejaVu (npm i dejavu-fonts-ttf) — путь можно задать через FONT_DIR.
 *
 * Запуск:  node game/tools/render_map.cjs
 * Результат: game/map/city_map.png  3840 × 2720
 */
const fs = require("fs");
const path = require("path");

let Resvg;
try {
  ({ Resvg } = require("@resvg/resvg-js"));
} catch (e) {
  ({ Resvg } = require("/tmp/tools/node_modules/@resvg/resvg-js"));
}

const here = __dirname;
const root = path.resolve(here, "..", "..");
const svgPath = path.join(root, "game", "map", "city_map.svg");
const outPath = path.join(root, "game", "map", "city_map.png");
const svg = fs.readFileSync(svgPath, "utf8");

const fontDir = process.env.FONT_DIR || "/tmp/tools/node_modules/dejavu-fonts-ttf/ttf";
const fontFiles = ["DejaVuSans.ttf", "DejaVuSans-Bold.ttf"].map((f) =>
  path.join(fontDir, f)
).filter((f) => fs.existsSync(f));

const resvg = new Resvg(svg, {
  fitTo: { mode: "width", value: 3840 },
  font: {
    loadSystemFonts: fontFiles.length === 0,
    fontFiles,
    defaultFontFamily: "DejaVu Sans",
  },
  background: "#0a2230",
});
fs.writeFileSync(outPath, resvg.render().asPng());
const size = fs.statSync(outPath).size / 1048576;
console.log(`game/map/city_map.png — 3840 × 2720, ${size.toFixed(2)} МБ`);
