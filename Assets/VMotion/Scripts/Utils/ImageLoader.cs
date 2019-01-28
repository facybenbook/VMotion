using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace nkjzm.VMotion.Utils
{
    public static class ImageLoader
    {
        public static IEnumerator Load(string path, RawImage image)
        {
            var www = new WWW(path);
            yield return www;
            image.texture = www.texture;
        }
    }
}