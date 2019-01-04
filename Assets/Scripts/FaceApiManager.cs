using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using static OpenCVForUnity.Imgcodecs;

/// <summary>
/// Printerからテクスチャのバイト配列を受け，FaceAPIに送って年齢を取得する
/// </summary>
public class FaceApiManager : MonoBehaviour
{
    public List<Face> Faces { get; private set; }
    
    const string SUBSCRIPTION_KEY = "b3560fbf21bb4f1c9e4cc1e8058e27a6";
    const string URI_BASE = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect";

    public void GetAge(byte[] textureBytes)
    {
        try{
            MakeAnalysisRequest(textureBytes);
        }
        catch (Exception e){
            Debug.Log("\n" + e.Message + "\nPress Enter to exit...\n");
        }
    }

    // Gets the analysis of the specified image by using the Face REST API.
    async void MakeAnalysisRequest(byte[] textureBytes)
    {
        var client = new HttpClient();

        // Request headers.
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);
        
        string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                                   "&returnFaceAttributes=age";

        // Assemble the URI for the REST API Call.
        string uri = URI_BASE + "?" + requestParameters;

        using (var content = new ByteArrayContent(textureBytes))
        {
            // This example uses content type "application/octet-stream".
            // The other content types you can use are "application/json"
            // and "multipart/form-data".
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await client.PostAsync(uri, content);
            var json = await response.Content.ReadAsStringAsync();

//            //外側の[]を削除
//            contentString = contentString.Substring(1, contentString.Length - 2);
            Debug.Log(json);
            if (string.IsNullOrEmpty(json)) {
                Debug.Log("empty");
                return;
            }
        
//            string json = contentString;
            Faces = JsonHelper.ListFromJson<Face>(json);

            Debug.Log("face id " + Faces[0].faceId);
            Debug.Log("face age " + Faces[0].faceAttributes.age);
        }
    }
}