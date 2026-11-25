using System;
using System.Collections.Generic;

namespace GTA6.Core
{
    public enum Lang { Ru = 0, En = 1 }

    /// <summary>Two-language string table. Pure C#, no UnityEngine types, so it can be unit tested.</summary>
    public static class Loc
    {
        static readonly Dictionary<string, string[]> _t = new Dictionary<string, string[]>(400);

        public static Lang Current = Lang.Ru;

        static Loc()
        {
            // ---- menu / shell ----
            A("game.title", "GTA VI: ЛЕОНИДА", "GTA VI: LEONIDA");
            A("game.subtitle", "фанатский мобильный концепт", "a fan-made mobile concept");
            A("menu.newgame", "Новая игра", "New game");
            A("menu.continue", "Продолжить", "Continue");
            A("menu.settings", "Настройки", "Settings");
            A("menu.controls", "Управление", "Controls");
            A("menu.credits", "Об игре", "About");
            A("menu.quit", "Выход", "Quit");
            A("menu.tap", "нажмите, чтобы начать", "tap to start");
            A("menu.reset", "Сбросить прогресс", "Reset progress");
            A("menu.reset.q", "Удалить сохранение и начать заново?", "Delete the save and start over?");
            A("menu.legal", "Фанатский некоммерческий концепт. Не связан с Rockstar Games.",
              "Non-commercial fan concept. Not affiliated with Rockstar Games.");
            A("menu.press", "Нажмите «Новая игра»", "Press New game");

            A("ui.back", "Назад", "Back");
            A("ui.close", "Закрыть", "Close");
            A("ui.ok", "ОК", "OK");
            A("ui.cancel", "Отмена", "Cancel");
            A("ui.yes", "Да", "Yes");
            A("ui.no", "Нет", "No");
            A("ui.save", "Сохранить", "Save");
            A("ui.load", "Загрузить", "Load");
            A("ui.saved", "Игра сохранена", "Game saved");
            A("ui.loading", "Загрузка...", "Loading...");
            A("ui.pause", "Пауза", "Paused");
            A("ui.resume", "Продолжить", "Resume");
            A("ui.restart", "Начать заново", "Restart");
            A("ui.confirm", "Подтвердить", "Confirm");
            A("ui.off", "выкл", "off");
            A("ui.on", "вкл", "on");
            A("ui.coming", "Скоро", "Coming soon");
            A("ui.empty", "Пусто", "Empty");
            A("ui.locked", "Заблокировано", "Locked");
            A("ui.level", "Уровень", "Level");

            // ---- settings ----
            A("set.quality", "Графика", "Graphics");
            A("set.quality.low", "Низкая", "Low");
            A("set.quality.med", "Средняя", "Medium");
            A("set.quality.high", "Высокая", "High");
            A("set.quality.ultra", "Ультра", "Ultra");
            A("set.sound", "Звук", "Sound");
            A("set.music", "Музыка", "Music");
            A("set.sens", "Чувствительность", "Sensitivity");
            A("set.invert", "Инверсия камеры по Y", "Invert camera Y");
            A("set.language", "Язык", "Language");
            A("set.shadows", "Тени", "Shadows");
            A("set.fps", "Счётчик FPS", "FPS counter");
            A("set.autoaim", "Автоприцел", "Auto-aim");
            A("set.vibration", "Вибрация", "Vibration");
            A("set.reset", "Сбросить настройки", "Reset settings");

            // ---- hud ----
            A("hud.health", "Здоровье", "Health");
            A("hud.armor", "Броня", "Armor");
            A("hud.cash", "$", "$");
            A("hud.ammo", "Патроны", "Ammo");
            A("hud.stars", "Розыск", "Wanted");
            A("hud.mission", "Задание", "Mission");
            A("hud.district", "Район", "District");
            A("hud.speed", "км/ч", "km/h");
            A("hud.enter", "Сесть в машину", "Enter vehicle");
            A("hud.exit", "Выйти", "Exit");
            A("hud.pickup", "Подобрать", "Pick up");
            A("hud.call", "Ответить", "Answer");
            A("hud.phone", "Телефон", "Phone");
            A("hud.map", "Карта", "Map");
            A("hud.objective", "Цель", "Objective");
            A("hud.distance", "м", "m");
            A("hud.wasted", "ВАС УБИЛИ", "WASTED");
            A("hud.busted", "ВАС АРЕСТОВАЛИ", "BUSTED");
            A("hud.mission.pass", "ЗАДАНИЕ ВЫПОЛНЕНО", "MISSION PASSED");
            A("hud.mission.fail", "ЗАДАНИЕ ПРОВАЛЕНО", "MISSION FAILED");
            A("hud.reward", "Награда", "Reward");
            A("hud.time", "Время", "Time");
            A("hud.tutorial", "Подсказка", "Hint");
            A("hud.day", "день", "day");

            // ---- controls ----
            A("ctl.move", "Джойстик слева — движение", "Left stick — move");
            A("ctl.look", "Правая половина экрана — камера", "Right side of the screen — camera");
            A("ctl.run", "Проведите джойстик до упора — бег", "Push the stick fully to sprint");
            A("ctl.jump", "Кнопка JUMP — прыжок", "JUMP button — jump");
            A("ctl.fire", "Кнопка FIRE — стрельба", "FIRE button — shoot");
            A("ctl.aim", "Прицел включается автоматически у врага", "Aim engages automatically near an enemy");
            A("ctl.car", "Кнопка CAR рядом с машиной — сесть/выйти", "CAR next to a vehicle — enter/exit");
            A("ctl.phone", "Кнопка PHONE — телефон с 16 приложениями", "PHONE button — the 16-app phone");
            A("ctl.keys", "На ПК: WASD, мышь, E, Shift, Space, F, Tab, Esc",
              "On desktop: WASD, mouse, E, Shift, Space, F, Tab, Esc");

            // ---- phone ----
            A("phone.title", "LeoPhone", "LeoPhone");
            A("phone.signal", "LeoNet 5G", "LeoNet 5G");
            A("phone.calls", "Недавние", "Recents");
            A("phone.call", "Позвонить", "Call");
            A("phone.hangup", "Сбросить", "Hang up");
            A("phone.answer", "Ответить", "Answer");
            A("phone.decline", "Отклонить", "Decline");
            A("phone.music", "Сейчас играет", "Now playing");
            A("phone.balance", "Баланс", "Balance");
            A("phone.send", "Отправить", "Send");
            A("phone.contact", "Контакт", "Contact");
            A("phone.unknown", "Неизвестный номер", "Unknown number");
            A("phone.busy", "Абонент недоступен", "The person you dialled is unavailable");
            A("phone.noapps", "Нет приложений", "No apps");
            A("phone.weather.sun", "Солнечно, +31°C", "Sunny, 31°C");
            A("phone.weather.rain", "Ливень, +24°C", "Downpour, 24°C");
            A("phone.weather.night", "Ясно, +26°C", "Clear, 26°C");
            A("phone.help1", "Проведите по экрану, чтобы сменить страницу.",
              "Swipe the screen to change page.");
            A("phone.help2", "Нажмите на иконку, чтобы открыть приложение.",
              "Tap an icon to open an app.");
            A("phone.home", "Домой", "Home");
            A("phone.rec", "Запись", "Recording");
            A("phone.photo", "Снимок", "Snapshot");
            A("phone.noheist", "В этом прототипе ограблений нет.",
              "Robberies are not in this prototype.");
            A("phone.search", "Поиск в LeoNet", "Search LeoNet");
            A("phone.offline", "Нет сети", "No signal");
            A("phone.shop", "Магазин", "Store");
            A("phone.buy", "Купить", "Buy");
            A("phone.owned", "Куплено", "Owned");
            A("phone.poor", "Недостаточно денег", "Not enough money");
            A("phone.installed", "Установлено", "Installed");
            A("phone.update", "Обновить", "Update");

            // ---- apps ----
            A("app.map", "Карты", "Maps");
            A("app.contacts", "Контакты", "Contacts");
            A("app.messages", "Сообщения", "Messages");
            A("app.calls", "Телефон", "Phone");
            A("app.radio", "Радио", "Radio");
            A("app.photo", "Камера", "Camera");
            A("app.gallery", "Галерея", "Gallery");
            A("app.music", "Музыка", "Music");
            A("app.bank", "Банк", "Bank");
            A("app.shop", "Магазин", "Store");
            A("app.missions", "Задания", "Jobs");
            A("app.garage", "Гараж", "Garage");
            A("app.weather", "Погода", "Weather");
            A("app.news", "Новости", "News");
            A("app.browser", "LeoNet", "LeoNet");
            A("app.settings", "Настройки", "Settings");

            // ---- districts' flavour ----
            A("news.1", "Полиция Леониды усилила патрули после серии угонов в Даунтауне.",
              "Leonida PD stepped up patrols after a wave of car thefts downtown.");
            A("news.2", "Штормовое предупреждение: к вечеру ожидается ливень и сильный ветер.",
              "Storm warning: heavy rain and strong wind expected by the evening.");
            A("news.3", "Открытие нового пирса в Порт-Геллхорне отложено на неделю.",
              "The new pier opening in Port Gellhorn is delayed by a week.");
            A("news.4", "Курс леонидского доллара вырос третий день подряд.",
              "The Leonida dollar rose for a third day in a row.");

            // ---- missions ----
            A("mis.prologue.title", "Первый день", "First Day");
            A("mis.prologue.desc", "Донни ждёт у трейлерного парка. Найди его.",
              "Donny is waiting by the trailer park. Find him.");
            A("mis.cash.title", "Наличные", "Cash Flow");
            A("mis.cash.desc", "Забери сумку в порту, пока её не забрал кто-то другой.",
              "Grab the bag at the docks before someone else does.");
            A("mis.debt.title", "Долг", "The Debt");
            A("mis.debt.desc", "Должники в Литл-Гаване не хотят платить. Убеди их.",
              "The debtors in Little Havana won't pay. Convince them.");
            A("mis.ride.title", "Угон", "Ride Out");
            A("mis.ride.desc", "Укради тачку с пляжа и довези её до гаража целой.",
              "Steal a car on the beach and deliver it to the garage in one piece.");
            A("mis.finale.title", "Финал", "Finale");
            A("mis.finale.desc", "Разборка на крыше отеля. Не дай им уйти.",
              "A showdown on the hotel roof. Don't let them walk away.");

            A("step.goto", "Дойдите до отметки", "Reach the marker");
            A("step.drive", "Доедьте до отметки", "Drive to the marker");
            A("step.kill", "Устраните всех", "Take them all out");
            A("step.survive", "Продержитесь", "Survive");
            A("step.deliver", "Доставьте машину целой", "Deliver the vehicle in one piece");
            A("step.escape", "Уйдите от полиции", "Lose the police");
            A("step.talk", "Поговорите", "Talk");
            A("step.progress", "Прогресс", "Progress");
            A("step.fail.dead", "Вы погибли", "You died");
            A("step.fail.busted", "Вас арестовали", "You were arrested");
            A("step.fail.car", "Машина уничтожена", "The vehicle was destroyed");

            // ---- radio ----
            A("radio.off", "Радио выключено", "Radio off");
            A("radio.next", "Следующая станция", "Next station");
            A("radio.prev", "Предыдущая станция", "Previous station");

            A("death.retry", "Возродиться в больнице", "Respawn at the hospital");
            A("death.cost", "Счёт за лечение", "Hospital bill");
            A("busted.fine", "Штраф", "Fine");
            A("crime.hit", "Нападение", "Assault");
            A("crime.shot", "Стрельба в городе", "Shots fired in public");
            A("crime.car", "Угон", "Grand theft auto");
            A("crime.kill", "Убийство", "Homicide");
            A("shop.ammo", "Патроны", "Ammo");
            A("shop.health", "Аптечка", "Medkit");
            A("shop.armor", "Бронежилет", "Body armour");
            A("shop.paint", "Покраска машины", "Respray");
            A("shop.buy", "Купить", "Buy");
            A("shop.bought", "Готово", "Done");
        }

        static void A(string key, string ru, string en) { _t[key] = new[] { ru, en }; }

        public static int KeyCount { get { return _t.Count; } }

        public static bool Has(string key) { return key != null && _t.ContainsKey(key); }

        public static void SetLang(Lang l) { Current = l; }

        public static void ToggleLang() { Current = Current == Lang.Ru ? Lang.En : Lang.Ru; }

        public static string S(string key)
        {
            string[] pair;
            if (key != null && _t.TryGetValue(key, out pair)) return pair[(int)Current];
            return key;
        }

        public static string F(string key, params object[] args)
        {
            string s = S(key);
            if (args == null || args.Length == 0) return s;
            try { return string.Format(s, args); }
            catch (FormatException) { return s; }
        }

        /// <summary>1234567 -> "1 234 567" (ru) or "1,234,567" (en).
        /// Grouped by hand so the separator is a plain space, which every font we use has.</summary>
        public static string Num(int value)
        {
            bool negative = value < 0;
            long v = negative ? -(long)value : value;
            string digits = v.ToString(System.Globalization.CultureInfo.InvariantCulture);
            char sep = Current == Lang.Ru ? ' ' : ',';
            System.Text.StringBuilder b = new System.Text.StringBuilder(digits.Length + digits.Length / 3 + 1);
            int first = digits.Length % 3;
            if (first == 0) first = 3;
            b.Append(digits, 0, first);
            for (int i = first; i < digits.Length; i += 3)
            {
                b.Append(sep);
                b.Append(digits, i, 3);
            }
            return negative ? "-" + b.ToString() : b.ToString();
        }

        public static string Money(int value) { return "$" + Num(value); }

        /// <summary>Seconds -> "3:07".</summary>
        public static string Clock(float seconds)
        {
            int total = (int)Math.Max(0, seconds);
            return (total / 60).ToString("0") + ":" + (total % 60).ToString("00");
        }

        /// <summary>In-game hour (0..24) -> "07:40".</summary>
        public static string Hour(float hours)
        {
            float h = hours % 24f;
            if (h < 0) h += 24f;
            int hh = (int)h;
            int mm = (int)Math.Round((h - hh) * 60f);
            if (mm >= 60) { mm = 0; hh = (hh + 1) % 24; }
            return hh.ToString("00") + ":" + mm.ToString("00");
        }

        public static string DayName(int day)
        {
            string[] ru = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            string[] en = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            int i = ((day % 7) + 7) % 7;
            return Current == Lang.Ru ? ru[i] : en[i];
        }
    }
}
