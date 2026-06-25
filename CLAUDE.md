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

`Vludik.Arbitrage.Shared.sln` currently contains a single project, `Vludik.Arbitrage.Events`. Each project that should be published sets `<IsPackable>true</IsPackable>` plus `PackageId`/`Version`/`Authors`/`Description` directly in its own `.csproj`. Versioning is per-project and is never read, parsed, or overridden by the CI pipeline — bumping a package's version means editing its `<Version>` element.

### Vludik.Arbitrage.Events

A net9.0 class library of plain C# `record` types — cross-service event/value-object contracts, with no behavior, intended to be serialized and passed between services:

- `Entities/ContractType.cs` — enum `Perpetual` | `Spot`.
- `Entities/ExchangeRef.cs` — `record ExchangeRef(string Name, ContractType Type)`, a reference to a market on a specific exchange.
- `JobCreatedEvent.cs` / `JobFinishedEvent.cs` — keyed by `JobId`; carry `Symbol`, buy/sell `ExchangeRef`, and `Timestamp`.
- `SubscriptionCreatedEvent.cs` / `SubscriptionDeletedEvent.cs` — keyed by `SubscriptionId` and `ConnectionId`; also carry `Symbol`, buy/sell `ExchangeRef`, and `Timestamp`. `SubscriptionDeletedEvent` additionally carries a `Reason` string (`"unsubscribed"` | `"expired"`).

### CI/CD — NuGet publishing (`.github/workflows/nuget-publish.yml`)

- Triggers: push to `main`, and manual `workflow_dispatch`.
- Runs on `ubuntu-latest` with .NET `9.0.x`.
- Sequence: restore → build (Release) → pack to `./artifacts` → attach push credentials to the `github` NuGet source via `dotnet nuget update source` (the source is already declared by the repo's `nuget.config`, so `add source` would collide on the name) → `dotnet nuget push`.
- No `--skip-duplicate` is used, so re-publishing a version that already exists in the feed fails the workflow intentionally.
- Auth uses a `GH_TOKEN` repository secret (not the default `GITHUB_TOKEN`) as both the NuGet source password and the push `--api-key`.
- Registry: `https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json`.

### `nuget.config` (repo root)

Configures *consuming* `Vludik.Arbitrage.*` packages from this repo's GitHub Packages feed (separate from the publish workflow's own auth):

- Sources: `nuget.org` (all packages) and `github` (mapped via `packageSourceMapping` to only the `Vludik.Arbitrage.*` pattern), so an invalid/placeholder GitHub credential never breaks restores that don't need that feed.
- The `github` source's `ClearTextPassword` is a literal placeholder (`YOUR_GITHUB_PAT_HERE`) that must be replaced with a real PAT (`read:packages` scope) for restores from that feed to actually succeed.

### `.github/copilot-instructions.md`

Unrelated to this repo's code: it instructs Copilot to always use Azure MCP tools/best-practices when a request involves Azure.
