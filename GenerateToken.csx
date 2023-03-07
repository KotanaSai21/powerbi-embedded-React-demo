#r "Newtonsoft.Json"
using System.Net;
using System.Text;
using Newtonsoft.Json;
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ILogger log)
{
    HttpClient authclient = new HttpClient();
    var isGCC = false;
    var powerBI_API_URL = "api.powerbi.com";
    var powerBI_API_Scope = "https://analysis.windows.net/powerbi/api/.default";
    if(isGCC) {
        powerBI_API_Scope = "https://analysis.usgovcloudapi.net/powerbi/api/.default";
        powerBI_API_URL = "api.powerbigov.us";
    }

    // Power BI Report Details
    var groupId = "<Workspace ID>";
    var reportId = "<Report ID>";
    // Azure App Registration
    var clientId = "<Client ID>";
    var clientSecret = "<Client Secret>";
    var tenantId = "<Tenant ID>";
     var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        new KeyValuePair<string, string>("client_id", clientId),
        new KeyValuePair<string, string>("scope", powerBI_API_Scope),
        new KeyValuePair<string, string>("client_secret", clientSecret)
    });
    // Generate Access Token to authenticate for Power BI
     var accessToken = await authclient.PostAsync($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", content).ContinueWith<string>((response) =>
    {
        log.LogInformation(response.Result.StatusCode.ToString());
        log.LogInformation(response.Result.ReasonPhrase.ToString());
        log.LogInformation(response.Result.Content.ReadAsStringAsync().Result);
        AzureAdTokenResponse tokenRes =
            JsonConvert.DeserializeObject<AzureAdTokenResponse>(response.Result.Content.ReadAsStringAsync().Result);
        return tokenRes?.AccessToken;;
    });
    // Get PowerBi report url and embed token
    HttpClient powerBiClient = new HttpClient();
    powerBiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    var embedUrl =
        await powerBiClient.GetAsync($"https://{powerBI_API_URL}/v1.0/myorg/groups/{groupId}/reports/{reportId}")
        .ContinueWith<string>((response) =>
        {
            log.LogInformation(response.Result.StatusCode.ToString());
            log.LogInformation(response.Result.ReasonPhrase.ToString());
            PowerBiReport report =
                JsonConvert.DeserializeObject<PowerBiReport>(response.Result.Content.ReadAsStringAsync().Result);
            return report?.EmbedUrl;
        });
    var tokenContent = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("accessLevel", "view")
    });
    var embedToken = await powerBiClient.PostAsync($"https://{powerBI_API_URL}/v1.0/myorg/groups/{groupId}/reports/{reportId}/GenerateToken", tokenContent)
        .ContinueWith<string>((response) =>
        {
            log.LogInformation(response.Result.StatusCode.ToString());
            log.LogInformation(response.Result.ReasonPhrase.ToString());
            PowerBiEmbedToken powerBiEmbedToken =
                JsonConvert.DeserializeObject<PowerBiEmbedToken>(response.Result.Content.ReadAsStringAsync().Result);
            return powerBiEmbedToken?.Token;
        });
    // JSON Response
    EmbedContent data = new EmbedContent
    {
        EmbedToken = embedToken,
        EmbedUrl = embedUrl,
        ReportId = reportId,
        AccessToken = accessToken
    };
    string jsonp = JsonConvert.SerializeObject(data);
    // Return Response
    return new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent(jsonp, Encoding.UTF8, "application/json")
    };
}
public class AzureAdTokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
}
public class PowerBiReport
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    [JsonProperty(PropertyName = "webUrl")]
    public string WebUrl { get; set; }
    [JsonProperty(PropertyName = "embedUrl")]
    public string EmbedUrl { get; set; }
    [JsonProperty(PropertyName = "datasetId")]
    public string DatasetId { get; set; }
}
public class PowerBiEmbedToken
{
    [JsonProperty(PropertyName = "token")]
    public string Token { get; set; }
    [JsonProperty(PropertyName = "tokenId")]
    public string TokenId { get; set; }
    [JsonProperty(PropertyName = "expiration")]
    public DateTime? Expiration { get; set; }
}
public class EmbedContent
{
    public string EmbedToken { get; set; }
    public string EmbedUrl { get; set; }
    public string ReportId { get; set; }
    public string AccessToken { get; set; }
}