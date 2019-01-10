using System;
using UnityEngine;

[Serializable]
public class Face
{
	public string faceId;
	public FaceRectangle faceRectangle;
	public FaceAttributes faceAttributes;
}

[Serializable]
public class FaceRectangle
{
	public Vector2 center;	//Pointだと2点間の距離を取るメソッドがないため
	public int top;
	public int left;
	public int width;
	public int height;
}

[Serializable]
public class FaceAttributes
{
	public int age;
}
