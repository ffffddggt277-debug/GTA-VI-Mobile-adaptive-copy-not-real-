# Подготовка моделей

_Обновлено: 2026-11-22_

Что сделано с моделями Sketchfab (дома, транспорт, оружие, персонажи):

- **Разделение на детали.** Каждый монолитный меш разбит по связным кускам: крупные
  оболочки стали отдельными узлами (`<имя>__part_001`, `…_002`, …), мелкая мелочь
  собрана в один узел `…__misc`. У моделей, где детали уже были отдельными узлами,
  разбиение не трогали — только нормализовали пивоты.
- **Пивот в центре детали.** Начало координат каждого узла перенесено в центр его
  bounding box. Дверь, колесо и магазин теперь крутятся вокруг себя без смещения:
  достаточно `node.rotation` вокруг нужной оси.
- **Кости.** Все люди/полицейские/SWAT/киборги уже пришли с полными скелетами
  (mixamo или Character Creator, 56–146 костей) и весами кожи — риг не добавлялся,
  чтобы не ломать оригинальную развесовку. Дрон и сцена `wip-chicago-crime` —
  не люди: у дрона анимация узлов, сцена собрана из статичных объектов.
- **Метаданные.** В каждом GLB есть `extras.prepared` (сколько деталей, способ пивота).
  Полный список деталей с центрами и размерами — в `prepared_manifest.json`.

Единицы у источников разные (м, см, мм, «свои» единицы Sketchfab), поэтому размеры в таблицах — как в файле; масштаб под свою сцену подбирайте при импорте.

Не тронуто: текстуры, материалы, UV, нормали, скины, существующие анимации. Геометрия и мировые габариты сверены с оригиналами (число треугольников и bbox совпадают).

Всего моделей: **69**, деталей: **9837**, треугольников: **11,928,702**, файлов с разбиением: **50**.

> Обозначения: «Деталей» — число узлов с мешем в файле; «Кости» — число узлов скелета;
> «Анимации» — готовые клипы внутри GLB.

## Дома (17)

| Модель | Файл | Деталей | Треугольников | Габариты, ед. | Кости | Анимации |
| --- | --- | ---: | ---: | --- | ---: | --- |
| A modern house design | `a-modern-house-design__e2c66f68ca6746df8d0c1aedafe24e56.glb` | 339 | 235,016 | 0.51×0.11×0.32 | — | — |
| American Colonial house | `american-colonial-house__3e536217e25545448762004ec55f1786.glb` | 105 | 78,716 | 21.95×11.78×13.02 | — | — |
| American House | `american-house__798ffaeadacf4e08a2665785422fb45d.glb` | 103 | 41,990 | 19.50×8.64×15.47 | — | — |
| country house | `country-house__8c5d5230b9374fa3934be5fe16c039b9.glb` | 81 | 283,656 | 58.11×20.12×58.54 | — | — |
| Final House | `final-house__20ea8edb2b7043b1a98a0b6ae18684bb.glb` | 57 | 137,986 | 11.49×9.11×13.96 | — | — |
| House | `house__a667b2ca217c439c97a57f20d62bb8e6.glb` | 81 | 340,618 | 46.33×11.21×60.66 | — | — |
| Macchiato Mansion | `macchiato-mansion__c9d13217f4cc4f098d8c221a9dbbbc27.glb` | 3789 | 125,572 | 16.89×11.65×15.35 | — | — |
| Modern luxury villa house building with pool | `modern-luxury-villa-house-building-with-pool__0e858284939343cb994233d40a48a20a.glb` | 1345 | 133,529 | 27.05×9.79×34.99 | — | — |
| Modular House l Garage l Shed l Pool l Kit | `modular-house-l-garage-l-shed-l-pool-l-kit__7dfa7d64895d4b1394162401366e806a.glb` | 203 | 1,329,452 | 26.72×5.99×35.35 | — | — |
| Old \| House \| Wooden \| Enterable \| Rusty | `old-house-wooden-enterable-rusty__7f496b3002d24341a618408cb8221e8c.glb` | 20 | 24,231 | 11.63×8.20×11.32 | — | — |
| residential complex modern apartment building | `residential-complex-modern-apartment-building__d4959af16d8f4524844e8b97d3e9772b.glb` | 64 | 192,960 | 49.10×44.74×88.33 | — | — |
| Russian residential building: P-44K | `russian-residential-building-p-44k__6536b723e61640c8a85c5d250985fd5a.glb` | 13 | 119,686 | 0.22×0.99×0.67 | — | — |
| Suburban House | `suburban-house__efcc220c62f641d29a1480e8b221c7b3.glb` | 84 | 146,269 | 16.43×7.45×15.58 | — | — |
| Suburban House V2 | `suburban-house-v2__92b305cf37ad4b0daec1630028e4499c.glb` | 85 | 82,399 | 16.43×7.45×15.58 | — | — |
| The House | `the-house__559897d3f08f4a4f860779dc6406b427.glb` | 40 | 88,072 | 1.93×1.12×1.56 | — | — |
| Urban Building | `urban-building__1704ecbc09a24f358332fd358c818d18.glb` | 45 | 112,566 | 8.37×9.53×5.88 | — | — |
| Winter Country House | `winter-country-house__7673ffdea07e4a2b9e6dfc31f16a5eee.glb` | 679 | 124,005 | 27.27×15.86×27.19 | — | — |

## Транспорт (15)

| Модель | Файл | Деталей | Треугольников | Габариты, ед. | Кости | Анимации |
| --- | --- | ---: | ---: | --- | ---: | --- |
| (FREE) Porsche 911 Carrera 4S | `free-porsche-911-carrera-4s__d01b254483794de3819786d93e0e1ebf.glb` | 45 | 652,660 | 2.75×1.26×5.08 | — | — |
| 2016 \| Ford Mustang Police Barricade | `2016-ford-mustang-police-barricade__d54ad13ac57d41c28479db79009ae71e.glb` | 81 | 713,723 | 2.36×1.69×5.82 | — | — |
| Ford F150 Raptor Police [ 2026 UPDATE ] | `ford-f150-raptor-police-2026-update__840ede58ed40499a9cf332e25129f0b2.glb` | 261 | 657,785 | 2.46×2.53×6.26 | — | Object_221Action, Object_93Action.001… |
| Generic civil service vehicles pack | `generic-civil-service-vehicles-pack__8ff2a13f30914932a70c7950cfa58465.glb` | 43 | 92,707 | 26.92×2.94×27.80 | — | — |
| Generic passenger car pack | `generic-passenger-car-pack__20f9af9b8a404d5cb022ac6fe87f21f5.glb` | 75 | 69,312 | 19.27×1.76×19.27 | — | — |
| Harley-Davidson Police | `harley-davidson-police__79dc00298bf449d7bcfdf6eb32bd15ac.glb` | 81 | 658,328 | 2945.61×1181.90×3667.08 | — | — |
| Hot Rod Pickup | `hot-rod-pickup__7b882dc2d7ed407689f8e851d5f8250f.glb` | 67 | 531,676 | 2.08×1.54×4.03 | — | — |
| Iveco Daily Van 2014 | `iveco-daily-van-2014__30003fba5b074d6eb7bc74fd5e1e7d19.glb` | 22 | 89,822 | 3.24×2.75×7.30 | — | CINEMA_4D_Main |
| Los Angeles Police Department car | `los-angeles-police-department-car__795fa0620bc44db8afcb06720a4dadd7.glb` | 38 | 241,902 | — | 64 | Take 01 |
| Mersedes CLS 63 \| www.vecarz.com | `mersedes-cls-63-www-vecarz-com__a5e6b676eb484211bf13d9fa48bb2c51.glb` | 156 | 126,859 | 0.08×0.06×0.19 | — | — |
| Oldsmobile Cutlass Supreme Sedan '71 | `oldsmobile-cutlass-supreme-sedan-71__78f76d386a4341b0b71745bdc50fd5ab.glb` | 81 | 70,258 | 278.90×66.55×278.90 | — | — |
| Police car | `police-car__9166b13b6ae341f4bfc093edb71d74f4.glb` | 13 | 256,420 | — | 64 | — |
| Porsche Mission R \| www.vecarz.com | `porsche-mission-r-www-vecarz-com__86c216df075a489f8dee85e3d891793c.glb` | 88 | 262,972 | 9.75×7.18×20.83 | — | — |
| Sports Bike | `sports-bike__a80259b859c842d5824c25c61e0fc421.glb` | 86 | 641,501 | 0.84×1.13×2.10 | — | — |
| Tesla Roadster 2020 | `tesla-roadster-2020__1fbf29e297bd4a17ac39a00a378441d8.glb` | 105 | 608,850 | 1.89×1.18×4.63 | — | — |

## Оружие (26)

| Модель | Файл | Деталей | Треугольников | Габариты, ед. | Кости | Анимации |
| --- | --- | ---: | ---: | --- | ---: | --- |
| Adaptive Combat Rifle | `adaptive-combat-rifle__a18a8008a299430caa5f1fe05563c949.glb` | 36 | 19,950 | 700.00×156.39×700.00 | — | — |
| AKM Assault Rifle - Animated | `akm-assault-rifle-animated__6348dc53e3e249c6b297cb8f3f1e3e9b.glb` | 10 | 33,304 | — | 60 | Scene |
| Assault rifle M4A1 | `assault-rifle-m4a1__1b858701dd7142d9b82bbe3fd121a97b.glb` | 54 | 58,599 | 8.85×34.49×104.20 | — | — |
| CZ Scorpion EVO 3 S1 | `cz-scorpion-evo-3-s1__fc8f5444abd54c37ae672c12b08a0560.glb` | 33 | 137,147 | 0.08×0.31×0.69 | — | — |
| Desert Eagle | `desert-eagle__cabde59f5cf24effaf80536e35d04e95.glb` | 51 | 13,279 | 4.91×15.75×27.68 | — | — |
| Franchi SPAS 12 | `franchi-spas-12__8793e33f94d4448197316aa1adf320a9.glb` | 27 | 14,853 | 0.06×0.25×1.19 | — | — |
| G17 Pistol - Animated | `g17-pistol-animated__7723ab14e70446ef8e06a134984f287f.glb` | 14 | 60,167 | — | 60 | Scene |
| GameReady: Colt Python Revolver | `gameready-colt-python-revolver__3def6e3980e64dfa832f298004ce1b94.glb` | 37 | 7,986 | 0.07×0.13×0.29 | — | — |
| HK 416 A7 (free download) | `hk-416-a7-free-download__a16768f0ffa24737a4fe324a411cfbd9.glb` | 25 | 30,088 | 86.63×25.68×22.35 | — | — |
| HK MP5 SMG | `hk-mp5-smg__fad0226e1dcf4af180f707a5509d3a4e.glb` | 28 | 32,171 | 0.08×0.37×0.90 | — | — |
| Kimber 1911 | `kimber-1911__12b70050f764456fb4db69a37a96c505.glb` | 28 | 19,489 | 0.35×1.34×2.46 | — | — |
| Kimber Stainless TLE/RL II 1911 .45ACP | `kimber-stainless-tle-rl-ii-1911-45acp__9b2503a1283b4c1fa506510c9dc7d1f5.glb` | 8 | 61,216 | — | 21 | — |
| M16 Assault Rifle | `m16-assault-rifle__339d0f7b21024387853dd926a5d51b50.glb` | 33 | 21,549 | 66.70×221.91×875.08 | — | — |
| Makarov Pistol - Animated | `makarov-pistol-animated__2d8353ff5a374517b9132a617939cfc9.glb` | 9 | 21,567 | — | 60 | Scene |
| Masterpiece Arms 30 SST SMG | `masterpiece-arms-30-sst-smg__279c4d07eae4456fa5cc169cc7e00766.glb` | 118 | 39,309 | 1.52×1.40×3.06 | — | — |
| Pistol 9mm Custom | `pistol-9mm-custom__7bb44ec1d8e14925aeebbfa8374b87cf.glb` | 254 | 92,804 | 0.23×0.29×1.58 | — | — |
| Remington 870 Police Magnum 12 Gauge Shotgun | `remington-870-police-magnum-12-gauge-shotgun__eea11de7e9d24b6683962b8388c319eb.glb` | 24 | 17,476 | 0.43×1.81×9.74 | — | — |
| Saiga-9mm | `saiga-9mm__3d031dfeb2ad45beb19e89977225a631.glb` | 62 | 24,320 | 0.94×0.35×0.19 | — | — |
| Shotgun Benelli M4 | `shotgun-benelli-m4__e0d23db5a8b64ff2980875834b425c4b.glb` | 37 | 27,722 | 3.61×1.43×0.24 | — | — |
| SLR AR-15 | `slr-ar-15__429b6ef8516b46dca8992e78c4bf7a55.glb` | 27 | 41,573 | 0.07×0.29×0.76 | — | — |
| SMG_Kriss_V | `smg-kriss-v__5d2a5e8bff1445a78aea42c8da7485d8.glb` | 92 | 44,996 | 837.58×772.43×1815.52 | — | — |
| Springfield M1903 Rifle | `springfield-m1903-rifle__224bd15dc9c544afa78af6c3a46fe22f.glb` | 27 | 32,128 | 1.47×0.12×2.00 | — | — |
| SR-2M Veresk Tactical / Tacticool / Custom | `sr-2m-veresk-tactical-tacticool-custom__13de87a73ac044709795373ab392b967.glb` | 87 | 55,272 | 0.09×0.28×0.74 | — | — |
| Steyr_Aug_A1 | `steyr-aug-a1__b15b50b5a2044439b5aa1abfe985c337.glb` | 32 | 673,057 | 6.99×23.50×68.95 | — | — |
| Thompson submachine gun | `thompson-submachine-gun__5fe11dfebf734b4aa49c43545397abd1.glb` | 33 | 20,802 | 2.00×0.48×0.36 | — | — |
| Triple barrel shotgun 4K textures | `triple-barrel-shotgun-4k-textures__79223862a1f84538ba84972fe1c8ee9a.glb` | 25 | 17,160 | 85.69×23.00×28.36 | — | — |

## Персонажи (11)

| Модель | Файл | Деталей | Треугольников | Габариты, ед. | Кости | Анимации |
| --- | --- | ---: | ---: | --- | ---: | --- |
| Buster Drone | `buster-drone__294e79652f494130ad2ab00a13fdbafd.glb` | 39 | 32,720 | 4.08×1.52×4.71 | — | Start_Liftoff |
| Connor RK900 - Detroit Become Human | `connor-rk900-detroit-become-human__623e3f52b7da4116a4576f38479eaaaf.glb` | 16 | 52,813 | — | 66 | idle |
| Cyberpunk character | `cyberpunk-character__019f4b3fd3c74ed0bc6c8dbe9cd50d51.glb` | 9 | 24,500 | — | 56 | A-pose |
| Gavin Reed - Detroit Become Human | `gavin-reed-detroit-become-human__0161bd8ecbaa44cb9da40ba4f2f80aaf.glb` | 14 | 50,000 | — | 66 | idle |
| IWI Male Character 02 | `iwi-male-character-02__774ca00136b94d359e7d3ba425a5c57a.glb` | 6 | 19,938 | — | 66 | mixamo.com |
| Police | `police__8f939bfdf22f476d91f480f4d6f5d72a.glb` | 7 | 23,725 | — | 66 | mixamo.com |
| PoliceMan | `policeman__d47bb1f61fa340588b5801cdb63812c8.glb` | 11 | 29,474 | — | 66 | mixamo.com |
| Robot Police Unit (animated) | `robot-police-unit-animated__36bc284bf5b84a6597db8f6db053c095.glb` | 1 | 20,053 | — | 102 | 3903460652032_TempMotion |
| S.W.A.T. Operator | `s-w-a-t-operator__9e82fabf26194896b5ad4a364d864eab.glb` | 42 | 106,711 | — | 66 | Animation |
| Stylized Sci-Fi Officer with Gun (Animated) | `stylized-sci-fi-officer-with-gun-animated__ea2b996c62024746adb892b2faf08223.glb` | 10 | 179,316 | — | 146 | Sci_fi_Soldier_black_orange\|Armature… |
| WIP - Chicago crime | `wip-chicago-crime__7c5fb45e696046e3b135406abd6ac58e.glb` | 96 | 499,970 | 264.64×30.00×283.07 | — | Object_0 |

## Как пользоваться

1. Импортируйте GLB в Unity 6 (URP) или Godot 4 — формат читается без конвертации.
   Если модель попала в таблицу «Масштаб», умножьте её корневой узел на `suggestedScale`.
2. Детали лежат дочерними узлами группы с именем исходного объекта:
   `Doorknob_1k` → `Doorknob_1k__part_001`, `Doorknob_1k__part_002`, …
3. Поворот детали: меняйте `rotation` самого узла детали — пивот уже в её центре.
4. Для физики берите `size` детали из `prepared_manifest.json` как размер коллайдера.
5. Оружие и персонажи с анимациями оставлены как есть — скины и клипы не менялись.

## Что не менялось

- `models/polyhaven/` — 446 готовых CC0-пропов (один объект = один меш, PBR-текстуры 1K):
  дробить кресло или бочку на осколки смысла нет.
- `models/generated/houses/` — 12 домов уже собраны из отдельных узлов, у дверей есть
  готовый клип `Door_FrontAction`.
- `models/khronos/` — эталонные модели glTF, включая `RiggedFigure` со скелетом.
- Анимированные модели (оружие с перезарядкой, машины с анимацией, персонажи) не
  переписывались: пересборка буферов сломала бы сэмплеры анимаций.

## Дальше

Анимации дверей/колёс/затворов не добавлялись — по договорённости это следующий шаг.
Все пивоты для них уже готовы.
