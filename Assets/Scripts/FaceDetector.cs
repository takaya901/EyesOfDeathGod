using System.Linq;
using UnityEngine;
using OpenCVForUnity;
using static OpenCVForUnity.Core;
using static OpenCVForUnity.Imgproc;
using static OpenCVForUnity.Utils;
using Rect = OpenCVForUnity.Rect;
using Text = UnityEngine.UI.Text;

public class FaceDetector : MonoBehaviour
{
	[SerializeField] Text _text;
	FaceApiManager _apiManager;
    CascadeClassifier _cascade = new CascadeClassifier();
    MatOfRect _faces = new MatOfRect();
    
    /// <summary>顔を検出したかどうか</summary>
    public bool IsDetected => !_faces.empty();

    static readonly Scalar COLOR = new Scalar(255, 255, 255);
    const int FONT_SCALE = 3;
    const int THICKNESS = 5;
    
	void Start ()
	{
		_apiManager = GetComponent<FaceApiManager>();
        _cascade.load(getFilePath("haarcascade_frontalface_alt.xml"));
    }

	//カメラ映像から顔を検出し，その上に年齢を表示する
	public Mat Detect(Mat webcamMat)
	{
		using (var gray = new Mat()) {
			cvtColor(webcamMat, gray, COLOR_RGBA2GRAY);
			equalizeHist(gray, gray);
			var minFaceSize = new Size(gray.cols() * 0.2, gray.rows() * 0.2);
			_cascade.detectMultiScale(gray, _faces, 1.1, 2, 2, minFaceSize, new Size());
		}

		var rects = _faces.toArray();
		_text.text = rects.Length + " faces";
		
		foreach (var rect in rects) {
//			rectangle(webcamMat, 
//				new Point(rect.x, rect.y), 
//				new Point(rect.x + rect.width, rect.y + rect.height), 
//				new Scalar(255, 0, 0, 255), 2);

			if (!(_apiManager.Faces?.Count > 0)) return webcamMat;
			
			//APIの検出矩形の中から一番近い物を探す
			var center = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
			var age = _apiManager.Faces.OrderBy(face => (center - face.faceRectangle.center).SqrMagnitude()).First().faceAttributes.age;
			webcamMat = PutAgeOnHead(webcamMat, rect, age);
		}
		
		return webcamMat;
	}

	//頭の上に年齢を表示する
	static Mat PutAgeOnHead(Mat webcamMat, Rect rect, int age)
	{
		var ageText = age.ToString();
		var textSize = getTextSize(ageText, FONT_HERSHEY_SCRIPT_COMPLEX, FONT_SCALE, THICKNESS, null);
		var xOrg = rect.x + rect.width / 2 - textSize.width / 2;	//年齢が頭上の中心に出るようx座標を調整
		var org = new Point(xOrg, rect.y);
		
		putText(webcamMat, ageText, org, FONT_HERSHEY_SCRIPT_COMPLEX, FONT_SCALE, COLOR, THICKNESS, Imgproc.LINE_AA);
		return webcamMat;
	}
}