using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame.UI
{
    public class FViewWindow : FView
    {
        public static readonly string titleName = "title";
        protected string _title = "标题";
        private FRichText __titleLabel;
        public FViewWindow(string package, string component) : base(package, component)
        {
            _layerOrder = 100;
        }

        public void SetTitle(string title)
        {
            if (__titleLabel != null)
            {
                __titleLabel.SetText(title);
            }
        }

        public string GetTitle()
        {
            if (__titleLabel != null)
            {
               return __titleLabel.GetText();
            }
            return null;
        }

        private void __InitWindowTitle()
        {
            __titleLabel = GetChild<FRichText>(titleName);
            SetTitle(_title);
        }

        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);

            __InitWindowTitle();

            return this;
        }
    }
}
