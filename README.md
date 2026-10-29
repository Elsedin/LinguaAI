# LinguaAI

AI-powered translation and writing assistant built with .NET 10, Angular 21, and SQL Server.

## Status

Phase 1 — project foundation. Core product features are not implemented yet.

## Solution structure

```text
LinguaAI/
├── backend/
│   ├── LinguaAI.slnx                 # .NET 10 solution
│   ├── LinguaAI.Api/                 # ASP.NET Core Web API (HTTP layer)
│   ├── LinguaAI.Application/         # Business logic and interfaces
│   ├── LinguaAI.Infrastructure/      # EF Core, Identity, AI providers
│   └── LinguaAI.Tests/               # xUnit tests
├── frontend/
│   └── lingua-ai/                    # Angular 21 app
├── .gitignore
└── README.md
```

## Prerequisites

- .NET 10 SDK
- Node.js 20.19+, 22.12+, or 24.x
- SQL Server (local instance)
- Git

## Run locally

### Backend

```bash
cd backend
dotnet run --project LinguaAI.Api
```

### Frontend

```bash
cd frontend/lingua-ai
npm start
```

The Angular app defaults to `http://localhost:4200`.

### Tests

```bash
cd backend
dotnet test
```

## Notes

- AI provider keys must never be committed. They will be stored in user secrets / environment variables.
- This repository is independent of any parent Git repository on the machine.
