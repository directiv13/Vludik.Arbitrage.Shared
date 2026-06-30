# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository purpose

Vludik.Arbitrage.Shared is a .NET 9 solution that hosts shared class libraries published as NuGet packages to GitHub Packages, for consumption by other services in the Vludik Arbitrage platform.

## Commands

```
dotnet restore Vludik.Arbitrage.Shared.sln
dotnet build Vludik.Arbitrage.Shared.sln --configuration Release
dotnet pack Vludik.Arbitrage.Shared.sln --configuration Release --no-build --output ./artifacts
```

`dotnet pack` against the `.sln` (rather than an individual `.csproj`) only emits a `.nupkg` for projects with `<IsPackable>true</IsPackable>`; other projects' `Pack` target is a no-op. This is how packable vs. non-packable projects are distinguished — there is no separate filtering step.

There are no test projects in the solution, so there is no test command.

## Architecture

### Solution layout and versioning

`Vludik.Arbitrage.Shared.sln` contains four projects: `Vludik.Arbitrage.Shared`, `Vludik.Arbitrage.JobsService.Shared`, `Vludik.Arbitrage.JobsService.Worker.Shared`, and `Vludik.Arbitrage.SubscriptionsService.Shared`. Each sets `<IsPackable>true</IsPackable>` plus `PackageId`/`Version`/`Authors`/`Description` directly in its own `.csproj`. Versioning is per-project and is never read, parsed, or overridden by the CI pipeline — bumping a package's version means editing its `<Version>` element.

`Vludik.Arbitrage.Events` still exists as a directory/`.csproj` on disk but is **not** referenced by the `.sln` — it was orphaned when the per-service projects below were split out and is excluded from restore/build/pack/CI entirely.

All four in-solution projects are net9.0 class libraries of plain C# `record`/`enum` types — cross-service event/value-object contracts, with no behavior, intended to be serialized and passed between services.

### Vludik.Arbitrage.Shared

The base package the other three depend on:

- `Enums/ContractType.cs` — enum `Perpetual` | `Spot`.
- `Models/ExchangeRef.cs` — `record ExchangeRef(string Name, ContractType Type)`, a reference to a market on a specific exchange.

### Vludik.Arbitrage.JobsService.Shared

Depends on `Vludik.Arbitrage.Shared` (`PackageReference`):

- `Enums/JobMode.cs` — `Open` | `Close`.
- `Enums/MarginType.cs` — `Cross` | `Isolated` | `Spot`.
- `Events/JobCreatedEvent.cs` / `Events/JobDeletedEvent.cs` — keyed by `JobId`; carry `Symbol`, buy/sell `ExchangeRef`, `JobMode`, `MarginType`, `Leverage`, `SpreadPercent`, `Size`, `ChunkSize`, and `Timestamp`.

### Vludik.Arbitrage.JobsService.Worker.Shared

No dependency on the other projects:

- `Events/JobWorkerCompletedEvent.cs` — `record JobWorkerCompletedEvent(Guid JobId, long Timestamp)`.
- `Events/JobWorkerFailedEvent.cs` — currently a stub (`internal class JobWorkerFailedEvent {}`), not yet implemented as a public event record.

### Vludik.Arbitrage.SubscriptionsService.Shared

Depends on `Vludik.Arbitrage.Shared` (`PackageReference`):

- `Events/SubscriptionCreatedEvent.cs` / `Events/SubscriptionDeletedEvent.cs` — keyed by `SubscriptionId` and `ConnectionId`; also carry `Symbol`, buy/sell `ExchangeRef`, and `Timestamp`. `SubscriptionDeletedEvent` additionally carries a `Reason` string (`"unsubscribed"` | `"expired"`).

### CI/CD — NuGet publishing (`.github/workflows/nuget-publish.yml`)

- Triggers: push to `main`, and manual `workflow_dispatch`.
- Runs on `ubuntu-latest` with .NET `9.0.x`.
- Sequence: attach credentials to the `github` NuGet source via `dotnet nuget update source` (the source is already declared by the repo's `nuget.config`, so `add source` would collide on the name) → restore → build (Release) → pack to `./artifacts` → `dotnet nuget push`. Credentials are attached **before** restore because `Vludik.Arbitrage.JobsService.Shared` and `Vludik.Arbitrage.SubscriptionsService.Shared` carry `Vludik.Arbitrage.*` `PackageReference`s that `nuget.config`'s `packageSourceMapping` routes through the `github` source — restoring before authenticating produces a 401.
- No `--skip-duplicate` is used, so re-publishing a version that already exists in the feed fails the workflow intentionally.
- Auth uses a `GH_TOKEN` repository secret (not the default `GITHUB_TOKEN`) as both the NuGet source password and the push `--api-key`.
- Registry: `https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json`.

### `nuget.config` (repo root)

Configures *consuming* `Vludik.Arbitrage.*` packages from this repo's GitHub Packages feed (separate from the publish workflow's own auth):

- Sources: `nuget.org` (all packages) and `github` (mapped via `packageSourceMapping` to only the `Vludik.Arbitrage.*` pattern), so an invalid/missing GitHub credential never breaks restores that don't need that feed.
- The committed `nuget.config` carries no credentials for the `github` source at all (the placeholder `packageSourceCredentials` block was deleted). CI injects a real credential at runtime via `dotnet nuget update source` (see above); local consumers must add their own (see README's install instructions) — restoring `Vludik.Arbitrage.*` packages without one fails with 401.

### `.github/copilot-instructions.md`

Unrelated to this repo's code: it instructs Copilot to always use Azure MCP tools/best-practices when a request involves Azure.
