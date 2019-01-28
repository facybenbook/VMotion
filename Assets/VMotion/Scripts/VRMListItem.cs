using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRoidSDK;

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
        CharacterModel model;

        public void Init(VRM.VRMMetaObject meta, string path)
        {
            this.meta = meta;
            this.path = path;
            Title.text = meta.Title;
            Version.text = meta.Version;
            ExVersion.text = meta.ExporterVersion;
            Thumbnail.texture = meta.Thumbnail;
        }
        public void Init(CharacterModel model)
        {
            this.model = model;
            Title.text = model.character.name + (string.IsNullOrEmpty(model.name) ? "" : "/" + model.name);
            Version.text = string.Empty;//model.latest_character_model_version.id;
            ExVersion.text = string.Empty;
            StartCoroutine(Utils.ImageLoader.Load(model.portrait_image.sq150.url, Thumbnail));
        }

        [SerializeField]
        Button Button = null;
        void Start()
        {
            Button.onClick.AddListener(() =>
            {
                if (meta != null)
                {
                    FindObjectOfType<VRMLoader>().SelectItem(meta, path);
                }
                else
                {
                    FindObjectOfType<VRMLoader>().SelectItem(model);
                }
            });
        }
    }
}