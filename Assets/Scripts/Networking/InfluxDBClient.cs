using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class InfluxDBClient
{
    const string influxUrl = "https://eu-central-1-1.aws.cloud2.influxdata.com";
    const string authToken = "rznFeib073JCyeFDTp4hWShxbrAzP65Y4mng7edGa9ajoNkZfgCv7p99mMzNd3XWHFjJCxqsMO542SrSbAc6Pw==";

    public static IEnumerator QueryInflux(string fluxQuery)
    {
        var bodyJson = new
        {
            query = fluxQuery,
            type = "flux"
        };
        string body = JsonUtility.ToJson(bodyJson);

        UnityWebRequest request = new UnityWebRequest(influxUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", $"Token {authToken}");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            yield return request.downloadHandler.nativeData;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            yield return null;
        }
    }
}
