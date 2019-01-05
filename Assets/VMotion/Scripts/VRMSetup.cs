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
        [SerializeField]
        Salsa3D DummyHead = null;

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

        VRMBlendShapeProxy blendshape = null;
        SkinnedMeshRenderer dummyBlendShape = null;
        IEnumerator Setup(GameObject go)
        {
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            yield return null;
            var lookAtHead = go.GetComponent<VRMLookAtHead>();
            lookAtHead.Target = camera;
            lookAtHead.UpdateType = UpdateType.LateUpdate;
            var skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skin in skins)
            {
                skin.updateWhenOffscreen = true;
            }
            var animator = go.GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            blendshape = go.GetComponent<VRMBlendShapeProxy>();
            blendshape.SetValue(BlendShapePreset.Joy, 0.5f);
            var blink = go.AddComponent<AutoBlinkForVrm>();
            blink.VRM = blendshape;
            blink.blinkParameters.ratioClose = 0.5f;
            blink.blinkParameters.interval = 0.9f;
            var source = DummyHead.GetComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.Play();
            dummyBlendShape = DummyHead.GetComponent<SkinnedMeshRenderer>();
            //salsa.skinnedMeshRenderer = blendshape.BlendShapeAvatar.GetClip(BlendShapePreset.A).Values[0].RelativePath
        }
        [SerializeField]
        BlendShapePreset Small, Medium, Large;
        void FixedUpdate()
        {
            if (blendshape == null)
            {
                return;
            }
            blendshape.SetValue(Small, dummyBlendShape.GetBlendShapeWeight(0) / 100f);
            blendshape.SetValue(Medium, dummyBlendShape.GetBlendShapeWeight(1) / 100f);
            blendshape.SetValue(Large, dummyBlendShape.GetBlendShapeWeight(2) / 100f);
        }
    }
}