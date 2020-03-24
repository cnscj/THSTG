using UnityEngine;
using System.Collections;

namespace ASGame
{
    //图集资源管理器
    public class GameObjectManager : BaseManager<GameObjectManager>
    {
        //自动管理prefab的加载,销毁等
        public void Instance(string path, string name, Transform parent = null)
        {

        }

        public void Destroy()
        {
            
        }
    }

}
