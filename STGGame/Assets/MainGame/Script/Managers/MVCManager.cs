using THGame.Package;
using UnityEngine;
using THMVCManager = THGame.Package.MVC.MVCManager;
namespace STGGame
{
	public class MVCManager : MonoSingleton<MVCManager>
    {
        private THMVCManager m_mvcManager = new THMVCManager();

        private void Awake()
        {
            m_mvcManager.AddController<MainUIController>(ModuleType.MAIN_UI);
            m_mvcManager.AddController<StageController>(ModuleType.STAGE);
        }

    }


}
