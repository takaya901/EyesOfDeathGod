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
	[SerializeField] Text _noFaceText;
	WebCamTextureToMatHelper _toMatHelper;
	WebCamTextureToMatHelperManager _toMatHelperMgr;
	FaceApiManager _apiManager;
	FaceDetector _detector;
	Mat _detected;
	ZeroMat _zeroMat;
	
	IEnumerator Start()
	{
		Input.backButtonLeavesApp = true;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		
		_detector = GetComponent<FaceDetector>();
		_apiManager = GetComponent<FaceApiManager>();
		_toMatHelperMgr = GetComponent<WebCamTextureToMatHelperManager>();
		_toMatHelper = GetComponent<WebCamTextureToMatHelper>();
		
		//カメラ等の初期化完了後，画像サイズを取得する
		_toMatHelper.Initialize();
		yield return WaitInitialization(); 
		var imgSize = new Size(_toMatHelper.GetWidth(), _toMatHelper.GetHeight());
		_zeroMat = new ZeroMat(imgSize);
	}
	
	IEnumerator WaitInitialization()
	{
		while (!_toMatHelperMgr.IsInitialized) {
			yield return null;
		}
	}
	
	//カメラ映像を赤くしてQuadに映す
	//タッチされたらFaceAPIに送る
	void Update() 
	{
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
		_noFaceText.gameObject.SetActive(true);
		yield return new WaitForSeconds(2);
		_noFaceText.gameObject.SetActive(false);
	}
	
	//G・Bチャンネルを0にする
	Mat CvtToRed(Mat rgba)
	{
		var matList = new List<Mat>();
		split(rgba, matList);
		matList[1] = _zeroMat.Instance;	//GとBチャンネルを0で置き換える
		matList[2] = _zeroMat.Instance;
		merge(matList, rgba);
		return rgba;
	}
	
	/// <summary>start()内でstatic readonlyの初期化ができないためこのクラスのプロパティとして設定</summary>
	class ZeroMat
	{
		public Mat Instance { get; }
		
		public ZeroMat(Size imgSize)
		{
			Instance = new Mat(imgSize, CV_8UC1, new Scalar(0));
		}
	}
}