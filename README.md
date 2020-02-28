# MultiFactor.AspNet.Sample
Sample ASP.NET project with multifactor authentication

Usage:
1. Login to https://admin.multifactor.ru
2. Create resource and get API keys
3. Edit web.config and set params
```xml
    <!-- mfa settings-->
    <add key="mfa-api-url" value="https://api.multifactor.ru" />
    <add key="mfa-api-key" value="" />
    <add key="mfa-api-secret" value="" />
```
