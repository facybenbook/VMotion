using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nkjzm.VMotion
{
    public class VRMInfoListItem : MonoBehaviour
    {
        [SerializeField]
        Text Label = null;
        [SerializeField]
        RegexHypertext Title = null;
        [SerializeField]
        Image Mark = null;
        [SerializeField]
        Sprite[] Sprites = null;
        const string RegexURL = "http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?";
        [SerializeField]
        Color green, red;
        public void Init(string label, string title, int status)
        {
            Label.text = label;
            Title.text = title;
            Title.SetClickableByRegex(RegexURL, Color.cyan, url => Application.OpenURL(url));
            Mark.sprite = status == 0 ? Sprites[0] : Sprites[1];
            Mark.color = status == 0 ? green : red;
            Mark.enabled = status != 2;
        }
    }
}