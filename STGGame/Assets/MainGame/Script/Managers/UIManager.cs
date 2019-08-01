
using UnityEngine;
using THGame.Package;
using System.Collections;

namespace STGGame
{
    public class UIManager : MonoSingleton<UIManager>
    {

        [HideInInspector]public GameObject uiRoot;
        public GameObject mainRoot;
        public GameObject windowRoot;

        private Stack m_wndStack = new Stack();         //窗口栈

        private void Awake()
        {
            uiRoot = new GameObject("UIRoot");
            uiRoot.transform.SetParent(gameObject.transform, true);
            {
                mainRoot = new GameObject("MainRoot");
                mainRoot.transform.SetParent(uiRoot.transform, true);

                windowRoot = new GameObject("WindowRoot");
                windowRoot.transform.SetParent(uiRoot.transform, true);
            }

        }

        public void Opne(string view)
        {

        }

        public void Close(string view)
        {

        }

        private UIManager() { }
    }
}