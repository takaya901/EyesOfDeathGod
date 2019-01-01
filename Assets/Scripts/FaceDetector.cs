using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using OpenCVForUnityExample;
using static OpenCVForUnity.Core;
using static OpenCVForUnity.Imgproc;
using static OpenCVForUnity.Utils;
using Rect = OpenCVForUnity.Rect;

public class FaceDetector : MonoBehaviour
{
	FaceApiManager _apiManager;
    CascadeClassifier _cascade = new CascadeClassifier();
    public MatOfRect _faces = new MatOfRect();

    static readonly Scalar COLOR = new Scalar(255, 255, 255);
    const int FONT_SCALE = 3;
    const int THICKNESS = 5;
    
	void Start ()
	{
		_apiManager = GetComponent<FaceApiManager>();
        _cascade.load(getFilePath("haarcascade_frontalface_alt.xml"));
    }

	//カメラ映像から顔を検出する
	public Mat Detect(Mat webcamMat)
	{
		using(var gray = new Mat()) 
		{
			cvtColor(webcamMat, gray, COLOR_RGBA2GRAY);
			equalizeHist(gray, gray);
			_cascade.detectMultiScale(gray, _faces, 1.1, 2, 2, new Size(gray.cols() * 0.2, gray.rows() * 0.2), new Size());
			
			var rects = _faces.toArray();
			foreach (var face in rects) {
				rectangle(webcamMat, 
					new Point(face.x, face.y), 
					new Point(face.x + face.width, face.y + face.height), 
					new Scalar(255, 0, 0, 255), 2);
			}

			//頭の上に年齢を表示
			if (_apiManager.Face != null && rects.Length > 0) {
				webcamMat = PutAgeOnHead(webcamMat, _apiManager.Face, rects);
			}
			return webcamMat;
		}
	}

	//頭の上に年齢を表示する
	static Mat PutAgeOnHead(Mat webcamMat, Face face, IReadOnlyList<Rect> rects)
	{
		var age = face.faceAttributes.age.ToString("d");
		var textSize = getTextSize(age, FONT_HERSHEY_SCRIPT_COMPLEX, FONT_SCALE, THICKNESS, null);
		var xOrg = rects[0].x + rects[0].width / 2 - textSize.width / 2;	//年齢が頭上の中心に出るよう調整
		var org = new Point(xOrg, rects[0].y);
		
		putText(webcamMat, age, org, FONT_HERSHEY_SCRIPT_COMPLEX, FONT_SCALE, COLOR, THICKNESS, Imgproc.LINE_AA);
		return webcamMat;
	}
}