using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame.UI
{
    public class BaseView : MonoBehaviour
    {
        void Start()
        {
            Enter();
        }
        void OnDestroy()
        {
            Exit();
        }
        /////////////////
        public void Enter()
        {
            

        }
        public void Exit()
        {

        }
        /////////////////
        protected void AddListener(int even)
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
    }


}
