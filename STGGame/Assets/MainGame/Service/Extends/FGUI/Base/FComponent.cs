﻿using UnityEngine;
using FairyGUI;
using System.Collections.Generic;

namespace STGService.UI
{

    public class FComponent : FObject
    {
        private FGraph __graph;
        private FScrollPane __scrollPane;

        public static T Create<T>(GObject obj) where T : FComponent, new()
        {
            if (obj != null)
            {
                T com = new T();
                if (com != null)
                {
                    com.InitWithObj(obj);
                }

                return com;
            }
            return null;
        }

        public static new FComponent Create(GObject obj)
        {
            return Create<FComponent>(obj);
        }

        public T GetChild<T>(string name) where T : FComponent, new()
        {
            GObject obj = this._obj.asCom.GetChild(name);
            return FComponent.Create<T>(obj);
        }
        public FComponent GetChild(string name)
        {
            return GetChild<FComponent>(name);
        }

        public FComponent[] GetChildren()
        {
            List<FComponent> _childList = new List<FComponent>();
            GObject[] children = _obj.asCom.GetChildren();
            foreach (var child in children)
            {
                FComponent fObj = FComponent.Create<FComponent>(child);
                _childList.Add(fObj);
            }
            return _childList.ToArray();
        }
        //

        public void DebugUI()
        {
#if DEBUG
            var size = GetSize();
            if (__graph != null)
            {
                __graph.Dispose();
                __graph = null;
            }

            if (__graph == null)
            {
                __graph = new FGraph().InitWithObj(new GGraph()) as FGraph;
                __graph.DrawRect(size.x, size.y, 5, new Color(0xff,0x00,0x00,0xff),new Color(0x00,0x00,0x00,0x00));
                __graph.SetTouchable(false);

                AddChild(__graph);
            }
#endif
        }
        //
        public virtual void AddChild(FComponent comp)
        {
            _obj.asCom.AddChild(comp.GetObject());
        }
        public virtual void AddChildAt(FComponent comp, int idx)
        {
            _obj.asCom.AddChildAt(comp.GetObject(), idx);
        }

        public virtual void RemoveChild(FComponent comp,bool isDisposed = false)
        {
            _obj.asCom.RemoveChild(comp.GetObject(), isDisposed);
        }
        public virtual void RemoveChildren()
        {
            _obj.asCom.RemoveChildren();
        }

        public virtual void RemoveAllChildren(bool isDisposed = false)
        {
            _obj.asCom.RemoveChildren(0, -1, isDisposed);
        }
        public virtual void RemoveFromParent()
        {
            _obj.asCom.RemoveFromParent();
        }

        public void SetChildIndexBefore(FComponent child, int index)
        {
            _obj.asCom.SetChildIndexBefore(child.GetObject(), index);
        }

        public void SetChildIndex(FComponent child, int index)
        {
            _obj.asCom.SetChildIndex(child.GetObject(), index);
        }

        public int GetChildIndex(FComponent child)
        {
            return _obj.asCom.GetChildIndex(child.GetObject());
        }



        // controller
        public FController GetController(string name)
        {
            Controller ctrl = _obj.asCom.GetController(name);
            return new FController().InitWithObj(ctrl) as FController;
        }

        public FTransition GetTransition(string name)
        {
            Transition trans = _obj.asCom.GetTransition(name);
            return new FTransition().InitWithObj(trans) as FTransition;
        }
        public FScrollPane GetScrollPane()
        {
            var obj = _obj.asCom.scrollPane;
            __scrollPane = (obj != null) ? (__scrollPane != null ? __scrollPane.InitWithObj(obj) as FScrollPane : new FScrollPane().InitWithObj(obj) as FScrollPane) : null;
            return __scrollPane;
        }

        //
        // 接收拖放之后的【放】
        public void OnDrop(EventCallback1 func)
        {
            _obj.asCom.onDrop.Add(func);
        }

        public void SetZIndex(int index)
        {
            var oldIndex = GetZIndex();
            if (oldIndex >= index)
            {
                GetParent().SetChildIndexBefore(this, index);
            }
            else
            {
                GetParent().SetChildIndex(this, index);
            }
        }

        public int GetZIndex()
        {
            return GetParent().GetChildIndex(this);
        }

        public void SetViewHeight(float height)
        {
            _obj.asCom.viewHeight = height;
        }
        public float GetViewHeight()
        {
            return _obj.asCom.viewHeight;
        }
    }

}