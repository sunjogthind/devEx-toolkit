# DevEx Toolkit Platform

A modular developer experience platform built for game development teams. Integrates with GitHub/GitLab, Slack, and JIRA to provide unified pipeline monitoring, build tracking, and team collaboration tools.

Built to demonstrate enterprise-level full-stack development with the following stack:

## Tech Stack

### Backend
- **ASP.NET Core 8 Web API** (C#) — REST API with controllers, services, middleware
- **Entity Framework Core** — SQL Server / SQLite relational database
- **MongoDB Driver** — NoSQL document store for activity logs and webhook events
- **HttpClient integrations** — GitHub API, Slack Webhooks, JIRA REST API

### Frontend
- **React 18** — Single Page Application with component architecture
- **React Router** — Client-side routing
- **Tailwind CSS** — Modern utility-first styling
- **Recharts** — Data visualization for pipeline metrics
- **Lucide React** — Icon library

### Infrastructure
- **Docker & Docker Compose** — Containerized deployment
- **Azure / AWS ready** — Cloud PaaS deployment configuration
- **Nginx** — Production frontend serving

## Features

- **Dashboard** — Real-time overview of all projects, builds, and pipeline health
- **Pipeline Monitor** — Track CI/CD pipeline runs across multiple game projects
- **Integration Hub** — Connect and manage GitHub, Slack, and JIRA integrations
- **Activity Feed** — NoSQL-backed real-time activity log with webhook event capture
- **Build Tracker** — Monitor build status, duration, and success rates per project
- **Webhook Receiver** — Ingest events from GitHub, GitLab, and Slack

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://www.docker.com/) (optional, for containerized setup)

### Backend
```bash
cd backend/DevExToolkit.Api
dotnet restore
dotnet run
```
API runs at `http://localhost:5062`

### Frontend
```bash
cd frontend
npm install
npm start
```
App runs at `http://localhost:3000`

### Docker (Full Stack)
```bash
docker-compose up --build
```

## Project Structure
```
devex-toolkit/
├── backend/
│   └── DevExToolkit.Api/
│       ├── Controllers/       # REST API endpoints
│       ├── Models/            # Domain entities
│       ├── Data/              # EF Core + MongoDB contexts
│       ├── Services/          # Business logic & integrations
│       ├── Middleware/        # Request logging, error handling
│       └── Program.cs         # App configuration & DI
├── frontend/
│   └── src/
│       ├── components/        # Reusable UI components
│       ├── pages/             # Route-level page components
│       ├── services/          # API client layer
│       └── App.jsx            # Root component with routing
├── docker-compose.yml
└── README.md
```

## Architecture

```
┌─────────────────────────────────────────────┐
│              React Frontend (SPA)           │
│  Dashboard │ Pipelines │ Integrations │ Logs│
├─────────────────────────────────────────────┤
│         ASP.NET Core Web API (C#)           │
│  REST Controllers │ Services │ Middleware   │
├──────────────┬──────────────────────────────┤
│  SQL/SQLite  │  MongoDB (NoSQL)             │
│  Projects,   │  Activity Logs,              │
│  Users,      │  Webhook Events,             │
│  Builds      │  Pipeline Metrics            │
├──────────────┴──────────────────────────────┤
│        External Integrations                │
│  GitHub API │ Slack Webhooks │ JIRA REST    │
└─────────────────────────────────────────────┘
```

## License
MIT
