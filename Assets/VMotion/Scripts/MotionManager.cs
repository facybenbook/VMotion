using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nkjzm.VMotion
{
    public class MotionManager : MonoBehaviour
    {
        [SerializeField]
        Button TitleButton = null;
        void Start()
        {
            TitleButton.onClick.AddListener(LoadScene);
        }
        void LoadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
    }
}