﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenCVForUnity;
using OpenCVForUnityExample;
using UnityEngine;
using UnityEngine.EventSystems;
using static OpenCVForUnity.Core;
using static OpenCVForUnity.CvType;
using static OpenCVForUnity.Utils;
using static TouchManager;
using Text = UnityEngine.UI.Text;
using UnityEngine.Rendering.PostProcessing;
using static UnityEngine.Mathf;

public class Printer : MonoBehaviour
{
	[SerializeField] Text _errMsg;
	[SerializeField] Text _debugText;
	[SerializeField] PostProcessVolume _volume;

	LensDistortion _distortion;
	WebCamTextureToMatHelper _toMatHelper;
	WebCamTextureToMatHelperManager _toMatHelperMgr;
	FaceApiManager _apiManager;
	FaceDetector _detector;
	Mat _detected;
	ZeroMat _zeroMat;
	const string NO_FACE_MSG = "No Face";
	float _current;
	bool _touched;
	
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
		
		_volume.profile.TryGetSettings(out _distortion);
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

		//ボタン以外がタッチされたら赤くする前のカメラ映像をFaceAPIに送る
		if (GetTouch() == TouchInfo.Began && !IsButtonTouched()) {
			SendToFaceApi();
		}

		if (_touched) {
			_distortion.intensity.value = SmoothDamp(_distortion.intensity.value, 100f, ref _current, 0.2f);
			_touched = !Approximately(_distortion.intensity.value, 100f);
		}
		if (!_apiManager.IsWaiting) {
			_distortion.intensity.value = SmoothDamp(_distortion.intensity.value, 0f, ref _current, 0.2f);
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
			_touched = false;
			StartCoroutine(ShowErrMsgCoroutine());
			return;
		}
		
		_touched = true;
		//テクスチャをバイト配列にしてFaceAPIに送る
		fastMatToTexture2D(_detected, _toMatHelperMgr.QuadTex);
		var bytes = _toMatHelperMgr.QuadTex.EncodeToJPG();
		_apiManager.GetAge(bytes);
	}

	//ボタンがタッチされたらFaceAPIに送らない
	static bool IsButtonTouched()
	{
		var pointer = new PointerEventData(EventSystem.current) { position = GetTouchPosition() };
		var result = new List<RaycastResult> ();
		EventSystem.current.RaycastAll(pointer, result);
		return result.Any();
	}
	
	IEnumerator ShowErrMsgCoroutine()
	{
		_errMsg.text = NO_FACE_MSG;
		_errMsg.gameObject.SetActive(true);
		yield return new WaitForSeconds(2);
		_errMsg.gameObject.SetActive(false);
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