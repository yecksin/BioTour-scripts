using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class Config : MonoBehaviour
{
    private const string API_URL = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/rest/v1/user_settings";
    private const string USER_ID = "64d714c5-f962-43e5-89a7-89b5633226c1";

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
        StartCoroutine(ManageUserSettingsCoroutine(50, 55));
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
        string getUrl = $"{API_URL}?user_id=eq.{USER_ID}&select=*";
        string response = await Request.SendRequest(getUrl, "GET", null, true);

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
        string jsonBody = $"{{\"sound_level\":{soundLevel},\"music_level\":{musicLevel},\"user_id\":\"{USER_ID}\"}}";
        string response = await Request.SendRequest(API_URL, "POST", jsonBody, true);

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
        UserSettings[] settings = JsonUtility.FromJson<UserSettings[]>(getResponse);
        if (settings != null && settings.Length > 0)
        {
            int settingsId = settings[0].id;
            string updateUrl = $"{API_URL}?id=eq.{settingsId}";
            string jsonBody = $"{{\"sound_level\":{soundLevel},\"music_level\":{musicLevel},\"user_id\":\"{USER_ID}\"}}";

            string response = await Request.SendRequest(updateUrl, "PATCH", jsonBody, true);

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
}
