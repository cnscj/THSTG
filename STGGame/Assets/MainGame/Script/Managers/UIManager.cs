﻿
using UnityEngine;
using Unity.Entities;
using THGame;
namespace STGGame
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [HideInInspector]public GameObject uiRoot;
        public GameObject mainRoot;
        public GameObject windowRoot;

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
        private UIManager() { }
    }
}