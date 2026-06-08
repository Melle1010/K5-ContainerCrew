# K5-ContainerCrew

## Broken deploy troubleshooting

A broken deployment is diagnosed entirely with the help of information stored in the ContainerAppConsoleLogs_CL table, which can be found inside of the Log Analytics workspace in Azure Portal. For our use case, we have identified three query views that are especially important. The first is the ability to see which HTTP requests were made. This helps with the confirmation that the application is reachable and that routing and ingress behave as expected. The second is the exception view. This one exposes any unhandled errors returned to the user and makes it possible to pinpoint logic issues, configuration problems, or dependency failures. The third is the execution‑time & logged message view, which shows how long different stages of the code take to run and a small message to clarify the position in the code. The following three queries represent each of these views respectively and can be used as an input directly into the Logs section inside of the Log Analytics workspace. Make sure to use KQL mode.

View 1 - POST Http requests
```
ContainerAppConsoleLogs_CL
| where Log_s contains "GET" or Log_s contains "POST"
| sort by TimeGenerated desc
```

View 2 - Exceptions
```
ContainerAppConsoleLogs_CL
| where Log_s contains "Exception"
| sort by TimeGenerated desc
```

View 3 - Execution time & Logged message
```
PLACEHOLDER
PLACEHOLDER
PLACEHOLDER
```
