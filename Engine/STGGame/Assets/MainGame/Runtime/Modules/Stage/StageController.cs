
using System.Collections.Generic;
using XLibGame;
using STGRuntime.MVC;

namespace STGRuntime
{
	public class StageController : Controller
	{
        protected override void OnAdded()
        {
            EventHelper.Dispatch(EventType.TEST_EVENT, this , "string12312");

            UnityEngine.Debug.Log(TestConfiger.GetResSrc("reimu"));
        }

    }
}
