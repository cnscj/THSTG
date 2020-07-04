
using System;
using FairyGUI;
using XLibrary.Package;

namespace THGame.UI
{
    public class PopupManager : MonoSingleton<PopupManager>
    {
        public PopupManager()
        {

        }

        public GComponent GetCurPopup()
        {
            return null;
        }

        public int Show<T>(ToolTipData data) where T : ToolTipBase, new()
        {
            var toolTip = GetOrCreateToolTip<T>();
            toolTip.SetToolTipData(data);

            UIManager.GetInstance().ShowPopup(toolTip);

            return -1;
        }

        public void Hide<T>() where T : ToolTipBase
        {

        }

        public void IsShow<T>() where T : ToolTipBase
        {

        }

        public void HideAll()
        {

        }

        public void Clear()
        {

        }

        private T GetOrCreateToolTip<T>() where T : ToolTipBase, new()
        {
            Type toolTipType = typeof(T);
            string poolName = toolTipType.FullName;

            var pool = UIPoolManager.GetInstance().GetPool(poolName);
            if (pool == null)
            {
                pool = UIPoolManager.GetInstance().CreatePool(poolName, toolTipType);
            }
            return pool.GetOrCreate() as T;
        }

    }
}

