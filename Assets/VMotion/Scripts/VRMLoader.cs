using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PygmyMonkey.FileBrowser;
using UnityEngine;
using UnityEngine.UI;
using VRM;

namespace nkjzm.VMotion
{
    public class VRMLoader : MonoBehaviour
    {
        [SerializeField]
        bool DebugAllAllow = false;
        public void OnOpenFileButtonClicked()
        {
            FileBrowser.OpenFilePanel("Open file Title", Environment.GetFolderPath(Environment.SpecialFolder.Cookies), null, null, (bool canceled, string filePath) =>
            {
                // m_RawImage.gameObject.SetActive(false);
                // m_ContentText.gameObject.SetActive(true);

                if (canceled)
                {
                    // m_ContentText.text = "[Open File]\nCanceled";
                    Debug.Log("きゃんせる");
                    return;
                }

                Debug.Log("選択: " + filePath);
                ImportVRMAsync_Net4(filePath, true);
                // m_ContentText.text = "[Open File]\n<b>Selected file</b>: " + filePath;
            });
        }

        [SerializeField]
        Transform ListParent = null;
        [SerializeField]
        VRMListItem VrmItemPrafab = null;

        [SerializeField]
        Button LoadFromFileButton = null;
        [SerializeField]
        Button StartButton = null;

        [SerializeField]
        string SceneName = string.Empty;
        void LoadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        }

        void Start()
        {
            LoadFromFileButton.onClick.AddListener(OnOpenFileButtonClicked);
            StartButton.onClick.AddListener(LoadScene);

            var path = Application.streamingAssetsPath;
            var files = Directory.GetFiles(Application.streamingAssetsPath, "*.vrm", System.IO.SearchOption.AllDirectories);
            foreach (var vrmFile in files)
            {
                ImportVRMAsync_Net4(vrmFile);
            }
        }

        void Update()
        {
            StartButton.interactable = !string.IsNullOrEmpty(GameManager.Instance.LoadVrmPath);
        }


        async Task ImportVRMAsync_Net4(string filePath, bool isSelect = false)
        {
            var meta = await VRMMetaImporter.ImportVRMMeta(filePath, true);

            Debug.LogFormat("meta: title:{0}", meta.Title);
            Debug.LogFormat("meta: meta.SexualUssage:{0}", meta.SexualUssage);
            var item = Instantiate(VrmItemPrafab, ListParent);
            item.Init(meta, filePath);
            if (isSelect)
            {
                SelectItem(meta, filePath);
            }
        }

        public void SelectItem(VRMMetaObject meta, string path)
        {
            foreach (Transform child in BasicInfoListParent)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in LicenseInfoListParent)
            {
                Destroy(child.gameObject);
            }
            ThumbnailImage.texture = meta.Thumbnail;
            var allowedUser = !meta.AllowedUser.Equals(AllowedUser.OnlyAuthor);
            //var violensUssage = meta.ViolentUssage.Equals(UssageLicense.Allow);
            var sexualUssage = meta.SexualUssage.Equals(UssageLicense.Allow);
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("タイトル", meta.Title, 2);
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("バージョン", meta.Version, 2);
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("作者", meta.Author, 2);
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("連絡先", meta.ContactInformation, 2);
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("親作品", meta.Reference, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("アバター利用", meta.AllowedUser.ToString(), allowedUser ? 0 : 1);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("暴力表現", meta.ViolentUssage.ToString(), 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("性的表現", meta.SexualUssage.ToString(), sexualUssage ? 0 : 1);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("商用利用", meta.CommercialUssage.ToString(), 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("その他制限", meta.OtherPermissionUrl.ToString(), 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("ライセンス", meta.LicenseType.ToString(), 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("その他ライセンス", meta.OtherLicenseUrl.ToString(), 2);
            GameManager.Instance.LoadVrmPath = (DebugAllAllow || (allowedUser && sexualUssage)) ? path : string.Empty;
        }
        [SerializeField]
        RawImage ThumbnailImage = null;
        [SerializeField]
        VRMInfoListItem InfoListItemsPrefab = null;
        [SerializeField]
        Transform BasicInfoListParent = null, LicenseInfoListParent = null;

        void aaa()
        {
            // OnOpenFileButtonClicked();

            // if (string.IsNullOrEmpty(path))
            // {
            //     return;
            // }

            // // Byte列を得る
            // var bytes = File.ReadAllBytes(path);

            // var context = new VRMImporterContext();

            // // GLB形式をParseしてチャンクからJSONを取得しParseします
            // context.ParseGlb(bytes);

            // // metaを取得
            // var meta = context.ReadMeta(true);
            // Debug.LogFormat("meta: title:{0}", meta.Title);
            // Debug.LogFormat("meta: meta.SexualUssage:{0}", meta.SexualUssage);

            // return;
            // // 非同期に実行する
            // var now = Time.time;
            // VRMImporter.LoadVrmAsync(context, go =>
            // {
            //     var delta = Time.time - now;
            //     Debug.LogFormat("LoadVrmAsync {0:0.0} seconds", delta);
            //     OnLoaded(go);
            // });
        }

        void OnLoaded(GameObject goa)
        {

        }
    }
}