/// <remarks>
/// The script is from https://gist.github.com/toiro/f449d1759ecd94b04f00140d47a6b338
/// </remarks>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRM;

public class AutoBlinkForVrm : MonoBehaviour
{

    [Tooltip("瞬きさせるモデル")]
    public VRMBlendShapeProxy VRM;

    [Tooltip("瞬きさせるかどうか")]
    public bool IsActive = true;

    [Tooltip("瞬きの強さ（表情の目の開き具合に合わせる）")]
    [Range(0, 2.0f)]
    public float ModulateRatio = 1.0f;

    public BlinkParameterSet blinkParameters = new BlinkParameterSet();

    public bool IsBlinking { get { return player != null && !player.IsFinished; } }

    private TransitionPlayer player;

    void Start()
    {
        StartCoroutine(BlinkSignaler());
    }

    void Update()
    {
        if (IsBlinking)
        {
            VRM.SetValue(BlendShapePreset.Blink, player.Next(Time.deltaTime));
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator BlinkSignaler()
    {
        while (true)
        {
            if (IsActive && !IsBlinking)
            {
                // randomThreshold の確率で瞬きしない
                float _seed = UnityEngine.Random.Range(0.0f, 1.0f);
                if (_seed > blinkParameters.randomThreshold)
                {
                    Blink();
                }
            }
            // interval だけ待つ
            yield return new WaitForSeconds(blinkParameters.interval);
        }
    }

    private void Blink()
    {
        player = new TransitionPlayer(CreateBlinkTransition(), VRM.GetValue(BlendShapePreset.Blink));
    }

    private AutoBlinkForVrm.Transition CreateBlinkTransition()
    {
        var closePartDuration = blinkParameters.closeDuration / 2;
        var openPartDuration = blinkParameters.openDuration / 2;
        return new AutoBlinkForVrm.Transition()
            .AddKey(blinkParameters.ratioHalf * ModulateRatio, closePartDuration)
            .AddKey(blinkParameters.ratioClose * ModulateRatio, closePartDuration)
            .AddKey(blinkParameters.ratioHalf * ModulateRatio, openPartDuration)
            .AddKey(0, openPartDuration);
    }

    [Serializable]
    public class BlinkParameterSet
    {
        [Range(0, 1.0f)]
        public float ratioHalf = 0.3f;
        [Range(0, 1.0f)]
        public float ratioClose = 0.9f;
        public float closeDuration = 0.1f;
        public float openDuration = 0.2f;
        public float interval = 1.5f;
        [Range(0, 1.0f)]
        public float randomThreshold = 0.7f;
    }

    #region Transition
    public class Transition
    {
        private List<TransitionKey> keys;

        public IEnumerable<TransitionKey> Keys { get { return keys.AsEnumerable(); } }

        public Transition()
        {
            keys = new List<TransitionKey>();
        }

        public Transition AddKey(float weight, float duration)
        {
            keys.Add(new TransitionKey(weight, duration));
            return this;
        }

        [Serializable]
        public class TransitionKey
        {
            public TransitionKey(float targetWeight, float duration)
            {
                this.targetWeight = targetWeight;
                this.duration = duration;
            }
            public float targetWeight;
            public float duration;
        }
    }

    private class TransitionPlayer
    {
        private Queue<Transition.TransitionKey> keys;
        public bool IsFinished { private set; get; }

        private Transition.TransitionKey previousKey;
        private Transition.TransitionKey currentKey;
        private float current;

        public TransitionPlayer(Transition t, float startingWeight)
        {
            keys = new Queue<Transition.TransitionKey>(t.Keys);
            previousKey = new Transition.TransitionKey(startingWeight, 0);
            currentKey = keys.Dequeue();
            current = 0;
            IsFinished = false;
        }

        public float Next(float timeDelta)
        {
            if (IsFinished) return currentKey.targetWeight;

            current += timeDelta;
            if (current > currentKey.duration)
            {
                if (keys.Count == 0)
                {
                    IsFinished = true;
                    return currentKey.targetWeight;
                }

                previousKey = currentKey;
                currentKey = keys.Dequeue();
                current -= currentKey.duration;
            }

            return Mathf.Lerp(previousKey.targetWeight, currentKey.targetWeight, current / currentKey.duration);
        }

        public void Abort()
        {
            IsFinished = true;
        }
    }
    #endregion
}