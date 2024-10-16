using UnityEngine;
using System;

public class login : MonoBehaviour
{
    public async void onLoginButtonClick()
    {
        Debug.Log("*****Login button clicked*****");
        string url = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/auth/v1/token?grant_type=password";
        string jsonBody = "{\"email\": \"test@gmail.com\", \"password\": \"111111\"}";

        // Note: We're passing false for requiresAuth since this is a login request
        string response = await Request.SendRequest(url, "POST", jsonBody, false);
        
        if (response != null)
        {
            Debug.Log("Response: " + response);
            SaveAccessToken(response);
        }
    }

    private void SaveAccessToken(string response)
    {
        try
        {
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response);
            Request.SaveAccessToken(loginResponse.access_token);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving access token: " + e.Message);
        }
    }

    [System.Serializable]
    private class LoginResponse
    {
        public string access_token;
    }
}
