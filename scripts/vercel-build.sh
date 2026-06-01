#!/usr/bin/env bash
set -euo pipefail

export DOTNET_ROOT="$PWD/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"

if ! command -v dotnet >/dev/null 2>&1 || ! dotnet --list-sdks | grep -q '^10\.'; then
  echo "Installing .NET 10 SDK for Vercel build..."
  curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
  bash dotnet-install.sh --channel 10.0 --install-dir "$DOTNET_ROOT"
fi

API_BASE_URL="${API_BASE_URL:-https://localhost:7000}"
JITSI_SERVER_URL="${JITSI_SERVER_URL:-https://meet.jit.si}"

cat > Web/wwwroot/appsettings.json <<EOF
{
  "ApiBaseUrl": "${API_BASE_URL}",
  "Jitsi": {
    "ServerUrl": "${JITSI_SERVER_URL}"
  }
}
EOF

dotnet publish Web/StudyConnect.Web.csproj -c Release -o dist
