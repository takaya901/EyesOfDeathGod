using System;
using OpenCVForUnity;
using OpenCVForUnityExample;
using UnityEngine;
using static OpenCVForUnity.Imgcodecs;
using static OpenCVForUnity.Imgproc;
using static OpenCVForUnity.Utils;

public class Printer : MonoBehaviour 
{
	WebCamTextureToMatHelper _toMatHelper;
	ToMatHelperManager _toMatHelperMgr;
	FaceApiManager _apiManager;
	FaceDetector _detector;
	Mat _webcamMat, _detected;
	Texture2D _quadTex;
	public byte[] _bytes;
	bool a;
	
	void Start()
	{
		_detector = GetComponent<FaceDetector>();
		_apiManager = GetComponent<FaceApiManager>();
		_toMatHelperMgr = GetComponent<ToMatHelperManager>();
		_toMatHelper = GetComponent<WebCamTextureToMatHelper>();
		_toMatHelper.Initialize();
	}
	
	void Update() 
	{
		if (!_toMatHelperMgr.IsInitialized) return;	//_quadTexが設定されるまで待つ
		if (!_toMatHelper.IsPlaying() || !_toMatHelper.DidUpdateThisFrame()) return;

		_webcamMat = _toMatHelper.GetMat();
		_detected = _detector.Detect(_webcamMat);
        
		fastMatToTexture2D(_detected, _toMatHelperMgr.QuadTex);
		if (_detector._faces.toArray().Length == 0 && !a) {
			return;
		}

		if (a) return;
		a = true;
		var bytes = _toMatHelperMgr.QuadTex.EncodeToJPG();
		_apiManager.GetAge(bytes);
	}
}
