using System;
using UnityEngine;

namespace STGGame.UI
{
    public class TestExampleView : FView
    {
        FList exampleList;
        public TestExampleView():base("Test", "TestExampleView")
        {
            
        }
        protected override void OnInitUI()
        {
            exampleList = GetChild<FList>("exampleList");
            exampleList.SetState((int index, FComponent comp) =>
            {


            });
        }

        protected override void OnInitEvent()
        {

        }

        protected override void OnEnter()
        {
            
        }

        protected override void OnExit()
        {

        }
    }

}
