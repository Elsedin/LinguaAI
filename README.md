# LinguaAI

AI-powered translation and writing assistant built with .NET 10, Angular 21, and SQL Server.

## Features

- Guest and signed-in translation (Gemini), with style control
- Writing assistant (rewrite, simplify, expand, tone, grammar)
- Cookie auth (register / login / logout)
- Translation history (search, favorites, delete) for signed-in users
- Personal dictionary
- User preferences (default languages and style)
- Dashboard (counts, top language pairs, 14-day activity, recent items)
- Rate limiting on AI endpoints (shared policy)

## Structure

```text
LinguaAI/
├── backend/          # Api, Application, Infrastructure, Tests
└── frontend/lingua-ai/
```

## Prerequisites

- .NET 10 SDK
- Node.js 20.19+, 22.12+, or 24.x
- SQL Server with a database named `LinguaAI` (or adjust the connection string)
- Google Gemini API key

## Setup

### 1. Gemini API key

Do not commit secrets.

```bash
cd backend/LinguaAI.Api
dotnet user-secrets set "Gemini:ApiKey" "YOUR_KEY_HERE"
```

Or set environment variable `GEMINI_API_KEY`.

### 2. SQL Server

Connection string: `backend/LinguaAI.Api/appsettings.json` → `ConnectionStrings:DefaultConnection`.

Apply migrations:

```bash
cd backend
dotnet ef database update --project LinguaAI.Infrastructure --startup-project LinguaAI.Api
```

If `dotnet ef` is missing: `dotnet tool install --global dotnet-ef`.

### 3. Frontend API URL

Default API base URL is `http://localhost:5182` in `frontend/lingua-ai/src/app/core/api.config.ts`.

CORS origin is `Cors:FrontendOrigin` in API `appsettings.json` (default `http://localhost:4200`).

## Run

Backend (`http://localhost:5182`):

```bash
cd backend
dotnet run --project LinguaAI.Api --launch-profile http
```

Frontend (`http://localhost:4200`):

```bash
cd frontend/lingua-ai
npm start
```

Open the frontend, register or log in, then use Translate, Writing, History, Dictionary, Dashboard, and Settings.

## Tests

```bash
cd backend
dotnet test
```

```bash
cd frontend/lingua-ai
npx ng build
```

## Security notes (local)

- Auth cookie: `HttpOnly`, `SameSite=Lax`, sliding 7-day expiry
- Password: min 8 chars, digit + lower + upper required
- Failed login lockout: 5 attempts → 5 minutes
- History, dictionary, preferences, and dashboard require auth and are scoped by user id
- Translate/writing are public but rate-limited (default 20 requests / 60 seconds per IP)
- Gemini key via user-secrets or env only

## Limitations

- Local development focused (single CORS origin, localhost SQL)
- AI quality depends on Gemini model/quota
- No file upload (TXT/DOCX/PDF) yet
- Writing results are not stored in history
- Package advisories may appear for transitive `System.Security.Cryptography.Pkcs` (monitor NuGet updates)
