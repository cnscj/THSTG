using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame.UI
{
	public class FView : FWidget
    {
        private Dictionary<string, object> m_args;
      
        protected bool _isAsync = false;
        protected int _layer = 0;
    
        public Dictionary<string, object> args { get { return m_args; } }

        public FView(string package, string component):base(package, component)
        {

        }

        public void Create(Dictionary<string, object> args = null)
        {
            m_args = args;
            if (_isAsync)
            {
                UIPackage.CreateObjectAsync(package, component, _OnCreateSuccess);
            }
            else
            {
                GObject obj = UIPackage.CreateObject(package, component);
                _OnCreateSuccess(obj);
            }
        }

        private void _OnCreateSuccess(GObject obj)
        {
            if (obj == null)
            {
                Debug.LogError(string.Format("{0} {1} => package not found | 没有加载到包或组件", package, component));
                return;
            }

            InitWithObj(obj);

            GRoot.inst.AddChild(obj);
        }

    }

}
