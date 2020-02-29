
using XLibGame;
using STGRuntime.MVC;

namespace STGRuntime
{
    public class MainUIController : Controller
    {
        protected override void OnAdded()
        {
            EventHelper.AddListener(EventType.TEST_EVENT, Test);
        }

        protected void Test(EventContext context)
        {
            UnityEngine.Debug.Log(Cache.Get<TestCache>().testString);
            UnityEngine.Debug.Log(context.args[0]);
        }
    }

}
