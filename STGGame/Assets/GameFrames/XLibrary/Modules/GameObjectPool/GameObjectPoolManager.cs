using UnityEngine;

namespace XLibGame
{
    public class GameObjectPoolManager : GameObjectPoolBaseManager
    {
        private static GameObjectPoolManager s_instance;
        public static GameObjectPoolManager GetInstance()
        {
            if (s_instance == null)
            {
                GameObject singleton = new GameObject();
                s_instance = singleton.AddComponent<GameObjectPoolManager>();
                singleton.name = "(singleton) " + typeof(GameObjectPoolManager).ToString();

                DontDestroyOnLoad(singleton);
            }
            return s_instance;
        }

        public void OnDestroy()
        {
            base.Dispose();
        }
    }
}
