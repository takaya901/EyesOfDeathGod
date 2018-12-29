﻿using System;

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
	public int top;
	public int left;
	public int width;
	public int height;
}

[Serializable]
public class FaceAttributes
{
	public float age;
}
