using FairyGUI;
using THGame.UI;
using System;
using UnityEngine;

namespace THGame
{
    /// <summary>
    /// 这个类的作用是封装一些Fgui的方法，避免在lua端大量调用fgui接口的操作。主要是针对Stage和GRoot，这2个类特别庞大，能不用wrap就不用。
    /// </summary>
    public static class FGUIUtil
    {
        public static DisplayObject stageFocus
        {
            get { return Stage.inst.focus; }
            set { Stage.inst.focus = value; }
        }

        public static GComponent CreateLayerObject(int sortingOrder, string layerName = null)
        {
            var obj = new GComponent();
            obj.sortingOrder = sortingOrder;
            obj.SetSize(GRoot.inst.width, GRoot.inst.height);
            obj.AddRelation(GRoot.inst, RelationType.Size);
            GRoot.inst.AddChild(obj);

            if (!string.IsNullOrEmpty(layerName))
            {
                obj.rootContainer.gameObject.name = layerName;
            }

            return obj;
        }

        // Stage
        public static void AddStageOnTouchBegin(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.Add(callback1);
        }
        public static void RemoveStageOnTouchBegin(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.Remove(callback1);
        }

        public static void AddStageOnTouchBeginCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.AddCapture(callback1);
        }

        public static void RemoveStageOnTouchBeginCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchBegin.RemoveCapture(callback1);
        }

        public static void AddStageOnTouchEndCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchEnd.AddCapture(callback1);
        }

        public static void RemoveStageOnTouchEndCapture(EventCallback1 callback1)
        {
            Stage.inst.onTouchEnd.RemoveCapture(callback1);
        }

        public static void SetStageOnKeyDown(EventCallback1 callback1)
        {
            Stage.inst.onKeyDown.Set(callback1);
        }

        public static void ClearStageOnKeyDown()
        {
            Stage.inst.onKeyDown.Clear();
        }

        public static void GetStageWidthAndHeight(out float width, out float height)
        {
            width = Stage.inst.size.x;
            height = Stage.inst.size.y;
        }

        //GRoot
        public static void SetRootContentScaleFactor(int width, int height)
        {
            GRoot.inst.SetContentScaleFactor(width, height);
        }

        public static void AddRootOnSizeChanged(EventCallback0 callback0)
        {
            GRoot.inst.onSizeChanged.Add(callback0);
        }

        public static void RemoveRootOnSizeChanged(EventCallback0 callback0)
        {
            GRoot.inst.onSizeChanged.Remove(callback0);
        }

    }
}
