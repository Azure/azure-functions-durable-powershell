# Function Chaining sample

## Pre-requisites

You'll need core tools version v4.0.5095+. Please run `func --version` to ensure your core tools version is compatible.

## How to try it out

This repo already contains the basic project structure for a function-chaining Durable Functions PowerShell app.
You may use this as the starting point for experimentation.

### 1. Install the SDK from the PowerShell Gallery

This has been done for you in `requirements.psd1`, by including the following line:

```json
'AzureFunctions.PowerShell.Durable.SDK' = '1.0.0-alpha'
```

### 2. Import the SDK in your `profile.ps1`.

This has been done for you in this starter project.
Please verify that the `profile.ps1` file contains the following line:

```powershell
Import-Module AzureFunctions.PowerShell.Durable.SDK -ErrorAction Stop
```

### 3. Set the env variable `ExternalDurablePowerShellSDK` to `"true"`.

This has been done for you in this repo's starter project.
Please verify that this setting is set in your `local.settings.json`.

### 4. Try it!

Run `func host start` and run the orchestrator with a GET request to `http://localhost:7071/api/orchestrators/DurableFunctionsOrchestrator`.

### 6. Confirm you're using the new SDK

Since the new SDK is backwards compatible with the old one, it's worth doing a sanity check that you are actually using the new experience.

To do this, run `func host start --verbose`, start the orchestrator by performing a GET request to `http://localhost:7071/api/orchestrators/DurableFunctionsOrchestrator`, and finally CTRL+F for the following log: `Utilizing external Durable Functions SDK: 'True'`. If you can find it, you're using the new experience.

## If you deploy to Azure

You will need to set the `ExternalDurablePowerShellSDK` application setting to `"true"`.