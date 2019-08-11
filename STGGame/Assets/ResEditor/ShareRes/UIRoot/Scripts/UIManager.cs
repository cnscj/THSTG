
using UnityEngine;
using THGame.Package;
using System.Collections;
using STGGame.UI;

namespace STGGame
{
    public class UIManager : UIBaseManager
    {
        public static UIManager s_instance;
        [HideInInspector]public GameObject uiRoot;
        public GameObject mainRoot;
        public GameObject windowRoot;

        private Stack m_wndStack = new Stack();         //窗口栈
        public static UIManager GetInstance()
        {
            return s_instance;
        }
        private void Awake()
        {
            s_instance = this;
        }
        public void Start()
        {
            var canvas = gameObject.GetComponent<Canvas>();
            if (canvas)
            {
                canvas.worldCamera = canvas.worldCamera ? canvas.worldCamera : CameraManager.GetInstance().uiCamera;
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