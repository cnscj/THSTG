
using XLibGame;
using STGGame.MVC;

namespace STGGame
{
    public class MainUIController : Controller
    {
        protected override void OnOpen()
        {
            DispatcherManager.GetInstance().Dispatch(EventType.TEST_EVENT);
        }
    }

}
