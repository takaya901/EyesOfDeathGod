using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Text = UnityEngine.UI.Text;

/// <summary>
/// Printerからテクスチャのバイト配列を受け，FaceAPIに送って年齢を取得する
/// </summary>
public class FaceApiManager : MonoBehaviour
{
    [SerializeField] Text _errMsg;
    List<Face> _faces;
    public IReadOnlyList<Face> Faces { get; private set; }    //外部参照用
    public bool IsWaiting { get; private set; }
    
    const string SUBSCRIPTION_KEY = "b3560fbf21bb4f1c9e4cc1e8058e27a6";
    const string URI_BASE = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect";
    const string NO_INTERNET_MSG = "No Internet Connection";
    const string JSON_EMPTY = "Try again";

    public void GetAge(byte[] textureBytes)
    {
        IsWaiting = true;
        MakeAnalysisRequest(textureBytes);
    }

    // Gets the analysis of the specified image by using the Face REST API.
    async void MakeAnalysisRequest(byte[] textureBytes)
    {
        var client = new HttpClient();
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

            string json = null;
            try {
                var response = await client.PostAsync(uri, content);
                json = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e) {
                StartCoroutine(ShowErrMsgCoroutine(NO_INTERNET_MSG));
            }

//            Debug.Log(json);
            if (string.IsNullOrEmpty(json)) {
                Debug.Log("empty");
                StartCoroutine(ShowErrMsgCoroutine(JSON_EMPTY));
                return;
            }
            
            _faces = JsonHelper.ListFromJson<Face>(json);
            Faces = _faces.AsReadOnly();
            CalcCenterPoints();
            IsWaiting = false;
        }
    }
    
    IEnumerator ShowErrMsgCoroutine(string errMsg)
    {
        _errMsg.text = errMsg;
        _errMsg.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        _errMsg.gameObject.SetActive(false);
    }

    //全矩形の重心を求めてFaceクラスにセットする
    void CalcCenterPoints()
    {
        foreach (var face in _faces) {
            var rect = face.faceRectangle;
            rect.center = new Vector2(rect.left + rect.width / 2, rect.top + rect.height / 2);
        }
    }
}