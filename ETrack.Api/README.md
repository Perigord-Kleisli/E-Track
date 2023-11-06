Rquires the following user-secrets to be defined:

```bash
dotnet user-secrets set "Smtp:User" <SMTP EmailAdress>
dotnet user-secrets set "Smtp:Pass" <SMTP Password>
dotnet user-secrets set "Smtp:Host" <SMTP Host>
dotnet user-secrets set "AppSettings:Token" <Issuer Signing Key>
```