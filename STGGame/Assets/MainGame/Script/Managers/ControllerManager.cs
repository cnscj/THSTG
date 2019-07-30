using THGame.Package;
using UnityEngine;
using THMVCManager = THGame.Package.MVC.MVCManager;
namespace STGGame
{
	public class ControllerManager : MonoSingleton<ControllerManager>
    {
		private static THMVCManager m_manager = THMVCManager.GetInstance();
		
    }


}
