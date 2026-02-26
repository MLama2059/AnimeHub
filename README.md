# AnimeHub

AnimeHub is a full-stack anime discovery and tracking platform built on the modern **.NET 10 ecosystem**.

It provides a centralized environment for searching, rating, reviewing, managing watchlists, and contributing to anime metadata. The platform focuses on structured data management and community-driven content rather than media streaming.

The system includes a **genre-based recommendation engine powered by the Jaccard Similarity Algorithm**, along with secure JWT authentication and Role-Based Access Control (RBAC).

![.NET 10](https://img.shields.io/badge/.NET%2010-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor%20WASM-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![MudBlazor](https://img.shields.io/badge/MudBlazor-7E6EEF?style=for-the-badge&logo=data:image/svg+xml;base64,<encoded_svg>)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white)

---

## Features

### Core Functionality
- Search anime by title
- Rate and review anime
- Personal watchlist management
- Genre-based recommendations using Jaccard Similarity

### Community Contribution System
- **Submit Proposals:** Users can propose:
  - Adding new anime
  - Editing existing anime details  
  (All submissions require admin approval)

### Administrative Controls
- **Proposal Dashboard:** Admin panel to review, approve, or reject user submissions
- JWT-based authentication
- Role-Based Access Control (Admin / User)
- Protected API endpoints using authorization policies

---

## Architecture

```
AnimeHub.sln
│
├── AnimeHubApi       → ASP.NET Core Web API (.NET 10)
├── AnimeHubClient    → Blazor WebAssembly (Standalone)
└── AnimeHub.Shared   → Shared Models, DTOs, Enums
```

- The **Client** communicates with the API via HTTP.
- The **Shared** project ensures consistent contracts between frontend and backend.
- The API handles authentication, authorization, proposal workflows, business logic, and database operations.
- Entity Framework Core manages relational data with SQL Server.

---

## Recommendation Engine

AnimeHub implements a content-based filtering approach using the **Jaccard Similarity Index**.

```
J(A, B) = |A ∩ B| / |A ∪ B|
```

Where:

- A = Genre set of the currently viewed anime  
- B = Genre set of another anime  

Anime with the highest similarity scores are displayed as recommendations on the details page.

---

## Tech Stack

* **Framework:** .NET 10
* **Frontend:** Blazor WebAssembly (Standalone)
* **Backend:** ASP.NET Core Web API
* **Database:** SQL Server (accessed via SSMS)
* **ORM:** Entity Framework Core
* **Authentication:** JWT (JSON Web Tokens) with RBAC
* **UI Library:** MudBlazor
* **Algorithms:** Jaccard Similarity Index (Recommendations)

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (Express or Developer)
- SSMS (recommended)

---

### 1. Clone the Repository

```bash
git clone https://github.com/MLama2059/AnimeHub.git
cd AnimeHub
```

---

### 2. Configure Database

Update `appsettings.json` inside **AnimeHubApi**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=AnimeHubDb;Trusted_Connection=true;TrustServerCertificate=true"
}
```

---

### 3. Apply Migrations

```bash
cd AnimeHubApi
dotnet ef database update
```

If migrations do not exist:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### 4. Run the API

```bash
dotnet run
```

---

### 5. Run the Client

In a new terminal:

```bash
cd AnimeHubClient
dotnet run
```

---

## Author

Manish Lopchan Lama  
Kathmandu, Nepal  

GitHub: https://github.com/MLama2059
