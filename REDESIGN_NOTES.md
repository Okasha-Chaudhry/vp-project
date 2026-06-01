# StudyConnect — Premium UI/UX Redesign

## What Changed

### ✅ Authentication (Google → Email/Password)
- **Removed** all Google OAuth / Google Sign-In code
- **Added** `/api/auth/register` — POST with `{ Name, Email, Password }`
- **Added** `/api/auth/login` — POST with `{ Email, Password }`
- Passwords hashed with PBKDF2/SHA-256 (100k iterations, 16-byte salt)
- SQLite local database — no external service required
- JWT tokens still used for session management (7-day expiry)

### ✅ Files Modified (API)
| File | Change |
|------|--------|
| `Models/User.cs` | Removed `GoogleId`, added `PasswordHash` |
| `Data/StudyConnectDbContext.cs` | Removed GoogleId unique index |
| `Dtos/AuthDtos.cs` | Added `RegisterRequest`, `LoginRequest` (removed Google DTOs) |
| `Services/Interfaces/IAuthService.cs` | `RegisterAsync`, `LoginAsync` instead of `GoogleLoginAsync` |
| `Services/AuthService.cs` | Full PBKDF2 password hashing implementation |
| `Repositories/Interfaces/IUserRepository.cs` | Added `GetByEmailAsync` |
| `Repositories/UserRepository.cs` | Implemented `GetByEmailAsync` |
| `Controllers/AuthController.cs` | `/register` and `/login` endpoints |
| `Program.cs` | Removed Google auth provider |
| `appsettings.json` | Removed Google credentials |

### ✅ Files Modified (Web)
| File | Change |
|------|--------|
| `Models/AuthModels.cs` | `RegisterRequest` (with confirm password), `LoginRequest` |
| `Services/IAuthService.cs` | `RegisterAsync`, `LoginAsync` |
| `Services/ClientAuthService.cs` | Calls new endpoints |
| `Pages/Login.razor` | Full redesign: tabbed Sign In / Create Account |
| `Pages/Dashboard.razor` | Premium stat cards, skeleton loading, glassmorphism modals |
| `Pages/Groups.razor` | Premium group cards with color variants |
| `Pages/Sessions.razor` | Upcoming/Past split, stat summary |
| `Pages/Tasks.razor` | Tab filters with live counts, priority visual system |
| `Pages/GroupDetail.razor` | Premium tabs, empty states, redesigned modals |
| `Shared/MainLayout.razor` | Premium sidebar with user profile footer |
| `Shared/NavMenu.razor` | Cleaner nav with active indicators |
| `wwwroot/css/app.css` | **Complete design system** (800+ lines) |
| `wwwroot/index.html` | Google script removed, Inter font added |
| `Program.cs` | Cleaned up (no Google) |

## Design System Highlights

### Colors
- Primary: `#2563EB` (blue)
- Accent: `#14B8A6` (teal)  
- Background: `#F8FAFC`
- Dark sidebar: `#0F172A`

### Typography
- Font: **Inter** (Google Fonts CDN)
- Clear weight hierarchy: 400 body, 500 medium, 600 semibold, 700 bold, 800 display

### Components
- Premium stat cards with gradient top border on hover
- Group cards with 6 gradient color variants + hover lift
- Task cards with priority color-coded left border
- Skeleton loading states for all data-heavy screens
- Glassmorphism modal overlays with blur backdrop
- Tab navigation with live count badges
- Proper empty states with action CTAs
- Toast-ready notification system
- Full responsive layout (mobile/tablet/desktop)

## How to Run

### Prerequisites
- .NET 8 SDK (or .NET 10 for API)
- No external database needed (SQLite)

### Start API
```bash
cd StudyConnect.API
dotnet run
# Runs on https://localhost:7000
```

### Start Web
```bash
cd StudyConnect.Web
dotnet run  
# Runs on https://localhost:7001
```

### First Use
1. Open https://localhost:7001
2. Click "Create Account" tab
3. Register with name, email, password
4. You're in! Create/join study groups.
