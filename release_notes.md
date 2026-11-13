* Add Version property to $Context
* Preserve host-provided management URL query parameters and the existing response shape. Key exclusion is controlled by the host's default-false opt-in; HTTP callers supply keys independently through `x-functions-key` when using key-free URLs.
