using UnityEngine;
using System.Threading.Tasks;

public class progress : MonoBehaviour
{
    // URL de la API para crear el progreso
    private const string API_URL = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/rest/v1/levels_progress";

    // Método para crear el progreso
    public async void CreateProgress()
    {
        string userId = "64d714c5-f962-43e5-89a7-89b5633226c1"; // Asegúrate de obtener el ID de usuario correcto
        string jsonBody = $"{{ \"completed\": false, \"user_id\": \"{userId}\" }}";

        string response = await Request.SendRequest(API_URL, "POST", jsonBody, true);

        if (response != null)
        {
            Debug.Log("Progress created successfully. Response: " + response);
        }
        else
        {
            Debug.LogError("Failed to create progress");
        }
    }

    // Start y Update se mantienen vacíos ya que no necesitamos inicializar nada automáticamente
    void Start() { }
    void Update() { }
}
