using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame.UI
{
    public class FViewTabBar : FView
    {
        public class ViewParams
        {
            public Type cls;
            public string key;
            public string title;
            public object args;
        }

        public class ViewInfo
        {
            public FView view;

        }
        public static readonly string barListName = "list";

        protected FList _list;
        protected List<ViewParams> _layers;
        protected List<ViewInfo> _children;

        private int __preIndex = -2;


        public FViewTabBar(string package,string component):base(package, component)
        {
        }

        public override void Close()
        {
            if (_children != null)
            {
                foreach(var child in _children)
                {
                    if (child.view != null)
                    {
                        child.view.Close();
                    }
                }
                _children.Clear();
                _children = null;
            }
        }

        private void __InitBarList()
        {
            _list = GetChild<FList>(barListName);
            if (_list != null)
            {
                _list.SetVirtual();

                _list.SetState((index, comp, data) =>
                {
                    var viewParams = data as ViewParams;
                    var title = comp.GetChild<FRichText>("title");
                    title.SetText(viewParams.title);
                });

                _list.AddClickItem((context) =>
                {
                    if (_children == null)
                    {
                        return;
                    }

                    var data = _list.GetSelectedData() as ViewParams;
                    var index = _list.GetSelectedIndex();

                    if (index == __preIndex)
                    {
                        return;
                    }
                    Debug.Log("!!!!");
                    var preView = (__preIndex >= 0 && __preIndex <= _children.Count - 1) ? _children[__preIndex].view : null;
                    if (preView != null)
                    {
                        if (preView.HasParent())
                        {
                            preView.RemoveFromParent();
                        }
                    }

                    var curIndex = index;
                    __preIndex = curIndex;
  
                    var curView = (curIndex >= 0 && curIndex <= _children.Count - 1) ? _children[curIndex].view : null;
                    if (curView != null)
                    {
                        if (curView.HasParent())
                        {
                            if (!curView.GetParent().IsDisposed())
                            {
                                curView.GetParent().AddChild(curView);
                            }
                        }
                        Debug.Log("####");
                    }
                    else
                    {
                        Debug.Log("%%%%");
                        var newData = _list.GetSelectedData() as ViewParams;
                        var newIndex = _list.GetSelectedIndex();
                        if (curIndex != newIndex)
                        {
                            return;
                        }
                        Debug.Log("^^^");
                        var newView = FGUIUtil.CreateView(newData.cls);
                        ViewInfo newViewInfo = new ViewInfo();
                        _children[curIndex] = newViewInfo;
                        newViewInfo.view = newView;

                        AddChild(newView);
                    }

                });
            }
        }
        private void __InitLayerStack()
        {
            OnInitTabBar();

            if (_layers != null && _list != null)
            {
                _children = (_children != null) ? _children : new List<ViewInfo>();
                _children.Clear();

                _list.SetDataProvider(_layers);
                _list.ScrollToView(_list.GetSelectedIndex());
            }
        }


        //
        protected virtual void OnInitTabBar()
        {
            
        }

        //重写后不希望再被重写
        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);

            __InitBarList();
            __InitLayerStack();
            
            return this;
        }
    }

}
