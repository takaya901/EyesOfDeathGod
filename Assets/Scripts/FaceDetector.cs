using UnityEngine;
using OpenCVForUnity;
using OpenCVForUnityExample;

public class FaceDetector : MonoBehaviour
{
	WebCamTextureToMatHelper _toMatHelper;
    CascadeClassifier _cascade;
//    Mat _webcamMat;
    MatOfRect _faces = new MatOfRect();
    Texture _quadTex;
    
	void Start ()
	{
		_toMatHelper = GetComponent<WebCamTextureToMatHelper>();
		_quadTex = GetComponent<Renderer>().material.mainTexture;
        _cascade = new CascadeClassifier();
        _cascade.load(Utils.getFilePath("lbpcascade_frontalface.xml"));
    }

	void Update()
	{
	}

	public Mat Detect(Mat webcamMat)
	{
		using (var gray = new Mat()) 
		{
//			_webcamMat = _toMatHelper.GetMat();
			Imgproc.cvtColor (webcamMat, gray, Imgproc.COLOR_RGBA2GRAY);
			Imgproc.equalizeHist (gray, gray);
			
			_cascade.detectMultiScale(gray, _faces, 1.1, 2, 2, new Size(gray.cols() * 0.2, gray.rows() * 0.2), new Size());
			
			var rects = _faces.toArray();
			for (int i = 0; i < rects.Length; i++) {
				Imgproc.rectangle(webcamMat, 
					new Point(rects [i].x, rects [i].y), 
					new Point (rects [i].x + rects [i].width, rects [i].y + rects [i].height), 
					new Scalar (255, 0, 0, 255), 2);
			}
                
			//Imgproc.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
			return webcamMat;
//			Utils.fastMatToTexture2D(_webcamMat, _quadTex);
		}
	}
}