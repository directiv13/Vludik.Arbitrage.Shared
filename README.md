# Vludik.Arbitrage.Shared

Shared .NET libraries for the Vludik Arbitrage platform, published as NuGet packages to GitHub Packages.

## Packages

| Package | Description |
|---|---|
| [`Vludik.Arbitrage.Shared`](Vludik.Arbitrage.Shared) | Base contracts shared by the packages below: `ExchangeRef`, `ContractType`. |
| [`Vludik.Arbitrage.JobsService.Shared`](Vludik.Arbitrage.JobsService.Shared) | Jobs service event contracts: `JobCreatedEvent`, `JobDeletedEvent`, `JobMode`, `MarginType`. |
| [`Vludik.Arbitrage.JobsService.Worker.Shared`](Vludik.Arbitrage.JobsService.Worker.Shared) | Jobs worker event contracts: `JobWorkerCompletedEvent`, `JobWorkerFailedEvent`. |
| [`Vludik.Arbitrage.SubscriptionsService.Shared`](Vludik.Arbitrage.SubscriptionsService.Shared) | Subscriptions service event contracts: `SubscriptionCreatedEvent`, `SubscriptionDeletedEvent`. |

## Building

```
dotnet restore Vludik.Arbitrage.Shared.sln
dotnet build Vludik.Arbitrage.Shared.sln --configuration Release
```

## Packing

```
dotnet pack Vludik.Arbitrage.Shared.sln --configuration Release --no-build --output ./artifacts
```

Only projects with `<IsPackable>true</IsPackable>` produce a `.nupkg`; each project's own `<Version>` in its `.csproj` is used as-is.

## Publishing

Packages are published automatically to GitHub Packages by [`.github/workflows/nuget-publish.yml`](.github/workflows/nuget-publish.yml) on every push to `main`, or on demand via the workflow's manual trigger. Re-publishing a version that already exists in the feed fails the workflow.

## Installing a package from GitHub Packages

1. Create a GitHub personal access token with the `read:packages` scope.
2. Register the feed (one-time, per machine):

   ```
   dotnet nuget add source "https://nuget.pkg.github.com/directiv13/index.json" \
     --name github \
     --username <your-github-username> \
     --password <YOUR_PAT> \
     --store-password-in-clear-text \
     --configfile "$env:APPDATA\NuGet\NuGet.Config"
   ```

3. Install the package:

   ```
   dotnet add package Vludik.Arbitrage.Shared --version 1.0.0
   ```

The [`nuget.config`](nuget.config) at the repo root declares the `github` source and scopes it to `Vludik.Arbitrage.*` packages, but carries no credentials — copying it into a consuming project still requires registering your own PAT via step 2 above (or a `packageSourceCredentials` block in your own config).
