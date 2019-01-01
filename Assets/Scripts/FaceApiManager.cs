using UnityEngine;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using static OpenCVForUnity.Imgcodecs;

public class FaceApiManager : MonoBehaviour
{
    Printer _printer;
    public Face Face { get; set; }
    const string SUBSCRIPTION_KEY = "b3560fbf21bb4f1c9e4cc1e8058e27a6";
    const string URI_BASE =
        "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=false";

    byte[] _bytes;
    
	void Start ()
    {
//        _printer = GetComponent<Printer>();
//        string imageFilePath = "/Users/takaya/Downloads/c638d75b.jpg";
//        var img = imread(imageFilePath);
//        _bytes = new byte[(int) (img.total() * img.channels())];
//        img.get(0, 0, _bytes);
//        Debug.Log("mat: " + _bytes.Length);
//
//        if (File.Exists(imageFilePath)){
//            try{
//                MakeAnalysisRequest(imageFilePath);
//            }
//            catch (Exception e){
//                Debug.Log("\n" + e.Message + "\nPress Enter to exit...\n");
//            }
//        }
//        else{
//            Debug.Log("\nInvalid file path.\nPress Enter to exit...\n");
//        }
    }

    public void GetAge(byte[] bytes)
    {
        try{
            MakeAnalysisRequest(bytes);
        }
        catch (Exception e){
            Debug.Log("\n" + e.Message + "\nPress Enter to exit...\n");
        }

//        return (int)Face.faceAttributes.age;
    }

    // Gets the analysis of the specified image by using the Face REST API.
    async void MakeAnalysisRequest(byte[] bytes)
    {
        var client = new HttpClient();

        // Request headers.
        client.DefaultRequestHeaders.Add(
            "Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);
        
        string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                                   "&returnFaceAttributes=age";

        // Assemble the URI for the REST API Call.
        string uri = URI_BASE + "?" + requestParameters;

        HttpResponseMessage response;

        // Request body. Posts a locally stored JPEG image.
//        var byteData = GetImageAsByteArray(imageFilePath);

        using (var content = new ByteArrayContent(bytes))
        {
            // This example uses content type "application/octet-stream".
            // The other content types you can use are "application/json"
            // and "multipart/form-data".
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // Execute the REST API call.
            response = await client.PostAsync(uri, content);

            // Get the JSON response.
            string contentString = await response.Content.ReadAsStringAsync();

            // Display the JSON response.
            //外側の[]を削除
            contentString = contentString.Substring(1, contentString.Length - 2);
            Debug.Log(contentString);
            if (string.IsNullOrEmpty(contentString)) {
                Debug.Log("empty");
                return;
            }
        
            string json = contentString;
            Face = JsonUtility.FromJson<Face>(json);

            Debug.Log("face id " + Face.faceId);
            Debug.Log("face top " + Face.faceRectangle.top);
            Debug.Log("face age " + Face.faceAttributes.age);
        }
    }

    // Returns the contents of the specified file as a byte array.
    static byte[] GetImageAsByteArray(string imageFilePath)
    {
        using (var fileStream =
            new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
        {
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}