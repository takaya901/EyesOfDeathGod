using System;
using System.Collections.Generic;
using OpenCVForUnity;
using OpenCVForUnityExample;
using UnityEngine;
using static OpenCVForUnity.Core;
using static OpenCVForUnity.CvType;
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
		Input.backButtonLeavesApp = true;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
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

		_detected = GetRedMat(_detected);
        
		fastMatToTexture2D(_detected, _toMatHelperMgr.QuadTex);
		if (_detector._faces.toArray().Length == 0 && !a) {
			return;
		}

		if (a) return;
		a = true;
		var bytes = _toMatHelperMgr.QuadTex.EncodeToJPG();
		_apiManager.GetAge(bytes);
	}

	//Rチャンネル以外0にする
	static Mat GetRedMat(Mat rgba)
	{
		using (var zeroMat = new Mat(rgba.size(), CV_8UC1, new Scalar(0))) {
			var matList = new List<Mat>();
			split(rgba, matList);
			matList[1] = zeroMat;	//GとBチャンネルを0で置き換える
			matList[2] = zeroMat;
			merge(matList, rgba);
		}
//		matList.GetEnumerator().Dispose();
		return rgba;
	}
}