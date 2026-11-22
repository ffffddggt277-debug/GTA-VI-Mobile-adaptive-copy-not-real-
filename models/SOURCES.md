# Каталог 3D-ассетов

Все модели скачаны из открытых источников и лежат локально в `models/`.
Формат: **glTF 2.0 / GLB + PBR-текстуры 1K** — импортируется в Blender, Unity, Unreal, Godot, three.js без конвертации.

Всего моделей: **467**

## Источники и лицензии

| Источник | Лицензия | Что покрывает |
|---|---|---|
| [Poly Haven](https://polyhaven.com/models) | CC0 1.0 (public domain) | 446 фотореалистичных моделей: мебель, интерьеры, техника, инструменты, еда, контейнеры, фасады, двери, оружие |
| [Sketchfab](https://sketchfab.com) | CC-BY 4.0 / CC-BY-SA 4.0 | 5 моделей с ригом и техники; автор обязателен к указанию (см. таблицу ниже) |
| Сгенерировано `tools/make_houses.py` | — | 12 домов: отдельные двери с анимацией, прозрачные стёкла (KHR_materials_transmission) |
| [Khronos glTF Sample Assets](https://github.com/KhronosGroup/glTF-Sample-Assets) | CC-BY 4.0 | CarConcept, CesiumMilkTruck, RiggedFigure. ToyCar / CesiumMan / Fox удалены как нереалистичные |
| Сгенерировано скриптом `tools/make_money.py` | — | пачки банкнот (деньги), без воспроизведения реального дизайна валюты |

CC0 — можно использовать в коммерческой игре без указания автора (указание приветствуется).
CC-BY 4.0 — требуется указать автора в титрах; текст лицензии лежит рядом с моделью в `LICENSE-README.md`.

## Транспорт, персонажи, оружие

| Модель | Категория | Лицензия |
|---|---|---|
| `models/khronos/CarConcept/CarConcept.glb` | Транспорт | CC-BY 4.0 (Darmstadt Graphics Group / Khronos) |
| `models/khronos/CesiumMilkTruck/CesiumMilkTruck.glb` | Транспорт | CC-BY 4.0 (Cesium) |
| `models/khronos/RiggedFigure/RiggedFigure.glb` | Персонаж с ригом | CC-BY 4.0 (Cesium) |
| `models/generated/money_stack.glb` | Деньги (процедурные) | генерируется локально |

RiggedFigure — со **скелетом и анимациями**; люди со Sketchfab — с ригом (см. таблицу).

## Poly Haven (CC0)

| Модель | Категории | Размер | Файл |
|---|---|---|---|
| [Arm Chair 01](https://polyhaven.com/a/ArmChair_01) | Мебель, seating | 0.77 МБ | `polyhaven/ArmChair_01/ArmChair_01_1k.gltf` |
| [Barber Shop Chair 01](https://polyhaven.com/a/BarberShopChair_01) | Мебель, seating | 0.88 МБ | `polyhaven/BarberShopChair_01/BarberShopChair_01_1k.gltf` |
| [Barrel_01](https://polyhaven.com/a/Barrel_01) | Контейнеры, industrial | 0.69 МБ | `polyhaven/Barrel_01/Barrel_01_1k.gltf` |
| [Barrel 02](https://polyhaven.com/a/Barrel_02) | collection: the_shed, Контейнеры, industrial | 0.42 МБ | `polyhaven/Barrel_02/Barrel_02_1k.gltf` |
| [Camera 01](https://polyhaven.com/a/Camera_01) | Электроника, Реквизит | 2.44 МБ | `polyhaven/Camera_01/Camera_01_1k.gltf` |
| [Cash Register 01](https://polyhaven.com/a/CashRegister_01) | Бытовая техника, Реквизит | 1.04 МБ | `polyhaven/CashRegister_01/CashRegister_01_1k.gltf` |
| [Chandelier 01](https://polyhaven.com/a/Chandelier_01) | Декор, Освещение | 1.31 МБ | `polyhaven/Chandelier_01/Chandelier_01_1k.gltf` |
| [Chandelier 02](https://polyhaven.com/a/Chandelier_02) | Освещение | 1.18 МБ | `polyhaven/Chandelier_02/Chandelier_02_1k.gltf` |
| [Chandelier 03](https://polyhaven.com/a/Chandelier_03) | Декор, Освещение | 1.36 МБ | `polyhaven/Chandelier_03/Chandelier_03_1k.gltf` |
| [CheeseBox_01](https://polyhaven.com/a/CheeseBox_01) | collection: the_shed, Контейнеры, Реквизит | 0.49 МБ | `polyhaven/CheeseBox_01/CheeseBox_01_1k.gltf` |
| [Classic Console 01](https://polyhaven.com/a/ClassicConsole_01) | Мебель, table | 0.74 МБ | `polyhaven/ClassicConsole_01/ClassicConsole_01_1k.gltf` |
| [Classic Nightstand 01](https://polyhaven.com/a/ClassicNightstand_01) | Мебель, table | 0.44 МБ | `polyhaven/ClassicNightstand_01/ClassicNightstand_01_1k.gltf` |
| [Coffee Cart 01](https://polyhaven.com/a/CoffeeCart_01) | Бытовая техника, Мебель, Офис, Реквизит | 1.84 МБ | `polyhaven/CoffeeCart_01/CoffeeCart_01_1k.gltf` |
| [Coffee Table 01](https://polyhaven.com/a/CoffeeTable_01) | Мебель, table | 0.96 МБ | `polyhaven/CoffeeTable_01/CoffeeTable_01_1k.gltf` |
| [Drill 01](https://polyhaven.com/a/Drill_01) | collection: the_shed, Электроника, Реквизит, Инструменты | 0.5 МБ | `polyhaven/Drill_01/Drill_01_1k.gltf` |
| [Gothic Bed 01](https://polyhaven.com/a/GothicBed_01) | bed, Мебель | 1.16 МБ | `polyhaven/GothicBed_01/GothicBed_01_1k.gltf` |
| [Gothic Cabinet 01](https://polyhaven.com/a/GothicCabinet_01) | Мебель, shelves | 1.02 МБ | `polyhaven/GothicCabinet_01/GothicCabinet_01_1k.gltf` |
| [Gothic Commode 01](https://polyhaven.com/a/GothicCommode_01) | Мебель, shelves | 0.6 МБ | `polyhaven/GothicCommode_01/GothicCommode_01_1k.gltf` |
| [Green Chair 01](https://polyhaven.com/a/GreenChair_01) | Мебель, seating | 0.61 МБ | `polyhaven/GreenChair_01/GreenChair_01_1k.gltf` |
| [Lantern 01](https://polyhaven.com/a/Lantern_01) | Освещение, Реквизит | 1.42 МБ | `polyhaven/Lantern_01/Lantern_01_1k.gltf` |
| [Megaphone 01](https://polyhaven.com/a/Megaphone_01) | Электроника, Реквизит, Инструменты | 0.78 МБ | `polyhaven/Megaphone_01/Megaphone_01_1k.gltf` |
| [Ottoman 01](https://polyhaven.com/a/Ottoman_01) | Мебель, seating | 0.65 МБ | `polyhaven/Ottoman_01/Ottoman_01_1k.gltf` |
| [Rockingchair 01](https://polyhaven.com/a/Rockingchair_01) | Мебель, seating | 0.8 МБ | `polyhaven/Rockingchair_01/Rockingchair_01_1k.gltf` |
| [School Chair 01](https://polyhaven.com/a/SchoolChair_01) | Мебель, seating | 0.48 МБ | `polyhaven/SchoolChair_01/SchoolChair_01_1k.gltf` |
| [School Desk 01](https://polyhaven.com/a/SchoolDesk_01) | Мебель | 0.42 МБ | `polyhaven/SchoolDesk_01/SchoolDesk_01_1k.gltf` |
| [Shelf 01](https://polyhaven.com/a/Shelf_01) | Мебель, shelves | 0.61 МБ | `polyhaven/Shelf_01/Shelf_01_1k.gltf` |
| [Sofa 01](https://polyhaven.com/a/Sofa_01) | Мебель, seating | 0.52 МБ | `polyhaven/Sofa_01/Sofa_01_1k.gltf` |
| [Television 01](https://polyhaven.com/a/Television_01) | collection: the_shed, Электроника, Реквизит | 0.53 МБ | `polyhaven/Television_01/Television_01_1k.gltf` |
| [Ukulele 01](https://polyhaven.com/a/Ukulele_01) | instrument, Реквизит | 0.81 МБ | `polyhaven/Ukulele_01/Ukulele_01_1k.gltf` |
| [Wet Floor Sign 01](https://polyhaven.com/a/WetFloorSign_01) | industrial, Офис, Реквизит | 0.27 МБ | `polyhaven/WetFloorSign_01/WetFloorSign_01_1k.gltf` |
| [Wooden Chair 01](https://polyhaven.com/a/WoodenChair_01) | Мебель, seating | 1.08 МБ | `polyhaven/WoodenChair_01/WoodenChair_01_1k.gltf` |
| [Wooden Table 01](https://polyhaven.com/a/WoodenTable_01) | Мебель | 0.55 МБ | `polyhaven/WoodenTable_01/WoodenTable_01_1k.gltf` |
| [Wooden Table 02](https://polyhaven.com/a/WoodenTable_02) | Мебель, table | 0.5 МБ | `polyhaven/WoodenTable_02/WoodenTable_02_1k.gltf` |
| [Wooden Table 03](https://polyhaven.com/a/WoodenTable_03) | collection: the_shed, Мебель | 0.64 МБ | `polyhaven/WoodenTable_03/WoodenTable_03_1k.gltf` |
| [Adjustable Wrench](https://polyhaven.com/a/adjustable_wrench) | collection: project_lighthouse, industrial, Реквизит, Инструменты | 2.64 МБ | `polyhaven/adjustable_wrench/adjustable_wrench_1k.gltf` |
| [Alarm Clock 01](https://polyhaven.com/a/alarm_clock_01) | Декор, Реквизит, С ригом | 1.66 МБ | `polyhaven/alarm_clock_01/alarm_clock_01_1k.gltf` |
| [All Purpose Cleaner](https://polyhaven.com/a/all_purpose_cleaner) | collection: the_shed, Контейнеры, Реквизит | 1.06 МБ | `polyhaven/all_purpose_cleaner/all_purpose_cleaner_1k.gltf` |
| [American Football](https://polyhaven.com/a/american_football) | Реквизит | 2.67 МБ | `polyhaven/american_football/american_football_1k.gltf` |
| [Ammo Box](https://polyhaven.com/a/ammo_box) | collection: project_lighthouse, industrial, Инструменты | 1.47 МБ | `polyhaven/ammo_box/ammo_box_1k.gltf` |
| [Anthurium Botany 01](https://polyhaven.com/a/anthurium_botany_01) | collection: smugglers_cove, collection: the_shed, nature, plants | 3.24 МБ | `polyhaven/anthurium_botany_01/anthurium_botany_01_1k.gltf` |
| [Antique Ceramic Vase 01](https://polyhaven.com/a/antique_ceramic_vase_01) | Контейнеры, Декор, vases | 0.53 МБ | `polyhaven/antique_ceramic_vase_01/antique_ceramic_vase_01_1k.gltf` |
| [Antique Estoc](https://polyhaven.com/a/antique_estoc) | Реквизит | 2.53 МБ | `polyhaven/antique_estoc/antique_estoc_1k.gltf` |
| [Antique Katana 01](https://polyhaven.com/a/antique_katana_01) | Реквизит | 0.65 МБ | `polyhaven/antique_katana_01/antique_katana_01_1k.gltf` |
| [Bananas](https://polyhaven.com/a/bananas) | collection: project_lighthouse, Еда, nature | 2.48 МБ | `polyhaven/bananas/bananas_1k.gltf` |
| [Bar Chair Round 01](https://polyhaven.com/a/bar_chair_round_01) | Мебель, seating | 2.81 МБ | `polyhaven/bar_chair_round_01/bar_chair_round_01_1k.gltf` |
| [Barrel 03](https://polyhaven.com/a/barrel_03) | Контейнеры, industrial | 0.57 МБ | `polyhaven/barrel_03/barrel_03_1k.gltf` |
| [Barrel Stove](https://polyhaven.com/a/barrel_stove) | collection: hidden_alley, Контейнеры, industrial | 2.15 МБ | `polyhaven/barrel_stove/barrel_stove_1k.gltf` |
| [Baseball 01](https://polyhaven.com/a/baseball_01) | Реквизит | 1.08 МБ | `polyhaven/baseball_01/baseball_01_1k.gltf` |
| [Baseball Bat](https://polyhaven.com/a/baseball_bat) | collection: hidden_alley, Реквизит | 1.67 МБ | `polyhaven/baseball_bat/baseball_bat_1k.gltf` |
| [Bench Vice 01](https://polyhaven.com/a/bench_vice_01) | collection: the_shed, industrial, С ригом, Инструменты | 2.44 МБ | `polyhaven/bench_vice_01/bench_vice_01_1k.gltf` |
| [Binder Notebook](https://polyhaven.com/a/binder_notebook) | books, collection: project_lighthouse, Реквизит | 1.89 МБ | `polyhaven/binder_notebook/binder_notebook_1k.gltf` |
| [Binoculars](https://polyhaven.com/a/binoculars) | collection: project_lighthouse, Реквизит, Инструменты | 1.95 МБ | `polyhaven/binoculars/binoculars_1k.gltf` |
| [Bleach Bottle](https://polyhaven.com/a/bleach_bottle) | collection: the_shed, Контейнеры, Реквизит | 1.27 МБ | `polyhaven/bleach_bottle/bleach_bottle_1k.gltf` |
| [Bolt Action Rifle 7.62](https://polyhaven.com/a/bolt_action_rifle_7_62) | collection: project_lighthouse, Огнестрельное оружие, Оружие | 6.04 МБ | `polyhaven/bolt_action_rifle_7_62/bolt_action_rifle_7_62_1k.gltf` |
| [Bolt Cutters 01](https://polyhaven.com/a/bolt_cutters_01) | collection: the_shed, industrial, Реквизит, С ригом, Инструменты | 2.5 МБ | `polyhaven/bolt_cutters_01/bolt_cutters_01_1k.gltf` |
| [Book Encyclopedia Set 01](https://polyhaven.com/a/book_encyclopedia_set_01) | books, Декор, Реквизит | 3.26 МБ | `polyhaven/book_encyclopedia_set_01/book_encyclopedia_set_01_1k.gltf` |
| [Boombox](https://polyhaven.com/a/boombox) | Бытовая техника, collection: hidden_alley, Электроника | 3.07 МБ | `polyhaven/boombox/boombox_1k.gltf` |
| [Brass Blowtorch](https://polyhaven.com/a/brass_blowtorch) | Реквизит, Инструменты | 2.33 МБ | `polyhaven/brass_blowtorch/brass_blowtorch_1k.gltf` |
| [Brass Candleholders](https://polyhaven.com/a/brass_candleholders) | Декор, Освещение | 9.5 МБ | `polyhaven/brass_candleholders/brass_candleholders_1k.gltf` |
| [Brass Diya Lantern](https://polyhaven.com/a/brass_diya_lantern) | Декор, Освещение | 1.97 МБ | `polyhaven/brass_diya_lantern/brass_diya_lantern_1k.gltf` |
| [Brass Goblets](https://polyhaven.com/a/brass_goblets) | Контейнеры, Декор, Посуда, Реквизит | 4.78 МБ | `polyhaven/brass_goblets/brass_goblets_1k.gltf` |
| [Brass Pan 01](https://polyhaven.com/a/brass_pan_01) | Контейнеры, Декор, Посуда, Реквизит | 1.62 МБ | `polyhaven/brass_pan_01/brass_pan_01_1k.gltf` |
| [Brass Pot 01](https://polyhaven.com/a/brass_pot_01) | Контейнеры, Декор, Реквизит | 1.52 МБ | `polyhaven/brass_pot_01/brass_pot_01_1k.gltf` |
| [Brass Pot 02](https://polyhaven.com/a/brass_pot_02) | Контейнеры, Декор, Посуда, Реквизит | 1.41 МБ | `polyhaven/brass_pot_02/brass_pot_02_1k.gltf` |
| [Brass Vase 01](https://polyhaven.com/a/brass_vase_01) | Декор, vases | 0.84 МБ | `polyhaven/brass_vase_01/brass_vase_01_1k.gltf` |
| [Brass Vase 02](https://polyhaven.com/a/brass_vase_02) | Декор, vases | 1.83 МБ | `polyhaven/brass_vase_02/brass_vase_02_1k.gltf` |
| [Brass Vase 03](https://polyhaven.com/a/brass_vase_03) | Контейнеры, Декор, Реквизит, vases | 1.58 МБ | `polyhaven/brass_vase_03/brass_vase_03_1k.gltf` |
| [Brass Vase 04](https://polyhaven.com/a/brass_vase_04) | Контейнеры, Посуда, Реквизит, vases | 0.39 МБ | `polyhaven/brass_vase_04/brass_vase_04_1k.gltf` |
| [Bronze Ray Statue](https://polyhaven.com/a/bronze_ray_statue) | collection: project_lighthouse, Декор | 2.02 МБ | `polyhaven/bronze_ray_statue/bronze_ray_statue_1k.gltf` |
| [Bronze Shark Statue](https://polyhaven.com/a/bronze_shark_statue) | collection: project_lighthouse, Декор | 2.07 МБ | `polyhaven/bronze_shark_statue/bronze_shark_statue_1k.gltf` |
| [Bronze Whale Statue](https://polyhaven.com/a/bronze_whale_statue) | collection: project_lighthouse, Декор | 2.19 МБ | `polyhaven/bronze_whale_statue/bronze_whale_statue_1k.gltf` |
| [Bull Head](https://polyhaven.com/a/bull_head) | Декор | 2.02 МБ | `polyhaven/bull_head/bull_head_1k.gltf` |
| [Bunsen Burner](https://polyhaven.com/a/bunsen_burner) | collection: project_lighthouse, industrial, Реквизит, Инструменты | 2.13 МБ | `polyhaven/bunsen_burner/bunsen_burner_1k.gltf` |
| [Caged Hanging Light](https://polyhaven.com/a/caged_hanging_light) | collection: project_lighthouse, industrial, Освещение | 3.07 МБ | `polyhaven/caged_hanging_light/caged_hanging_light_1k.gltf` |
| [Calathea Orbifolia 01](https://polyhaven.com/a/calathea_orbifolia_01) | collection: smugglers_cove, ground cover, nature, plants | 1.88 МБ | `polyhaven/calathea_orbifolia_01/calathea_orbifolia_01_1k.gltf` |
| [Can Rusted](https://polyhaven.com/a/can_rusted) | collection: the_shed, Контейнеры | 2.11 МБ | `polyhaven/can_rusted/can_rusted_1k.gltf` |
| [Cannon 01](https://polyhaven.com/a/cannon_01) | collection: smugglers_cove, Реквизит, С ригом | 4.06 МБ | `polyhaven/cannon_01/cannon_01_1k.gltf` |
| [Cardboard Box 01](https://polyhaven.com/a/cardboard_box_01) | collection: the_shed, Контейнеры, industrial | 2.17 МБ | `polyhaven/cardboard_box_01/cardboard_box_01_1k.gltf` |
| [Carrot Cake](https://polyhaven.com/a/carrot_cake) | Еда, Реквизит | 3.36 МБ | `polyhaven/carrot_cake/carrot_cake_1k.gltf` |
| [Carved Wooden Elephant](https://polyhaven.com/a/carved_wooden_elephant) | Декор, Реквизит | 1.86 МБ | `polyhaven/carved_wooden_elephant/carved_wooden_elephant_1k.gltf` |
| [Carved Wooden Plate](https://polyhaven.com/a/carved_wooden_plate) | Декор, Посуда, Реквизит | 1.56 МБ | `polyhaven/carved_wooden_plate/carved_wooden_plate_1k.gltf` |
| [Cassette Player](https://polyhaven.com/a/cassette_player) | Электроника, Реквизит | 0.94 МБ | `polyhaven/cassette_player/cassette_player_1k.gltf` |
| [Ceiling Fan](https://polyhaven.com/a/ceiling_fan) | Бытовая техника, Электроника | 1.87 МБ | `polyhaven/ceiling_fan/ceiling_fan_1k.gltf` |
| [Cement Bag](https://polyhaven.com/a/cement_bag) | collection: project_lighthouse, Реквизит | 1.45 МБ | `polyhaven/cement_bag/cement_bag_1k.gltf` |
| [Ceramic Pot](https://polyhaven.com/a/ceramic_pot) | collection: project_lighthouse, Контейнеры, Декор, Посуда, Реквизит | 1.64 МБ | `polyhaven/ceramic_pot/ceramic_pot_1k.gltf` |
| [Ceramic Vase 01](https://polyhaven.com/a/ceramic_vase_01) | Контейнеры, Декор, vases | 0.42 МБ | `polyhaven/ceramic_vase_01/ceramic_vase_01_1k.gltf` |
| [Ceramic Vase 02](https://polyhaven.com/a/ceramic_vase_02) | Контейнеры, Декор, vases | 0.75 МБ | `polyhaven/ceramic_vase_02/ceramic_vase_02_1k.gltf` |
| [Ceramic Vase 03](https://polyhaven.com/a/ceramic_vase_03) | Контейнеры, Декор, vases | 0.77 МБ | `polyhaven/ceramic_vase_03/ceramic_vase_03_1k.gltf` |
| [Ceramic Vase 04](https://polyhaven.com/a/ceramic_vase_04) | Контейнеры, Декор, vases | 0.6 МБ | `polyhaven/ceramic_vase_04/ceramic_vase_04_1k.gltf` |
| [Chemistry Set](https://polyhaven.com/a/chemistry_set) | collection: project_lighthouse, industrial, Реквизит, Инструменты | 3.21 МБ | `polyhaven/chemistry_set/chemistry_set_1k.gltf` |
| [Chess Set](https://polyhaven.com/a/chess_set) | Декор, Реквизит | 6.98 МБ | `polyhaven/chess_set/chess_set_1k.gltf` |
| [Chinese Armchair](https://polyhaven.com/a/chinese_armchair) | Мебель, seating | 1.7 МБ | `polyhaven/chinese_armchair/chinese_armchair_1k.gltf` |
| [Chinese Cabinet](https://polyhaven.com/a/chinese_cabinet) | Мебель, shelves | 2.34 МБ | `polyhaven/chinese_cabinet/chinese_cabinet_1k.gltf` |
| [Chinese Chandelier](https://polyhaven.com/a/chinese_chandelier) | Декор, Освещение | 2.16 МБ | `polyhaven/chinese_chandelier/chinese_chandelier_1k.gltf` |
| [Chinese Commode](https://polyhaven.com/a/chinese_commode) | Мебель, shelves | 1.91 МБ | `polyhaven/chinese_commode/chinese_commode_1k.gltf` |
| [Chinese Console Table](https://polyhaven.com/a/chinese_console_table) | Мебель, table | 2.79 МБ | `polyhaven/chinese_console_table/chinese_console_table_1k.gltf` |
| [Chinese Screen Panels](https://polyhaven.com/a/chinese_screen_panels) | Мебель | 1.69 МБ | `polyhaven/chinese_screen_panels/chinese_screen_panels_1k.gltf` |
| [Chinese Sofa](https://polyhaven.com/a/chinese_sofa) | Мебель, seating | 1.57 МБ | `polyhaven/chinese_sofa/chinese_sofa_1k.gltf` |
| [Chinese Stool](https://polyhaven.com/a/chinese_stool) | Мебель, seating | 1.79 МБ | `polyhaven/chinese_stool/chinese_stool_1k.gltf` |
| [Chinese Tea Table](https://polyhaven.com/a/chinese_tea_table) | Мебель, table | 1.58 МБ | `polyhaven/chinese_tea_table/chinese_tea_table_1k.gltf` |
| [Cigarette Case](https://polyhaven.com/a/cigarette_case) | collection: project_lighthouse, Реквизит | 2.07 МБ | `polyhaven/cigarette_case/cigarette_case_1k.gltf` |
| [Cigarette Pack](https://polyhaven.com/a/cigarette_pack) | collection: project_lighthouse, Контейнеры, Реквизит | 1.36 МБ | `polyhaven/cigarette_pack/cigarette_pack_1k.gltf` |
| [Circuit Board](https://polyhaven.com/a/circuit_board) | collection: project_lighthouse, Электроника | 3.29 МБ | `polyhaven/circuit_board/circuit_board_1k.gltf` |
| [Classic Laptop](https://polyhaven.com/a/classic_laptop) | collection: project_lighthouse, Электроника, Офис | 2.18 МБ | `polyhaven/classic_laptop/classic_laptop_1k.gltf` |
| [Cleaner Tin 01](https://polyhaven.com/a/cleaner_tin_01) | collection: the_shed, Контейнеры, industrial, Инструменты | 1.82 МБ | `polyhaven/cleaner_tin_01/cleaner_tin_01_1k.gltf` |
| [Clipboard](https://polyhaven.com/a/clipboard) | collection: project_lighthouse, Офис, Реквизит | 1.79 МБ | `polyhaven/clipboard/clipboard_1k.gltf` |
| [Coast Land Rocks 02](https://polyhaven.com/a/coast_land_rocks_02) | collection: smugglers_cove, nature, rocks | 39.61 МБ | `polyhaven/coast_land_rocks_02/coast_land_rocks_02_1k.gltf` |
| [Coast Land Rocks 03](https://polyhaven.com/a/coast_land_rocks_03) | collection: smugglers_cove, nature, rocks | 34.0 МБ | `polyhaven/coast_land_rocks_03/coast_land_rocks_03_1k.gltf` |
| [Coast Land Rocks 04](https://polyhaven.com/a/coast_land_rocks_04) | collection: smugglers_cove, nature, rocks | 33.62 МБ | `polyhaven/coast_land_rocks_04/coast_land_rocks_04_1k.gltf` |
| [Coast Line 01](https://polyhaven.com/a/coast_line_01) | collection: smugglers_cove, nature, rocks | 17.73 МБ | `polyhaven/coast_line_01/coast_line_01_1k.gltf` |
| [Coast Line 02](https://polyhaven.com/a/coast_line_02) | collection: smugglers_cove, nature, rocks | 23.92 МБ | `polyhaven/coast_line_02/coast_line_02_1k.gltf` |
| [Coast Rocks 01](https://polyhaven.com/a/coast_rocks_01) | collection: smugglers_cove, nature, rocks | 21.68 МБ | `polyhaven/coast_rocks_01/coast_rocks_01_1k.gltf` |
| [Coast Rocks 02](https://polyhaven.com/a/coast_rocks_02) | collection: smugglers_cove, nature, rocks | 38.46 МБ | `polyhaven/coast_rocks_02/coast_rocks_02_1k.gltf` |
| [Coast Rocks 03](https://polyhaven.com/a/coast_rocks_03) | collection: smugglers_cove, nature, rocks | 25.64 МБ | `polyhaven/coast_rocks_03/coast_rocks_03_1k.gltf` |
| [Coast Rocks 05](https://polyhaven.com/a/coast_rocks_05) | collection: smugglers_cove, collection: verdant_trail, nature, rocks | 25.28 МБ | `polyhaven/coast_rocks_05/coast_rocks_05_1k.gltf` |
| [Coastal Cliff 01](https://polyhaven.com/a/coastal_cliff_01) | collection: smugglers_cove, nature | 15.38 МБ | `polyhaven/coastal_cliff_01/coastal_cliff_01_1k.gltf` |
| [Coastal Cliff 02](https://polyhaven.com/a/coastal_cliff_02) | collection: smugglers_cove, collection: verdant_trail, nature | 29.16 МБ | `polyhaven/coastal_cliff_02/coastal_cliff_02_1k.gltf` |
| [Coastal Cliff 04](https://polyhaven.com/a/coastal_cliff_04) | collection: smugglers_cove, collection: verdant_trail, nature, rocks | 46.48 МБ | `polyhaven/coastal_cliff_04/coastal_cliff_04_1k.gltf` |
| [Coffee Table Round 01](https://polyhaven.com/a/coffee_table_round_01) | Мебель, table | 1.56 МБ | `polyhaven/coffee_table_round_01/coffee_table_round_01_1k.gltf` |
| [Combination Wrench](https://polyhaven.com/a/combination_wrench) | collection: project_lighthouse, industrial, Инструменты | 1.46 МБ | `polyhaven/combination_wrench/combination_wrench_1k.gltf` |
| [Compost Bag 02](https://polyhaven.com/a/compost_bag_02) | collection: the_shed, Контейнеры, industrial | 2.35 МБ | `polyhaven/compost_bag_02/compost_bag_02_1k.gltf` |
| [Compost Bags](https://polyhaven.com/a/compost_bags) | collection: the_shed, Реквизит | 2.6 МБ | `polyhaven/compost_bags/compost_bags_1k.gltf` |
| [Concrete Cat Statue](https://polyhaven.com/a/concrete_cat_statue) | Декор | 3.1 МБ | `polyhaven/concrete_cat_statue/concrete_cat_statue_1k.gltf` |
| [Concrete Road Barrier](https://polyhaven.com/a/concrete_road_barrier) | collection: hidden_alley, industrial, Сооружения и улица | 3.75 МБ | `polyhaven/concrete_road_barrier/concrete_road_barrier_1k.gltf` |
| [Concrete Road Barrier 02](https://polyhaven.com/a/concrete_road_barrier_02) | collection: hidden_alley, industrial, Сооружения и улица | 2.93 МБ | `polyhaven/concrete_road_barrier_02/concrete_road_barrier_02_1k.gltf` |
| [Covered Car](https://polyhaven.com/a/covered_car) | collection: hidden_alley, industrial, Реквизит | 2.35 МБ | `polyhaven/covered_car/covered_car_1k.gltf` |
| [Croissant](https://polyhaven.com/a/croissant) | Еда, Реквизит | 2.1 МБ | `polyhaven/croissant/croissant_1k.gltf` |
| [Cross Pein Hammer](https://polyhaven.com/a/cross_pein_hammer) | collection: project_lighthouse, Реквизит, Инструменты | 1.51 МБ | `polyhaven/cross_pein_hammer/cross_pein_hammer_1k.gltf` |
| [Crowbar 01](https://polyhaven.com/a/crowbar_01) | collection: the_shed, industrial, Реквизит, Инструменты | 1.59 МБ | `polyhaven/crowbar_01/crowbar_01_1k.gltf` |
| [Dartboard](https://polyhaven.com/a/dartboard) | collection: the_shed, Декор, Реквизит, wall decoration | 3.03 МБ | `polyhaven/dartboard/dartboard_1k.gltf` |
| [Desk Lamp Arm 01](https://polyhaven.com/a/desk_lamp_arm_01) | collection: the_shed, Освещение, С ригом | 2.88 МБ | `polyhaven/desk_lamp_arm_01/desk_lamp_arm_01_1k.gltf` |
| [Digital Wrist Watch](https://polyhaven.com/a/digital_wrist_watch) | collection: project_lighthouse, Электроника, Реквизит | 2.16 МБ | `polyhaven/digital_wrist_watch/digital_wrist_watch_1k.gltf` |
| [Dining Chair 02](https://polyhaven.com/a/dining_chair_02) | Мебель, seating | 0.92 МБ | `polyhaven/dining_chair_02/dining_chair_02_1k.gltf` |
| [Dining Table](https://polyhaven.com/a/dining_table) | collection: project_lighthouse, Мебель, table | 2.28 МБ | `polyhaven/dining_table/dining_table_1k.gltf` |
| [Dirty Football](https://polyhaven.com/a/dirty_football) | Реквизит | 2.0 МБ | `polyhaven/dirty_football/dirty_football_1k.gltf` |
| [Drain Cleaner](https://polyhaven.com/a/drain_cleaner) | industrial, Реквизит, Инструменты | 1.65 МБ | `polyhaven/drain_cleaner/drain_cleaner_1k.gltf` |
| [Drawer Cabinet](https://polyhaven.com/a/drawer_cabinet) | Мебель | 1.14 МБ | `polyhaven/drawer_cabinet/drawer_cabinet_1k.gltf` |
| [Drill Press 01](https://polyhaven.com/a/drill_press_01) | Бытовая техника, collection: project_lighthouse, Электроника, industrial, Инструменты | 4.03 МБ | `polyhaven/drill_press_01/drill_press_01_1k.gltf` |
| [Dry Branches Medium 01](https://polyhaven.com/a/dry_branches_medium_01) | collection: pine_forest, collection: verdant_trail, ground cover, nature, plants, Реквизит | 3.25 МБ | `polyhaven/dry_branches_medium_01/dry_branches_medium_01_1k.gltf` |
| [Dustpan](https://polyhaven.com/a/dustpan) | collection: project_lighthouse, Реквизит, Инструменты | 2.28 МБ | `polyhaven/dustpan/dustpan_1k.gltf` |
| [Dutch Ship Large 01](https://polyhaven.com/a/dutch_ship_large_01) | collection: smugglers_cove, ships | 10.8 МБ | `polyhaven/dutch_ship_large_01/dutch_ship_large_01_1k.gltf` |
| [Dutch Ship Large 02](https://polyhaven.com/a/dutch_ship_large_02) | collection: smugglers_cove, ships | 9.82 МБ | `polyhaven/dutch_ship_large_02/dutch_ship_large_02_1k.gltf` |
| [Dutch Ship Medium](https://polyhaven.com/a/dutch_ship_medium) | collection: smugglers_cove, ships | 8.74 МБ | `polyhaven/dutch_ship_medium/dutch_ship_medium_1k.gltf` |
| [Electric Stove](https://polyhaven.com/a/electric_stove) | Бытовая техника, Электроника, Реквизит, С ригом | 2.05 МБ | `polyhaven/electric_stove/electric_stove_1k.gltf` |
| [Exterior Aircon Unit](https://polyhaven.com/a/exterior_aircon_unit) | Бытовая техника, collection: hidden_alley, Электроника, industrial | 7.67 МБ | `polyhaven/exterior_aircon_unit/exterior_aircon_unit_1k.gltf` |
| [Fancy Picture Frame 01](https://polyhaven.com/a/fancy_picture_frame_01) | Декор, wall decoration | 0.74 МБ | `polyhaven/fancy_picture_frame_01/fancy_picture_frame_01_1k.gltf` |
| [Fancy Picture Frame 02](https://polyhaven.com/a/fancy_picture_frame_02) | Декор, wall decoration | 1.73 МБ | `polyhaven/fancy_picture_frame_02/fancy_picture_frame_02_1k.gltf` |
| [Fern 02](https://polyhaven.com/a/fern_02) | collection: pine_forest, collection: smugglers_cove, nature, plants | 1.15 МБ | `polyhaven/fern_02/fern_02_1k.gltf` |
| [Filmstrip Projector 8mm](https://polyhaven.com/a/filmstrip_projector_8mm) | Бытовая техника, collection: project_lighthouse, Электроника | 3.12 МБ | `polyhaven/filmstrip_projector_8mm/filmstrip_projector_8mm_1k.gltf` |
| [Fire Alarm](https://polyhaven.com/a/fire_alarm) | collection: project_lighthouse, industrial, Офис, wall decoration | 1.95 МБ | `polyhaven/fire_alarm/fire_alarm_1k.gltf` |
| [Fire Hydrant](https://polyhaven.com/a/fire_hydrant) | collection: hidden_alley, industrial, Сооружения и улица | 5.52 МБ | `polyhaven/fire_hydrant/fire_hydrant_1k.gltf` |
| [Fish Knife](https://polyhaven.com/a/fish_knife) | collection: project_lighthouse, Реквизит | 2.57 МБ | `polyhaven/fish_knife/fish_knife_1k.gltf` |
| [Fishermans Hat](https://polyhaven.com/a/fishermans_hat) | collection: project_lighthouse, Реквизит | 2.34 МБ | `polyhaven/fishermans_hat/fishermans_hat_1k.gltf` |
| [Flathead Screwdriver](https://polyhaven.com/a/flathead_screwdriver) | collection: the_shed, industrial, Реквизит, Инструменты | 1.88 МБ | `polyhaven/flathead_screwdriver/flathead_screwdriver_1k.gltf` |
| [Flood Lights](https://polyhaven.com/a/flood_lights) | Электроника, industrial, Освещение | 6.88 МБ | `polyhaven/flood_lights/flood_lights_1k.gltf` |
| [Folding Wooden Stool](https://polyhaven.com/a/folding_wooden_stool) | Мебель, seating | 2.27 МБ | `polyhaven/folding_wooden_stool/folding_wooden_stool_1k.gltf` |
| [Food Apple 01](https://polyhaven.com/a/food_apple_01) | Еда, nature | 0.47 МБ | `polyhaven/food_apple_01/food_apple_01_1k.gltf` |
| [Food Avocado 01](https://polyhaven.com/a/food_avocado_01) | Еда, nature | 0.59 МБ | `polyhaven/food_avocado_01/food_avocado_01_1k.gltf` |
| [Food Ginger 01](https://polyhaven.com/a/food_ginger_01) | Еда, nature | 0.59 МБ | `polyhaven/food_ginger_01/food_ginger_01_1k.gltf` |
| [Food Kiwi 01](https://polyhaven.com/a/food_kiwi_01) | Еда, nature | 0.79 МБ | `polyhaven/food_kiwi_01/food_kiwi_01_1k.gltf` |
| [Food Lime 01](https://polyhaven.com/a/food_lime_01) | Еда, nature | 0.58 МБ | `polyhaven/food_lime_01/food_lime_01_1k.gltf` |
| [Food Lychee 01](https://polyhaven.com/a/food_lychee_01) | Еда, nature | 0.77 МБ | `polyhaven/food_lychee_01/food_lychee_01_1k.gltf` |
| [Pears Asian 01](https://polyhaven.com/a/food_pears_asian_01) | Еда, nature | 1.72 МБ | `polyhaven/food_pears_asian_01/food_pears_asian_01_1k.gltf` |
| [Food Pomegranate 01](https://polyhaven.com/a/food_pomegranate_01) | Еда, nature | 0.61 МБ | `polyhaven/food_pomegranate_01/food_pomegranate_01_1k.gltf` |
| [Football](https://polyhaven.com/a/football) | collection: the_shed, Реквизит | 2.76 МБ | `polyhaven/football/football_1k.gltf` |
| [Gallinera Chair](https://polyhaven.com/a/gallinera_chair) | Мебель, seating | 2.36 МБ | `polyhaven/gallinera_chair/gallinera_chair_1k.gltf` |
| [Gallinera Table](https://polyhaven.com/a/gallinera_table) | Мебель, table | 1.84 МБ | `polyhaven/gallinera_table/gallinera_table_1k.gltf` |
| [Gamepad](https://polyhaven.com/a/gamepad) | Реквизит | 1.32 МБ | `polyhaven/gamepad/gamepad_1k.gltf` |
| [Gaming Console](https://polyhaven.com/a/gaming_console) | Бытовая техника, Электроника | 2.98 МБ | `polyhaven/gaming_console/gaming_console_1k.gltf` |
| [Garden Gloves 01](https://polyhaven.com/a/garden_gloves_01) | collection: the_shed, Реквизит | 2.9 МБ | `polyhaven/garden_gloves_01/garden_gloves_01_1k.gltf` |
| [Garden Gnome](https://polyhaven.com/a/garden_gnome) | collection: the_shed, Декор | 2.45 МБ | `polyhaven/garden_gnome/garden_gnome_1k.gltf` |
| [Garden Hose Wall Mounted 01](https://polyhaven.com/a/garden_hose_wall_mounted_01) | collection: the_shed, plants, wall decoration | 2.58 МБ | `polyhaven/garden_hose_wall_mounted_01/garden_hose_wall_mounted_01_1k.gltf` |
| [Garden Sprinkler 01](https://polyhaven.com/a/garden_sprinkler_01) | Бытовая техника, collection: the_shed, Инструменты | 2.92 МБ | `polyhaven/garden_sprinkler_01/garden_sprinkler_01_1k.gltf` |
| [Gate Latch 01](https://polyhaven.com/a/gate_latch_01) | Здания / фасады / двери / ворота, collection: the_shed, industrial, Сооружения и улица | 2.08 МБ | `polyhaven/gate_latch_01/gate_latch_01_1k.gltf` |
| [Gothic Coffee Table](https://polyhaven.com/a/gothic_coffee_table) | Мебель, table | 3.27 МБ | `polyhaven/gothic_coffee_table/gothic_coffee_table_1k.gltf` |
| [Gothic Statue](https://polyhaven.com/a/gothic_statue) | collection: project_lighthouse, Декор | 3.98 МБ | `polyhaven/gothic_statue/gothic_statue_1k.gltf` |
| [Grass Bermuda 01](https://polyhaven.com/a/grass_bermuda_01) | collection: smugglers_cove, grass, ground cover, nature | 1.45 МБ | `polyhaven/grass_bermuda_01/grass_bermuda_01_1k.gltf` |
| [Hamburger Buns](https://polyhaven.com/a/hamburger_buns) | collection: project_lighthouse, Еда, Реквизит | 2.31 МБ | `polyhaven/hamburger_buns/hamburger_buns_1k.gltf` |
| [Hand Plane No4](https://polyhaven.com/a/hand_plane_no4) | collection: the_shed, Инструменты | 2.55 МБ | `polyhaven/hand_plane_no4/hand_plane_no4_1k.gltf` |
| [Hand Truck](https://polyhaven.com/a/hand_truck) | collection: project_lighthouse, industrial, Реквизит | 3.18 МБ | `polyhaven/hand_truck/hand_truck_1k.gltf` |
| [Handsaw Wood](https://polyhaven.com/a/handsaw_wood) | collection: the_shed, industrial, Реквизит, Инструменты | 2.48 МБ | `polyhaven/handsaw_wood/handsaw_wood_1k.gltf` |
| [Hanging Industrial Lamp](https://polyhaven.com/a/hanging_industrial_lamp) | Освещение | 3.24 МБ | `polyhaven/hanging_industrial_lamp/hanging_industrial_lamp_1k.gltf` |
| [Hanging Picture Frame 01](https://polyhaven.com/a/hanging_picture_frame_01) | Декор, wall decoration | 0.4 МБ | `polyhaven/hanging_picture_frame_01/hanging_picture_frame_01_1k.gltf` |
| [Hanging Picture Frame 02](https://polyhaven.com/a/hanging_picture_frame_02) | Декор, wall decoration | 0.7 МБ | `polyhaven/hanging_picture_frame_02/hanging_picture_frame_02_1k.gltf` |
| [Hanging Picture Frame 03](https://polyhaven.com/a/hanging_picture_frame_03) | Декор, wall decoration | 1.26 МБ | `polyhaven/hanging_picture_frame_03/hanging_picture_frame_03_1k.gltf` |
| [Hatchet](https://polyhaven.com/a/hatchet) | Инструменты | 1.97 МБ | `polyhaven/hatchet/hatchet_1k.gltf` |
| [Horse Head](https://polyhaven.com/a/horse_head) | Декор | 1.77 МБ | `polyhaven/horse_head/horse_head_1k.gltf` |
| [Horse Statue 01](https://polyhaven.com/a/horse_statue_01) | Декор, Реквизит | 1.28 МБ | `polyhaven/horse_statue_01/horse_statue_01_1k.gltf` |
| [Industrial Caged Sconce](https://polyhaven.com/a/industrial_caged_sconce) | collection: project_lighthouse, industrial, Освещение | 3.13 МБ | `polyhaven/industrial_caged_sconce/industrial_caged_sconce_1k.gltf` |
| [Industrial Coffee Table](https://polyhaven.com/a/industrial_coffee_table) | Мебель, table | 3.24 МБ | `polyhaven/industrial_coffee_table/industrial_coffee_table_1k.gltf` |
| [Industrial Microscope](https://polyhaven.com/a/industrial_microscope) | Бытовая техника, collection: project_lighthouse, industrial, Инструменты | 2.46 МБ | `polyhaven/industrial_microscope/industrial_microscope_1k.gltf` |
| [Industrial Plastic Container](https://polyhaven.com/a/industrial_pastic_container) | collection: project_lighthouse, Контейнеры, industrial, Офис | 1.57 МБ | `polyhaven/industrial_pastic_container/industrial_pastic_container_1k.gltf` |
| [Industrial Pipe Lamp](https://polyhaven.com/a/industrial_pipe_lamp) | collection: project_lighthouse, industrial, Освещение | 2.41 МБ | `polyhaven/industrial_pipe_lamp/industrial_pipe_lamp_1k.gltf` |
| [Industrial Storage Cart](https://polyhaven.com/a/industrial_storage_cart) | collection: project_lighthouse, Мебель, industrial, table | 2.64 МБ | `polyhaven/industrial_storage_cart/industrial_storage_cart_1k.gltf` |
| [Industrial Wall Lamp](https://polyhaven.com/a/industrial_wall_lamp) | Освещение | 3.78 МБ | `polyhaven/industrial_wall_lamp/industrial_wall_lamp_1k.gltf` |
| [Industrial Wall Sconce](https://polyhaven.com/a/industrial_wall_sconce) | Декор, Освещение | 1.84 МБ | `polyhaven/industrial_wall_sconce/industrial_wall_sconce_1k.gltf` |
| [Jug 01](https://polyhaven.com/a/jug_01) | Контейнеры, Декор, Посуда, Реквизит | 1.23 МБ | `polyhaven/jug_01/jug_01_1k.gltf` |
| [Katana Stand 01](https://polyhaven.com/a/katana_stand_01) | Декор, Реквизит | 0.56 МБ | `polyhaven/katana_stand_01/katana_stand_01_1k.gltf` |
| [Kite Shield](https://polyhaven.com/a/kite_shield) | collection: the_shed, Реквизит | 3.82 МБ | `polyhaven/kite_shield/kite_shield_1k.gltf` |
| [Korean Fire Extinguisher 01](https://polyhaven.com/a/korean_fire_extinguisher_01) | industrial, Реквизит | 1.62 МБ | `polyhaven/korean_fire_extinguisher_01/korean_fire_extinguisher_01_1k.gltf` |
| [Korean Public Payphone 01](https://polyhaven.com/a/korean_public_payphone_01) | Реквизит | 5.58 МБ | `polyhaven/korean_public_payphone_01/korean_public_payphone_01_1k.gltf` |
| [Ladder Sectioned 01](https://polyhaven.com/a/ladder_sectioned_01) | collection: the_shed, industrial, Инструменты | 2.57 МБ | `polyhaven/ladder_sectioned_01/ladder_sectioned_01_1k.gltf` |
| [Lambis Shell](https://polyhaven.com/a/lambis_shell) | Декор, nature | 2.25 МБ | `polyhaven/lambis_shell/lambis_shell_1k.gltf` |
| [Lantern Chandelier 01](https://polyhaven.com/a/lantern_chandelier_01) | Освещение | 1.48 МБ | `polyhaven/lantern_chandelier_01/lantern_chandelier_01_1k.gltf` |
| [Large Castle Door](https://polyhaven.com/a/large_castle_door) | Здания / фасады / двери / ворота, collection: hidden_alley, Сооружения и улица | 3.27 МБ | `polyhaven/large_castle_door/large_castle_door_1k.gltf` |
| [Large Iron Gate](https://polyhaven.com/a/large_iron_gate) | Здания / фасады / двери / ворота, collection: hidden_alley, Сооружения и улица | 3.13 МБ | `polyhaven/large_iron_gate/large_iron_gate_1k.gltf` |
| [Lateral Sea Marker](https://polyhaven.com/a/lateral_sea_marker) | Реквизит | 2.48 МБ | `polyhaven/lateral_sea_marker/lateral_sea_marker_1k.gltf` |
| [Leather Cleaner Can](https://polyhaven.com/a/leather_cleaner_can) | collection: the_shed, Контейнеры, Реквизит | 0.95 МБ | `polyhaven/leather_cleaner_can/leather_cleaner_can_1k.gltf` |
| [Lemon](https://polyhaven.com/a/lemon) | Еда, Реквизит | 1.85 МБ | `polyhaven/lemon/lemon_1k.gltf` |
| [Life Jacket](https://polyhaven.com/a/life_jacket) | collection: project_lighthouse, Реквизит | 2.33 МБ | `polyhaven/life_jacket/life_jacket_1k.gltf` |
| [Lifebuoy](https://polyhaven.com/a/lifebuoy) | collection: project_lighthouse, industrial, wall decoration | 2.62 МБ | `polyhaven/lifebuoy/lifebuoy_1k.gltf` |
| [Lightbulb 01](https://polyhaven.com/a/lightbulb_01) | collection: the_shed, Освещение | 1.04 МБ | `polyhaven/lightbulb_01/lightbulb_01_1k.gltf` |
| [Lightbulb Led](https://polyhaven.com/a/lightbulb_led) | collection: the_shed, Электроника, Освещение | 1.23 МБ | `polyhaven/lightbulb_led/lightbulb_led_1k.gltf` |
| [Lion Head](https://polyhaven.com/a/lion_head) | Декор | 3.2 МБ | `polyhaven/lion_head/lion_head_1k.gltf` |
| [Long Life Food](https://polyhaven.com/a/long_life_food) | collection: project_lighthouse, Контейнеры, Еда, Реквизит | 2.03 МБ | `polyhaven/long_life_food/long_life_food_1k.gltf` |
| [Lubricant Spray](https://polyhaven.com/a/lubricant_spray) | collection: the_shed, Контейнеры, Декор, Реквизит, Инструменты | 1.76 МБ | `polyhaven/lubricant_spray/lubricant_spray_1k.gltf` |
| [Machete](https://polyhaven.com/a/machete) | collection: the_shed, Реквизит | 2.42 МБ | `polyhaven/machete/machete_1k.gltf` |
| [Magnifying Glass 01](https://polyhaven.com/a/magnifying_glass_01) | Реквизит, Инструменты | 0.47 МБ | `polyhaven/magnifying_glass_01/magnifying_glass_01_1k.gltf` |
| [Mantel Clock 01](https://polyhaven.com/a/mantel_clock_01) | Бытовая техника, Декор, С ригом | 2.43 МБ | `polyhaven/mantel_clock_01/mantel_clock_01_1k.gltf` |
| [Marble Bust 01](https://polyhaven.com/a/marble_bust_01) | Декор | 0.9 МБ | `polyhaven/marble_bust_01/marble_bust_01_1k.gltf` |
| [Measuring Tape 01](https://polyhaven.com/a/measuring_tape_01) | collection: the_shed, industrial, Реквизит, Инструменты | 0.45 МБ | `polyhaven/measuring_tape_01/measuring_tape_01_1k.gltf` |
| [Medical Box](https://polyhaven.com/a/medical_box) | collection: project_lighthouse, Контейнеры, Реквизит | 1.98 МБ | `polyhaven/medical_box/medical_box_1k.gltf` |
| [Medical Tape](https://polyhaven.com/a/medical_tape) | collection: project_lighthouse, industrial, Реквизит, Инструменты | 1.6 МБ | `polyhaven/medical_tape/medical_tape_1k.gltf` |
| [Metal Detector](https://polyhaven.com/a/metal_detector) | Электроника, Реквизит | 2.05 МБ | `polyhaven/metal_detector/metal_detector_1k.gltf` |
| [Metal Jerrycan](https://polyhaven.com/a/metal_jerrycan) | collection: project_lighthouse, Контейнеры, Реквизит | 3.03 МБ | `polyhaven/metal_jerrycan/metal_jerrycan_1k.gltf` |
| [Metal Jerrycan Green](https://polyhaven.com/a/metal_jerrycan_green) | collection: project_lighthouse, Контейнеры, Реквизит | 2.66 МБ | `polyhaven/metal_jerrycan_green/metal_jerrycan_green_1k.gltf` |
| [Metal Jug](https://polyhaven.com/a/metal_jug) | Контейнеры, Декор, vases | 1.79 МБ | `polyhaven/metal_jug/metal_jug_1k.gltf` |
| [Metal Office Desk](https://polyhaven.com/a/metal_office_desk) | collection: project_lighthouse, Мебель, Офис, table | 1.61 МБ | `polyhaven/metal_office_desk/metal_office_desk_1k.gltf` |
| [Metal Stool 01](https://polyhaven.com/a/metal_stool_01) | collection: project_lighthouse, Мебель, seating | 2.29 МБ | `polyhaven/metal_stool_01/metal_stool_01_1k.gltf` |
| [Metal Stool 02](https://polyhaven.com/a/metal_stool_02) | collection: project_lighthouse, Мебель, seating | 2.54 МБ | `polyhaven/metal_stool_02/metal_stool_02_1k.gltf` |
| [Metal Stool 03](https://polyhaven.com/a/metal_stool_03) | collection: project_lighthouse, Мебель, seating | 2.29 МБ | `polyhaven/metal_stool_03/metal_stool_03_1k.gltf` |
| [Metal Tool Chest](https://polyhaven.com/a/metal_tool_chest) | collection: the_shed, Контейнеры, industrial, Реквизит, С ригом, Инструменты | 2.36 МБ | `polyhaven/metal_tool_chest/metal_tool_chest_1k.gltf` |
| [Metal Toolbox](https://polyhaven.com/a/metal_toolbox) | collection: project_lighthouse, Контейнеры, industrial, Инструменты | 2.75 МБ | `polyhaven/metal_toolbox/metal_toolbox_1k.gltf` |
| [Metal Trash Can](https://polyhaven.com/a/metal_trash_can) | collection: hidden_alley, Контейнеры, industrial | 4.99 МБ | `polyhaven/metal_trash_can/metal_trash_can_1k.gltf` |
| [Mid Century Lounge Chair](https://polyhaven.com/a/mid_century_lounge_chair) | Мебель, seating | 2.09 МБ | `polyhaven/mid_century_lounge_chair/mid_century_lounge_chair_1k.gltf` |
| [Modern Arm Chair 01](https://polyhaven.com/a/modern_arm_chair_01) | Мебель, Офис, seating | 2.7 МБ | `polyhaven/modern_arm_chair_01/modern_arm_chair_01_1k.gltf` |
| [Modern Ceiling Lamp 01](https://polyhaven.com/a/modern_ceiling_lamp_01) | Освещение | 0.44 МБ | `polyhaven/modern_ceiling_lamp_01/modern_ceiling_lamp_01_1k.gltf` |
| [Modern Coffee Table 01](https://polyhaven.com/a/modern_coffee_table_01) | Мебель, table | 1.33 МБ | `polyhaven/modern_coffee_table_01/modern_coffee_table_01_1k.gltf` |
| [Modern Coffee Table 02](https://polyhaven.com/a/modern_coffee_table_02) | Мебель, table | 2.96 МБ | `polyhaven/modern_coffee_table_02/modern_coffee_table_02_1k.gltf` |
| [Modern Wooden Cabinet](https://polyhaven.com/a/modern_wooden_cabinet) | Мебель, table | 2.86 МБ | `polyhaven/modern_wooden_cabinet/modern_wooden_cabinet_1k.gltf` |
| [Modified Thermos](https://polyhaven.com/a/modified_thermos) | collection: project_lighthouse, Контейнеры, Реквизит | 2.33 МБ | `polyhaven/modified_thermos/modified_thermos_1k.gltf` |
| [Modular Airduct Circular 01](https://polyhaven.com/a/modular_airduct_circular_01) | collection: hidden_alley, industrial, Сооружения и улица | 1.99 МБ | `polyhaven/modular_airduct_circular_01/modular_airduct_circular_01_1k.gltf` |
| [Modular Airduct Rectangular 01](https://polyhaven.com/a/modular_airduct_rectangular_01) | industrial | 2.77 МБ | `polyhaven/modular_airduct_rectangular_01/modular_airduct_rectangular_01_1k.gltf` |
| [Modular Chainlink Fence](https://polyhaven.com/a/modular_chainlink_fence) | Здания / фасады / двери / ворота, collection: hidden_alley, industrial, Сооружения и улица | 7.29 МБ | `polyhaven/modular_chainlink_fence/modular_chainlink_fence_1k.gltf` |
| [Modular Electric Cables](https://polyhaven.com/a/modular_electric_cables) | industrial | 6.02 МБ | `polyhaven/modular_electric_cables/modular_electric_cables_1k.gltf` |
| [Modular Electricity Poles](https://polyhaven.com/a/modular_electricity_poles) | collection: hidden_alley, industrial, Сооружения и улица | 12.42 МБ | `polyhaven/modular_electricity_poles/modular_electricity_poles_1k.gltf` |
| [Modular Factory Facade](https://polyhaven.com/a/modular_factory_facade) | Здания / фасады / двери / ворота, collection: hidden_alley, Сооружения и улица | 17.63 МБ | `polyhaven/modular_factory_facade/modular_factory_facade_1k.gltf` |
| [Modular Fire Escape](https://polyhaven.com/a/modular_fire_escape) | collection: hidden_alley, industrial, Сооружения и улица | 4.81 МБ | `polyhaven/modular_fire_escape/modular_fire_escape_1k.gltf` |
| [Modular Fort 01](https://polyhaven.com/a/modular_fort_01) | Здания / фасады / двери / ворота, collection: smugglers_cove, Сооружения и улица | 10.64 МБ | `polyhaven/modular_fort_01/modular_fort_01_1k.gltf` |
| [Modular Industrial Pipes 01](https://polyhaven.com/a/modular_industrial_pipes_01) | industrial, Сооружения и улица | 4.82 МБ | `polyhaven/modular_industrial_pipes_01/modular_industrial_pipes_01_1k.gltf` |
| [Modular Metal Gutter](https://polyhaven.com/a/modular_metal_gutter) | Здания / фасады / двери / ворота, collection: hidden_alley, industrial, Сооружения и улица | 2.21 МБ | `polyhaven/modular_metal_gutter/modular_metal_gutter_1k.gltf` |
| [Modular Pipes](https://polyhaven.com/a/modular_pipes) | industrial, Сооружения и улица | 5.82 МБ | `polyhaven/modular_pipes/modular_pipes_1k.gltf` |
| [Modular Pipes Plastic 01](https://polyhaven.com/a/modular_pipes_plastic_01) | collection: the_shed, industrial, Сооружения и улица | 3.12 МБ | `polyhaven/modular_pipes_plastic_01/modular_pipes_plastic_01_1k.gltf` |
| [Modular Street Seating](https://polyhaven.com/a/modular_street_seating) | collection: hidden_alley, Мебель, seating | 7.62 МБ | `polyhaven/modular_street_seating/modular_street_seating_1k.gltf` |
| [Modular Urban Apartments Facade](https://polyhaven.com/a/modular_urban_apartments_facade) | Здания / фасады / двери / ворота, collection: hidden_alley, Сооружения и улица | 16.49 МБ | `polyhaven/modular_urban_apartments_facade/modular_urban_apartments_facade_1k.gltf` |
| [Modular Wooden Pier](https://polyhaven.com/a/modular_wooden_pier) | collection: smugglers_cove, Сооружения и улица | 7.84 МБ | `polyhaven/modular_wooden_pier/modular_wooden_pier_1k.gltf` |
| [Mounted Fluorescent Lights](https://polyhaven.com/a/mounted_fluorescent_lights) | collection: project_lighthouse, industrial, Освещение | 1.86 МБ | `polyhaven/mounted_fluorescent_lights/mounted_fluorescent_lights_1k.gltf` |
| [Mousetrap](https://polyhaven.com/a/mousetrap) | Реквизит | 2.23 МБ | `polyhaven/mousetrap/mousetrap_1k.gltf` |
| [Multi Cleaner 5 Litre](https://polyhaven.com/a/multi_cleaner_5_litre) | collection: the_shed, Контейнеры, industrial, Реквизит | 1.37 МБ | `polyhaven/multi_cleaner_5_litre/multi_cleaner_5_litre_1k.gltf` |
| [Multi Cleaner Bottle](https://polyhaven.com/a/multi_cleaner_bottle) | collection: the_shed, Контейнеры, Реквизит | 1.38 МБ | `polyhaven/multi_cleaner_bottle/multi_cleaner_bottle_1k.gltf` |
| [Nettle Plant](https://polyhaven.com/a/nettle_plant) | collection: the_shed, ground cover, nature, plants | 2.48 МБ | `polyhaven/nettle_plant/nettle_plant_1k.gltf` |
| [Ocean Buoy](https://polyhaven.com/a/ocean_buoy) | collection: project_lighthouse, Реквизит | 3.03 МБ | `polyhaven/ocean_buoy/ocean_buoy_1k.gltf` |
| [Office Notepads](https://polyhaven.com/a/office_notepads) | collection: project_lighthouse, Офис, Реквизит | 1.33 МБ | `polyhaven/office_notepads/office_notepads_1k.gltf` |
| [Oil Tin](https://polyhaven.com/a/oil_tin) | collection: project_lighthouse, Контейнеры, industrial, Реквизит | 2.09 МБ | `polyhaven/oil_tin/oil_tin_1k.gltf` |
| [Old Bed Frame](https://polyhaven.com/a/old_bed_frame) | bed, collection: project_lighthouse, Мебель | 4.04 МБ | `polyhaven/old_bed_frame/old_bed_frame_1k.gltf` |
| [Old Drill Press](https://polyhaven.com/a/old_drill_press) | collection: project_lighthouse, industrial, Инструменты | 2.35 МБ | `polyhaven/old_drill_press/old_drill_press_1k.gltf` |
| [Old Gas Mask](https://polyhaven.com/a/old_gas_mask) | collection: project_lighthouse, industrial, Реквизит, Инструменты | 2.26 МБ | `polyhaven/old_gas_mask/old_gas_mask_1k.gltf` |
| [Old Military Compressor](https://polyhaven.com/a/old_military_compressor) | collection: project_lighthouse, industrial, Инструменты | 4.43 МБ | `polyhaven/old_military_compressor/old_military_compressor_1k.gltf` |
| [Old Military Crate](https://polyhaven.com/a/old_military_crate) | collection: project_lighthouse, Контейнеры, Реквизит | 2.5 МБ | `polyhaven/old_military_crate/old_military_crate_1k.gltf` |
| [Old Tyre](https://polyhaven.com/a/old_tyre) | collection: hidden_alley, industrial, Реквизит | 2.24 МБ | `polyhaven/old_tyre/old_tyre_1k.gltf` |
| [Ornate Medieval Dagger](https://polyhaven.com/a/ornate_medieval_dagger) | Реквизит | 2.14 МБ | `polyhaven/ornate_medieval_dagger/ornate_medieval_dagger_1k.gltf` |
| [Ornate Medieval Mace](https://polyhaven.com/a/ornate_medieval_mace) | Реквизит | 2.76 МБ | `polyhaven/ornate_medieval_mace/ornate_medieval_mace_1k.gltf` |
| [Ornate Mirror 01](https://polyhaven.com/a/ornate_mirror_01) | Декор, Мебель, wall decoration | 0.68 МБ | `polyhaven/ornate_mirror_01/ornate_mirror_01_1k.gltf` |
| [Ornate War Hammer](https://polyhaven.com/a/ornate_war_hammer) | Реквизит | 2.09 МБ | `polyhaven/ornate_war_hammer/ornate_war_hammer_1k.gltf` |
| [Outdoor Table Chair Set 01](https://polyhaven.com/a/outdoor_table_chair_set_01) | Мебель, seating | 1.24 МБ | `polyhaven/outdoor_table_chair_set_01/outdoor_table_chair_set_01_1k.gltf` |
| [Overhead Crane](https://polyhaven.com/a/overhead_crane) | collection: project_lighthouse, industrial, Инструменты | 7.33 МБ | `polyhaven/overhead_crane/overhead_crane_1k.gltf` |
| [Pachira Aquatica 01](https://polyhaven.com/a/pachira_aquatica_01) | collection: smugglers_cove, nature, plants | 5.14 МБ | `polyhaven/pachira_aquatica_01/pachira_aquatica_01_1k.gltf` |
| [Painted Wooden Bench](https://polyhaven.com/a/painted_wooden_bench) | Мебель, seating | 1.99 МБ | `polyhaven/painted_wooden_bench/painted_wooden_bench_1k.gltf` |
| [Painted Wooden Cabinet](https://polyhaven.com/a/painted_wooden_cabinet) | Мебель, shelves | 1.86 МБ | `polyhaven/painted_wooden_cabinet/painted_wooden_cabinet_1k.gltf` |
| [Painted Wooden Cabinet 02](https://polyhaven.com/a/painted_wooden_cabinet_02) | Мебель, shelves | 1.6 МБ | `polyhaven/painted_wooden_cabinet_02/painted_wooden_cabinet_02_1k.gltf` |
| [Painted Wooden Chair 01](https://polyhaven.com/a/painted_wooden_chair_01) | Мебель, seating | 0.49 МБ | `polyhaven/painted_wooden_chair_01/painted_wooden_chair_01_1k.gltf` |
| [Painted Wooden Chair 02](https://polyhaven.com/a/painted_wooden_chair_02) | Мебель, seating | 2.4 МБ | `polyhaven/painted_wooden_chair_02/painted_wooden_chair_02_1k.gltf` |
| [Painted Wooden Nightstand](https://polyhaven.com/a/painted_wooden_nightstand) | Мебель, table | 2.15 МБ | `polyhaven/painted_wooden_nightstand/painted_wooden_nightstand_1k.gltf` |
| [Painted Wooden Shelves](https://polyhaven.com/a/painted_wooden_shelves) | Мебель, shelves | 2.11 МБ | `polyhaven/painted_wooden_shelves/painted_wooden_shelves_1k.gltf` |
| [Painted Wooden Sofa](https://polyhaven.com/a/painted_wooden_sofa) | Мебель, seating | 2.1 МБ | `polyhaven/painted_wooden_sofa/painted_wooden_sofa_1k.gltf` |
| [Painted Wooden Stool](https://polyhaven.com/a/painted_wooden_stool) | Мебель, seating | 1.87 МБ | `polyhaven/painted_wooden_stool/painted_wooden_stool_1k.gltf` |
| [Painted Wooden Table](https://polyhaven.com/a/painted_wooden_table) | Мебель, table | 1.78 МБ | `polyhaven/painted_wooden_table/painted_wooden_table_1k.gltf` |
| [Plastic Torch 6V](https://polyhaven.com/a/pastic_torch_6v) | collection: project_lighthouse, Электроника, Освещение, Реквизит, Инструменты | 1.6 МБ | `polyhaven/pastic_torch_6v/pastic_torch_6v_1k.gltf` |
| [Picke Dirty 01](https://polyhaven.com/a/picke_dirty_01) | collection: the_shed, Реквизит, Инструменты | 2.24 МБ | `polyhaven/picke_dirty_01/picke_dirty_01_1k.gltf` |
| [Pipe Wrench](https://polyhaven.com/a/pipe_wrench) | collection: hidden_alley, industrial, Инструменты | 3.02 МБ | `polyhaven/pipe_wrench/pipe_wrench_1k.gltf` |
| [Planter Box 01](https://polyhaven.com/a/planter_box_01) | collection: the_shed, Контейнеры, Декор, potted plants | 2.34 МБ | `polyhaven/planter_box_01/planter_box_01_1k.gltf` |
| [Planter Box 02](https://polyhaven.com/a/planter_box_02) | collection: the_shed, Контейнеры, Декор, potted plants | 2.47 МБ | `polyhaven/planter_box_02/planter_box_02_1k.gltf` |
| [Planter Box 03](https://polyhaven.com/a/planter_box_03) | collection: the_shed, Контейнеры, Декор, potted plants | 2.56 МБ | `polyhaven/planter_box_03/planter_box_03_1k.gltf` |
| [Planter Pot Clay](https://polyhaven.com/a/planter_pot_clay) | collection: the_shed, Контейнеры, vases | 1.8 МБ | `polyhaven/planter_pot_clay/planter_pot_clay_1k.gltf` |
| [Plastic Bottle Gallon](https://polyhaven.com/a/plastic_bottle_gallon) | collection: the_shed, Контейнеры | 1.39 МБ | `polyhaven/plastic_bottle_gallon/plastic_bottle_gallon_1k.gltf` |
| [Plastic Broom](https://polyhaven.com/a/plastic_broom) | collection: project_lighthouse, Реквизит, Инструменты | 1.46 МБ | `polyhaven/plastic_broom/plastic_broom_1k.gltf` |
| [Plastic Container](https://polyhaven.com/a/plastic_container) | collection: project_lighthouse, Контейнеры, Реквизит | 1.34 МБ | `polyhaven/plastic_container/plastic_container_1k.gltf` |
| [Plastic Crate 01](https://polyhaven.com/a/plastic_crate_01) | collection: project_lighthouse, Контейнеры, Реквизит | 3.19 МБ | `polyhaven/plastic_crate_01/plastic_crate_01_1k.gltf` |
| [Plastic Crate 02](https://polyhaven.com/a/plastic_crate_02) | collection: project_lighthouse, Контейнеры, Реквизит | 1.84 МБ | `polyhaven/plastic_crate_02/plastic_crate_02_1k.gltf` |
| [Plastic Crate 03](https://polyhaven.com/a/plastic_crate_03) | collection: project_lighthouse, Контейнеры, industrial | 3.34 МБ | `polyhaven/plastic_crate_03/plastic_crate_03_1k.gltf` |
| [Plastic Jerrycan](https://polyhaven.com/a/plastic_jerrycan) | collection: project_lighthouse, Контейнеры, Реквизит | 2.51 МБ | `polyhaven/plastic_jerrycan/plastic_jerrycan_1k.gltf` |
| [Plastic Monobloc Chair 01](https://polyhaven.com/a/plastic_monobloc_chair_01) | collection: the_shed, Мебель, seating | 1.95 МБ | `polyhaven/plastic_monobloc_chair_01/plastic_monobloc_chair_01_1k.gltf` |
| [Plastic Thermos](https://polyhaven.com/a/plastic_thermos) | collection: project_lighthouse, Контейнеры, Реквизит | 1.51 МБ | `polyhaven/plastic_thermos/plastic_thermos_1k.gltf` |
| [Pliers](https://polyhaven.com/a/pliers) | collection: project_lighthouse, industrial, Реквизит, Инструменты | 2.02 МБ | `polyhaven/pliers/pliers_1k.gltf` |
| [Plunger](https://polyhaven.com/a/plunger) | Инструменты | 1.8 МБ | `polyhaven/plunger/plunger_1k.gltf` |
| [Pocket Watch](https://polyhaven.com/a/pocket_watch) | collection: project_lighthouse, Реквизит | 2.08 МБ | `polyhaven/pocket_watch/pocket_watch_1k.gltf` |
| [Portable Cassette Player](https://polyhaven.com/a/portable_cassette_player) | collection: project_lighthouse, Электроника, Реквизит | 2.55 МБ | `polyhaven/portable_cassette_player/portable_cassette_player_1k.gltf` |
| [Portable Generator](https://polyhaven.com/a/portable_generator) | Бытовая техника, collection: project_lighthouse, industrial | 3.43 МБ | `polyhaven/portable_generator/portable_generator_1k.gltf` |
| [Portable Searchlight](https://polyhaven.com/a/portable_searchlight) | collection: project_lighthouse, Электроника, Освещение, Реквизит, Инструменты | 1.93 МБ | `polyhaven/portable_searchlight/portable_searchlight_1k.gltf` |
| [Portable Welding Cart](https://polyhaven.com/a/portable_welding_cart) | collection: project_lighthouse, Контейнеры, industrial | 2.8 МБ | `polyhaven/portable_welding_cart/portable_welding_cart_1k.gltf` |
| [Postcard Set 01](https://polyhaven.com/a/postcard_set_01) | Реквизит | 0.52 МБ | `polyhaven/postcard_set_01/postcard_set_01_1k.gltf` |
| [Pot Enamel 01](https://polyhaven.com/a/pot_enamel_01) | Декор, Посуда, Реквизит | 1.74 МБ | `polyhaven/pot_enamel_01/pot_enamel_01_1k.gltf` |
| [Potted Plant 01](https://polyhaven.com/a/potted_plant_01) | Декор, nature, plants, potted plants | 6.33 МБ | `polyhaven/potted_plant_01/potted_plant_01_1k.gltf` |
| [Potted Plant 02](https://polyhaven.com/a/potted_plant_02) | Декор, nature, plants, potted plants | 2.63 МБ | `polyhaven/potted_plant_02/potted_plant_02_1k.gltf` |
| [Potted Plant 04](https://polyhaven.com/a/potted_plant_04) | Декор, nature, plants, potted plants, succulent | 2.12 МБ | `polyhaven/potted_plant_04/potted_plant_04_1k.gltf` |
| [Power Box 01](https://polyhaven.com/a/power_box_01) | collection: the_shed, Электроника, industrial, Реквизит, С ригом, Инструменты | 2.73 МБ | `polyhaven/power_box_01/power_box_01_1k.gltf` |
| [Projector Screen](https://polyhaven.com/a/projector_screen) | collection: project_lighthouse, Офис | 3.11 МБ | `polyhaven/projector_screen/projector_screen_1k.gltf` |
| [Propane Tank](https://polyhaven.com/a/propane_tank) | collection: project_lighthouse, industrial, Реквизит | 2.21 МБ | `polyhaven/propane_tank/propane_tank_1k.gltf` |
| [Propane Torch](https://polyhaven.com/a/propane_torch) | collection: project_lighthouse, Реквизит, Инструменты | 1.74 МБ | `polyhaven/propane_torch/propane_torch_1k.gltf` |
| [Propane Torch 02](https://polyhaven.com/a/propane_torch_02) | collection: project_lighthouse, industrial, Реквизит, Инструменты | 1.66 МБ | `polyhaven/propane_torch_02/propane_torch_02_1k.gltf` |
| [Pull Chain Light Socket](https://polyhaven.com/a/pull_chain_light_socket) | collection: hidden_alley, Освещение | 2.05 МБ | `polyhaven/pull_chain_light_socket/pull_chain_light_socket_1k.gltf` |
| [Ratchet Wrench](https://polyhaven.com/a/ratchet_wrench) | collection: the_shed, industrial, Реквизит, С ригом, Инструменты | 1.52 МБ | `polyhaven/ratchet_wrench/ratchet_wrench_1k.gltf` |
| [Retro Multimeter](https://polyhaven.com/a/retro_multimeter) | collection: project_lighthouse, Электроника, industrial, instrument, Реквизит, Инструменты | 1.39 МБ | `polyhaven/retro_multimeter/retro_multimeter_1k.gltf` |
| [Rollershutter Door](https://polyhaven.com/a/rollershutter_door) | Здания / фасады / двери / ворота, collection: hidden_alley, industrial, Сооружения и улица | 2.34 МБ | `polyhaven/rollershutter_door/rollershutter_door_1k.gltf` |
| [Rollershutter Window 01](https://polyhaven.com/a/rollershutter_window_01) | Здания / фасады / двери / ворота, collection: hidden_alley, industrial, Сооружения и улица | 2.24 МБ | `polyhaven/rollershutter_window_01/rollershutter_window_01_1k.gltf` |
| [Rollershutter Window 02](https://polyhaven.com/a/rollershutter_window_02) | Здания / фасады / двери / ворота, collection: hidden_alley, industrial, Сооружения и улица | 2.12 МБ | `polyhaven/rollershutter_window_02/rollershutter_window_02_1k.gltf` |
| [Rollershutter Window 03](https://polyhaven.com/a/rollershutter_window_03) | Здания / фасады / двери / ворота, collection: hidden_alley, industrial, Сооружения и улица | 2.28 МБ | `polyhaven/rollershutter_window_03/rollershutter_window_03_1k.gltf` |
| [Round Spectacles](https://polyhaven.com/a/round_spectacles) | collection: project_lighthouse, Реквизит | 2.07 МБ | `polyhaven/round_spectacles/round_spectacles_1k.gltf` |
| [Round Wooden Table 01](https://polyhaven.com/a/round_wooden_table_01) | Мебель, table | 0.55 МБ | `polyhaven/round_wooden_table_01/round_wooden_table_01_1k.gltf` |
| [Round Wooden Table 02](https://polyhaven.com/a/round_wooden_table_02) | Мебель, table | 1.92 МБ | `polyhaven/round_wooden_table_02/round_wooden_table_02_1k.gltf` |
| [Rubber Boots](https://polyhaven.com/a/rubber_boots) | collection: project_lighthouse, Реквизит | 2.01 МБ | `polyhaven/rubber_boots/rubber_boots_1k.gltf` |
| [Rubber Duck Toy](https://polyhaven.com/a/rubber_duck_toy) | collection: the_shed, Декор, Реквизит | 0.74 МБ | `polyhaven/rubber_duck_toy/rubber_duck_toy_1k.gltf` |
| [Russian Food Cans 01](https://polyhaven.com/a/russian_food_cans_01) | Контейнеры, Еда, Реквизит | 1.5 МБ | `polyhaven/russian_food_cans_01/russian_food_cans_01_1k.gltf` |
| [Rusted Hacksaw](https://polyhaven.com/a/rusted_hacksaw) | collection: project_lighthouse, Реквизит, Инструменты | 2.16 МБ | `polyhaven/rusted_hacksaw/rusted_hacksaw_1k.gltf` |
| [Rusted Spade 01](https://polyhaven.com/a/rusted_spade_01) | collection: the_shed, Реквизит, Инструменты | 2.42 МБ | `polyhaven/rusted_spade_01/rusted_spade_01_1k.gltf` |
| [Rusted Wheel Rim 01](https://polyhaven.com/a/rusted_wheel_rim_01) | collection: hidden_alley, industrial, Реквизит | 2.6 МБ | `polyhaven/rusted_wheel_rim_01/rusted_wheel_rim_01_1k.gltf` |
| [Rusted Wheel Rim 02](https://polyhaven.com/a/rusted_wheel_rim_02) | collection: hidden_alley, industrial, Реквизит | 2.6 МБ | `polyhaven/rusted_wheel_rim_02/rusted_wheel_rim_02_1k.gltf` |
| [Sand Rocks Small 01](https://polyhaven.com/a/sand_rocks_small_01) | collection: smugglers_cove, collection: verdant_trail, nature, rocks | 23.81 МБ | `polyhaven/sand_rocks_small_01/sand_rocks_small_01_1k.gltf` |
| [Scandinavian Masonry Heater](https://polyhaven.com/a/scandinavian_masonry_heater) | Бытовая техника | 2.46 МБ | `polyhaven/scandinavian_masonry_heater/scandinavian_masonry_heater_1k.gltf` |
| [Screwdriver](https://polyhaven.com/a/screwdriver) | collection: project_lighthouse, Реквизит, Инструменты | 1.54 МБ | `polyhaven/screwdriver/screwdriver_1k.gltf` |
| [Screwdrivers 02](https://polyhaven.com/a/screwdrivers_02) | collection: project_lighthouse, Реквизит, Инструменты | 2.08 МБ | `polyhaven/screwdrivers_02/screwdrivers_02_1k.gltf` |
| [Seadogs Compass](https://polyhaven.com/a/seadogs_compass) | collection: project_lighthouse, Декор, Реквизит, Инструменты | 2.7 МБ | `polyhaven/seadogs_compass/seadogs_compass_1k.gltf` |
| [Security Camera 01](https://polyhaven.com/a/security_camera_01) | Бытовая техника, collection: hidden_alley, Электроника, industrial, С ригом | 1.76 МБ | `polyhaven/security_camera_01/security_camera_01_1k.gltf` |
| [Security Camera 02](https://polyhaven.com/a/security_camera_02) | Бытовая техника, collection: hidden_alley, Электроника, industrial, С ригом | 1.69 МБ | `polyhaven/security_camera_02/security_camera_02_1k.gltf` |
| [Security Light](https://polyhaven.com/a/security_light) | collection: hidden_alley, industrial, Освещение | 2.39 МБ | `polyhaven/security_light/security_light_1k.gltf` |
| [Seedling Tray 01](https://polyhaven.com/a/seeding_tray_01) | collection: the_shed, Контейнеры | 2.03 МБ | `polyhaven/seeding_tray_01/seeding_tray_01_1k.gltf` |
| [Service Pistol](https://polyhaven.com/a/service_pistol) | collection: project_lighthouse, Огнестрельное оружие, Реквизит, Оружие | 3.3 МБ | `polyhaven/service_pistol/service_pistol_1k.gltf` |
| [Ship Pinnace](https://polyhaven.com/a/ship_pinnace) | collection: smugglers_cove, С ригом, ships | 21.07 МБ | `polyhaven/ship_pinnace/ship_pinnace_1k.gltf` |
| [Shrub Sorrel 01](https://polyhaven.com/a/shrub_sorrel_01) | collection: smugglers_cove, flowers, nature, plants | 1.19 МБ | `polyhaven/shrub_sorrel_01/shrub_sorrel_01_1k.gltf` |
| [Side Table 01](https://polyhaven.com/a/side_table_01) | Мебель, table | 0.51 МБ | `polyhaven/side_table_01/side_table_01_1k.gltf` |
| [Side Table Tall 01](https://polyhaven.com/a/side_table_tall_01) | Мебель, table | 1.55 МБ | `polyhaven/side_table_tall_01/side_table_tall_01_1k.gltf` |
| [Signal Flashlight](https://polyhaven.com/a/signal_flashlight) | collection: project_lighthouse, Электроника, Реквизит | 2.04 МБ | `polyhaven/signal_flashlight/signal_flashlight_1k.gltf` |
| [Sledgehammer 01](https://polyhaven.com/a/sledgehammer_01) | collection: the_shed, industrial, Реквизит, Инструменты | 1.78 МБ | `polyhaven/sledgehammer_01/sledgehammer_01_1k.gltf` |
| [Small Lpg Tank](https://polyhaven.com/a/small_lpg_tank) | collection: project_lighthouse, industrial, Реквизит | 2.78 МБ | `polyhaven/small_lpg_tank/small_lpg_tank_1k.gltf` |
| [Small Oil Can 01](https://polyhaven.com/a/small_oil_can_01) | collection: the_shed, industrial, Реквизит, Инструменты | 0.93 МБ | `polyhaven/small_oil_can_01/small_oil_can_01_1k.gltf` |
| [Small Plastic Torch](https://polyhaven.com/a/small_plastic_torch) | collection: project_lighthouse, Реквизит, Инструменты | 2.33 МБ | `polyhaven/small_plastic_torch/small_plastic_torch_1k.gltf` |
| [Small Wooden Table 01](https://polyhaven.com/a/small_wooden_table_01) | Мебель, table | 0.41 МБ | `polyhaven/small_wooden_table_01/small_wooden_table_01_1k.gltf` |
| [Sofa 02](https://polyhaven.com/a/sofa_02) | Мебель, seating | 0.43 МБ | `polyhaven/sofa_02/sofa_02_1k.gltf` |
| [Sofa 03](https://polyhaven.com/a/sofa_03) | Мебель, seating | 0.67 МБ | `polyhaven/sofa_03/sofa_03_1k.gltf` |
| [Spinning Wheel 01](https://polyhaven.com/a/spinning_wheel_01) | Реквизит, Инструменты | 2.13 МБ | `polyhaven/spinning_wheel_01/spinning_wheel_01_1k.gltf` |
| [Spray Paint Bottles](https://polyhaven.com/a/spray_paint_bottles) | collection: the_shed, Контейнеры, industrial, Реквизит | 2.26 МБ | `polyhaven/spray_paint_bottles/spray_paint_bottles_1k.gltf` |
| [Spray Paint Bottles 02](https://polyhaven.com/a/spray_paint_bottles_02) | collection: hidden_alley, industrial, Реквизит, Инструменты | 2.23 МБ | `polyhaven/spray_paint_bottles_02/spray_paint_bottles_02_1k.gltf` |
| [Standing Chalkboard 01](https://polyhaven.com/a/standing_chalkboard_01) | Декор | 2.62 МБ | `polyhaven/standing_chalkboard_01/standing_chalkboard_01_1k.gltf` |
| [Standing Picture Frame 01](https://polyhaven.com/a/standing_picture_frame_01) | Декор | 0.45 МБ | `polyhaven/standing_picture_frame_01/standing_picture_frame_01_1k.gltf` |
| [Standing Picture Frame 02](https://polyhaven.com/a/standing_picture_frame_02) | Декор | 0.57 МБ | `polyhaven/standing_picture_frame_02/standing_picture_frame_02_1k.gltf` |
| [Stationery Supplies](https://polyhaven.com/a/stationery_supplies) | collection: project_lighthouse, Офис, Реквизит | 1.99 МБ | `polyhaven/stationery_supplies/stationery_supplies_1k.gltf` |
| [Steel Frame Shelves 01](https://polyhaven.com/a/steel_frame_shelves_01) | collection: the_shed, Мебель, industrial, shelves | 1.6 МБ | `polyhaven/steel_frame_shelves_01/steel_frame_shelves_01_1k.gltf` |
| [Steel Frame Shelves 02](https://polyhaven.com/a/steel_frame_shelves_02) | collection: the_shed, Мебель, industrial, shelves | 0.54 МБ | `polyhaven/steel_frame_shelves_02/steel_frame_shelves_02_1k.gltf` |
| [Steel Frame Shelves 03](https://polyhaven.com/a/steel_frame_shelves_03) | Мебель, shelves | 3.56 МБ | `polyhaven/steel_frame_shelves_03/steel_frame_shelves_03_1k.gltf` |
| [Stick Grenade](https://polyhaven.com/a/stick_grenade) | collection: project_lighthouse, Оружие | 1.9 МБ | `polyhaven/stick_grenade/stick_grenade_1k.gltf` |
| [Stone Fire Pit](https://polyhaven.com/a/stone_fire_pit) | Здания / фасады / двери / ворота, collection: the_shed, Мебель, Сооружения и улица | 2.54 МБ | `polyhaven/stone_fire_pit/stone_fire_pit_1k.gltf` |
| [Strawberry Chocolate Cake](https://polyhaven.com/a/strawberry_chocolate_cake) | Еда, Реквизит | 3.33 МБ | `polyhaven/strawberry_chocolate_cake/strawberry_chocolate_cake_1k.gltf` |
| [Street Lamp 01](https://polyhaven.com/a/street_lamp_01) | collection: hidden_alley, industrial, Освещение | 2.17 МБ | `polyhaven/street_lamp_01/street_lamp_01_1k.gltf` |
| [Street Lamp 02](https://polyhaven.com/a/street_lamp_02) | collection: hidden_alley, industrial, Освещение | 1.87 МБ | `polyhaven/street_lamp_02/street_lamp_02_1k.gltf` |
| [Street Rat](https://polyhaven.com/a/street_rat) | collection: hidden_alley, Существа, nature | 3.61 МБ | `polyhaven/street_rat/street_rat_1k.gltf` |
| [Sungka Board](https://polyhaven.com/a/sungka_board) | collection: the_shed, Декор, Реквизит | 1.33 МБ | `polyhaven/sungka_board/sungka_board_1k.gltf` |
| [Sungka Board 02](https://polyhaven.com/a/sungka_board_02) | Декор, Реквизит | 3.02 МБ | `polyhaven/sungka_board_02/sungka_board_02_1k.gltf` |
| [Sweet Potato](https://polyhaven.com/a/sweet_potato) | Еда, nature | 1.77 МБ | `polyhaven/sweet_potato/sweet_potato_1k.gltf` |
| [Tea Set 01](https://polyhaven.com/a/tea_set_01) | Посуда, Реквизит | 2.28 МБ | `polyhaven/tea_set_01/tea_set_01_1k.gltf` |
| [Television 02](https://polyhaven.com/a/television_02) | collection: project_lighthouse, Электроника | 1.24 МБ | `polyhaven/television_02/television_02_1k.gltf` |
| [Throw Pillows 01](https://polyhaven.com/a/throw_pillows_01) | Декор | 1.19 МБ | `polyhaven/throw_pillows_01/throw_pillows_01_1k.gltf` |
| [Tire Pump](https://polyhaven.com/a/tire_pump) | collection: the_shed, Реквизит, Инструменты | 1.67 МБ | `polyhaven/tire_pump/tire_pump_1k.gltf` |
| [Tongue & Groove Pliers](https://polyhaven.com/a/tongue_groove_pliers) | collection: project_lighthouse, Инструменты | 2.02 МБ | `polyhaven/tongue_groove_pliers/tongue_groove_pliers_1k.gltf` |
| [Tool Cart](https://polyhaven.com/a/tool_cart) | collection: project_lighthouse, Мебель, industrial, table | 3.17 МБ | `polyhaven/tool_cart/tool_cart_1k.gltf` |
| [Trashbag](https://polyhaven.com/a/trashbag) | collection: project_lighthouse, Реквизит | 1.7 МБ | `polyhaven/trashbag/trashbag_1k.gltf` |
| [Treasure Chest](https://polyhaven.com/a/treasure_chest) | collection: smugglers_cove, Контейнеры, Декор, Реквизит | 5.48 МБ | `polyhaven/treasure_chest/treasure_chest_1k.gltf` |
| [Trowel 01](https://polyhaven.com/a/trowel_01) | collection: the_shed, Реквизит, Инструменты | 1.97 МБ | `polyhaven/trowel_01/trowel_01_1k.gltf` |
| [Utility Box 01](https://polyhaven.com/a/utility_box_01) | collection: hidden_alley, industrial, Сооружения и улица | 1.98 МБ | `polyhaven/utility_box_01/utility_box_01_1k.gltf` |
| [Utility Box 02](https://polyhaven.com/a/utility_box_02) | collection: hidden_alley, industrial, Сооружения и улица | 2.03 МБ | `polyhaven/utility_box_02/utility_box_02_1k.gltf` |
| [Vintage Binoculars](https://polyhaven.com/a/vintage_binocular) | collection: project_lighthouse, instrument, Реквизит | 2.75 МБ | `polyhaven/vintage_binocular/vintage_binocular_1k.gltf` |
| [Vintage Cabinet 01](https://polyhaven.com/a/vintage_cabinet_01) | Мебель, shelves | 2.87 МБ | `polyhaven/vintage_cabinet_01/vintage_cabinet_01_1k.gltf` |
| [Vintage Crutches 01](https://polyhaven.com/a/vintage_crutches_01) | collection: the_shed, Реквизит | 0.79 МБ | `polyhaven/vintage_crutches_01/vintage_crutches_01_1k.gltf` |
| [Vintage Day Bed](https://polyhaven.com/a/vintage_day_bed) | bed, collection: project_lighthouse, Мебель, seating | 2.43 МБ | `polyhaven/vintage_day_bed/vintage_day_bed_1k.gltf` |
| [Vintage Electric Kettle](https://polyhaven.com/a/vintage_electric_kettle) | Бытовая техника, collection: project_lighthouse, Декор | 2.39 МБ | `polyhaven/vintage_electric_kettle/vintage_electric_kettle_1k.gltf` |
| [Vintage Flashlight](https://polyhaven.com/a/vintage_flashlight) | collection: project_lighthouse, Электроника, Освещение, Реквизит, Инструменты | 1.84 МБ | `polyhaven/vintage_flashlight/vintage_flashlight_1k.gltf` |
| [Vintage Grandfather Clock 01](https://polyhaven.com/a/vintage_grandfather_clock_01) | Бытовая техника, Декор, С ригом | 2.36 МБ | `polyhaven/vintage_grandfather_clock_01/vintage_grandfather_clock_01_1k.gltf` |
| [Vintage Hand Drill](https://polyhaven.com/a/vintage_hand_drill) | collection: the_shed, Реквизит, С ригом, Инструменты | 3.1 МБ | `polyhaven/vintage_hand_drill/vintage_hand_drill_1k.gltf` |
| [Vintage Lighter](https://polyhaven.com/a/vintage_lighter) | collection: project_lighthouse, Реквизит, Инструменты | 1.75 МБ | `polyhaven/vintage_lighter/vintage_lighter_1k.gltf` |
| [Vintage Microscope](https://polyhaven.com/a/vintage_microscope) | collection: project_lighthouse, industrial, instrument, Инструменты | 2.31 МБ | `polyhaven/vintage_microscope/vintage_microscope_1k.gltf` |
| [Vintage Microwave](https://polyhaven.com/a/vintage_microwave) | Бытовая техника, collection: project_lighthouse, Электроника | 1.63 МБ | `polyhaven/vintage_microwave/vintage_microwave_1k.gltf` |
| [Vintage Oil Lamp](https://polyhaven.com/a/vintage_oil_lamp) | Декор, Освещение | 2.07 МБ | `polyhaven/vintage_oil_lamp/vintage_oil_lamp_1k.gltf` |
| [Vintage Pocket Watch](https://polyhaven.com/a/vintage_pocket_watch) | Реквизит | 1.99 МБ | `polyhaven/vintage_pocket_watch/vintage_pocket_watch_1k.gltf` |
| [Vintage Radio Transceiver](https://polyhaven.com/a/vintage_radio_transceiver) | Бытовая техника, collection: project_lighthouse, Электроника | 6.3 МБ | `polyhaven/vintage_radio_transceiver/vintage_radio_transceiver_1k.gltf` |
| [Vintage Spacecraft Instrument](https://polyhaven.com/a/vintage_spacecraft_instrument) | collection: project_lighthouse, Электроника, industrial, instrument | 3.67 МБ | `polyhaven/vintage_spacecraft_instrument/vintage_spacecraft_instrument_1k.gltf` |
| [Vintage Stapler](https://polyhaven.com/a/vintage_stapler) | collection: project_lighthouse, Офис, Реквизит, Инструменты | 2.32 МБ | `polyhaven/vintage_stapler/vintage_stapler_1k.gltf` |
| [Vintage Suitcase](https://polyhaven.com/a/vintage_suitcase) | Декор, Реквизит | 4.29 МБ | `polyhaven/vintage_suitcase/vintage_suitcase_1k.gltf` |
| [Vintage Telephone Wall Clock](https://polyhaven.com/a/vintage_telephone_wall_clock) | collection: project_lighthouse, Декор, Реквизит, wall decoration | 2.73 МБ | `polyhaven/vintage_telephone_wall_clock/vintage_telephone_wall_clock_1k.gltf` |
| [Vintage Video Camera](https://polyhaven.com/a/vintage_video_camera) | Бытовая техника, Электроника | 2.71 МБ | `polyhaven/vintage_video_camera/vintage_video_camera_1k.gltf` |
| [Vintage Wooden Drawer 01](https://polyhaven.com/a/vintage_wooden_drawer_01) | Мебель, shelves, table | 2.09 МБ | `polyhaven/vintage_wooden_drawer_01/vintage_wooden_drawer_01_1k.gltf` |
| [Wall Clock](https://polyhaven.com/a/wall_clock) | Бытовая техника, collection: project_lighthouse, Декор | 1.56 МБ | `polyhaven/wall_clock/wall_clock_1k.gltf` |
| [Water Manhole Cover](https://polyhaven.com/a/water_manhole_cover) | collection: hidden_alley, industrial, Сооружения и улица | 2.33 МБ | `polyhaven/water_manhole_cover/water_manhole_cover_1k.gltf` |
| [Watering Can Metal 01](https://polyhaven.com/a/watering_can_metal_01) | collection: the_shed, Контейнеры, Реквизит, Инструменты | 2.61 МБ | `polyhaven/watering_can_metal_01/watering_can_metal_01_1k.gltf` |
| [Weed Plant 02](https://polyhaven.com/a/weed_plant_02) | collection: the_shed, ground cover, nature, plants | 2.05 МБ | `polyhaven/weed_plant_02/weed_plant_02_1k.gltf` |
| [Wheelchair 01](https://polyhaven.com/a/wheelchair_01) | Реквизит | 1.75 МБ | `polyhaven/wheelchair_01/wheelchair_01_1k.gltf` |
| [Wicker Basket 01](https://polyhaven.com/a/wicker_basket_01) | collection: the_shed, Контейнеры, Декор | 2.73 МБ | `polyhaven/wicker_basket_01/wicker_basket_01_1k.gltf` |
| [Wicker Basket 02](https://polyhaven.com/a/wicker_basket_02) | Контейнеры, Декор | 3.06 МБ | `polyhaven/wicker_basket_02/wicker_basket_02_1k.gltf` |
| [Wine Barrel 01](https://polyhaven.com/a/wine_barrel_01) | Контейнеры | 0.92 МБ | `polyhaven/wine_barrel_01/wine_barrel_01_1k.gltf` |
| [Wine Bottles 01](https://polyhaven.com/a/wine_bottles_01) | Контейнеры, Посуда, Реквизит | 3.91 МБ | `polyhaven/wine_bottles_01/wine_bottles_01_1k.gltf` |
| [Wooden Axe](https://polyhaven.com/a/wooden_axe) | Реквизит | 2.03 МБ | `polyhaven/wooden_axe/wooden_axe_1k.gltf` |
| [Wooden Axe 02](https://polyhaven.com/a/wooden_axe_02) | Реквизит | 2.16 МБ | `polyhaven/wooden_axe_02/wooden_axe_02_1k.gltf` |
| [Wooden Axe 03](https://polyhaven.com/a/wooden_axe_03) | Реквизит, Инструменты | 2.27 МБ | `polyhaven/wooden_axe_03/wooden_axe_03_1k.gltf` |
| [Wooden Barrels 01](https://polyhaven.com/a/wooden_barrels_01) | collection: smugglers_cove, Контейнеры, Реквизит | 7.49 МБ | `polyhaven/wooden_barrels_01/wooden_barrels_01_1k.gltf` |
| [Wooden Bookshelf Worn](https://polyhaven.com/a/wooden_bookshelf_worn) | collection: the_shed, Мебель, shelves | 2.78 МБ | `polyhaven/wooden_bookshelf_worn/wooden_bookshelf_worn_1k.gltf` |
| [Wooden Bowl 01](https://polyhaven.com/a/wooden_bowl_01) | Декор, Посуда, Реквизит | 1.82 МБ | `polyhaven/wooden_bowl_01/wooden_bowl_01_1k.gltf` |
| [Wooden Bowl 02](https://polyhaven.com/a/wooden_bowl_02) | Декор, Посуда, Реквизит | 2.51 МБ | `polyhaven/wooden_bowl_02/wooden_bowl_02_1k.gltf` |
| [Wooden Broom](https://polyhaven.com/a/wooden_broom) | collection: project_lighthouse, Реквизит, Инструменты | 3.42 МБ | `polyhaven/wooden_broom/wooden_broom_1k.gltf` |
| [Wooden Bucket 01](https://polyhaven.com/a/wooden_bucket_01) | collection: smugglers_cove, Контейнеры, Реквизит | 2.33 МБ | `polyhaven/wooden_bucket_01/wooden_bucket_01_1k.gltf` |
| [Wooden Bucket 02](https://polyhaven.com/a/wooden_bucket_02) | collection: smugglers_cove, Контейнеры, Реквизит | 2.51 МБ | `polyhaven/wooden_bucket_02/wooden_bucket_02_1k.gltf` |
| [Wooden Candlestick](https://polyhaven.com/a/wooden_candlestick) | Декор, Освещение | 8.43 МБ | `polyhaven/wooden_candlestick/wooden_candlestick_1k.gltf` |
| [Wooden Crate 01](https://polyhaven.com/a/wooden_crate_01) | collection: smugglers_cove, Контейнеры | 2.28 МБ | `polyhaven/wooden_crate_01/wooden_crate_01_1k.gltf` |
| [Wooden Crate 02](https://polyhaven.com/a/wooden_crate_02) | collection: smugglers_cove, Контейнеры, Реквизит | 2.24 МБ | `polyhaven/wooden_crate_02/wooden_crate_02_1k.gltf` |
| [Wooden Cutting Board](https://polyhaven.com/a/wooden_cutting_board) | Еда, Реквизит | 1.97 МБ | `polyhaven/wooden_cutting_board/wooden_cutting_board_1k.gltf` |
| [Wooden Display Shelves 01](https://polyhaven.com/a/wooden_display_shelves_01) | Мебель, shelves | 0.47 МБ | `polyhaven/wooden_display_shelves_01/wooden_display_shelves_01_1k.gltf` |
| [Wooden Hammer 01](https://polyhaven.com/a/wooden_hammer_01) | collection: the_shed, industrial, Реквизит, Инструменты | 1.89 МБ | `polyhaven/wooden_hammer_01/wooden_hammer_01_1k.gltf` |
| [Wooden Handle Saber](https://polyhaven.com/a/wooden_handle_saber) | collection: hidden_alley, Реквизит | 1.82 МБ | `polyhaven/wooden_handle_saber/wooden_handle_saber_1k.gltf` |
| [Wooden Ladder](https://polyhaven.com/a/wooden_ladder) | collection: project_lighthouse, industrial, Инструменты | 2.59 МБ | `polyhaven/wooden_ladder/wooden_ladder_1k.gltf` |
| [Wooden Ladder 02](https://polyhaven.com/a/wooden_ladder_02) | collection: project_lighthouse, Инструменты | 2.34 МБ | `polyhaven/wooden_ladder_02/wooden_ladder_02_1k.gltf` |
| [Wooden Lantern 01](https://polyhaven.com/a/wooden_lantern_01) | collection: smugglers_cove, Освещение, Реквизит | 3.77 МБ | `polyhaven/wooden_lantern_01/wooden_lantern_01_1k.gltf` |
| [Wooden Military Crate](https://polyhaven.com/a/wooden_military_crate) | collection: project_lighthouse, Контейнеры | 3.05 МБ | `polyhaven/wooden_military_crate/wooden_military_crate_1k.gltf` |
| [Wooden Picnic Table](https://polyhaven.com/a/wooden_picnic_table) | Мебель, seating, table | 4.31 МБ | `polyhaven/wooden_picnic_table/wooden_picnic_table_1k.gltf` |
| [Wooden Spoon](https://polyhaven.com/a/wooden_spoon) | Посуда, Реквизит | 1.97 МБ | `polyhaven/wooden_spoon/wooden_spoon_1k.gltf` |
| [Wooden Stool 01](https://polyhaven.com/a/wooden_stool_01) | collection: the_shed, Мебель, seating | 0.96 МБ | `polyhaven/wooden_stool_01/wooden_stool_01_1k.gltf` |
| [Wooden Stool 02](https://polyhaven.com/a/wooden_stool_02) | Мебель, seating | 2.66 МБ | `polyhaven/wooden_stool_02/wooden_stool_02_1k.gltf` |
| [Wooden Table 02](https://polyhaven.com/a/wooden_table_02) | Мебель, table | 0.49 МБ | `polyhaven/wooden_table_02/wooden_table_02_1k.gltf` |
| [Worn Metal Rack](https://polyhaven.com/a/worn_metal_rack) | collection: project_lighthouse, Мебель, industrial, shelves | 2.01 МБ | `polyhaven/worn_metal_rack/worn_metal_rack_1k.gltf` |
| [Yellow Onion](https://polyhaven.com/a/yellow_onion) | Еда, Реквизит | 1.81 МБ | `polyhaven/yellow_onion/yellow_onion_1k.gltf` |

## Дома (сгенерированы локально)

Каждый GLB содержит статичные стены/крышу, **отдельные объекты дверей с анимацией открывания** (`Door_FrontAction`) и **прозрачные стёкла** (`KHR_materials_transmission`, alpha BLEND).

| Файл | Размер |
|---|---|
| `models/generated/houses/house_00.glb` | 47 КБ |
| `models/generated/houses/house_01.glb` | 30 КБ |
| `models/generated/houses/house_02.glb` | 50 КБ |
| `models/generated/houses/house_03.glb` | 47 КБ |
| `models/generated/houses/house_04.glb` | 50 КБ |
| `models/generated/houses/house_05.glb` | 40 КБ |
| `models/generated/houses/house_06.glb` | 47 КБ |
| `models/generated/houses/house_07.glb` | 40 КБ |
| `models/generated/houses/house_08.glb` | 50 КБ |
| `models/generated/houses/house_09.glb` | 47 КБ |
| `models/generated/houses/house_10.glb` | 50 КБ |
| `models/generated/houses/house_11.glb` | 50 КБ |

## Sketchfab (CC-BY / CC-BY-SA)

Для этих моделей **обязательно указать автора** в титрах — лицензия CC-BY требует атрибуции.

| Модель | Категория | Автор | Лицензия | Риг | Ссылка |
|---|---|---|---|---|---|
| Gavin Reed - Detroit Become Human | characters | Q.SARDOR | by | да | [0161bd8e](https://sketchfab.com/3d-models/0161bd8ecbaa44cb9da40ba4f2f80aaf) |
| Connor RK900 - Detroit Become Human | characters | Q.SARDOR | by | да | [623e3f52](https://sketchfab.com/3d-models/623e3f52b7da4116a4576f38479eaaaf) |
| IWI Male Character 02 | characters | ilkay.ivrendi | by | да | [774ca001](https://sketchfab.com/3d-models/774ca00136b94d359e7d3ba425a5c57a) |
| Police | characters | Alihan | by | да | [8f939bfd](https://sketchfab.com/3d-models/8f939bfdf22f476d91f480f4d6f5d72a) |
| PoliceMan | characters | SPP Animation Studio | by | да | [d47bb1f6](https://sketchfab.com/3d-models/d47bb1f61fa340588b5801cdb63812c8) |

## Важные ограничения

- В бесплатных библиотеках **нет фот��реалистичных людей, полицейских и целых зданий**. Персонажи и техника — модели с прямой загрузкой; фотореалистичных людей/машин/полиции нет.
- Poly Haven даёт только **модульные куски архитектуры** (фасады, двери, ворота, заборы) — целые дома собраны процедурно в `tools/make_houses.py`.
- Люди и техника со Sketchfab — **не фотосканы**: это авторские модели под CC-BY, стиль и качество у них разные, атрибуция обязательна.
- Модели с лицензиями NC/ND со Sketchfab скрипт **пропускает**, чтобы проект можно было использовать коммерчески.
- У домов двери — отдельные объекты с анимацией, стёкла прозрачные; у остальных моделей подвижных деталей (капот/дверцы машин) нет — их нужно разрезать в Blender (`P` → Separate) и анимировать.

## Как перекачать / перерендерить

```bash
python3 tools/fetch_polyhaven.py      # скачать модели (Poly Haven API)
python3 tools/make_sheets.py          # пересобрать контактные листы
python3 tools/write_sources.py        # обновить этот файл
```
