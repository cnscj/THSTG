using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame
{
	public class FView
	{
		private GObject m_root;
		private string m_package;
		private string m_component;
        public string package { get { return m_package; } }
        public string component { get { return m_component; } }

        public FView(string pack,string comp)
        {
            m_package = pack;
            m_component = comp;
        }

        public void ToCreate(GObject obj)
        {
            m_root = obj;
        }

        protected virtual void OnInitUI()
        {

        }

        protected virtual void OnInitEvent()
        {

        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnExit()
        {

        }
    }

}
