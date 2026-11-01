# IMPORTANT

For Local runs, We will either use master or 'feature/api_local'

For Deployment of ChatAPI Or say
For the ChatAPI APP SERVICE in Azure we've used the branch 'feature/scaffolded_database_22_10_26'

For Deployment of Gateway Or say
For the ChatBridgeGateway APP SERVICE in Azure we've used the branch 'feature/azure_gateway_deployment_branch'

Do NOT Merge any of these to master branch EXCEPT 'feature/api_local'

FOR NON-DEPLOYMENT changes use 'feature/api_local'

DB-Schema For the Project is mentioned in the Chat.API, under DataBase folder.


## Configuration and Secret Management VERY IMPORTANT TO SAVE YOUR PERSONAL CRDENTIALS

ChatBridge uses secure configuration management to handle sensitive database credentials and connection strings.

### Local Development

.NET User Secrets is used to store sensitive configuration values during local development.

Initialize User Secrets:

```IN Developer powershell Inside the Proj```
dotnet user-secrets init


Configure the database connection string:

dotnet user-secrets set "ConnectionStrings:ChatBridgeConnection" "YOUR_CONNECTION_STRING"

The connection string is accessed through ASP.NET Core configuration:

```IN csharp```
builder.Configuration.GetConnectionString("ChatBridgeConnection");

### Azure Deployment

For production deployment, the Azure SQL connection string is configured through Azure App Service Environment Variables under the Connection Strings section.

| Configuration | Value |
|---|---|
| Name | ChatBridgeConnection |
| Type | SQLAzure |
| Storage | Azure App Service Configuration |

### Security Considerations

- Sensitive credentials are not hardcoded into application source code.
- Local secrets are maintained outside the project directory using .NET User Secrets.
- Production connection strings are maintained through Azure App Service configuration.
- Sensitive configuration files and credentials must not be committed to source control.