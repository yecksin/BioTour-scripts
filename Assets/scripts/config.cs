using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class Config : MonoBehaviour
{
    private const string API_URL = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/rest/v1/user_settings";

    [System.Serializable]
    private class UserSettings
    {
        public int id;
        public int sound_level;
        public int music_level;
        public string user_id;
    }

    public void OnManageSettingsButtonClick()
    {
        StartCoroutine(ManageUserSettingsCoroutine(10, 55));
    }

    private IEnumerator ManageUserSettingsCoroutine(int soundLevel, int musicLevel)
    {
        Task task = ManageUserSettings(soundLevel, musicLevel);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error managing settings: {task.Exception.Message}");
        }
    }

    private async Task ManageUserSettings(int soundLevel, int musicLevel)
    {
        string userId = login.GetUserId();
        string getUrl = $"{API_URL}?user_id=eq.{userId}&select=*";
        string response = await Request.SendRequest(getUrl, "GET", null);

        if (string.IsNullOrEmpty(response) || response == "[]")
        {
            await CreateUserSettings(soundLevel, musicLevel);
        }
        else
        {
            await UpdateUserSettings(soundLevel, musicLevel, response);
        }
    }

    private async Task CreateUserSettings(int soundLevel, int musicLevel)
    {
        string userId = login.GetUserId();
        string jsonBody = $"{{\"sound_level\":{soundLevel},\"music_level\":{musicLevel},\"user_id\":\"{userId}\"}}";
        string response = await Request.SendRequest(API_URL, "POST", jsonBody);

        if (response != null)
        {
            Debug.Log("User settings created successfully.");
        }
        else
        {
            Debug.LogError("Failed to create user settings.");
        }
    }

    private async Task UpdateUserSettings(int soundLevel, int musicLevel, string getResponse)
    {
        UserSettings[] settings = JsonHelper.FromJson<UserSettings>(getResponse);
        if (settings != null && settings.Length > 0)
        {
            int settingsId = settings[0].id;
            string userId = login.GetUserId();
            string updateUrl = $"{API_URL}?id=eq.{settingsId}";
            string jsonBody = $"{{\"sound_level\":{soundLevel},\"music_level\":{musicLevel},\"user_id\":\"{userId}\"}}";

            string response = await Request.SendRequest(updateUrl, "PATCH", jsonBody);

            if (response != null)
            {
                Debug.Log("User settings updated successfully.");
            }
            else
            {
                Debug.LogError("Failed to update user settings.");
            }
        }
        else
        {
            Debug.LogError("No settings found to update.");
        }
    }

    public void OnGetUserSettingsButtonClick()
    {
        StartCoroutine(GetUserSettingsCoroutine());
    }

    private IEnumerator GetUserSettingsCoroutine()
    {
        Task<UserSettings[]> task = GetUserSettings();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error getting user settings: {task.Exception.Message}");
        }
        else
        {
            UserSettings[] settings = task.Result;
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    Debug.Log($"ID: {setting.id}, Sound Level: {setting.sound_level}, Music Level: {setting.music_level}");
                }
            }
        }
    }

    private async Task<UserSettings[]> GetUserSettings()
    {
                    Debug.Log(login.GetUserId());

        string getUrl = $"{API_URL}?user_id=eq.{login.GetUserId()}&select=*";
        string response = await Request.SendRequest(getUrl, "GET", null);

        if (!string.IsNullOrEmpty(response) && response != "[]")
        {
            UserSettings[] settings = JsonHelper.FromJson<UserSettings>(response);
            Debug.Log("User settings retrieved successfully.");
            return settings;
        }
        else
        {
            Debug.LogError("Failed to retrieve user settings or no settings found.");
            return null;
        }
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
