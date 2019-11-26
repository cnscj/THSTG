using System.Drawing;
using UnityEngine;

namespace STGGame
{
    public static class DirectorUtil
    {
        public static Size GetViewportSize()
        {
            return new Size(Screen.width, Screen.height);
        }

        public static float GetPixelPerPot()
        {
            Vector3 v3 = Camera.main.ScreenToWorldPoint(Vector3.zero);
            var winSize = new Vector2(Mathf.Abs(v3.x) * 2, Mathf.Abs(v3.y) * 2);
            var pixelPerPot = winSize.x / Screen.width;

            return pixelPerPot;
        }

        public static Rect GetViewportRect(bool isPixel = false)
        {
            var size = GetViewportSize();
            if (!isPixel)
            {
                var pixelPerPot = GetPixelPerPot();
                return new Rect(-pixelPerPot * size.Width * 0.5f, -pixelPerPot * size.Height * 0.5f, pixelPerPot * size.Width, pixelPerPot * size.Height);
            }
            else
            {
                return new Rect(-Screen.width * 0.5f, -Screen.height * 0.5f, Screen.width, Screen.height);
            }

        }
    }
}
