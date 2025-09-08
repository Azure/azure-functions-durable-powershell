# Contributor Onboarding
This contributor guide explains how to make and test changes to Durable Functions in PowerShell.
Thank you for taking the time to contribute to the Durable Functions PowerShell SDK!

## Table of Contents

- [Relevant Docs](#relevant-docs)
- [Prerequisites](#prerequisites)
- [Pull Request Change Flow](#pull-request-change-flow)
- [Testing with a Durable Functions app](#testing-with-a-durable-functions-app)
- [Debugging .NET packages from a Durable Functions PowerShell app](#debugging-net-packages-from-a-durable-functions-java-app)

## Relevant Docs
- [Durable Functions Overview](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview)
- [Durable Functions Application Patterns](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=in-process%2Cnodejs-v3%2Cv1-model&pivots=java#application-patterns)
- [Azure Functions PowerShell Quickstart](https://learn.microsoft.com/en-us/azure/azure-functions/durable/quickstart-powershell-vscode)

## Prerequisites
- Visual Studio Code
- [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-java)

## Pull Request Change Flow

The general flow for making a change to the library is:

1. 🍴 Fork the repo (add the fork via `git remote add me <clone url here>`)
2. 🌳 Create a branch for your change (generally branch from dev) (`git checkout -b my-change`)
3. 🛠 Make your change
4. ✔️ Test your change
5. ⬆️ Push your changes to your fork (`git push me my-change`)
6. 💌 Open a PR to the main branch
7. 📢 Address feedback and make sure tests pass (yes even if it's an "unrelated" test failure)
8. 📦 [Rebase](https://git-scm.com/docs/git-rebase) your changes into meaningful commits (`git rebase -i HEAD~N` where `N` is commits you want to squash)
9. :shipit: Rebase and merge (This will be done for you if you don't have contributor access)
10. ✂️ Delete your branch (optional)

## Testing with a Durable Functions app

The following instructions explain how to test changes in a Durable Functions PowerShell app.

1. After making changes, start Azurite.
2. Run `./build.ps1`
3. Remove 'AzureFunctions.PowerShell.Durable.SDK' = '2.*' from requirements.psd1 in your test app
4. Copy the Modules folder from the durableApp E2E test app in the PowerShell SDK to your test app
5. In Visual Studio Code, run `func host start` or press F5

## Debugging .NET packages from a Durable Functions PowerShell app

If you want to debug into the Durable Task or any of the .NET bits, follow the instructions below:

1. If you would like to debug a custom local WebJobs extension package then create the custom package, place it in a local directory, and then run `func extensions install --package Microsoft.Azure.WebJobs.Extensions.DurableTask --version <VERSION>`. If you update the version while debugging and the new version doesn't get picked up, then try running `func extensions install` to get the new changes.
2. Make sure the Durable Functions PowerShell debugging is set up already and the debugger has started the `func` process.
3. In the VSCode editor for DurableTask, click Debug -> .NET Core Attach Process, search for the functions dotnet.exe process and attach to it.
4. Add a breakpoint in both editors and continue debugging.
