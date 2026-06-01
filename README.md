# StudyConnect — Setup Guide (.NET 10)

## Prerequisites
- .NET 10 SDK installed → verify with `dotnet --version`

## Step 1 — Run the API

Open a terminal in the `API/` folder:

```bash
cd StudyConnect_Redesigned/API
dotnet restore
dotnet run --urls "http://localhost:5000"
```

First run auto-creates `studyconnect.db` (SQLite). You should see:

```
Now listening on: http://localhost:5000
```

Swagger UI is available at: http://localhost:5000/swagger

## Step 2 — Run the Web App

Open a **second** terminal in the `Web/` folder:

```bash
cd StudyConnect_Redesigned/Web
dotnet restore
dotnet run --urls "http://localhost:5001"
```

## Step 3 — Open in Browser

Go to http://localhost:5001  
Click **"Create Account"** → register with any email + password → you're in.

---

## Configuration Files

| File | Purpose |
|------|---------|
| `API/appsettings.json` | API config — JWT settings, CORS allowed origins |
| `Web/wwwroot/appsettings.json` | Web config — API base URL |

## Troubleshooting

**CORS error** — both ports must match between `API/appsettings.json` (`ClientUrls:Blazor`) and `Web/wwwroot/appsettings.json` (`ApiBaseUrl`).

**Port already in use** — change `5000`/`5001` to any free ports, update both config files to match.

**"Project file not found"** — make sure you're running `dotnet run` from inside `API/` or `Web/`, not from the root.

**Package restore fails** — requires internet access. If on a restricted network, set up a NuGet proxy.
