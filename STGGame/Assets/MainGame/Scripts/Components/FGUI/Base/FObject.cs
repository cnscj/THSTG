using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FObject
    {
        protected GObject _obj;
        public FObject()
        {

        }

        public virtual void InitWithObj(GObject obj)
        {
            this._obj = obj;
        }

        public GObject GetObject()
        {
            return _obj;
        }

        public GObject GetParent()
        {
            return _obj.parent;
        }

        public void DebugUI()
        {
            //TODO:
        }

        public void SetX(float x)
        {
            _obj.x = x;
        }
        public float GetX()
        {
            return _obj.x;
        }
        public void SetY(float y)
        {
            _obj.y = y;
        }
        public float GetY()
        {
            return _obj.y;
        }
        public void SetXY(float x, float y)
        {
            _obj.xy = new Vector2(x, y);
        }
        public Vector2 GetXY()
        {
            return _obj.xy;
        }
        public void SetZ(float z)
        {
            _obj.z = z;
        }
        public float GetZ()
        {
            return _obj.z;
        }

        public void SetXYZ(float x, float y, float z)
        {
            _obj.x = x;
            _obj.y = y;
            _obj.z = z;
        }
        public Vector3 GetXYZ()
        {
            return new Vector3(_obj.x, _obj.y, _obj.z);
        }

        public void SetWidth(float width)
        {
            _obj.width = width;
        }

        public float GetWidth()
        {
            return _obj.width;
        }
        public void SetHeight(float height)
        {
            _obj.height = height;
        }
        public float GetHeight()
        {
            return _obj.height;
        }

        public void SetMaxWidth(int width)
        {
            _obj.maxWidth = width;
        }
        public void SetMaxHeight(int height)
        {
            _obj.maxHeight = height;
        }
        public void SetMinWidth(int width)
        {
            _obj.minWidth = width;
        }
        public void SetMinHeight(int height)
        {
            _obj.minHeight = height;
        }
        public void SetInitWidth()
        {
            _obj.width = _obj.initWidth;
        }
        public int GetInitWidth()
        {
            return _obj.initWidth;
        }
        public void SetInitHeight()
        {
            _obj.height = _obj.initHeight;
        }
        public int GetInitHeight()
        {
            return _obj.initHeight;
        }
        public Vector2 GetSize()
        {
            return new Vector2(_obj.width, _obj.height);
        }
        public void SetSize(float width, float height)
        {
            _obj.width = width;
            _obj.height = height;
        }

        public Vector2 GetCenter()
        {
            var size = GetSize();
            return new Vector2(_obj.x + size.x / 2, _obj.y + size.y / 2);
        }
    }


}
