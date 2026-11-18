# LinguaAI

AI-powered translation and writing assistant built with .NET 10, Angular 21, and SQL Server.

## Features

- Translate and writing assistant (Gemini)
- Auth, translation history, and personal dictionary (signed-in)

## Structure

```text
LinguaAI/
├── backend/          # .NET API, Application, Infrastructure, Tests
└── frontend/lingua-ai/
```

## Prerequisites

- .NET 10 SDK
- Node.js 20.19+, 22.12+, or 24.x
- SQL Server (database `LinguaAI`)
- Gemini API key

## Setup

Gemini key (do not commit secrets):

```bash
cd backend/LinguaAI.Api
dotnet user-secrets set "Gemini:ApiKey" "YOUR_KEY_HERE"
```

Or set environment variable `GEMINI_API_KEY`.

SQL connection string is in `backend/LinguaAI.Api/appsettings.json` (`ConnectionStrings:DefaultConnection`).

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

Tests:

```bash
cd backend
dotnet test
```
