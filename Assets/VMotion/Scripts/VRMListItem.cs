using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nkjzm.VMotion
{
    public class VRMListItem : MonoBehaviour
    {
        [SerializeField]
        Text Title = null, Version = null, ExVersion = null;
        [SerializeField]
        RawImage Thumbnail = null;
        VRM.VRMMetaObject meta = null;
        string path = string.Empty;
        public void Init(VRM.VRMMetaObject meta, string path)
        {
            this.meta = meta;
            this.path = path;
            Title.text = meta.Title;
            Version.text = meta.Version;
            ExVersion.text = meta.ExporterVersion;
            Thumbnail.texture = meta.Thumbnail;
        }

        [SerializeField]
        Button Button = null;
        void Start()
        {
            Button.onClick.AddListener(() => FindObjectOfType<VRMLoader>().SelectItem(meta, path));
        }
    }
}