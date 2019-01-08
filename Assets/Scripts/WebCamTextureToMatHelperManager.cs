using OpenCVForUnityExample;
using UnityEngine;

/// <summary>
/// WebCamTextureToMatHelperの初期化・破棄時イベントを実行する
/// </summary>
public class WebCamTextureToMatHelperManager : MonoBehaviour
{
    public Texture2D QuadTex { get; private set; }
    public bool IsInitialized { get; private set; }

    WebCamTextureToMatHelper _toMatHelper;
    FpsMonitor _fpsMonitor;

    void Start()
    {
        _toMatHelper = GetComponent<WebCamTextureToMatHelper>();
        _fpsMonitor = GetComponent<FpsMonitor>();
    }

    public void OnWebCamTextureToMatHelperInitialized()
    {
        Debug.Log ("OnWebCamTextureToMatHelperInitialized");

        var webCamTextureMat = _toMatHelper.GetMat();
        QuadTex = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = QuadTex;

        Debug.Log ("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

        if (_fpsMonitor != null){
            _fpsMonitor.Add ("width", webCamTextureMat.width().ToString());
            _fpsMonitor.Add ("height", webCamTextureMat.height().ToString());
            _fpsMonitor.Add ("orientation", Screen.orientation.ToString());
        }
        
        float width = webCamTextureMat.width();
        float height = webCamTextureMat.height();
                                
        float widthScale = Screen.width / width;
        float heightScale = Screen.height / height;
        if (widthScale < heightScale) {
            Camera.main.orthographicSize = (width * Screen.height / Screen.width) / 2;
        } else {
            Camera.main.orthographicSize = height / 2;
        }
        
        //Quadを画面いっぱいにリサイズ
        //https: //blog.narumium.net/2016/12/11/unityでスマホカメラを全面表示する/
        var quadHeight = Camera.main.orthographicSize * 2;
        var quadWidth = quadHeight * Camera.main.aspect;
        gameObject.transform.localScale = new Vector3(quadWidth, quadHeight, 1);
        IsInitialized = true;
    }

    public void OnWebCamTextureToMatHelperDisposed()
    {
        Debug.Log("OnWebCamTextureToMatHelperDisposed");
        if (QuadTex != null) {
            Destroy(QuadTex);
            QuadTex = null;
        }
    }
    
    public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode){
        Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
    }
    
    void OnDestroy()
    {
        _toMatHelper.Dispose();
    }
}
