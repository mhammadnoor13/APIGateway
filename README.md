# Consultant‑AI Platform – API Gateway

A lightweight **.NET 8** reverse‑proxy that fronts every micro‑service in the Consultant‑AI platform. Built with [YARP](https://github.com/microsoft/reverse-proxy) for high‑performance routing and an easy config‑file experience.

---

## ✨ Key Features

| Capability                | Details                                                                                                                       |
| ------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| **Reverse proxy**         | Powered by YARP; routes & clusters are 100 % config‑driven.                                                                   |
| **CORS policy**           | `AllowReactDev` lets the React front‑end at [http://localhost:3000](http://localhost:3000) call the gateway during local dev. |
| **Swagger / OpenAPI**     | Auto‑generated docs & test UI available when `ASPNETCORE_ENVIRONMENT=Development`.                                            |
| **Zero auth by default**  | JWT authentication middleware has been removed – great for rapid prototyping. Easily re‑enable later if needed.               |
| **Minimal hosting model** | Clean `Program.cs` with only the essentials.                                                                                  |

---

## ⚡ Quick Start

```bash
# 1 – prerequisite
🟢 .NET 8 SDK  (https://dotnet.microsoft.com/download)

# 2 – clone & run
> git clone <repo‑url>
> cd ApiGateway
> dotnet run

# 3 – test‑drive
Navigate to http://localhost:5100/swagger  (port shown in console) and invoke any endpoint – requests will be proxied to downstream services.
```

> **Tip :** add `--project ApiGateway` if you run from the repo root.

---

## 🗂️ Project Structure

```
├── src/
│   └── ApiGateway/              # this project
│       ├── Program.cs           # minimal host + YARP
│       ├── appsettings.json     # routes & clusters live here
│       └── ...
└── test/                        # (future) integration tests
```

---

## 🔧 Configuration Guide

### Routes & Clusters

Edit `src/ApiGateway/appsettings.json` → `ReverseProxy` section.

```jsonc
"ReverseProxy": {
  "Routes": {
    "caseService": {
      "ClusterId": "caseCluster",
      "Match": { "Path": "/cases/{**catch-all}" }
    },
    "consultantService": {
      "ClusterId": "consultantCluster",
      "Match": { "Path": "/consultants/{**catch-all}" }
    }
  },
  "Clusters": {
    "caseCluster": {
      "Destinations": {
        "case1": { "Address": "http://localhost:5010/" }
      }
    }
  }
}
```

Add new services by duplicating a route & cluster pair, adjusting the path and destination address.

### CORS origins

Change the URL in `Program.cs` if your front‑end runs elsewhere:

```csharp
policy.WithOrigins("http://localhost:5173"); // Vite, for example
```

---

## 🚀 Roadmap

* ✅ Basic routing, CORS, Swagger (this commit)
* ⏳ Health‑check aggregation endpoint
* ⏳ Request logging & distributed tracing
* ⏳ Pluggable authentication / auth‑z policies
* ⏳ Rate‑limiting / circuit‑breaker middleware

---

## 🤝 Contributing

1. Fork & create a feature branch.
2. Follow the **Conventional Commits** style for messages (`feat: …`, `fix: …`, etc.).
3. Submit a PR – all feedback welcome!

---

## 📝 License

MIT © 2025 Consultant‑AI team
