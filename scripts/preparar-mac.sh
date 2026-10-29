#!/usr/bin/env bash
set -euo pipefail

repo_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_dir"

if [[ -x "$HOME/.dotnet/dotnet" ]]; then
    export DOTNET_ROOT="$HOME/.dotnet"
    export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"
fi

if ! command -v dotnet >/dev/null; then
    echo "No se encuentra el SDK de .NET 9. Configura la ruta de .NET antes de continuar."
    exit 1
fi

if ! docker info >/dev/null 2>&1; then
    if [[ -x "$HOME/.orbstack/bin/orbctl" ]]; then
        open -a OrbStack
        "$HOME/.orbstack/bin/orbctl" start
    else
        echo "Inicia tu motor de Docker y vuelve a ejecutar este script."
        exit 1
    fi
fi

# La contraseña se conserva entre ejecuciones; cambiarla no cambia un volumen existente.
# .env está ignorado por Git. No se imprimen las credenciales.
python3 - <<'PY'
from pathlib import Path
import secrets
env = Path('.env')
content = env.read_text() if env.exists() else ''
if not any(line.startswith('BIBLIOTECA_SQL_PASSWORD=') for line in content.splitlines()):
    prefix = '\n' if content and not content.endswith('\n') else ''
    with env.open('a') as handle:
        handle.write(prefix + 'BIBLIOTECA_SQL_PASSWORD=Biblio9!' + secrets.token_hex(20) + '\n')
    env.chmod(0o600)
PY

echo "Iniciando SQL Server de desarrollo en 127.0.0.1:14333..."
docker compose up -d --wait --wait-timeout 180

# User Secrets sustituye LocalDB solo para el usuario local en Development.
python3 - <<'PY'
from pathlib import Path
import json
import subprocess
password = next(line.split('=', 1)[1] for line in Path('.env').read_text().splitlines()
                if line.startswith('BIBLIOTECA_SQL_PASSWORD='))
connection = ('Server=127.0.0.1,14333;Database=BibliotecaJoseHernandez;'
              'User Id=sa;Password=' + password + ';Encrypt=True;TrustServerCertificate=True')
result = subprocess.run(['dotnet', 'user-secrets', 'set', '--project', 'BibliotecaMVC/BibliotecaMVC.csproj'],
                        input=json.dumps({'ConnectionStrings:BibliotecaConnection': connection}),
                        text=True, capture_output=True)
if result.returncode:
    raise SystemExit('No se pudo guardar la conexión local en User Secrets.')
print('Conexión local configurada sin publicar la contraseña en Git.')
PY

if ! dotnet ef --version >/dev/null 2>&1; then
    dotnet tool install --global dotnet-ef --version 9.0.10
fi

echo "Aplicando las migraciones a la base local de desarrollo..."
ASPNETCORE_ENVIRONMENT=Development DOTNET_ENVIRONMENT=Development \
    dotnet ef database update --project BibliotecaMVC/BibliotecaMVC.csproj

echo "Base preparada. Detén y vuelve a ejecutar el perfil http en Rider."
echo "Aplicación: http://localhost:5087"
