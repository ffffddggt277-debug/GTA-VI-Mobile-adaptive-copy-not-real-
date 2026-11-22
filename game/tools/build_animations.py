#!/usr/bin/env python3
"""Печёт анимации в GLB персонажей.

Запуск:  python3 game/tools/build_animations.py
Результат: game/animations/{player,npc_civilian,npc_police}.glb + clips.json + ANIMATIONS.md
"""

import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)

from animlib import Rig, write_glb  # noqa: E402
from clips import to_pose  # noqa: E402
import clips as C  # noqa: E402
from actions import ACT  # noqa: E402
import actions2  # noqa: E402,F401  (регистрирует боевые/полицейские/бытовые действия)

ROOT = os.path.abspath(os.path.join(HERE, "..", ".."))
MODELS = os.path.join(ROOT, "models", "sketchfab")
OUTDIR = os.path.join(ROOT, "game", "animations")
FPS = 30

CHARACTERS = {
    "player": dict(
        file="iwi-male-character-02__774ca00136b94d359e7d3ba425a5c57a.glb",
        out="player.glb", label="Игрок", role="player",
    ),
    "civilian": dict(
        file="gavin-reed-detroit-become-human__0161bd8ecbaa44cb9da40ba4f2f80aaf.glb",
        out="npc_civilian.glb", label="Гражданский NPC", role="civilian",
    ),
    "police": dict(
        file="policeman__d47bb1f61fa340588b5801cdb63812c8.glb",
        out="npc_police.glb", label="Полицейский NPC", role="police",
    ),
}


# --------------------------------------------------------------------------- #
# конструкторы спецификаций
# --------------------------------------------------------------------------- #

def L(name, desc, **cfg):
    return dict(name=name, kind="loco", cat="locomotion", desc=desc, cfg=cfg, loop=True)


def I(name, desc, **cfg):
    return dict(name=name, kind="idle", cat="idle", desc=desc, cfg=cfg, loop=True)


def X(name, desc=None, **over):
    a = ACT[name]
    cfg = dict(dur=a["dur"], keys=a["keys"], crouch=a["crouch"], arms=a["arms"],
               ease=a["ease"], lean=0.0)
    cfg.update(over.get("cfg", {}))
    return dict(name=over.get("as", name), kind="action",
                cat=over.get("cat", a["cat"]),
                desc=desc or a["desc"], cfg=cfg,
                loop=over.get("loop", a["loop"]))


def F(name, desc, **cfg):
    cfg.setdefault("loop", False)
    return dict(name=name, kind="firearm", cat="firearm", desc=desc, cfg=cfg,
                loop=cfg["loop"])


# --------------------------------------------------------------------------- #
# ИГРОК
# --------------------------------------------------------------------------- #

PLAYER = [
    # --- локомоция ---
    L("walk", "Обычная ходьба", stride=24, cycle=1.0),
    L("walk_slow", "Медленная ходьба", stride=15, cycle=1.4),
    L("walk_fast", "Быстрая ходьба", stride=31, cycle=0.8, arm=28),
    L("walk_heavy", "Тяжёлая походка", stride=26, cycle=1.05, bob=3.2, arm=14, lean=8),
    L("walk_sneak", "Крадётся", stride=16, cycle=1.6, crouch=0.45, arm=8),
    L("walk_tired", "Устало идёт", stride=18, cycle=1.35, limp=0.3, lean=12, bob=2.2),
    L("walk_limp", "Хромает", stride=20, cycle=1.2, limp=0.9),
    L("walk_injured", "Идёт с ранением", stride=17, cycle=1.55, limp=1.0, lean=14, crouch=0.15),
    L("walk_drunk", "Идёт пьяным", stride=20, cycle=1.4, sway=5.0),
    L("walk_phone", "Идёт с телефоном", stride=20, cycle=1.1, arms="phone"),
    L("walk_smoke", "Идёт с сигаретой", stride=20, cycle=1.15, arms="smoke"),
    L("walk_back", "Идёт спиной вперёд", stride=16, cycle=1.25, back=1),
    L("walk_strafe_l", "Шаг влево", stride=15, cycle=1.1, side=-1),
    L("walk_strafe_r", "Шаг вправо", stride=15, cycle=1.1, side=1),
    L("walk_diag_fl", "Шаг вперёд-влево", stride=18, cycle=1.05, side=-0.7),
    L("walk_diag_fr", "Шаг вперёд-вправо", stride=18, cycle=1.05, side=0.7),
    L("walk_diag_bl", "Шаг назад-влево", stride=14, cycle=1.3, back=0.7, side=-0.7),
    L("walk_diag_br", "Шаг назад-вправо", stride=14, cycle=1.3, back=0.7, side=0.7),
    L("walk_carry_box", "Несёт коробку", stride=18, cycle=1.25, arms="box"),
    L("walk_carry_body", "Тащит тело", stride=16, cycle=1.35, arms="body", lean=6),
    L("walk_slope_up", "Подъём в гору", stride=20, cycle=1.3, knee=58, lean=10),
    L("walk_slope_down", "Спуск с горы", stride=20, cycle=1.15, knee=40, lean=2),
    L("walk_mud", "Идёт по грязи", stride=18, cycle=1.6, knee=62, bob=2.4),
    L("walk_wet", "Идёт по мокрому", stride=22, cycle=1.1, sway=2.6),
    L("walk_guard", "Идёт в стойке", stride=18, cycle=1.15, arms="guard"),
    L("walk_pistol", "Идёт с пистолетом", stride=18, cycle=1.2, arms="pistol_low"),
    L("walk_rifle", "Идёт с винтовкой", stride=18, cycle=1.25, arms="rifle"),
    L("walk_aim", "Идёт с прицелом", stride=14, cycle=1.4, arms="pistol"),
    L("run", "Бег", stride=34, cycle=0.72, knee=78, arm=34, bob=3.4, lean=14, elbow=40),
    L("run_jog", "Лёгкий бег", stride=26, cycle=0.92, knee=60, arm=24, bob=2.2, lean=8, elbow=30),
    L("run_sprint", "Спринт", stride=42, cycle=0.58, knee=92, arm=48, bob=4.2, lean=19, elbow=58),
    L("run_fast", "Быстрый бег", stride=38, cycle=0.66, knee=86, arm=40, bob=3.8, lean=16, elbow=50),
    L("run_pistol", "Бег с пистолетом", stride=30, cycle=0.78, arms="pistol_low", lean=12),
    L("run_rifle", "Бег с винтовкой", stride=28, cycle=0.8, arms="rifle", lean=12),
    L("run_panic", "Бежит в панике", stride=36, cycle=0.62, arm=56, lean=20, sway=3.0),
    L("run_back", "Бежит спиной", stride=22, cycle=0.9, back=1),
    L("run_slope_up", "Бег в гору", stride=26, cycle=0.82, lean=20, knee=84),
    L("run_slope_down", "Бег с горы", stride=32, cycle=0.7, lean=8, knee=70),
    L("run_limp", "Бежит хромая", stride=28, cycle=0.85, limp=0.8),
    L("run_wounded", "Бежит раненым", stride=24, cycle=1.02, limp=1.0, lean=16, crouch=0.2),
    L("crouch_walk", "Ходьба в приседе", stride=14, cycle=1.45, crouch=0.8, arm=10),
    L("crouch_walk_slow", "Медленный присед-шаг", stride=10, cycle=1.95, crouch=0.9, arm=8),
    L("crouch_walk_fast", "Быстрый присед-шаг", stride=18, cycle=1.0, crouch=0.75, arm=12),
    L("crouch_walk_back", "Присед назад", stride=12, cycle=1.6, crouch=0.8, back=1),
    L("crouch_walk_l", "Присед влево", stride=12, cycle=1.5, crouch=0.8, side=-1),
    L("crouch_walk_r", "Присед вправо", stride=12, cycle=1.5, crouch=0.8, side=1),
    L("crouch_sneak", "Крадётся в приседе", stride=12, cycle=2.2, crouch=1.0, arm=6),
    L("crouch_pistol_walk", "Присед с пистолетом", stride=12, cycle=1.55, crouch=0.8,
      arms="pistol"),
    L("crouch_rifle_walk", "Присед с винтовкой", stride=12, cycle=1.6, crouch=0.8,
      arms="rifle"),

    # --- idle ---
    I("idle", "Стоит и дышит", cycle=4.0),
    I("idle_alert", "Стоит настороже", cycle=4.5, look=2.2, arms="guard", sway=0.6),
    I("idle_cold", "Дрожит от холода", cycle=3.0, shiver=1.0, breath=1.0),
    I("idle_tired", "Стоит уставший", cycle=5.0, tired=1.0),
    I("idle_phone", "Смотрит в телефон", cycle=6.0, arms="phone"),
    I("idle_smoke", "Курит", cycle=6.0, arms="smoke"),
    I("idle_lean_wall", "Опирается на стену", cycle=5.0, lean=8.0, sway=0.6),
    I("idle_look_around", "Озирается", cycle=7.0, look=3.2),
    I("idle_weapon_low", "Стоит с оружием опущенным", cycle=5.0, arms="pistol_low"),
    I("idle_rifle_ready", "Стоит с винтовкой", cycle=5.0, arms="rifle"),
    I("idle_crouch", "Сидит в приседе", cycle=4.5, crouch=0.85),
    I("idle_crouch_alert", "Присед настороже", cycle=4.0, crouch=0.85, look=2.6),
    I("idle_crouch_pistol", "Присед с пистолетом", cycle=5.0, crouch=0.8, arms="pistol"),
    I("idle_exhausted", "Тяжело дышит", cycle=2.4, tired=1.0, breath=3.2),
    I("idle_impatient", "Нетерпеливо ждёт", cycle=3.0, impatient=1.0),
    I("idle_arms_crossed", "Стоит со скрещёнными руками", cycle=5.0, arms="crossed", sway=0.5),
    I("idle_hands_pockets", "Руки в карманах", cycle=5.0, arms="pockets"),
    I("idle_drunk", "Стоит пьяный, качает", cycle=5.0, drunk=1.0, sway=2.0),
    I("idle_combat", "Боевая стойка", cycle=3.0, arms="guard", sway=0.4),

    # --- взаимодействия ---
    X("door_open"), X("door_open_slow"), X("door_close"), X("door_knock"),
    X("door_kick"), X("door_lockpick"), X("window_open"), X("window_climb"),
    X("pick_up"), X("pick_up_ground"), X("loot_body"), X("search_pockets"),
    X("register_take", desc="Забирает деньги из кассы"),
    X("drawer_open"), X("atm_use"), X("vending_use"), X("shop_buy"),
    X("eat"), X("drink"), X("take_photo"), X("phone_call"), X("check_watch"),
    X("wave"), X("point"), X("salute"), X("thumbs_up"), X("beckon"), X("shrug"),
    X("facepalm"), X("clap"), X("stretch"), X("crack_neck"), X("wipe_brow"),
    X("throw_object"), X("throw_grenade"), X("aim_grenade"), X("plant_bomb"),
    X("detonate"), X("push_object"), X("jack_car"), X("fuel_car"), X("car_repair"),
    X("car_trunk"), X("car_open_door"), X("car_enter"), X("car_drive_idle"),
    X("car_exit"), X("hotwire"), X("handcuff"),
    X("sit_down"), X("sit_idle"), X("stand_up"),

    # --- паркур ---
    X("jump_standing"), X("jump_run"), X("jump_forward"), X("land_soft"),
    X("land_hard"), X("roll_land"), X("vault_low"), X("vault_high"),
    X("mantle"), X("climb_wall"), X("climb_ladder"), X("ledge_hang"),
    X("shimmy", desc="Двигается вдоль уступа"), X("climb_fence"), X("drop_down"),

    # --- рукопашный бой ---
    X("punch_jab"), X("punch_cross"), X("punch_jab_left"), X("punch_cross_left"),
    X("punch_hook_l"), X("punch_hook_r"), X("punch_uppercut"), X("punch_combo_3"),
    X("punch_running"), X("punch_crouch"),
    X("elbow_strike"), X("headbutt"), X("tackle"), X("grapple_throw"),
    X("kick_front"), X("kick_low"), X("kick_side"), X("knee_strike"), X("stomp"),
    X("block_hold"), X("block_impact"), X("dodge_left"), X("dodge_right"),
    X("dodge_back"), X("bat_swing"), X("bat_overhead"), X("crowbar_swing"),
    X("knife_slash"), X("knife_stab"),

    # --- попадания и смерть ---
    X("hit_light"), X("hit_heavy"), X("hit_head"), X("hit_back"),
    X("knockdown"), X("getup"), X("death_forward"), X("death_back"),
    X("death_headshot"), X("death_crouch"), X("stumble"), X("fall_loop"),

    # --- оружие ---
    X("draw_pistol"), X("holster_pistol"), X("draw_rifle"), X("holster_rifle"),
    X("reload_pistol"), X("reload_rifle"), X("reload_shotgun"),
    F("aim_pistol", "Целится из пистолета (цикл)", aim="pistol", loop=True,
      dur=4.0, sway=1.1),
    F("aim_pistol_low", "Держит пистолет опущенным (цикл)", aim="pistol_low",
      loop=True, dur=4.0, sway=1.0),
    F("aim_rifle", "Целится из винтовки (цикл)", aim="rifle", loop=True,
      dur=5.0, sway=0.9),
    F("aim_shotgun", "Целится из дробовика (цикл)", aim="shotgun", loop=True,
      dur=5.0, sway=0.9),
    F("aim_crouch_pistol", "Целится из пистолета в приседе", aim="pistol",
      crouch=0.8, loop=True, dur=4.0, sway=0.9),
    F("aim_crouch_rifle", "Целится из винтовки в приседе", aim="rifle",
      crouch=0.8, loop=True, dur=5.0, sway=0.8),
    F("shoot_pistol", "Выстрел из пистолета", aim="pistol", shots=1, dur=1.2),
    F("shoot_pistol_double", "Дубль из пистолета", aim="pistol", shots=2, dur=1.3),
    F("shoot_pistol_crouch", "Выстрел из пистолета в приседе", aim="pistol",
      crouch=0.8, shots=1, dur=1.2),
    F("shoot_rifle", "Выстрел из винтовки", aim="rifle", shots=1, dur=1.5),
    F("shoot_rifle_burst", "Очередь из винтовки", aim="rifle", burst=True,
      shots=1, dur=1.6),
    F("shoot_shotgun", "Выстрел из дробовика", aim="shotgun", shots=1, dur=1.7,
      recoil=13),
    F("shoot_cover_right", "Стреляет из-за укрытия справа", aim="rifle",
      cover="right", shots=3, dur=2.0),
    F("shoot_cover_left", "Стреляет из-за укрытия слева", aim="rifle",
      cover="left", shots=3, dur=2.0),
    F("blindfire_right", "Стреляет вслепую", aim="pistol_low", cover="right",
      shots=2, dur=1.6),
]

# --------------------------------------------------------------------------- #
# ГРАЖДАНСКИЕ
# --------------------------------------------------------------------------- #

CIVILIAN = [
    L("walk", "Обычная ходьба", stride=23, cycle=1.05),
    L("walk_slow", "Прогулочный шаг", stride=16, cycle=1.5),
    L("walk_fast", "Спешит", stride=30, cycle=0.85, arm=26),
    L("walk_phone", "Идёт с телефоном", stride=20, cycle=1.15, arms="phone"),
    L("walk_bag", "Идёт с сумкой", stride=20, cycle=1.1, arms="box"),
    L("walk_elderly", "Пожилая походка", stride=13, cycle=1.7, lean=12, crouch=0.12),
    L("walk_child_style", "Детская походка", stride=20, cycle=1.0, bob=2.6, arm=30),
    L("walk_heavy", "Тяжёлая походка", stride=25, cycle=1.1, bob=3.0, arm=14, lean=8),
    L("walk_drunk", "Пьяная походка", stride=19, cycle=1.45, sway=5.5),
    L("walk_tired", "Устал", stride=17, cycle=1.4, limp=0.3, lean=12),
    L("walk_injured", "Хромает", stride=18, cycle=1.5, limp=1.0, lean=10),
    L("walk_back", "Пятится", stride=15, cycle=1.25, back=1),
    L("walk_strafe_l", "Шаг влево", stride=15, cycle=1.15, side=-1),
    L("walk_strafe_r", "Шаг вправо", stride=15, cycle=1.15, side=1),
    L("walk_diag_fl", "Шаг вперёд-влево", stride=18, cycle=1.1, side=-0.7),
    L("walk_diag_fr", "Шаг вперёд-вправо", stride=18, cycle=1.1, side=0.7),
    L("walk_diag_bl", "Шаг назад-влево", stride=13, cycle=1.35, back=0.7, side=-0.7),
    L("walk_diag_br", "Шаг назад-вправо", stride=13, cycle=1.35, back=0.7, side=0.7),
    L("walk_shopping", "Идёт из магазина", stride=19, cycle=1.2, arms="box"),
    L("walk_mud", "По грязи", stride=17, cycle=1.6, knee=58, bob=2.2),
    L("run", "Бежит", stride=30, cycle=0.8, knee=74, arm=32, bob=3.0, lean=12, elbow=40),
    L("run_jog", "Пробежка", stride=24, cycle=0.98, knee=58, arm=22, bob=2.0, lean=8, elbow=28),
    L("run_sprint", "Бежит со всех ног", stride=40, cycle=0.6, knee=90, arm=46, bob=4.0,
      lean=18, elbow=56),
    L("run_panic", "Бежит в панике", stride=36, cycle=0.62, arm=56, lean=22, sway=3.0),
    L("crouch_walk", "Присед-шаг", stride=13, cycle=1.5, crouch=0.8, arm=10),
    L("crouch_walk_slow", "Осторожный присед-шаг", stride=9, cycle=2.0, crouch=0.9),
    L("crouch_sneak", "Прячется в приседе", stride=11, cycle=2.2, crouch=1.0, arm=6),

    I("idle", "Стоит и дышит", cycle=4.2),
    I("idle_phone", "Смотрит в телефон", cycle=6.0, arms="phone"),
    I("idle_smoke", "Курит", cycle=6.5, arms="smoke"),
    I("idle_look_around", "Озирается", cycle=7.0, look=3.4),
    I("idle_impatient", "Ждёт и нервничает", cycle=3.0, impatient=1.0),
    I("idle_cold", "Дрожит", cycle=3.2, shiver=1.0),
    I("idle_tired", "Устал стоять", cycle=5.0, tired=1.0),
    I("idle_arms_crossed", "Скрестил руки", cycle=5.0, arms="crossed"),
    I("idle_hands_pockets", "Руки в карманах", cycle=5.5, arms="pockets"),
    I("idle_drunk", "Качается пьяный", cycle=5.0, drunk=1.0, sway=2.0),
    I("idle_crouch", "Присел", cycle=4.5, crouch=0.85),
    I("idle_alert", "Испуганно оглядывается", cycle=3.6, look=3.0),
    I("idle_lean_wall", "Опирается на стену", cycle=5.0, lean=8.0),
    I("idle_waiting", "Ждёт на остановке", cycle=6.0, look=1.6),

    X("door_open"), X("door_close"), X("door_knock"), X("door_lockpick"),
    X("window_open"), X("pick_up"), X("pick_up_ground"),
    X("atm_use"), X("vending_use"), X("shop_buy"), X("drawer_open"),
    X("eat"), X("drink"), X("smoke_drag"), X("take_photo"), X("phone_call"),
    X("check_watch"), X("read_phone"), X("selfie"),
    X("sit_down"), X("sit_idle"), X("stand_up"), X("sit_ground"),
    X("sit_ground_idle"),
    X("wave"), X("point"), X("thumbs_up"), X("beckon"), X("shrug"),
    X("facepalm"), X("clap"), X("stretch"), X("crack_neck"), X("wipe_brow"),
    X("throw_object"), X("push_object"),
    X("groceries"), X("dance"), X("chat"), X("laugh"), X("argue"),
    X("construction_work"), X("cook"), X("clean_floor"), X("jog_stretch"),
    X("cold_shiver"), X("squat_exercise"), X("bus_wait"),
    X("car_open_door"), X("car_enter"), X("car_drive_idle"), X("car_exit"),
    X("car_trunk"), X("fuel_car"),
    X("jump_standing"), X("jump_run"), X("land_soft"), X("drop_down"),
    X("climb_fence"), X("vault_low"),
    X("hit_light"), X("hit_heavy"), X("hit_head"), X("knockdown"), X("getup"),
    X("death_forward"), X("death_back"), X("death_crouch"), X("stumble"),
    X("fall_loop"), X("panic"), X("surrender"), X("suspect_kneel"),
    X("punch_jab"), X("punch_cross"), X("kick_front"), X("tackle"),
    X("block_hold"), X("dodge_back"),
]

# --------------------------------------------------------------------------- #
# ПОЛИЦИЯ
# --------------------------------------------------------------------------- #

POLICE = [
    L("walk", "Патрульная ходьба", stride=24, cycle=1.05),
    L("walk_slow", "Медленный обход", stride=16, cycle=1.5),
    L("walk_fast", "Спешит на вызов", stride=30, cycle=0.85, arm=26),
    L("walk_patrol", "Патрулирует", stride=22, cycle=1.15, look=1.0),
    L("walk_pistol", "Идёт с пистолетом", stride=18, cycle=1.25, arms="pistol_low"),
    L("walk_rifle", "Идёт с винтовкой", stride=18, cycle=1.3, arms="rifle"),
    L("walk_guard", "Идёт в стойке", stride=18, cycle=1.2, arms="guard"),
    L("walk_back", "Пятится", stride=15, cycle=1.3, back=1),
    L("walk_strafe_l", "Шаг влево", stride=15, cycle=1.15, side=-1),
    L("walk_strafe_r", "Шаг вправо", stride=15, cycle=1.15, side=1),
    L("walk_diag_fl", "Шаг вперёд-влево", stride=18, cycle=1.1, side=-0.7),
    L("walk_diag_fr", "Шаг вперёд-вправо", stride=18, cycle=1.1, side=0.7),
    L("walk_injured", "Хромает после ранения", stride=18, cycle=1.5, limp=1.0, lean=10),
    L("walk_heavy", "Тяжёлая походка (броня)", stride=25, cycle=1.1, bob=3.0, lean=8),
    L("run", "Бег", stride=32, cycle=0.75, knee=76, arm=32, bob=3.2, lean=13, elbow=40),
    L("run_jog", "Пробежка", stride=24, cycle=0.95, knee=58, arm=22, bob=2.0, lean=8, elbow=28),
    L("run_sprint", "Спринт", stride=40, cycle=0.6, knee=90, arm=46, bob=4.0, lean=18, elbow=56),
    L("run_pistol", "Бежит с пистолетом", stride=28, cycle=0.8, arms="pistol_low", lean=12),
    L("run_back", "Бежит спиной", stride=22, cycle=0.95, back=1),
    L("run_slope_up", "Бег в гору", stride=26, cycle=0.85, lean=18, knee=82),
    L("crouch_walk", "Присед-шаг", stride=13, cycle=1.5, crouch=0.8, arm=10),
    L("crouch_walk_slow", "Осторожный присед-шаг", stride=10, cycle=1.95, crouch=0.9),
    L("crouch_walk_fast", "Быстрый присед-шаг", stride=17, cycle=1.05, crouch=0.75),
    L("crouch_walk_l", "Присед влево", stride=12, cycle=1.5, crouch=0.8, side=-1),
    L("crouch_walk_r", "Присед вправо", stride=12, cycle=1.5, crouch=0.8, side=1),
    L("crouch_pistol_walk", "Присед с пистолетом", stride=12, cycle=1.55, crouch=0.8,
      arms="pistol"),
    L("crouch_rifle_walk", "Присед с винтовкой", stride=12, cycle=1.6, crouch=0.8,
      arms="rifle"),

    I("idle", "Стоит на посту", cycle=4.5),
    I("idle_patrol", "Патрульный покой", cycle=4.5, look=2.0),
    I("idle_alert", "Настороже", cycle=3.6, look=2.6, arms="guard"),
    I("idle_radio", "Слушает рацию", cycle=5.0, arms="guard", sway=0.5),
    I("idle_pistol_low", "Держит пистолет опущенным", cycle=5.0, arms="pistol_low"),
    I("idle_rifle_ready", "Держит винтовку", cycle=5.0, arms="rifle"),
    I("idle_crouch", "В приседе", cycle=4.5, crouch=0.85),
    I("idle_crouch_alert", "Присед, наблюдает", cycle=4.0, crouch=0.85, look=2.6),
    I("idle_crouch_pistol", "Присед с пистолетом", cycle=5.0, crouch=0.8, arms="pistol"),
    I("idle_check_belt", "Поправляет снаряжение", cycle=6.0, arms="pockets"),
    I("idle_tired", "Устал на смене", cycle=5.0, tired=1.0),
    I("idle_impatient", "Ждёт подкрепление", cycle=3.2, impatient=1.0),
    I("idle_cold", "Мёрзнет", cycle=3.2, shiver=0.8),

    X("radio_call"), X("write_ticket"), X("pat_down"), X("arrest_push"),
    X("handcuff"), X("suspect_kneel"), X("taser_draw"), X("taser_fire"),
    X("baton_draw"), X("baton_swing"), X("shield_block"), X("shield_bash"),
    X("flashlight_scan"), X("direct_traffic"),
    X("door_open"), X("door_kick"), X("door_knock"), X("pick_up"),
    X("loot_body"), X("search_pockets"), X("register_take"),
    X("draw_pistol"), X("holster_pistol"), X("draw_rifle"), X("holster_rifle"),
    X("reload_pistol"), X("reload_rifle"), X("reload_shotgun"),
    X("car_open_door"), X("car_enter"), X("car_drive_idle"), X("car_exit"),
    X("car_trunk"), X("sit_down"), X("sit_idle"), X("stand_up"),
    X("wave"), X("point"), X("salute"), X("beckon"), X("check_watch"),
    X("phone_call"), X("push_object"), X("jump_run"), X("land_hard"),
    X("vault_low"), X("vault_high"), X("mantle"), X("climb_fence"),
    X("hit_light"), X("hit_heavy"), X("hit_head"), X("knockdown"), X("getup"),
    X("death_forward"), X("death_back"), X("death_headshot"), X("stumble"),
    X("block_hold"), X("block_impact"), X("dodge_left"), X("dodge_right"),
    X("punch_jab"), X("punch_cross"), X("kick_front"), X("tackle"),

    F("aim_pistol", "Целится из пистолета (цикл)", aim="pistol", loop=True,
      dur=4.0, sway=1.0),
    F("aim_pistol_low", "Пистолет опущен (цикл)", aim="pistol_low", loop=True,
      dur=4.0, sway=1.0),
    F("aim_rifle", "Целится из винтовки (цикл)", aim="rifle", loop=True,
      dur=5.0, sway=0.9),
    F("aim_shotgun", "Целится из дробовика (цикл)", aim="shotgun", loop=True,
      dur=5.0, sway=0.9),
    F("aim_crouch_pistol", "Целится в приседе", aim="pistol", crouch=0.8,
      loop=True, dur=4.0, sway=0.9),
    F("shoot_pistol", "Выстрел из пистолета", aim="pistol", shots=1, dur=1.2),
    F("shoot_pistol_double", "Дубль из пистолета", aim="pistol", shots=2, dur=1.3),
    F("shoot_pistol_crouch", "Выстрел из пистолета в приседе", aim="pistol",
      crouch=0.8, shots=1, dur=1.2),
    F("shoot_rifle", "Выстрел из винтовки", aim="rifle", shots=1, dur=1.5),
    F("shoot_rifle_burst", "Очередь из винтовки", aim="rifle", burst=True,
      shots=1, dur=1.6),
    F("shoot_shotgun", "Выстрел из дробовика", aim="shotgun", shots=1, dur=1.7,
      recoil=13),
    F("shoot_cover_right", "Стреляет из-за укрытия справа", aim="rifle",
      cover="right", shots=3, dur=2.0),
    F("shoot_cover_left", "Стреляет из-за укрытия слева", aim="rifle",
      cover="left", shots=3, dur=2.0),
    F("blindfire_right", "Стреляет вслепую", aim="pistol_low", cover="right",
      shots=2, dur=1.6),
]

SETS = {"player": PLAYER, "civilian": CIVILIAN, "police": POLICE}


# --------------------------------------------------------------------------- #
# сборка
# --------------------------------------------------------------------------- #

def frames_for(spec):
    cfg = dict(spec["cfg"])
    if spec["kind"] == "loco":
        fn = C.locomotion(cfg)
        dur = cfg.get("cycle", 1.0)
    elif spec["kind"] == "idle":
        fn = C.idle(cfg)
        dur = cfg.get("cycle", 4.0)
    elif spec["kind"] == "action":
        fn = C.action(cfg)
        dur = cfg["dur"]
    else:
        fn = C.firearm(cfg)
        dur = cfg.get("dur", 1.0)
    n = max(3, int(round(dur * FPS)) + 1)
    return [to_pose(fn(i / (n - 1))) for i in range(n)], dur


def attribution(fname):
    path = os.path.join(ROOT, "models", "sketchfab_manifest.json")
    if not os.path.exists(path):
        return {}
    data = json.load(open(path))
    entries = data.get("models", data)
    items = entries.items() if isinstance(entries, dict) else [(e.get("file", ""), e) for e in entries]
    short = fname.split("__")[0]
    for key, val in items:
        if short in str(key) and isinstance(val, dict):
            return {"author": val.get("author"), "license": val.get("license"),
                    "source": val.get("source")}
    return {}


def build():
    os.makedirs(OUTDIR, exist_ok=True)
    manifest = {"schema": 1, "fps": FPS, "characters": {}}
    for key, meta in CHARACTERS.items():
        src = os.path.join(MODELS, meta["file"])
        rig = Rig(src)
        # исходный одиночный клип источника заменяем на полный набор
        rig.js.pop("animations", None)
        specs = SETS[key]
        seen = set()
        animations = []
        for spec in specs:
            name = spec["name"]
            if name in seen:
                raise SystemExit(f"дубль имени клипа: {key}/{name}")
            seen.add(name)
            frames, dur = frames_for(spec)
            info = rig.add_animation(name, FPS, frames, spec["loop"], extras={
                "category": spec["cat"], "description": spec["desc"]})
            animations.append({"name": name, "category": spec["cat"],
                               "description": spec["desc"],
                               "duration": info["duration"], "loop": spec["loop"],
                               "channels": info["channels"]})
        out = os.path.join(OUTDIR, meta["out"])
        write_glb(out, rig.js, rig.bin)
        size = os.path.getsize(out) / (1024 * 1024)
        manifest["characters"][key] = {
            "label": meta["label"],
            "file": meta["out"],
            "sourceModel": "models/sketchfab/" + meta["file"],
            "source": attribution(meta["file"]),
            "sizeMB": round(size, 2),
            "clips": len(animations),
            "animations": animations,
        }
        print(f"{meta['out']}: {len(animations)} клипов, {size:.1f} МБ")

    with open(os.path.join(OUTDIR, "clips.json"), "w") as f:
        json.dump(manifest, f, ensure_ascii=False, indent=1)
    write_docs(manifest)


CATS = [
    ("locomotion", "Локомоция"),
    ("idle", "Покой"),
    ("melee", "Рукопашный бой"),
    ("firearm", "Огнестрел"),
    ("reaction", "Реакции и смерть"),
    ("parkour", "Паркур"),
    ("interaction", "Взаимодействия"),
    ("gesture", "Жесты"),
    ("vehicle", "Транспорт"),
    ("police", "Полицейские процедуры"),
    ("civilian", "Бытовые / гражданские"),
]


def write_docs(manifest):
    lines = ["# Анимации персонажей", ""]
    total = sum(c["clips"] for c in manifest["characters"].values())
    lines += [
        f"Всего **{total} клипов** в трёх наборах. Клипы вшиты в GLB как обычные "
        "glTF-анимации: имя клипа лежит в `animations[].name`, метаданные — в "
        "`animations[].extras` (`category`, `description`, `fps`, `loop`, `duration`).",
        "",
        "| Набор | Файл | Клипов | Размер | Исходная модель |",
        "| --- | --- | ---: | ---: | --- |",
    ]
    for key, ch in manifest["characters"].items():
        lines.append(f"| {ch['label']} (`{key}`) | `{ch['file']}` | {ch['clips']} | "
                     f"{ch['sizeMB']} МБ | `{os.path.basename(ch['sourceModel'])}` |")
    lines += [
        "",
        "Скелет у всех персонажей общий — mixamo, 66 костей (`mixamorig:Hips`, "
        "`Spine`, `LeftArm`, …). Значит, любой клип из набора можно ретаргетить по "
        "именам костей на остальных mixamo-персонажей из `models/sketchfab/`.",
        "",
        "## Как смотреть",
        "",
        "Откройте `game/animations/viewer.html` (нужен локальный сервер, например "
        "`python3 -m http.server` из корня репозитория) — там переключатель наборов, "
        "список клипов, ползунок времени и проигрывание.",
        "",
        "## Технические соглашения",
        "",
        "- частота дискретизации: **30 fps**, интерполяция `LINEAR`, каналы — "
        "`rotation` на анимируемых костях + `translation` таза;",
        "- все клипы **in-place** (без root motion): перемещение в мире делает "
        "движок, анимация только двигает таз в пределах шага;",
        "- циклы помечены `loop: true`, у них первый и последний кадр совпадают;",
        "- масштаб моделей в сантиметрах (таз ~87–137 units), Y — вверх.",
        "",
        "## Полные списки",
        "",
    ]
    for key, ch in manifest["characters"].items():
        lines += [f"### {ch['label']}", ""]
        src = ch.get("source") or {}
        if src.get("author"):
            lines += [f"Модель: {src['author']} · лицензия {src.get('license', '?')} · "
                      f"{src.get('source', '')}", ""]
        by_cat = {}
        for a in ch["animations"]:
            by_cat.setdefault(a["category"], []).append(a)
        for cat, title in CATS:
            items = by_cat.get(cat)
            if not items:
                continue
            lines += [f"**{title}** — {len(items)}", "",
                      "| Клип | Сек | Цикл | Описание |", "| --- | ---: | :---: | --- |"]
            for a in items:
                lines.append(f"| `{a['name']}` | {a['duration']:.2f} | "
                             f"{'да' if a['loop'] else '—'} | {a['description']} |")
            lines.append("")
    with open(os.path.join(OUTDIR, "ANIMATIONS.md"), "w") as f:
        f.write("\n".join(lines))


if __name__ == "__main__":
    build()
