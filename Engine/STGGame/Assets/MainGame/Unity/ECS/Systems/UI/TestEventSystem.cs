using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class TestEventSystem : IInitializeSystem
    {
        public TestEventSystem(Contexts contexts)
        {
           
        }

        public void Initialize()
        {
            DispatcherManager.GetInstance().AddListener(EventType.TEST_EVENT, Test);
        }

        ///////

        public void Test()
        {
            Debug.Log("ECS里的函数");
        }
    }

}
