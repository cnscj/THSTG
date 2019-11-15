
using System.Collections.Generic;
using XLibGame;
using STGService.MVC;

namespace STGService
{
	public class StageController : Controller
	{
        protected override void OnAdded()
        {
            EventSystem.Dispatch(EventType.TEST_EVENT, this , "string12312");

            UnityEngine.Debug.Log(ResourceConfiger.GetResSrc("1001"));
            UnityEngine.Debug.Log(TestConfiger.GetResSrc("reimu"));
        }

    }
}
