using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PygmyMonkey.FileBrowser;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class VRMLoader : MonoBehaviour
{
    public void OnOpenFileButtonClicked()
    {
        FileBrowser.OpenFilePanel("Open file Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), null, null, (bool canceled, string filePath) =>
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
            // m_ContentText.text = "[Open File]\n<b>Selected file</b>: " + filePath;
        });
    }

    [SerializeField]
    Transform ListParent = null;
    [SerializeField]
    VRMListItem VrmItemPrafab = null;

    [SerializeField]
    Button LoadFromFileButton = null;

    void Start()
    {
        LoadFromFileButton.onClick.AddListener(OnOpenFileButtonClicked);

        var path = Application.streamingAssetsPath;
        var files = Directory.GetFiles(Application.streamingAssetsPath, "*.vrm", System.IO.SearchOption.AllDirectories);
        foreach (var vrmFile in files)
        {
            Debug.Log(vrmFile);

            // Byte列を得る
            var bytes = File.ReadAllBytes(vrmFile);
            var context = new VRMImporterContext();
            // GLB形式をParseしてチャンクからJSONを取得しParseします
            context.ParseGlb(bytes);
            // metaを取得
            var meta = context.ReadMeta(true);
            Debug.LogFormat("meta: title:{0}", meta.Title);
            Debug.LogFormat("meta: meta.SexualUssage:{0}", meta.SexualUssage);
            var item = Instantiate(VrmItemPrafab, ListParent);
            item.Init(meta.Title, meta.Version, meta.ExporterVersion, meta.Thumbnail);
        }

        return;
        OnOpenFileButtonClicked();

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

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

    // Update is called once per frame
    void Update()
    {

    }
}
