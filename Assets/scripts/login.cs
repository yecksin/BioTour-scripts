using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // Add this line
using System.Text;

public class login : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void onLoginButtonClick()
    {
        Debug.Log("*****Login button clicked*****");
        StartCoroutine(PostLoginRequest());
    }

    IEnumerator PostLoginRequest()
    {
        string url = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/auth/v1/token?grant_type=password";
        string jsonBody = "{\"email\": \"test@gmail.com\", \"password\": \"111111\"}";

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("apikey", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZ3bGtkanBjZmNkaWltbWtxeHJ4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MjgyNjgxNzMsImV4cCI6MjA0Mzg0NDE3M30.rEzatvw8q--aFLcx86SQsSlYsZHYVQTUPkVh2VJxWCU");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
            }
        }
    }


}
