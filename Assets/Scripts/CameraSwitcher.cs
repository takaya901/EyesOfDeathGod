using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnityExample;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    WebCamTextureToMatHelper _toMatHelper;
    const string USE_CAMERA_KEY = "USE CAMERA"; //PlayerPrefabsのKey（リア/フロント）

    void Start()
    {
        _toMatHelper = GetComponent<WebCamTextureToMatHelper>();
    }

    //リアカメラとフロントカメラを切り替える
    public void OnCameraSwitch()
    {
        //PlayerPrefsの使用カメラを書き換える
        PlayerPrefs.SetInt(USE_CAMERA_KEY, Convert.ToInt32(!_toMatHelper.requestedIsFrontFacing));
        _toMatHelper.requestedIsFrontFacing = !_toMatHelper.requestedIsFrontFacing;
    }
}
