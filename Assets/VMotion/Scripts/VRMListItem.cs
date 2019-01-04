using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRMListItem : MonoBehaviour
{
    [SerializeField]
    Text Title = null, Version = null, ExVersion = null;
    [SerializeField]
    RawImage Thumbnail = null;
    public void Init(string title, string version, string exVersion, Texture2D tex)
    {
        Title.text = title;
        Version.text = version;
        ExVersion.text = exVersion;
        Thumbnail.texture = tex;
    }
}
