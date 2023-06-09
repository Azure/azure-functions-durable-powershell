# Durable Functions for PowerShell

This repo hosts the standalone Durable Functions SDK for PowerShell. Compared to the Durable Functions SDK that comes built-in with the Azure Functions PowerShell worker, this standalone SDK contains performance enhancements, more features, and key bug fixes that would have required a breaking release. For more information on this release, please see [this article](TODO).

> The standalone Durable Functions SDK implementation is currently in **preview**. For production workloads, we recommend continuing to use the built-in Durable Functions SDK which is hosted in [this repo](https://github.com/Azure/azure-functions-powershell-worker).

⚡ Find us in the [PowerShell Gallery](https://www.powershellgallery.com/packages/AzureFunctions.PowerShell.Durable.SDK) ⚡.


## About Durable Functions
 [Durable Functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview) is an extension of [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview) that lets you write stateful functions in a serverless compute environment. The extension lets you define stateful workflows by writing orchestrator functions and stateful entities by writing entity functions using the Azure Functions programming model. Behind the scenes, the extension manages state, checkpoints, and restarts for you, allowing you to focus on your business logic.

You can find more information at the following links:

* [Azure Functions overview](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview)
* [Azure Functions PowerShell developers guide](https://learn.microsoft.com/en-us/azure/azure-functions/functions-reference-powershell)
* [Durable Functions overview](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview)
* [Core concepts and features overview](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-types-features-overview).

> Durable Functions expects certain programming constraints to be followed. Please read the documentation linked above for more information.

## Getting Started

Follow these instructions to get started with Durable Functions in PowerShell:

**🚀 [PowerShell Durable Functions quickstart](https://learn.microsoft.com/en-us/azure/azure-functions/durable/quickstart-powershell-vscode)**

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
