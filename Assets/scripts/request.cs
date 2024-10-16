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
            
            bool isModifyingRequest = method.Equals("POST", System.StringComparison.OrdinalIgnoreCase) ||
                                      method.Equals("PATCH", System.StringComparison.OrdinalIgnoreCase) ||
                                      method.Equals("PUT", System.StringComparison.OrdinalIgnoreCase);

            if (isModifyingRequest)
            {
                webRequest.SetRequestHeader("Prefer", "return=representation");
            }
            
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
                string response = webRequest.downloadHandler.text;
                if (isModifyingRequest)
                {
                    // Para POST, PATCH, PUT, tomar solo el primer elemento del array
                    return ExtractFirstElementFromJsonArray(response);
                }
                return response;
            }
        }
    }

    private static string ExtractFirstElementFromJsonArray(string jsonArray)
    {
        if (string.IsNullOrEmpty(jsonArray))
        {
            return "[]";
        }

        jsonArray = jsonArray.Trim();
        if (jsonArray.StartsWith("[") && jsonArray.EndsWith("]"))
        {
            int firstObjectStart = jsonArray.IndexOf('{');
            if (firstObjectStart != -1)
            {
                int firstObjectEnd = jsonArray.IndexOf('}', firstObjectStart);
                if (firstObjectEnd != -1)
                {
                    return jsonArray.Substring(firstObjectStart, firstObjectEnd - firstObjectStart + 1);
                }
            }
        }

        Debug.LogWarning("Response is not a JSON array or is empty");
        return "[]";
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
