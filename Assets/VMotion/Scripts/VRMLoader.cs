using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PygmyMonkey.FileBrowser;
using UnityEngine;
using UnityEngine.UI;
using VRM;
using VRoidSDK;

namespace nkjzm.VMotion
{
    public class VRMLoader : MonoBehaviour
    {
        [SerializeField]
        bool DebugAllAllow = false;
        [SerializeField]
        Text VersionLabel = null;
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
            VersionLabel.text = Application.version;
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

        public void LoadFromVRoidHub()
        {
            HubApi.GetAccountCharacterModels(
                count: PagerField.PerPageCount,
                onSuccess: (List<CharacterModel> characterModels) =>
                {
                    for (int i = 0; i < characterModels.Count; ++i)
                    {
                        var item = Instantiate(VrmItemPrafab, ListParent);
                        item.Init(characterModels[i]);
                    }
                },
                onError: (error) =>
                {
                    Debug.LogError(error.message);
                }
            );
            HubApi.GetHearts(
                count: PagerField.PerPageCount,
                onSuccess: (List<CharacterModel> characterModels) =>
                {
                    Debug.Log(characterModels.Count);
                    for (int i = 0; i < characterModels.Count; ++i)
                    {
                        var item = Instantiate(VrmItemPrafab, ListParent);
                        item.Init(characterModels[i]);
                    }
                },
                onError: (error) =>
                {
                    Debug.LogError(error.message);
                }
            );
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
            // meta.OtherLicenseUrl = null;
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
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("性的表現", meta.SexualUssage.ToString(), sexualUssage ? 0 : 1);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("暴力表現", meta.ViolentUssage.ToString(), 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("商用利用", meta.CommercialUssage.ToString(), 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("その他制限", meta.OtherPermissionUrl, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("ライセンス", meta.LicenseType.ToString(), 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("その他ライセンス", meta.OtherLicenseUrl, 2);
            GameManager.Instance.LoadVrmPath = (DebugAllAllow || (allowedUser && sexualUssage)) ? path : string.Empty;
            GameManager.Instance.FromVRoidHub = false;
        }
        public void SelectItem(CharacterModel model)
        {
            foreach (Transform child in BasicInfoListParent)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in LicenseInfoListParent)
            {
                Destroy(child.gameObject);
            }
            // meta.OtherLicenseUrl = null;
            StartCoroutine(Utils.ImageLoader.Load(model.portrait_image.sq300.url, ThumbnailImage));

            var allowedUser = true;
            var sexualUssage = model.license.sexual_expression.Equals("allow");
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("名前", model.character.name, 2);
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("モデル", model.name, 2);
            Instantiate(InfoListItemsPrefab, BasicInfoListParent).Init("作者", model.character.user.name, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("改変", model.license.modification, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("再配布", model.license.redistribution, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("アバター利用", model.license.characterization_allowed_user.ToString(), allowedUser ? 0 : 1);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("性的表現", model.license.sexual_expression, sexualUssage ? 0 : 1);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("暴力表現", model.license.violent_expression, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("商用利用", model.license.sexual_expression, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("法人の商用利用", model.license.corporate_commercial_use, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("営利目的での活動", model.license.personal_commercial_use, 2);
            Instantiate(InfoListItemsPrefab, LicenseInfoListParent).Init("クレジット表記", model.license.credit, 2);
            GameManager.Instance.LoadVrmPath = (DebugAllAllow || (allowedUser && sexualUssage)) ? model.id : string.Empty;
            GameManager.Instance.FromVRoidHub = true;
        }

        [SerializeField]
        RawImage ThumbnailImage = null;
        [SerializeField]
        VRMInfoListItem InfoListItemsPrefab = null;
        [SerializeField]
        Transform BasicInfoListParent = null, LicenseInfoListParent = null;
    }
}