﻿using System;
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
            btnList.SetClass(typeof(FButton));
            btnList.SetState((index,comp,data) =>
            {
                var item = data as ItemData;
                var btn = comp as FButton;
                comp.SetText(item.btnName);
                comp.OnClick(item.btnFunc);


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

