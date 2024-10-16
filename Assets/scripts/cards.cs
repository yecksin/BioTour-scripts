using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class cards : MonoBehaviour
{
    private const string API_URL = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/rest/v1/collected_cards";
    private const string PROGRESS_ID_KEY = "CurrentProgressID";

    public async Task CreateCard(int birdTypeId, int modelIndexId)
    {
        string progressId = PlayerPrefs.GetString(PROGRESS_ID_KEY, "");
        if (string.IsNullOrEmpty(progressId))
        {
            Debug.LogError("No progress ID found. Create progress first.");
            return;
        }

        string userId = "64d714c5-f962-43e5-89a7-89b5633226c1"; // Asegúrate de obtener el ID de usuario correcto
        string jsonBody = $"{{ \"bird_type_id\": {birdTypeId}, \"model_index_id\": {modelIndexId}, \"level_progress_id\": {progressId}, \"user_id\": \"{userId}\" }}";

        string response = await Request.SendRequest(API_URL, "POST", jsonBody, true, true);

        if (response != null)
        {
            Debug.Log("Card created successfully. Response: " + response);
        }
        else
        {
            Debug.LogError("Failed to create card");
        }
    }

    public async Task<string> GetCardsByProgressId()
    {
        string progressId = PlayerPrefs.GetString(PROGRESS_ID_KEY, "");
        if (string.IsNullOrEmpty(progressId))
        {
            Debug.LogError("No progress ID found in storage.");
            return null;
        }

        string url = $"{API_URL}?level_progress_id=eq.{progressId}&select=*";
                Debug.Log(url);

        string response = await Request.SendRequest(url, "GET", null);

        if (response != null)
        {
            Debug.Log("Cards retrieved successfully. Response: " + response);
            return response;
        }
        else
        {
            Debug.LogError("Failed to retrieve cards");
            return null;
        }
    }

    // Método para ser asignado al botón
    public void OnGetCardsButtonClick()
    {
        StartCoroutine(GetCardsCoroutine());
    }

    private System.Collections.IEnumerator GetCardsCoroutine()
    {
        Task<string> task = GetCardsByProgressId();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error getting cards: {task.Exception.Message}");
        }
        else
        {
            string cards = task.Result;
            Debug.Log($"Cards retrieved: {cards}");
            // Aquí puedes procesar las cartas obtenidas si es necesario
        }
    }

    // Ejemplo de uso
    public async void CreateExampleCard()
    {
        await CreateCard(1, 1);
    }
}
