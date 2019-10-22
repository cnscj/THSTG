using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame.UI
{
    public class FWindow : FView
    {
        public static readonly string titleName = "title";
        protected string _title = "标题";
        private FRichText __titleLabel;
        public FWindow(string package, string component) : base(package, component)
        {
            _layerOrder = 100;
        }

        public void SetTitle(string title)
        {

        }

        private void __InitWindowTitle()
        {
            __titleLabel = GetChild<FRichText>(titleName);
            if (__titleLabel != null)
            {
                __titleLabel.SetText(_title);
            }
        }

        public override FWrapper<GObject> InitWithObj(GObject obj)
        {
            return base.InitWithObj(obj);
        }
    }
}
