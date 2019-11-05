
using XLibGame;
using STGService.MVC;

namespace STGService
{
    public class MainUIController : Controller
    {
        protected override void OnAdded()
        {
            EventSystem.Dispatch(EventType.TEST_EVENT);
        }
    }

}
