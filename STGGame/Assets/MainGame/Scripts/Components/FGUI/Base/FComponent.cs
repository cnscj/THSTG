using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FComponent : FObject
    {
        protected FGraph _graph;
        protected FScrollPane _scrollPane;

        public T GetChild<T>(string name) where T : FComponent, new()
        {
            GObject obj = this._obj.asCom.GetChild(name);
            return FGUIUtil.CreateComponent<T>(obj);
        }

        public FComponent[] GetChildren()
        {
            List<FComponent> _childList = new List<FComponent>();
            GObject[] children = _obj.asCom.GetChildren();
            foreach (var child in children)
            {
                FComponent fObj = FGUIUtil.CreateComponent<FComponent>(child);
                _childList.Add(fObj);
            }
            return _childList.ToArray();
        }
        //

        public void DebugUI()
        {
            var size = GetSize();
            if (_graph != null)
            {
                _graph.Dispose();
                _graph = null;
            }
            else
            {
                _graph = new FGraph().InitWithObj(new GGraph()) as FGraph;
                _graph.DrawRect(size.x, size.y, 5, new Color(0xff,0x00,0x00),new Color(0x00,0x00,0x00));
                _graph.SetTouchable(false);

                AddChild(_graph);
            }
        }
        //
        public void AddChild(FComponent comp)
        {
            _obj.asCom.AddChild(comp.GetObject());
        }
        public void AddChildAt(FComponent comp, int idx)
        {
            _obj.asCom.AddChildAt(comp.GetObject(), idx);
        }

        public void RemoveChild(FComponent comp,bool isDisposed = false)
        {
            _obj.asCom.RemoveChild(comp.GetObject(), isDisposed);
        }
        public void RemoveChildren()
        {
            _obj.asCom.RemoveChildren();
        }

        public void RemoveAllChildren(bool isDisposed = false)
        {
            _obj.asCom.RemoveChildren(0, -1, isDisposed);
        }
        public void RemoveFromParent()
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
            return new FController(ctrl);
        }

        public FTransition GetTransition(string name)
        {
            Transition trans = _obj.asCom.GetTransition(name);
            return new FTransition(trans);
        }
        public FScrollPane GetScrollPane()
        {
            _scrollPane = (_scrollPane != null) ? _scrollPane : new FScrollPane(_obj.asCom.scrollPane);
            return _scrollPane;
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
