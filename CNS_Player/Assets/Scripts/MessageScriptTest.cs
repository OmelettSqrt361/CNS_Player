using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Collections.Generic;

[System.Serializable]
public class KeyValue
{
    public string key;
    public int value;

    public KeyValue(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class KeyValueList
{
    public List<KeyValue> items = new List<KeyValue>();
}

public class MessageScriptTest : MonoBehaviour
{
    public string serverUrl = "http://86.49.165.87:5000/upload";

    // Updated function: takes List<(string,int)>
    public void SendJsonFile(List<(string, int)> tupleData)
    {
        // Convert tuples to KeyValue objects
        List<KeyValue> keyValues = new List<KeyValue>();
        foreach (var t in tupleData)
        {
            keyValues.Add(new KeyValue(t.Item1, t.Item2));
        }

        // Wrap for JsonUtility
        KeyValueList wrapper = new KeyValueList();
        wrapper.items = keyValues;

        // Serialize to JSON
        string json = JsonUtility.ToJson(wrapper);
        Debug.Log("Sending JSON: " + json);

        // Send to server
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
}
