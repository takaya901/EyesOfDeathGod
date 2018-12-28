using UnityEngine;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

public class FaceDetector : MonoBehaviour 
{
    const string SUBSCRIPTION_KEY = "b3560fbf21bb4f1c9e4cc1e8058e27a6";
    const string URI_BASE =
        "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=false";
    
	void Start () 
	{
        string imageFilePath = "/Users/takaya/Downloads/c638d75b.jpg";

        if (File.Exists(imageFilePath)){
            try{
                MakeAnalysisRequest(imageFilePath);
            }
            catch (Exception e){
                Debug.Log("\n" + e.Message + "\nPress Enter to exit...\n");
            }
        }
        else{
            Debug.Log("\nInvalid file path.\nPress Enter to exit...\n");
        }
    }
	
	void Update () 
	{
		
	}
	
	// Gets the analysis of the specified image by using the Face REST API.
    static async void MakeAnalysisRequest(string imageFilePath)
    {
        var client = new HttpClient();

        // Request headers.
        client.DefaultRequestHeaders.Add(
            "Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);

        // Request parameters. A third optional parameter is "details".
//        string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
//            "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
//            "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
        string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                                   "&returnFaceAttributes=age";

        // Assemble the URI for the REST API Call.
        string uri = URI_BASE + "?" + requestParameters;

        HttpResponseMessage response;

        // Request body. Posts a locally stored JPEG image.
        var byteData = GetImageAsByteArray(imageFilePath);

        using (var content = new ByteArrayContent(byteData))
        {
            // This example uses content type "application/octet-stream".
            // The other content types you can use are "application/json"
            // and "multipart/form-data".
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            // Execute the REST API call.
            response = await client.PostAsync(uri, content);

            // Get the JSON response.
            string contentString = await response.Content.ReadAsStringAsync();

            // Display the JSON response.
//            Debug.Log(JsonPrettyPrint(contentString));
            contentString = contentString.Substring(1, contentString.Length - 2);
            Debug.Log(contentString);
        
//            string itemJson = "{\"faceId\": \"4ba77ed4-d523-4d97-871a-96856a1b72c8\", \"faceRectangle\": {\"top\": 128,\"left\": 189,\"width\": 202,\"height\": 202}, \"faceAttributes\": {\"age\": 26.0}}";
            string itemJson = contentString;
            var face = JsonUtility.FromJson<Face>(itemJson);

            Debug.Log("face id " + face.faceId);
            Debug.Log("face top " + face.faceRectangle.top);
            Debug.Log("face age " + face.faceAttributes.age);
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

    // Formats the given JSON string by adding line breaks and indents.
    static string JsonPrettyPrint(string json)
    {
        if (string.IsNullOrEmpty(json))
            return string.Empty;

        json = json.Replace(Environment.NewLine, "").Replace("\t", "");

        var sb = new StringBuilder();
        bool quote = false;
        bool ignore = false;
        int offset = 0;
        int indentLength = 3;

        foreach (char ch in json)
        {
            switch (ch)
            {
                case '"':
                    if (!ignore) quote = !quote;
                    break;
                case '\'':
                    if (quote) ignore = !ignore;
                    break;
            }

            if (quote)
                sb.Append(ch);
            else
            {
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', ++offset * indentLength));
                        break;
                    case '}':
                    case ']':
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', --offset * indentLength));
                        sb.Append(ch);
                        break;
                    case ',':
                        sb.Append(ch);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', offset * indentLength));
                        break;
                    case ':':
                        sb.Append(ch);
                        sb.Append(' ');
                        break;
                    default:
                        if (ch != ' ') sb.Append(ch);
                        break;
                }
            }
        }

        return sb.ToString().Trim();
    }
}

[Serializable]
public class Face
{
    public string faceId;
    public FaceRectangle faceRectangle;
    public FaceAttributes faceAttributes;
}

[Serializable]
public class FaceRectangle
{
    public int top;
    public int left;
    public int width;
    public int height;
}

[Serializable]
public class FaceAttributes
{
    public float age;
}