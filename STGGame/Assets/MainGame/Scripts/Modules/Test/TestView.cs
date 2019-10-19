using System;
using UnityEngine;

namespace STGGame.UI
{
    public class TestView : FView
    {

        public TestView():base("Test","TestView")
        {
            Debug.Log("Test-ctor");
        }
        protected override void OnInitUI()
        {

        }

        protected override void OnInitEvent()
        {

        }

        protected override void OnEnter()
        {
            Debug.Log("Test-enter");
        }

        protected override void OnExit()
        {

        }
    }

}
