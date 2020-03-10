using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGRuntime.UI
{
    public class TestContainerComp : FWidget
    {
        public class ItemData
        {
            public string btnName;
            public EventCallback0 btnFunc;
        }
        List<ItemData> data = new List<ItemData>()
        {
            new ItemData()
            {
                btnName = "例子",
                btnFunc = () =>
                {
                    ViewManager.GetInstance().Open<TestExampleView>();
                }
            },
            new ItemData()
            {
                btnName = "测试",
                btnFunc = () =>
                {
                    Debug.Log("测试");
                    EventHelper.Dispatch(STGU3D.EventType.TEST_EVENT);
                }
            },
            new ItemData()
            {
                btnName = "设置",
                btnFunc = () =>
                {
                    ViewManager.GetInstance().Open<SettingView>();
                }
            },
        };
        FList btnList;
        public TestContainerComp() : base("Test", "TestContainerComp") { }
        protected override void OnInitUI()
        {
            btnList = GetChild<FList>("btnList");
            btnList.SetVirtual();
            btnList.SetState<ItemData,FButton>((index, data, comp) =>
            {
                comp.SetText(data.btnName);
                comp.OnClick(data.btnFunc);
            });
            
        }

        protected override void OnInitEvent()
        {

        }

        protected override void OnEnter()
        {
            btnList.SetDataProvider(data);
        }

        protected override void OnExit()
        {

        }
    }
}

