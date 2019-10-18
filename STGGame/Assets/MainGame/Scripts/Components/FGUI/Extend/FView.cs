using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame
{
	public class FView : FComponent
    {
        private Dictionary<string, System.Object> m_args;
        private string m_package;
        private string m_component;

        protected bool _isAsync = false;
        protected int _layer = 0;

        public string package { get { return m_package; } }
        public string component { get { return m_component; } }
        public Dictionary<string, System.Object> args { get { return m_args; } }

        public FView(string package, string component)
        {
            m_package = package;
            m_component = component;

        }

        public void Create(Dictionary<string, System.Object> args = null)
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

            Init(obj);

            GRoot.inst.AddChild(obj);
        }

    }

}
