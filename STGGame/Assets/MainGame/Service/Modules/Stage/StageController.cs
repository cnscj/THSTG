
using System.Collections.Generic;
using XLibGame;
using STGService.MVC;

namespace STGService
{
	public class StageController : Controller
	{
        protected override void OnOpen()
        {
            EventSystem.AddListener(EventType.TEST_EVENT, this.Test);

            UnityEngine.Debug.Log(ResourceConfiger.GetResSrc("1001"));
            UnityEngine.Debug.Log(TestConfiger.GetResSrc("reimu"));
        }

        protected void Test(int eventId, object args)
        {
            UnityEngine.Debug.Log(Cache.Get<TestCache>().testString);
        }
    }
}
