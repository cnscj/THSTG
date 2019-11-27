using UnityEngine;

namespace STGGame
{
    public static class DirectorUtil
    {
        public static Vector2 GetViewportSize()
        {
            return new Vector2(Screen.width, Screen.height);
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
                return new Rect(-pixelPerPot * size.x * 0.5f, -pixelPerPot * size.y * 0.5f, pixelPerPot * size.x, pixelPerPot * size.y);
            }
            else
            {
                return new Rect(-Screen.width * 0.5f, -Screen.height * 0.5f, Screen.width, Screen.height);
            }

        }
    }
}
