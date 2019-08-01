
using System.Collections.Generic;
using THGame.Package.MVC;
using UnityEngine;

namespace STGGame
{
	public class StageController : Controller
	{
        protected override void OnOpen()
        {
            DispatcherManager.GetInstance().AddListener(EventType.TEST_EVENT, this.Test);
        }

        protected void Test(int eventId, Dictionary<string, object> args)
        {
            Debug.Log(StageCache.GetInstance().testString);
        }
    }
}
