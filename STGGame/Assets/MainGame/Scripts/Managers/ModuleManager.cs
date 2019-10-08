
using XLibrary.MVC;
using XLibrary.Package;

namespace STGGame
{
	public class ModuleManager : MonoSingleton<ModuleManager>
    {
        private void Awake()
        {
            MVCManager.GetInstance().AddController<MainUIController>(ModuleType.MAIN_UI);
            MVCManager.GetInstance().AddController<StageController>(ModuleType.STAGE);
            MVCManager.GetInstance().AddController<TestController>(ModuleType.TEST);

        }

    }


}
