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
        protected Dictionary<int, ViewInfo> _children;

        private int __preIndex = -1;


        public FViewTabBar(string package,string component):base(package, component)
        {
        }

        public override void Close()
        {
            if (_children != null)
            {
                foreach(var pair in _children)
                {
                    var viewInfo = pair.Value;
                    if (viewInfo.view != null)
                    {
                        viewInfo.view.Close();
                    }
                }
                _children.Clear();
                _children = null;
            }
            base.Close();
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
                    _children = (_children != null) ? _children : new Dictionary<int, ViewInfo>();

                    var data = _list.GetSelectedData() as ViewParams;
                    var index = _list.GetSelectedIndex();

                    if (index == __preIndex)
                    {
                        return;
                    }

                    ViewInfo preViewInfo = null;
                    if (_children.TryGetValue(__preIndex, out preViewInfo))
                    {
                        var preView = preViewInfo.view;
                        if (preView != null)
                        {
                            preView.RemoveFromParent();
                        }
                    }

                    var curIndex = index;
                    __preIndex = curIndex;

                    bool isNeedCreate = true;
                    ViewInfo curViewInfo = null;
                    if (_children.TryGetValue(curIndex, out curViewInfo))
                    {
                        var curView = curViewInfo.view;
                        if (curView != null)
                        {
                            if (!curView.IsDisposed())
                            {
                                AddChild(curView);
                                isNeedCreate = false;
                            }
                        }
                    }

                    if (isNeedCreate)
                    {
                        var newData = _list.GetSelectedData() as ViewParams;
                        var newIndex = _list.GetSelectedIndex();
                        if (curIndex != newIndex)
                        {
                            return;
                        }
                        
                        var newView = FGUIUtil.CreateView(newData.cls);
                        ViewInfo newViewInfo = new ViewInfo();
                        newViewInfo.view = newView;

                        _children[curIndex] = newViewInfo;

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
                if (_children != null)
                {
                    _children.Clear();
                }
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
