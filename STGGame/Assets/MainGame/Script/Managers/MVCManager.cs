using UnityEngine;
using THMVCManager = THGame.Package.MVC.MVCManager;
namespace STGGame
{
	public class MVCManager : MonoBehaviour
	{
		private static THMVCManager m_manager = new THMVCManager();
		public static THMVCManager GetInstance()
		{
			return m_manager;
		}

        private void Awake()
        {
            GetInstance().AddController<MainUIController>(ModuleType.MAIN_UI);

        }
    }


}
