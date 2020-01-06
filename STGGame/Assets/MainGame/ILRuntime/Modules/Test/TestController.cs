

using FairyGUI;
using STGService.MVC;
using STGU3D;
using UnityEngine;

namespace STGService
{
    public class TestController : MVC.Controller
    {
        protected override void OnAdded()
        {
            FGUIUtil.SetStageOnKeyDown((context) =>
            {
                OnKeyDown(context);
            });
        }


        protected void OnKeyDown(EventContext context)
        {
            var keyCode = context.inputEvent.keyCode;
            if (keyCode == KeyCode.M)
            {
                var heroEntity = EntityCache.GetInstance().GetHero();
                var u3dVIew = heroEntity.view.view as UnityView;
                var viewCtrl = u3dVIew.viewCtrl;
                viewCtrl.shaderEffectCom.SetGray(true);
            }
        }
    }

}
