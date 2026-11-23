# Жилые дома Marisol

24 обычных дома для кварталов города. Все модели — процедурные GLB в метрах, с UV
и PBR-текстурами. Галерея: [`game/houses.html`](../../../game/houses.html).

## Формат

- единицы — **метры**, Y вверх;
- origin — по центру дома на уровне земли (`y = 0`);
- фасад смотрит в **+Z**, вход и крыльцо — с этой стороны;
- крыша, стены, двери, окна, стекло, заборы — отдельные материалы;
- текстуры вынесены в `textures/` и общие для всех домов: 12 файлов по 256²;
- окна используют `KHR_materials_transmission` и `KHR_materials_volume`;
  GLB ссылаются на текстуры относительным путём `textures/<name>.png`, поэтому
  папку `textures/` нужно класть рядом с моделями.

## Каталог

| Файл | Дом | English | Ш × Г × В, м | Крыша | Стены / крыша | Детали |
| --- | --- | --- | --- | --- | --- | --- |
| `house_00.glb` | Бунгало | Bungalow | 9 × 8 × 3 | hip | stucco / tile | крыльцо, забор, дымоход |
| `house_01.glb` | Ранчо | Ranch | 14 × 9 × 3.2 | gable | siding / shingle | гараж, забор |
| `house_02.glb` | Двухэтажный | Two-storey | 9.5 × 9 × 6.2 | hip | stucco / tile | 2 этажа, крыльцо, дымоход |
| `house_03.glb` | Таунхаус | Townhouse | 7 × 12 × 6.4 | flat | brick / metal | 2 этажа |
| `house_04.glb` | Коттедж | Cottage | 8 × 7 × 3.1 | gable | stucco / shingle | крыльцо, забор, дымоход |
| `house_05.glb` | Дуплекс | Duplex | 15 × 8.5 × 3.3 | gable | siding / shingle | — |
| `house_06.glb` | Узкий дом | Shotgun | 6.2 × 14 × 3.5 | gable | siding / metal | крыльцо |
| `house_07.glb` | Вилла | Villa | 16 × 12 × 3.8 | flat | stucco / concrete | бассейн |
| `house_08.glb` | Хижина | Cabin | 7.4 × 6.4 × 2.9 | gable | wood / shingle | крыльцо, дымоход |
| `house_09.glb` | Модерн | Modern | 12 × 9 × 5.8 | flat | stucco / metal | 2 этажа, гараж |
| `house_10.glb` | Дом с гаражом | Garage house | 13 × 9.5 × 3.2 | gable | siding / shingle | гараж, забор |
| `house_11.glb` | Угловой дом | Corner house | 10 × 10 × 6 | hip | brick / tile | 2 этажа, крыльцо, дымоход |
| `house_12.glb` | Садовый коттедж | Garden cottage | 8.4 × 7.2 × 3 | hip | stucco / shingle | крыльцо, забор |
| `house_13.glb` | Кирпичный ряд | Brick rowhouse | 7.4 × 11 × 6.4 | flat | brick / metal | 2 этажа |
| `house_14.glb` | Пляжное бунгало | Beach bungalow | 10 × 8 × 2.9 | hip | siding / shingle | крыльцо, забор |
| `house_15.glb` | Семейный дом | Family house | 13 × 10 × 3.3 | gable | siding / shingle | гараж |
| `house_16.glb` | Узкий двухэтажный | Narrow house | 6.4 × 10 × 6 | flat | brick / metal | 2 этажа |
| `house_17.glb` | Деревянная хижина | Wood cabin | 8 × 7 × 3 | gable | wood / shingle | крыльцо, дымоход |
| `house_18.glb` | Современная вилла | Modern villa | 17 × 11 × 3.8 | flat | stucco / concrete | бассейн |
| `house_19.glb` | Малый ранчо | Small ranch | 12 × 8 × 3.1 | gable | stucco / shingle | гараж |
| `house_20.glb` | Домик у озера | Lake cottage | 8.2 × 6.4 × 2.9 | gable | wood / shingle | крыльцо |
| `house_21.glb` | Двойной дуплекс | Twin duplex | 15 × 8.4 × 3.4 | gable | siding / shingle | — |
| `house_22.glb` | Солнечный дом | Sun house | 9 × 8.4 × 3 | hip | stucco / tile | крыльцо, дымоход |
| `house_23.glb` | Угловой двухэтажный | Corner two-storey | 10 × 10 × 6 | hip | brick / tile | 2 этажа, крыльцо, дымоход |

## Текстуры

`textures/` — 12 бесшовных PBR-текстур 256²:

| Файл | Где используется |
| --- | --- |
| `stucco.png` | штукатурка фасадов |
| `brick.png` | кирпичная кладка |
| `siding.png` | сайдинг |
| `shingle.png` | битумная черепица |
| `tile.png` | керамическая черепица |
| `concrete.png` | плоские крыши и бетон |
| `wood.png` | деревянные хижины |
| `door.png` | полотно двери |
| `trim.png` | наличники, откосы, забор |
| `glass.png` | окна (альфа и transmission) |
| `water.png` | бассейн |
| `metal.png` | фальцевая кровля и водостоки |

## Старые корпуса

`shop_shells/shell_00..11.glb` — прежние 12 домов-корпусов с дверьми и кассовой
стойкой. Они не входят в галерею жилых домов и оставлены как оболочки магазинов
и интерьеров. Формат тот же: метры, origin на земле, фасад в +Z.

## Пересборка и проверка

```bash
python3 game/tools/make_houses.py
node game/tools/validate_glb.cjs models/generated/houses/house_*.glb
```

Генератор задаёт все 24 стиля в `STYLES` (размеры, крыша, материалы, окна, гараж,
бассейн, забор). Валидатор читает внешние текстуры с диска; у всех 24 моделей —
0 ошибок и 0 предупреждений.
