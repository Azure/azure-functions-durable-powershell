# Copilot instructions

## Repository structure

- `src/DurableSDK/` builds `AzureFunctions.PowerShell.Durable.SDK.dll`, the binary PowerShell module loaded into the worker's default assembly load context (ALC). It contains exported cmdlets, module initialization, and the ALC bootstrap.
- `src/DurableEngine/` builds `DurableEngine.dll`, which contains orchestration replay, Durable Task integration, models, and other implementation that depends on Durable Task assemblies.
- `src/AzureFunctions.PowerShell.Durable.SDK.psm1` contains the script-based public commands.
- `src/AzureFunctions.PowerShell.Durable.SDK.psd1` is the module manifest and defines the exported commands, aliases, and minimum PowerShell version.
- `src/Help/` contains the source Markdown for generated external help.
- `test/E2E/AzureFunctions.PowerShell.Durable.SDK.E2E/` contains the xUnit test project. Most tests launch a real Functions host against the packaged module.
- `test/E2E/durableApp/` is the PowerShell Functions app used by E2E tests. `build.ps1` publishes the module into its `Modules/` directory.
- `build.ps1` is the authoritative packaging path. It publishes both C# projects, places `DurableEngine.dll` and its dependencies under `Dependencies/`, copies the top-level SDK assembly and module files, and generates external help.

## Assembly load context and dependency isolation

The module intentionally separates its assemblies to avoid conflicts with assemblies already loaded by the PowerShell worker:

1. `AzureFunctions.PowerShell.Durable.SDK.dll` loads in the default ALC as a nested PowerShell module.
2. Its `ModuleInitializer` registers a default-ALC resolver for `DurableEngine` only.
3. `DurableEngine.dll` loads in `DependencyAssemblyLoadContext`.
4. That custom ALC resolves assemblies from the packaged `Dependencies/` directory, including `DurableTask.Core` and the Microsoft Durable Task worker/client libraries.

Preserve these contracts:

- Code that directly references Durable Task implementation types, including `DurableTask.Core.*`, belongs in `DurableEngine`, not `DurableSDK`.
- Do not add Durable Task package references directly to `DurableSDK`.
- Keep the default/custom ALC boundary limited to SDK-owned types and framework types whose identity is shared with the worker, such as `System.*` and `System.Management.Automation`.
- Do not return, cast, store in framework collections, or deserialize custom-ALC Durable Task objects in default-ALC code. Type names can match while runtime type identity differs, causing missing-assembly, invalid-cast, or array-type-mismatch failures.
- Perform creation, serialization, deserialization, and collection construction involving Durable Task types entirely inside `DurableEngine`. Expose an SDK-owned operation or result across the boundary instead of exposing Durable Task objects.
- Treat Newtonsoft.Json carefully at the boundary. Type-preserving payloads can contain `$type` metadata for Durable Task classes; ensure those types are resolved and materialized in the same ALC as their destination model and collections.
- If a new dependency is needed by orchestration internals, add it to `DurableEngine` and confirm `build.ps1` places it under `Dependencies/`.
- Do not broaden the default ALC resolver to load all dependencies unless the isolation design is intentionally being replaced and fully tested.

Unit tests that reference the projects directly run in a single test-process loading environment and do not prove packaged-module ALC correctness. Any change involving project references, assembly placement, JSON type metadata, or Durable Task types must also be tested through the packaged module and a real Functions host.

## Build and test

- Use `dotnet build .\src\DurableSDK.sln --configuration Release` for a fast compile check.
- Use `.\build.ps1 -Configuration Release` to validate the actual module layout.
- Run focused xUnit tests with `dotnet test .\test\E2E\AzureFunctions.PowerShell.Durable.SDK.E2E\AzureFunctions.PowerShell.Durable.SDK.E2E.csproj --filter <filter>`.
- Run `.\test\E2E\Start-E2ETest.ps1 -NoBuild` after packaging for changes affecting orchestration, module loading, dependency resolution, or worker/extension payloads. Ensure Azurite is available and running.

Keep changes compatible with both legacy Durable extension payloads and current type-preserving payloads unless a deliberate breaking change is approved.
