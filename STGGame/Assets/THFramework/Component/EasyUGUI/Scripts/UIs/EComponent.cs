using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{
    //TODO:
    public class EComponent : EObject
    {
       
        /////////////////
        public void Enter()
        {
            OnEnter();
            OnInitUI();
            OnInitListener();
            StartTimer();
            StopTimer();
        }
        public void Exit()
        {
            RemoveListener();
            StopTimer();
            OnExit();

        }
        /////////////////
        protected Object GetChild(string name)
        {
            return GetChild<Object>(name);
        }
        protected T GetChild<T>(string name) where T : Object
        {
            return null;
        }
        protected void AddListener(int even)
        {

        }
        protected void RemoveListener()
        {

        }


        /////////////////
        protected virtual void OnInitUI()
        {

        }

        protected virtual void OnInitListener()
        {

        }
        protected virtual void OnTick()
        {

        }
        protected virtual void OnEnter()
        {

        }
        protected virtual void OnExit()
        {

        }

       /////////////////////////////////////////////
        private void StartTimer()
        {

        }


        private void StopTimer()
        {

        }
    }

}

