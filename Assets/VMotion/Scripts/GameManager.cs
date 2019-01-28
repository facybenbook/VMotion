using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nkjzm.VMotion
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public string LoadVrmPath = string.Empty;
        public bool FromVRoidHub = false;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}