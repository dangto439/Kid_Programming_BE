using KidPrograming.Core.Utils;
using System.Text;
using System.Text.Json;

public class FcmService
{
    private static readonly string FcmUrl = "https://fcm.googleapis.com/v1/projects/kid-programming-edu/messages:send";
    private FirebaseAuthHelper _firebaseAuthHelper;
    public FcmService(FirebaseAuthHelper firebaseAuthHelper)
    {
        _firebaseAuthHelper = firebaseAuthHelper;
    }
    public async Task SendNotificationAsync(string deviceToken, string title, string body)
    {
        var accessToken = await _firebaseAuthHelper.GetAccessTokenAsync();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var payload = new
            {
                message = new
                {
                    token = deviceToken,
                    notification = new
                    {
                        title = title,
                        body = body,
                        image = "https://neil.fraser.name/software/Blockly-History/blockly-big.png"
                    }
                }
            };
            ;

            string jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(FcmUrl, content);
            string result = await response.Content.ReadAsStringAsync();
        }
    }
}
