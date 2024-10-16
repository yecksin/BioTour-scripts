using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;

public static class Request
{
    private const string ACCESS_TOKEN_KEY = "AccessToken";
    private const string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZ3bGtkanBjZmNkaWltbWtxeHJ4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjgyNjgxNzMsImV4cCI6MjA0Mzg0NDE3M30.rEzatvw8q--aFLcx86SQsSlYsZHYVQTUPkVh2VJxWCU";

    public static async Task<string> SendRequest(string url, string method, string body = null, bool requiresAuth = true)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(body))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("apikey", API_KEY);
            
            if (requiresAuth)
            {
                string accessToken = PlayerPrefs.GetString(ACCESS_TOKEN_KEY, "");
                if (string.IsNullOrEmpty(accessToken))
                {
                    Debug.LogError("Access token not found. User may need to log in.");
                    return null;
                }
                webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
            }

            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
                return null;
            }
            else
            {
                return webRequest.downloadHandler.text;
            }
        }
    }

    public static void SaveAccessToken(string accessToken)
    {
        PlayerPrefs.SetString(ACCESS_TOKEN_KEY, accessToken);
        PlayerPrefs.Save();
        Debug.Log("Access token saved successfully");
    }

    public static void ClearAccessToken()
    {
        PlayerPrefs.DeleteKey(ACCESS_TOKEN_KEY);
        PlayerPrefs.Save();
        Debug.Log("Access token cleared");
    }
}
