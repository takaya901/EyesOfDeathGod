using OpenCVForUnity;
using OpenCVForUnityExample;
using UnityEngine;

/// <summary>
/// WebCamTextureToMatHelperの初期化・破棄を行う．
/// WebCamManagerに書くと長いので分けた
/// </summary>
public class ToMatHelperManager : MonoBehaviour
{
    Texture2D _quadTex;
    WebCamTextureToMatHelper _toMatHelper;
    FpsMonitor _fpsMonitor;
    FaceDetector _detector;
    Mat _webcamMat, _detected;

    void Start()
    {
        _toMatHelper = GetComponent<WebCamTextureToMatHelper>();
        _fpsMonitor = GetComponent<FpsMonitor>();
        _detector = GetComponent<FaceDetector>();
        _toMatHelper.Initialize();
    }

    void Update()
    {
        if (!_toMatHelper.IsPlaying() || !_toMatHelper.DidUpdateThisFrame()) return;

        _webcamMat = _toMatHelper.GetMat();
        _detected = _detector.Detect(_webcamMat);
        Utils.fastMatToTexture2D(_detected, _quadTex);
    }

    public void OnWebCamTextureToMatHelperInitialized()
    {
        Debug.Log ("OnWebCamTextureToMatHelperInitialized");

        var webCamTextureMat = _toMatHelper.GetMat();
        _quadTex = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = _quadTex;

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
    }

    public void OnWebCamTextureToMatHelperDisposed()
    {
        Debug.Log ("OnWebCamTextureToMatHelperDisposed");
        if (_quadTex != null) {
            Destroy(_quadTex);
            _quadTex = null;
        }
    }
    
    public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode){
        Debug.Log ("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
    }
    
    void OnDestroy()
    {
        _toMatHelper.Dispose();
    }
}
