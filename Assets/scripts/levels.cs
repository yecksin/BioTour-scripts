using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class levels : MonoBehaviour
{
    [System.Serializable]
    private class Level
    {
        public int id;
        public string name;
        public int difficulty;
        // Añade aquí más campos según la estructura de tus datos de nivel
    }

    [System.Serializable]
    private class LevelList
    {
        public List<Level> items;
    }

    private List<Level> levelList = new List<Level>();

    // Este método se puede llamar cuando necesites cargar los niveles
    public async void onRequestLevels()
    {
        Debug.Log("Requesting levels...");
        string url = "https://vwlkdjpcfcdiimmkqxrx.supabase.co/rest/v1/levels?select=*";
        string response = await Request.SendRequest(url, "GET");

        if (response != null)
        {
            Debug.Log("Levels Response: " + response);
            ParseAndPrintLevels(response);
        }
        else
        {
            Debug.LogError("Failed to fetch levels");
        }
    }

    private void ParseAndPrintLevels(string json)
    {
        try
        {
            levelList = JsonUtility.FromJson<LevelList>("{\"items\":" + json + "}").items;
            Debug.Log($"Parsed {levelList.Count} levels");

            // Imprimir información de cada nivel
            foreach (var level in levelList)
            {
                Debug.Log($"Level ID: {level.id}, Name: {level.name}, Difficulty: {level.difficulty}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing levels: {e.Message}");
        }
    }

    // Start y Update se mantienen vacíos ya que no necesitamos inicializar nada automáticamente
    void Start() { }
    void Update() { }
}
