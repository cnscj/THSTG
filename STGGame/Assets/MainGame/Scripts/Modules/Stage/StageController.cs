
using System.Collections.Generic;
using UnityEngine;
using XLibGame;
using XLibrary.MVC;

namespace STGGame
{
	public class StageController : Controller
	{
        protected override void OnOpen()
        {
            DispatcherManager.GetInstance().AddListener(EventType.TEST_EVENT, this.Test);

            Debug.Log(ResourceConfiger.GetResSrc("1001"));
            Debug.Log(TestConfiger.GetResSrc("reimu"));
        }

        protected void Test(int eventId, object args)
        {
            Debug.Log(StageCache.GetInstance().testString);
        }
    }
}
