using UnityEngine;

namespace STGGame
{
    public static class DirectorUtil
    {
        public static Vector2 GetScreenSize()
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

        public static Rect GetScreenRect()
        {
            return new Rect(0, 0, Screen.width, Screen.height);
        }

        //世界转屏幕矩形
        public static Rect WorldToScreenRect(Rect worldRect)
        {
            var pixelPerPot = GetPixelPerPot();
            return new Rect(worldRect.x / pixelPerPot, worldRect.y / pixelPerPot, worldRect.width / pixelPerPot, worldRect.height / pixelPerPot);
        }
        //屏幕转世界矩形
        public static Rect ScreenToWorldRect(Rect screenRect)
        {
            var pixelPerPot = GetPixelPerPot();
            return new Rect(-0.5f * screenRect.width * pixelPerPot, 0.5f * screenRect.height * pixelPerPot, screenRect.width * pixelPerPot, screenRect.height * pixelPerPot);
        }

        //世界转屏幕坐标
        public static Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            return Camera.main.WorldToScreenPoint(worldPosition);
        }
        //屏幕转世界坐标
        public static Vector3 ScreenToWorldPoint(Vector3 screenPosition)
        {
            //return Camera.main.ScreenToWorldPoint(screenPosition);
            var pixelPerPot = GetPixelPerPot();
            return screenPosition * pixelPerPot;
        }


    }
}
