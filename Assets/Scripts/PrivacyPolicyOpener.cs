using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyOpener : MonoBehaviour
{
    const string URL = "https://docs.google.com/document/d/1cReJ7kNsQw_0SOyaqHgDtK-LPBiYFcyu6tynPEu3Lns/edit?usp=sharing";

    public void OnPrivacyPolicySelected()
    {
        Application.OpenURL(URL);
    }
}
