using UnityEngine;

namespace STGGame
{
    public static class DirectorUtil
    {
        public static Rect GetWorldRect()
        {
            Rect worldRect = new Rect();
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                var tx = theCamera.transform;
                Vector3 leftButtonPos = theCamera.ViewportToWorldPoint(new Vector3(0f, 0f, -tx.position.z));
                Vector3 rightTopPos = theCamera.ViewportToWorldPoint(new Vector3(1f, 1f, -tx.position.z));
                worldRect.x = leftButtonPos.x;
                worldRect.y = leftButtonPos.y;
                worldRect.width = 2 * leftButtonPos.x;
                worldRect.height = 2 * leftButtonPos.y;
            }
            return worldRect;
        }

        public static Rect GetScreenRect()
        {
            Rect screenRect = new Rect();
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                screenRect.x = 0;
                screenRect.y = 0;
                screenRect.width = theCamera.pixelWidth;
                screenRect.height = theCamera.pixelHeight;
            }
            return screenRect;
        }

        public static Vector3 ScreenSizeInWorld(Vector3 size)
        {
            Vector3 ret = new Vector3();
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                var tx = theCamera.transform;
                Vector3 startPos = theCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, -tx.position.z));
                Vector3 worldSize = theCamera.ScreenToWorldPoint(new Vector3(startPos.x + size.x, startPos.y + size.y, -tx.position.z));
                ret.x = worldSize.x;
                ret.y = worldSize.y;
                ret.z = worldSize.z;
            }
            return ret;
        }

        public static Vector3 WorldSizeInScreen(Vector3 size)
        {
            Vector3 ret = new Vector3();
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                var tx = theCamera.transform;
                Vector3 startPos = theCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -tx.position.z));
                Vector3 screenSize = theCamera.WorldToScreenPoint(new Vector3(startPos.x + size.x, startPos.y + size.y, -tx.position.z));
                ret.x = screenSize.x;
                ret.y = screenSize.y;
                ret.z = screenSize.z;
            }
            return ret;
        }


        //世界矩形在屏幕中的坐标
        public static Rect WorldRectInScreen(Rect worldRect)
        {
            Rect ret = new Rect();
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                var tx = theCamera.transform;
                Vector3 pos = theCamera.WorldToScreenPoint(new Vector3(worldRect.x, worldRect.y, -tx.position.z));
                Vector3 size = theCamera.WorldToScreenPoint(new Vector3(worldRect.width, worldRect.height, -tx.position.z));
                ret.x = pos.x;
                ret.y = pos.y;
                ret.width = size.x;
                ret.height = size.y;
            }
            return ret;
        }

        //屏幕矩形在世界里的坐标
        public static Rect ScreenRectInWorld(Rect screenRect)
        {
            Rect ret = new Rect();
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                var tx = theCamera.transform;
                Vector3 startPos = theCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, -tx.position.z));
                Vector3 pos = theCamera.ScreenToWorldPoint(new Vector3(screenRect.x, screenRect.y, -tx.position.z));
                Vector3 size = theCamera.ScreenToWorldPoint(new Vector3(startPos.x + screenRect.width, startPos.y + screenRect.height, -tx.position.z));
                ret.x = pos.x;
                ret.y = pos.y;
                ret.width = size.x;
                ret.height = size.y;
            }
            return ret;
        }


        public static Rect WorldRectInScreen()
        {
            Rect worldRect = GetWorldRect();
            return WorldRectInScreen(worldRect);
        }
        public static Rect ScreenRectInWorld()
        {
            Rect screenRect = new Rect();
            return ScreenRectInWorld(screenRect);
        }


        public static Vector3 WorldPointInScreen(Vector3 position)
        {
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                var tx = theCamera.transform;
                return theCamera.WorldToScreenPoint(new Vector3(position.x, position.y, theCamera.orthographic ? -tx.position.z : position.z));
            }
            return position;
        }

        public static Vector3 ScreenPointInWorld(Vector3 position)
        {
            var theCamera = Camera.main;
            if (theCamera != null)
            {
                var tx = theCamera.transform;
                return theCamera.ScreenToWorldPoint(new Vector3(position.x, position.y, theCamera.orthographic ? -tx.position.z : position.z));
            }
            return position;
        }
    }
}
