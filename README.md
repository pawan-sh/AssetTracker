# AssetTracker

AssetTracker is an ASP.NET Core MVC application (Razor Views) for managing assets, employee assignments, reported issues and repairs. It uses EF Core with SQL Server, ASP.NET Core Identity for authentication and roles, and Azure Event Grid for notifications.

---

## At-a-glance

- Framework: .NET 8, C# 12  
- App type: ASP.NET Core MVC (Razor Views)  
- Data: EF Core (Identity integrated in same DbContext)  
- Notifications: Azure Event Grid via EventGridService  
- File uploads: saved under `wwwroot/uploads/invoices/`

---

## Prerequisites

- Visual Studio 2022 (or later) with .NET 8 SDK
- SQL Server (localdb is supported)
- (Optional) Azure subscription for Event Grid and Logic Apps
- dotnet-ef (optional, for CLI migrations):
  - `dotnet tool install --global dotnet-ef`

---

## Key project layout

- AssetTracker.Web
  - Controllers: AssetsController, EmployeesController, IssueController, RepairController, AccountController, etc.
  - Views: /Views/Issue, /Views/Repair, /Views/Assets, /Views/Employees, /Views/Account
  - Services: EventGridService.cs
  - Program.cs: DI, Identity configuration, role seeding
- AssetTracker.Infrastructure
  - AssetTrackerDbContext (inherits IdentityDbContext<IdentityUser>)
  - Migrations (Issues, Repairs, completion message, etc.)
- AssetTracker.Core
  - Domain models: Asset, Employee, AssetAssignment, Issue, Repair

---

## Configuration

1. Connection string — set `ConnectionStrings:DefaultConnection` in `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AssetTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

2. (Optional) Event Grid — set environment values (App Service config or local user secrets):
- `EVENTGRID_TOPIC_ENDPOINT` — Event Grid topic URI
- `EVENTGRID_TOPIC_KEY` — Event Grid key

EventGridService will silently no-op if these values are not present.

---

## Local setup — step-by-step

1. Open solution in Visual Studio 2022 and set `AssetTracker.Web` as startup project.
2. Update the connection string in `appsettings.json`.
3. Apply EF Core migrations:
   - Using Package Manager Console (set Default Project = `AssetTracker.Infrastructure`):
     - `Update-Database -StartupProject AssetTracker.Web`
   - Or CLI:
     - `dotnet ef database update --project AssetTracker.Infrastructure --startup-project AssetTracker.Web`
4. Build the solution: **Build ? Build Solution**
5. Run: **Debug ? Start Debugging** (F5) or Start Without Debugging (Ctrl+F5)

On first run the app seeds roles: `Admin`, `HR`, `Director`, `Employee`.

---

## Common flows

- Employees
  - EmployeeLogin: `/Account/EmployeeLogin` (auto-registers Identity user if email exists in Employees table)
  - Report issue: `/Issue/Create`
  - My issues: `/Issue/MyIssues`

- Admins
  - List all issues: `/Issue/List`
  - Send to repair: `/Repair/Create?issueId={id}`
  - Close issue: `/Issue/Close/{id}` (supports invoice upload)
  - Manage assets: `/Assets`
  - Manage employees: `/Employees`

- Events
  - Repair started ? `Asset.RepairStarted`
  - Repair completed ? `Asset.RepairCompleted`

---

## Implementation details & observations

- DbContext: `AssetTrackerDbContext : IdentityDbContext<IdentityUser>` — identity tables live in same DB.
- Decimal precision: `Repair.RepairCost` and `Issue.Cost` configured to `decimal(18,2)`.
- File uploads: stored in `wwwroot/uploads/invoices/` with a GUID prefix; `Issue.InvoicePath` stores the web-relative path (e.g., `/uploads/invoices/{file}`).
- `EventGridService` publishes `EventGridEvent` with JSON-serialized data.
- Controllers use role-based authorization attributes to protect admin functionality.

---

## Security & production recommendations (must do before production)

- Do NOT use email-as-password flows in production. Replace auto-passwords and implement secure invite / password reset flows.
- Harden Identity: strengthen password policy, enable account lockout, add MFA.
- Move secrets (DB connection string, Event Grid key) to a secure store (Azure Key Vault or App Service configuration).
- File upload hardening:
  - Enforce server-side file extension and MIME checks.
  - Enforce file size limits.
  - Consider storing invoices in Azure Blob Storage with signed URLs rather than in web root.
- Add logging (ILogger) and telemetry (Application Insights).
- Wrap multi-entity updates (e.g., create Repair + update Issue) in a transaction to ensure consistency.

---

## Suggested immediate improvements (high priority)

- Add DataAnnotations to domain models and check `ModelState` in all POST actions.
- Add server-side file validation and a max upload size.
- Add an initial admin seed (securely) to avoid manual role assignment.
- Add unit tests for controllers and an integration test for EF Core flows.

---

## Troubleshooting

- DB connection issues: verify `DefaultConnection` and that migration ran.
- Roles not present: restart application (roles seeded at startup). If still missing, inspect `Program.cs` role seeding code.
- EventGrid messages not received: ensure endpoint/key are set and that downstream subscriptions (Logic App) exist.
- Upload permission errors: ensure `wwwroot/uploads/invoices` exists and the app process can write to it.

---

If you want, I will:
- commit this `README.md` into the repository (done), or
- also add server-side file validation and an admin-seed user.  

Tell me which to do next.