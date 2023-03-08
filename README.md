# Embed a Report using Service Principal in React App

## Set Up Application

Create an Azure AD app and enable app's service principal to your workspace. Refer to [Embed Content using Service Principal](https://learn.microsoft.com/en-us/power-bi/developer/embedded/embed-service-principal)

Follow the [Embed Content for your Customers](https://learn.microsoft.com/en-us/power-bi/developer/embedded/embed-customer-app)

## Generate a Embed Token from Azure portal

Create a HTTP Trigger Function in Azure. Refer to [Create your first function in the Azure portal](https://learn.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal)

After the function created, Go to Code + Test and paste the code from [GenerateToken.csx](/GenerateToken.csx)

Enter the details and Click on Test/Run. You will get HTTP response with EmbedToken, Embed URL.

Click on the Get function URL and copy the function URL

## Demo App

Embeded the Report using [powerbi-client-react-npm](https://www.npmjs.com/package/powerbi-client-react) library and the sample code from [powerbi-client-react-github](https://github.com/microsoft/powerbi-client-react)

Paste the Function URL in [App.tsx](/demo/src/App.tsx)

Install the Dependencies

```
npm i
```
Run the Demo App

```
npm run demo
```



