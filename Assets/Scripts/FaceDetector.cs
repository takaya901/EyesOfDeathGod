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
	[SerializeField] Text _numFace;
	[SerializeField] Transform _ageCanvas;
	[SerializeField] Text _ageText;
	RectTransform _rt;

	[SerializeField] GameObject _sphere;
	
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
		_numFace.text = rects.Length + " faces";
		var a = GameObject.FindGameObjectsWithTag("Age");
		foreach (var age in a) {
			Destroy(age);
		}
		
		foreach (var rect in rects) {
			if (!(_apiManager.Faces?.Count > 0)) return webcamMat;
			
			//APIの検出矩形の中から一番近い物を探す
			var center = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
			var age = _apiManager.Faces.OrderBy(face => (center - face.faceRectangle.center).SqrMagnitude()).First().faceAttributes.age;
			PutAgeOnHead(rect, age, new Vector2(webcamMat.width(), webcamMat.height()));
		}
		
		return webcamMat;
	}

	//頭の上に年齢を表示する
	void PutAgeOnHead(Rect rect, int age, Vector2 size)
	{
		_ageText.text = age.ToString();
		var pos = new Vector3(rect.x - 320 + rect.width / 2f, 240 - rect.y, 0f);
		_sphere.transform.position = new Vector3(rect.x - 320 + rect.width / 2f, 240 - rect.y, -4);
//		_sphere.transform.Translate(rect.x - 320 + rect.width / 2f, 240 - rect.y, -4);
//		var pos = Vector3.zero;
//		Instantiate(_ageText, pos, Quaternion.identity, _ageCanvas);
		var text = Instantiate(_ageText, pos, Quaternion.identity);
		text.transform.SetParent(_ageCanvas, false);
		text.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
	}
}