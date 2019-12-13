using UnityEngine;

namespace STGGame
{
    public static class DirectorUtil
    {
        public static float GetPixelPerPot()
        {
            Vector3 v3 = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, -Camera.main.transform.position.z)); //最后一个Z必须,不过在正交摄像机(或2d)下意义不大
            float winSizeX = Mathf.Abs(v3.x) * 2;
            var pixelPerPot = winSizeX / Screen.width;
            return pixelPerPot;
        }
        public static float ScreenLengthInWorld(float a)
        {
            var pixelPerPot = GetPixelPerPot();
            return a * pixelPerPot;
        }

        public static float WorldLengthInScreen(float a)
        {
            var pixelPerPot = GetPixelPerPot();
            return a / pixelPerPot;
        }

        public static Vector2 GetScreenSize()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        public static Vector2 GetWorldSize()
        {
            return new Vector2(ScreenLengthInWorld(Screen.width), ScreenLengthInWorld(Screen.height));
        }

        public static Vector2 ScreenSizeInWorld(Vector2 size)
        {
            return new Vector2(ScreenLengthInWorld(size.x), ScreenLengthInWorld(size.y));
        }

        public static Vector2 WorldSizeInScreen(Vector2 size)
        {
            return new Vector2(WorldLengthInScreen(size.x), WorldLengthInScreen(size.y));
        }

        //世界矩形在屏幕中的坐标
        public static Rect WorldRectInScreen(Rect worldRect)
        {
            var pixelPerPot = GetPixelPerPot();
            return new Rect(worldRect.x / pixelPerPot, worldRect.y / pixelPerPot, worldRect.width / pixelPerPot, worldRect.height / pixelPerPot);
        }
        //屏幕矩形在世界里的坐标
        public static Rect ScreenRectInWorld(Rect screenRect)
        {
            var pixelPerPot = GetPixelPerPot();
            return new Rect(-0.5f * screenRect.width * pixelPerPot, -0.5f * screenRect.height * pixelPerPot, screenRect.width * pixelPerPot, screenRect.height * pixelPerPot);
        }
        ///
        /*
         *  关于Rect的定义
         *  左下角为起点,右上角为(w,h)
         *  屏幕左下角永远为(0,0)
         */

        public static Rect GetScreenRect()
        {
            return Camera.main.pixelRect;
        }

        public static Rect GetWorldRect()
        {
            Vector3 v3 = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, -Camera.main.transform.position.z));
            return new Rect(0,0, 2 * v3.x, 2 * v3.y);
        }

        //世界转屏幕坐标
        public static Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            return Camera.main.WorldToScreenPoint(worldPosition);
        }
        //屏幕转世界坐标
        public static Vector3 ScreenToWorldPoint(Vector3 screenPosition)
        {
            return Camera.main.ScreenToWorldPoint(screenPosition);
        }
    }
}
