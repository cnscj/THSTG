
using System.Collections.Generic;
using XLibGame;
using STGGame.MVC;

namespace STGGame
{
	public class StageController : Controller
	{
        protected override void OnOpen()
        {
            DispatcherManager.GetInstance().AddListener(EventType.TEST_EVENT, this.Test);

            UnityEngine.Debug.Log(ResourceConfiger.GetResSrc("1001"));
            UnityEngine.Debug.Log(TestConfiger.GetResSrc("reimu"));
        }

        protected void Test(int eventId, object args)
        {
            UnityEngine.Debug.Log(Cache.Get<TestCache>().testString);
        }
    }
}
