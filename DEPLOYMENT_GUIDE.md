# StudyConnect — Free Hosting & PostgreSQL Migration Guide

## Part 1: Set Up Supabase (Free PostgreSQL)

### Step 1: Create Supabase Account
1. Go to https://supabase.com → **Sign Up** (free)
2. Verify email
3. Create new project:
   - **Project Name**: `StudyConnect`
   - **Database Password**: Create strong password (save it!)
   - **Region**: Choose closest to you
   - Click **Create new project** (takes 2-3 minutes)

### Step 2: Get Connection Details
1. In Supabase dashboard → **Settings** → **Database**
2. Copy these details:
   - **Host**: `db.XXXXX.supabase.co`
   - **Port**: `5432`
   - **Database**: `postgres`
   - **User**: `postgres`
   - **Password**: Your database password

3. **Connection String** format:
   ```
   Server=db.XXXXX.supabase.co;Port=5432;Database=postgres;User Id=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;
   ```

---

## Part 2: Update .NET API for PostgreSQL

### Step 1: Install PostgreSQL NuGet Package
```bash
cd API
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### Step 2: Update `Program.cs` Database Configuration
Replace the SQLite config with PostgreSQL:

```csharp
// Remove SQLite line:
// options.UseSqlite("Data Source=studyconnect.db"));

// Add PostgreSQL:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=YOUR_HOST;Port=5432;Database=postgres;User Id=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;";

builder.Services.AddDbContext<StudyConnectDbContext>(options =>
    options.UseNpgsql(connectionString));
```

### Step 3: Update `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=db.XXXXX.supabase.co;Port=5432;Database=postgres;User Id=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "your-super-secret-key-that-is-at-least-32-characters-long-for-hmac-sha256",
    "Issuer": "StudyConnect",
    "Audience": "StudyConnectClient",
    "ExpireDays": 7
  },
  "ClientUrls": {
    "Blazor": "https://studyconnect.vercel.app"
  },
  "Jitsi": {
    "ServerUrl": "https://meet.jit.si"
  }
}
```

### Step 4: Update `StudyConnectDbContext.cs`
Add `using Npgsql.EntityFrameworkCore.PostgreSQL` at top if not present.

### Step 5: Create & Apply Migration
```bash
# Remove old SQLite database
rm studyconnect.db

# Create migration
dotnet ef migrations add InitialPostgresSetup

# Apply to Supabase
dotnet ef database update
```

✅ Your database is now in Supabase!

---

## Part 3: Prepare for Render.com Deployment

### Step 1: Add `.gitignore` (Important!)
Create `.gitignore` in root if not present:
```
*.db
*.db-wal
*.db-shm
bin/
obj/
.vs/
.vscode/
appsettings.Development.json
```

### Step 2: Create `appsettings.Production.json` (API folder)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Jwt": {
    "Key": "your-super-secret-key-that-is-at-least-32-characters-long-for-hmac-sha256",
    "Issuer": "StudyConnect",
    "Audience": "StudyConnectClient",
    "ExpireDays": 7
  },
  "ClientUrls": {
    "Blazor": "https://studyconnect.vercel.app"
  },
  "Jitsi": {
    "ServerUrl": "https://meet.jit.si"
  }
}
```

### Step 3: Push to GitHub
```bash
git init
git add .
git commit -m "Initial commit - ready for deployment"
git remote add origin https://github.com/YOUR_USERNAME/StudyConnect.git
git branch -M main
git push -u origin main
```

---

## Part 4: Deploy API to Render.com (Free)

### Step 1: Create Render Account
- Go to https://render.com → **Sign Up** (free)
- Connect GitHub account

### Step 2: Deploy .NET API
1. Dashboard → **New+** → **Web Service**
2. Select your **StudyConnect** GitHub repository
3. Configure:
   - **Name**: `studyconnect-api`
   - **Region**: Frankfurt (or closest to you)
   - **Branch**: `main`
   - **Runtime**: `.NET`
   - **Build Command**: `cd API && dotnet build`
   - **Start Command**: `cd API && dotnet run --urls "http://0.0.0.0:${PORT}"`
4. Click **Advanced** and add Environment Variables:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://+:${PORT}
   ```
5. Click **Create Web Service** (deploys in 3-5 minutes)

### Step 3: Get API URL
After deployment, Render gives you: `https://studyconnect-api.onrender.com`

### Step 4: Update Web App Config
Update `Web/wwwroot/appsettings.json`:
```json
{
  "ApiBaseUrl": "https://studyconnect-api.onrender.com",
  "Jitsi": {
    "ServerUrl": "https://meet.jit.si"
  }
}
```

---

## Part 5: Deploy Blazor Frontend to Vercel

### Step 1: Publish Blazor WASM
```bash
cd Web
dotnet publish -c Release -o publish
```

### Step 2: Create `vercel.json` in Web folder
```json
{
  "buildCommand": "dotnet publish -c Release -o publish",
  "outputDirectory": "publish/wwwroot",
  "env": {
    "NODE_VERSION": "18.17.0"
  },
  "rewrites": [
    {
      "source": "/(.*)",
      "destination": "index.html"
    }
  ]
}
```

### Step 3: Create Vercel Account & Deploy
1. Go to https://vercel.com → **Sign Up** (free)
2. Connect GitHub
3. **Import Project** → Select your repository
4. **Framework Preset**: Other
5. **Build Command**: `cd Web && dotnet publish -c Release -o ../api/publish/wwwroot`
6. **Output Directory**: `api/publish/wwwroot`
7. Click **Deploy** (5-10 minutes)

✅ Your frontend is live!

---

## Part 6: Configure CORS (Important!)

Update `Program.cs` CORS settings:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel", builder =>
    {
        builder.WithOrigins(
            "http://localhost:5001",
            "https://studyconnect.vercel.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

app.UseCors("AllowVercel");
```

---

## ✅ Final Checklist

- [ ] Supabase account created & DB running
- [ ] `appsettings.json` updated with Supabase connection string
- [ ] PostgreSQL NuGet package installed
- [ ] EF migrations created and applied
- [ ] GitHub repository created and pushed
- [ ] Render.com deployment complete (API URL noted)
- [ ] Vercel deployment complete (Frontend URL noted)
- [ ] CORS configured in `Program.cs`
- [ ] `appsettings.json` in Web updated with Render API URL

---

## 🚀 Test Your Setup

1. Visit `https://studyconnect.vercel.app`
2. Create account
3. Create a group
4. Create a session
5. Join video call (Jitsi works on public server)

---

## 💡 Free Tier Limits

| Service | Free Tier | Limit |
|---------|-----------|-------|
| **Supabase** | PostgreSQL | 500 MB storage |
| **Render.com** | Web Service | 750 hours/month (free) |
| **Vercel** | Blazor WASM | 100 GB bandwidth/month |

---

## 🆘 Troubleshooting

**Connection fails to Supabase?**
- Check IP whitelist: Supabase → Settings → Network → Add `0.0.0.0/0`

**Render says "Build failed"?**
- Check build logs (Render dashboard)
- Ensure `Program.cs` uses environment variables

**Vercel deployment stuck?**
- Clear cache: Vercel dashboard → Project → Settings → Deployments → Clear Cache

---

## 📝 Production Considerations

1. **Change JWT Secret**: Update to a long, random string in production
2. **Enable HTTPS**: Already enabled on Render & Vercel ✅
3. **Database Backups**: Enable in Supabase dashboard
4. **Monitoring**: Use Render & Vercel dashboards for logs
5. **Custom Domain** (Optional): Add domain to Vercel & Render

