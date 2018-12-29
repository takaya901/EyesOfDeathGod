using System;
using OpenCVForUnity;
using OpenCVForUnityExample;
using UnityEngine;
using static OpenCVForUnity.Imgcodecs;
using static OpenCVForUnity.Imgproc;

public class Printer : MonoBehaviour 
{
//	WebCamTextureToMatHelper _toMatHelper;
//	ToMatHelperManager _toMatHelperManager;
//	FpsMonitor _fpsMonitor;
	WebCamTextureToMatHelper _toMatHelper;
	FaceDetector _detector;
	Mat Img;
	Texture2D _quadTex;
	public byte[] _bytes;
	
	void Awake()
	{
		_toMatHelper = GetComponent<WebCamTextureToMatHelper>();
//		_fpsMonitor = GetComponent<FpsMonitor>();
		_detector = GetComponent<FaceDetector>();
		
//		Img = imread("/Users/takaya/Downloads/c638d75b.jpg");
//		cvtColor(Img, Img, COLOR_BGR2RGB);
//		_quadTex = new Texture2D(Img.cols(), Img.rows(), TextureFormat.RGB24, false);
//		GetComponent<Renderer>().material.mainTexture = _quadTex;
//		Utils.fastMatToTexture2D(Img, _quadTex);
//		_bytes = _quadTex.EncodeToJPG();
	}
	
	void Update () 
	{
//		if (_detector.Face == null) {
//			return;
//		}
//
//		var face = _detector.Face;
//		var pt1 = new Point(face.faceRectangle.left, face.faceRectangle.top);
//		var pt2 = new Point(pt1.x + face.faceRectangle.width, pt1.y + face.faceRectangle.height);
//		
//		rectangle(Img, pt1, pt2, new Scalar(0, 0, 255), 10);
//		Utils.fastMatToTexture2D(Img, _quadTex);
	}
}
