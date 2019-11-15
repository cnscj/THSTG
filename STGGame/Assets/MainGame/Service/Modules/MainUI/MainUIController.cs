
using XLibGame;
using STGService.MVC;

namespace STGService
{
    public class MainUIController : Controller
    {
        protected override void OnAdded()
        {
            EventSystem.AddListener(EventType.TEST_EVENT, Test);
        }

        protected void Test(EventContext context)
        {
            UnityEngine.Debug.Log(Cache.Get<TestCache>().testString);
            UnityEngine.Debug.Log(context.args[0]);
        }
    }

}
