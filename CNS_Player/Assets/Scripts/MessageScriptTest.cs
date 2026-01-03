using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class MessageScriptTest : MonoBehaviour
{
    // Put your server URL here
    public string serverUrl = "http://86.49.165.87:5000/upload";

    // Example class to send
    [System.Serializable]
    public class PlayerData
    {
        public string name;
        public int score;
    }

    // Call this to send JSON
    public void SendJsonFile(PlayerData data)
    {
        string json = JsonUtility.ToJson(data); // Convert object to JSON
        StartCoroutine(PostRequest(serverUrl, json));
    }

    IEnumerator PostRequest(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (!request.isNetworkError && !request.isHttpError)
        {
            Debug.Log("Upload complete: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error sending JSON: " + request.error);
        }
    }

    // Example usage: send a test JSON on start
    void Start()
    {
        PlayerData example = new PlayerData();
        example.name = "PetrPavel";
        example.score = 6969;

        SendJsonFile(example);
    }
}
