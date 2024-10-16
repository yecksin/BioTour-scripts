using UnityEngine;
using System.Threading.Tasks;
using System;

public class progress : MonoBehaviour
{
    // URL de la API para crear el progreso
    private const string API_URL = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/rest/v1/levels_progress";
    private const string PROGRESS_ID_KEY = "CurrentProgressID";

    // Método para crear el progreso
    public async Task<string> CreateProgress()
    {
        string userId = "64d714c5-f962-43e5-89a7-89b5633226c1"; // Asegúrate de obtener el ID de usuario correcto
        string jsonBody = $"{{ \"completed\": false, \"user_id\": \"{userId}\" }}";

        string response = await Request.SendRequest(API_URL, "POST", jsonBody, true, true);

        if (response != null)
        {
            Debug.Log("Progress created successfully. Response: " + response);
            // Parseamos la respuesta para obtener el ID
            string progressId = ParseProgressId(response);
            if (!string.IsNullOrEmpty(progressId))
            {
                // Guardamos el ID en PlayerPrefs
                PlayerPrefs.SetString(PROGRESS_ID_KEY, progressId);
                PlayerPrefs.Save();
                return progressId;
            }
        }
        
        Debug.LogError("Failed to create progress");
        return null;
    }

    private string ParseProgressId(string response)
    {
        // Asumiendo que la respuesta es un objeto JSON con un campo "id"
        try
        {
            var responseObj = JsonUtility.FromJson<ProgressResponse>(response);
            return responseObj.id.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing progress ID: {e.Message}");
            return null;
        }
    }

    [System.Serializable]
    private class ProgressResponse
    {
        public int id;
    }

    // Nuevo método para borrar el PROGRESS_ID_KEY del almacenamiento
    public void ClearProgressId()
    {
        if (PlayerPrefs.HasKey(PROGRESS_ID_KEY))
        {
            PlayerPrefs.DeleteKey(PROGRESS_ID_KEY);
            PlayerPrefs.Save();
            Debug.Log("Progress ID cleared from storage.");
        }
        else
        {
            Debug.Log("No Progress ID found in storage.");
        }
    }

    // Método para ser asignado al botón
    public void OnCreateProgressButtonClick()
    {
        StartCoroutine(CreateProgressCoroutine());
    }

    private System.Collections.IEnumerator CreateProgressCoroutine()
    {
        Task<string> task = CreateProgress();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error creating progress: {task.Exception.Message}");
        }
        else
        {
            string progressId = task.Result;
            Debug.Log($"Progress created with ID: {progressId}");
            // Aquí puedes hacer algo con el progressId si es necesario
        }
    }

    // Start y Update se mantienen vacíos ya que no necesitamos inicializar nada automáticamente
    void Start() { }
    void Update() { }
}
