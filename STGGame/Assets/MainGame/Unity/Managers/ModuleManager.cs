
using STGGame.MVC;
using XLibrary.Package;

namespace STGGame
{
	public class ModuleManager : MonoSingleton<ModuleManager>
    {
        private void Awake()
        {
            MVCManager.GetInstance().AddController<MainUIController>();
            MVCManager.GetInstance().AddController<StageController>();
            MVCManager.GetInstance().AddController<TestController>();

        }

    }


}
