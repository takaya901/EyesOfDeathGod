using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity;
using OpenCVForUnityExample;
using UnityEngine;
using UnityEngine.UI;
using static OpenCVForUnity.Core;
using static OpenCVForUnity.CvType;
using static OpenCVForUnity.Imgcodecs;
using static OpenCVForUnity.Imgproc;
using static OpenCVForUnity.Utils;
using Text = UnityEngine.UI.Text;

public class Printer : MonoBehaviour
{
	[SerializeField] Text _text;
	WebCamTextureToMatHelper _toMatHelper;
	WebCamTextureToMatHelperManager _toMatHelperMgr;
	FaceApiManager _apiManager;
	FaceDetector _detector;
	Mat _detected;
	
	void Start()
	{
		Input.backButtonLeavesApp = true;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		_detector = GetComponent<FaceDetector>();
		_apiManager = GetComponent<FaceApiManager>();
		_toMatHelperMgr = GetComponent<WebCamTextureToMatHelperManager>();
		_toMatHelper = GetComponent<WebCamTextureToMatHelper>();
		_toMatHelper.Initialize();
	}
	
	//カメラ映像を処理してQuadに映す
	//タッチされたらFaceAPIに送る
	void Update() 
	{
		if (!_toMatHelperMgr.IsInitialized) return;	//_quadTexが設定されるまで待つ
		if (!_toMatHelper.IsPlaying() || !_toMatHelper.DidUpdateThisFrame()) return;

		_detected = _detector.Detect(_toMatHelper.GetMat());

		//タッチされたら赤くする前のカメラ映像をFaceAPIに送る
		if (TouchManager.GetTouch() == TouchInfo.Began) {
			SendToFaceApi();
		}
		
		//カメラ映像を赤くしてQuadに映す
		_detected = CvtToRed(_detected);
		fastMatToTexture2D(_detected, _toMatHelperMgr.QuadTex);
	}

	//赤くする前のカメラ映像をFaceAPIに送る
	void SendToFaceApi()
	{
		//顔が検出されていなければ送らない
		if (!_detector.IsDetected) {
			StartCoroutine(ShowTextCoroutine());
			return;
		}
		
		//テクスチャをバイト配列にしてFaceAPIに送る
		fastMatToTexture2D(_detected, _toMatHelperMgr.QuadTex);
		var bytes = _toMatHelperMgr.QuadTex.EncodeToJPG();
		_apiManager.GetAge(bytes);
	}

	IEnumerator ShowTextCoroutine()
	{
		_text.gameObject.SetActive(true);
		yield return new WaitForSeconds(2);
		_text.gameObject.SetActive(false);
	}
	
	//G・Bチャンネルを0にする
	static Mat CvtToRed(Mat rgba)
	{
		using (var zeroMat = new Mat(rgba.size(), CV_8UC1, new Scalar(0))) {
			var matList = new List<Mat>();
			split(rgba, matList);
			matList[1] = zeroMat;	//GとBチャンネルを0で置き換える
			matList[2] = zeroMat;
			merge(matList, rgba);
		}
		return rgba;
	}
}