#!/usr/bin/env python3
"""Веб-форма активации Unity Personal для этой VM.

Зачем: Unity 6 требует логин/пароль Unity ID для получения лицензии (ULF), а
вводить пароль в чат нельзя. Эта страница принимает email и пароль в браузере
и сразу передаёт их в штатный Unity.Licensing.Client.

Безопасность:
  * со страницы нельзя выполнить произвольную команду — только один вызов
    Unity.Licensing.Client --activate-ulf;
  * пароль не пишется на диск и не попадает в логи: он живёт только в памяти
    процесса дочернего вызова;
  * в ответе показывается вывод клиента лицензий, без пароля.

Запуск:  python3 tools/license-web.py   (порт по умолчанию 8099)
"""

import html
import json
import os
import subprocess
import sys
import time
import urllib.parse
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer

CLIENT = os.path.expanduser(
    "~/tools/unity/Editor/Data/Resources/Licensing/Client/Unity.Licensing.Client"
)
LICENSE_DIR = os.path.expanduser("~/.local/share/unity3d/Unity")
PORT = int(os.environ.get("PORT", "8099"))
LOG = "/tmp/license-web.log"

PAGE = """<!doctype html>
<html lang="ru"><head><meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<title>Активация Unity — GTA VI Mobile</title>
<style>
  :root {{ color-scheme: dark; }}
  * {{ box-sizing: border-box; }}
  body {{ margin:0; min-height:100vh; display:flex; align-items:center; justify-content:center;
         background:#0d1117; color:#e6edf3;
         font:15px/1.5 -apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,sans-serif; padding:24px; }}
  .card {{ width:100%; max-width:560px; background:#161b22; border:1px solid #30363d;
          border-radius:14px; padding:26px 26px 22px; }}
  h1 {{ margin:0 0 6px; font-size:19px; }}
  p.sub {{ margin:0 0 20px; color:#8b949e; font-size:13px; }}
  label {{ display:block; margin:14px 0 6px; font-size:13px; color:#c9d1d9; }}
  input {{ width:100%; padding:11px 13px; border-radius:9px; border:1px solid #30363d;
          background:#0d1117; color:#e6edf3; font-size:15px; }}
  input:focus {{ outline:none; border-color:#2f81f7; }}
  button {{ margin-top:20px; width:100%; padding:12px; border:0; border-radius:9px;
           background:#238636; color:#fff; font-size:15px; font-weight:600; cursor:pointer; }}
  button:hover {{ background:#2ea043; }}
  button[disabled] {{ background:#30363d; cursor:progress; }}
  .note {{ margin:16px 0 0; font-size:12px; color:#8b949e; }}
  pre {{ margin:12px 0 0; padding:14px; background:#0d1117; border:1px solid #30363d;
        border-radius:9px; white-space:pre-wrap; word-break:break-word;
        font:12.5px/1.5 ui-monospace,SFMono-Regular,Menlo,monospace; max-height:340px; overflow:auto; }}
  .ok {{ color:#3fb950; }} .bad {{ color:#f85149; }} .wait {{ color:#d29922; }}
</style></head><body>
<div class="card">
  <h1>Активация Unity Personal</h1>
  <p class="sub">Логин и пароль уходят только в Unity.Licensing.Client на этой машине.
     Пароль не сохраняется ни в файлах, ни в логах.</p>
  <form id="f" method="post" action="/activate">
    <label for="email">Unity ID (email)</label>
    <input id="email" name="email" type="email" value="{email}" autocomplete="username" required>
    <label for="password">Пароль Unity</label>
    <input id="password" name="password" type="password" autocomplete="current-password" required autofocus>
    <button id="go" type="submit">Активировать лицензию</button>
  </form>
  <div id="result">{result}</div>
  <p class="note">Кнопка запускает одну фиксированную операцию: получение
     <code>Unity_lic.ulf</code> для этого компьютера. Через страницу нельзя выполнить другие команды.</p>
</div>
<script>
  const f = document.getElementById('f'), go = document.getElementById('go'),
        out = document.getElementById('result');
  f.addEventListener('submit', async (e) => {{
    e.preventDefault();
    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    if (!email || !password) {{
      out.innerHTML = '<p class="bad">Заполните оба поля.</p>';
      return;
    }}
    go.disabled = true; go.textContent = 'Активирую... (до минуты)';
    out.innerHTML = '<p class="wait">Отправляю запрос в Unity, подождите…</p>';
    try {{
      const r = await fetch('/activate', {{
        method: 'POST',
        headers: {{ 'Content-Type': 'application/json' }},
        body: JSON.stringify({{ email, password }})
      }});
      out.innerHTML = await r.text();
    }} catch (err) {{
      out.innerHTML = '<p class="bad">Сеть: ' + err + '</p>';
    }} finally {{
      go.disabled = false; go.textContent = 'Активировать лицензию';
    }}
  }});
</script>
</body></html>
"""


def log(msg: str) -> None:
    try:
        with open(LOG, "a", encoding="utf-8") as fh:
            fh.write(f"{time.strftime('%H:%M:%S')} {msg}\n")
    except OSError:
        pass


def page(email: str = "", result: str = "") -> bytes:
    return PAGE.format(email=html.escape(email, quote=True), result=result).encode()


def diagnostics(ctype: str, length: int, raw: bytes) -> str:
    """Что пришло от прокси — без значений полей."""
    return (
        '<p class="bad">Данные формы не дошли до сервера.</p><pre>'
        f"content-type: {html.escape(ctype or '(нет)')}\n"
        f"content-length: {length}\n"
        f"получено байт: {len(raw)}\n"
        "Скопируйте этот текст и пришлите его в чат."
        "</pre>"
    )


def parse_body(ctype: str, raw: bytes) -> dict:
    ctype = (ctype or "").lower()
    if not raw:
        return {}
    if "application/json" in ctype:
        try:
            data = json.loads(raw.decode("utf-8", "replace"))
            return {k: str(v) for k, v in data.items()} if isinstance(data, dict) else {}
        except ValueError:
            return {}
    if "multipart/form-data" in ctype:
        try:
            import email.parser
            import email.policy

            msg = email.parser.BytesParser(policy=email.policy.default).parsebytes(
                b"Content-Type: " + ctype.encode() + b"\r\n\r\n" + raw
            )
            out = {}
            if msg.is_multipart():
                for part in msg.iter_parts():
                    name = part.get_param("name", header="content-disposition")
                    if name:
                        out[name] = (part.get_payload(decode=True) or b"").decode("utf-8", "replace")
            return out
        except Exception:  # noqa: BLE001
            return {}
    return {k: v[0] for k, v in urllib.parse.parse_qs(raw.decode("utf-8", "replace")).items() if v}


def read_body(handler: BaseHTTPRequestHandler) -> bytes:
    length = handler.headers.get("Content-Length")
    if length:
        return handler.rfile.read(int(length))
    if (handler.headers.get("Transfer-Encoding") or "").lower() == "chunked":
        chunks = []
        while True:
            line = handler.rfile.readline().strip()
            if not line:
                break
            size = int(line.split(b";")[0], 16)
            if size == 0:
                handler.rfile.readline()
                break
            chunks.append(handler.rfile.read(size))
            handler.rfile.readline()
        return b"".join(chunks)
    return b""


def run_activation(email: str, password: str) -> str:
    env = dict(os.environ)
    libs = os.path.expanduser("~/libs/usr/lib/x86_64-linux-gnu")
    env["LD_LIBRARY_PATH"] = libs + (":" + env["LD_LIBRARY_PATH"] if env.get("LD_LIBRARY_PATH") else "")
    cmd = [CLIENT, "--activate-ulf", "--username", email, "--password", password]
    try:
        done = subprocess.run(
            cmd, env=env, stdin=subprocess.DEVNULL,
            stdout=subprocess.PIPE, stderr=subprocess.STDOUT, timeout=180,
        )
        out = done.stdout.decode("utf-8", "replace")
        code = done.returncode
    except subprocess.TimeoutExpired as exc:
        out = (exc.stdout or b"").decode("utf-8", "replace") + "\n[timeout]"
        code = -1
    except Exception as exc:  # noqa: BLE001
        out, code = f"[ошибка запуска] {exc}", -1
    # на всякий случай: пароль в выводе клиента быть не должно
    out = out.replace(password, "***")

    ulf = []
    if os.path.isdir(LICENSE_DIR):
        ulf = sorted(f for f in os.listdir(LICENSE_DIR) if f.endswith(".ulf"))
    if code == 0 and ulf:
        head = '<p class="ok">Готово: лицензия получена — %s</p>' % ", ".join(html.escape(u) for u in ulf)
        head += '<p class="sub">Теперь можно запускать сборку APK: bash tools/build-apk.sh</p>'
    else:
        head = '<p class="bad">Активация не завершилась (код %s).</p>' % html.escape(str(code))
    return head + "<pre>" + html.escape(out.strip() or "(пустой вывод)") + "</pre>"


class Handler(BaseHTTPRequestHandler):
    server_version = "unity-lic"
    sys_version = ""

    def log_message(self, fmt, *args):  # без вывода в консоль
        pass

    def _send(self, body: bytes, code: int = 200):
        self.send_response(code)
        self.send_header("Content-Type", "text/html; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.send_header("Cache-Control", "no-store")
        self.send_header("Referrer-Policy", "no-referrer")
        self.end_headers()
        self.wfile.write(body)

    def do_GET(self):
        if self.path.split("?")[0] not in ("/", "/index.html"):
            self._send(b"not found", 404)
            return
        self._send(page())

    def do_POST(self):
        if self.path.split("?")[0] != "/activate":
            self._send(b"not found", 404)
            return
        ctype = self.headers.get("Content-Type") or ""
        raw = read_body(self)
        form = parse_body(ctype, raw)
        email = (form.get("email") or "").strip()
        password = form.get("password") or ""
        # в лог — только факт запроса, без значений
        log(f"POST /activate ctype={ctype!r} bytes={len(raw)} fields={sorted(form)}")
        if not email or not password:
            self._send(page(email, diagnostics(ctype, len(raw), raw)))
            return
        result = run_activation(email, password)
        log("activate done")
        self._send(page(email, result))


def main() -> int:
    if not os.access(CLIENT, os.X_OK):
        print(f"Нет Unity.Licensing.Client: {CLIENT}", file=sys.stderr)
        print("Сначала: bash tools/setup-unity-android.sh", file=sys.stderr)
        return 1
    server = ThreadingHTTPServer(("0.0.0.0", PORT), Handler)
    print(f"license-web: http://0.0.0.0:{PORT}", flush=True)
    server.serve_forever()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
