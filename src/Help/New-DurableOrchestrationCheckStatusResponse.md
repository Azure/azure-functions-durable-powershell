---
external help file: AzureFunctions.PowerShell.Durable.SDK-help.xml
Module Name: AzureFunctions.PowerShell.Durable.SDK
online version:
schema: 2.0.0
---

# New-DurableOrchestrationCheckStatusResponse

## SYNOPSIS

Create a check status response for a durable orchestration.

## SYNTAX

```
New-DurableOrchestrationCheckStatusResponse [-Request] <Object> [-InstanceId] <String> [[-DurableClient] <Object>] [<CommonParameters>]
```

## DESCRIPTION

Creates an HTTP response with status check URLs for a durable orchestration instance.
This response includes URLs for checking status, terminating, suspending, resuming, and raising events.
URL query parameters are supplied by the Functions host and preserved by this
cmdlet. By default, generated URLs contain the Durable system key in `code`.
On a supporting host, `httpSettings.excludeManagementKeyFromUrls=true` excludes
that key from generated URLs. HTTP callers must then obtain an authorized key
independently and send `x-functions-key` on each management request. The response
does not return authentication headers or credentials.

## EXAMPLES

### Example 1

```powershell
New-DurableOrchestrationCheckStatusResponse -Request $Request -InstanceId "example-instance-id"
```

Creates a standard orchestration check status response with management URLs.

## PARAMETERS

### -Request

The HTTP request object that triggered the orchestration.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -InstanceId

The ID of the orchestration instance for which to create the check status response.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DurableClient

The durable client context object used to generate management URLs for the orchestration instance.

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: 3
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object (Request)

You can pipe HTTP request objects to create status check responses for multiple orchestrations.

### System.String (InstanceId)

You can pipe instance ID strings to generate status check responses for specific orchestration instances.

## OUTPUTS

### HttpResponseContext

Returns an HTTP response context with status code 202 (Accepted) and management URLs for checking orchestration status, sending external events, and terminating the orchestration.

## NOTES

- This cmdlet is typically used in HTTP trigger functions after starting an orchestration to provide management URLs.
- The response includes a 202 (Accepted) status code following the standard async HTTP pattern.
- The host option defaults to `false`, preserving existing URL authentication. Enable it only after the app's HTTP management callers support independently supplied `x-functions-key` authentication.
- SDK management commands normally use the local RPC endpoint. Keep RPC enabled for authenticated apps with key-free URLs; the SDK does not automatically supply a key to its legacy public-HTTP fallback.
- Treat keys as secrets. Do not log them, append them to URLs, or forward them to unrelated origins.
- The option does not redact incoming requests or existing logs. Keys manually supplied in query strings are still accepted and can still be logged.
- The key remains an app-wide management credential, not an instance-scoped token.
- The response follows the Durable Functions HTTP API conventions for orchestration management.

## RELATED LINKS

[Durable Functions for PowerShell](https://github.com/Azure/azure-functions-durable-powershell)
