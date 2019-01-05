using System.Collections;
using System.Collections.Generic;
using System.IO;
using CrazyMinnow.SALSA;
using UnityEngine;
using VRM;

namespace nkjzm.VMotion
{
    public class VRMSetup : MonoBehaviour
    {
        [SerializeField]
        RuntimeAnimatorController animatorController = null;
        [SerializeField]
        AudioClip clip = null;
        [SerializeField]
        Transform camera = null;
        void Start()
        {
            var path = GameManager.Instance.LoadVrmPath;

            // Byte列を得る
            var bytes = File.ReadAllBytes(path);

            var context = new VRMImporterContext();
            // GLB形式をParseしてチャンクからJSONを取得しParseします
            context.ParseGlb(bytes);

            // 非同期に実行する
            VRMImporter.LoadVrmAsync(context, go =>
            {
                StartCoroutine(Setup(go));
            });
        }

        IEnumerator Setup(GameObject go)
        {
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            yield return null;
            var lookAtHead = go.GetComponent<VRMLookAtHead>();
            lookAtHead.Target = camera;
            lookAtHead.UpdateType = UpdateType.LateUpdate;
            var animator = go.GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            var blink = go.AddComponent<AutoBlinkForVrm>();
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            // var salsa = go.AddComponent<Salsa3D>();
            // salsa.skinnedMeshRenderer = go.GetComponentInChildren<blend
        }
    }
}