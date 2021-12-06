using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public static class TextureUtil
    {
        ///////////////////////////
        public static Texture2D Bytes2Texture2d(byte[] bytes)
        {
            if (bytes == null)
                return default;

            Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false);
            texture.LoadImage(bytes);

            return texture;
        }

        public static Texture2D Texture2Texture2d(Texture texture, Rect source)
        {
            if (texture == null)
                return default;

            Texture2D texture2D = new Texture2D((int)source.width, (int)source.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(source, 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;

        }

        //长变宽,宽边长
        public static Texture2D Texture2DRotate(Texture2D ori)
        {
            Texture2D newTexture = new Texture2D(ori.height, ori.width, ori.format, false);
            for (int i = 0; i <= ori.width - 1; i++)
            {
                for (int j = 0; j <= ori.height - 1; j++)
                {
                    Color color = ori.GetPixel(i, j);
                    newTexture.SetPixel(j, newTexture.height - 1 - i, color);
                }
            }
            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTex"></param>
        /// <returns></returns>
        public static Texture2D NTexture2Texture2d(NTexture nTex)
        {
            if (nTex == null)
                return default;

            bool isRotated = nTex.rotated;
            Texture2D oriT2d = (Texture2D)nTex.nativeTexture;

            var uvX = nTex.uvRect.x * oriT2d.width;
            var uvY = 0f;

#if UNITY_STANDALONE_WIN
            var nTexHeight = isRotated ? nTex.width : nTex.height;
            uvY = oriT2d.height - nTex.uvRect.y * oriT2d.height - nTexHeight;
#else
            uvY = nTex.uvRect.y * oriT2d.height;
#endif
            Texture2D newT2d = Texture2Texture2d(oriT2d, new Rect(uvX, uvY, isRotated ? nTex.height : nTex.width, isRotated ? nTex.width : nTex.height));

            //这里需要处理下UV偏移和翻转问题,整个图像向右旋转90

            if (isRotated)
            {
                newT2d = Texture2DRotate(newT2d);
            }

            return newT2d;
        }
    }
}