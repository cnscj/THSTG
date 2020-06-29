
using System;
using FairyGUI;
using XLibrary.Package;

namespace THGame.UI
{

    public class PopUpManager : Singleton<PopUpManager>
    {
        public void RegisterTip(string tipType, Type tipClass)
        {

        }

        public GComponent GetCurPopup()
        {
            return null;
        }

        public int Show(PopUpParams args)
        {
            return 0;
        }

        public void Hide(int toolTipId)
        {

        }

        public void IsShow(int toolTipId)
        {

        }

        public void HideAll()
        {

        }

        public void Clear()
        {

        }
    }
}

