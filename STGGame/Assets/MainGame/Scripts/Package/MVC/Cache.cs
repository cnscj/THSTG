namespace STGGame.MVC
{
    public class Cache
    {
        public static T Get<T>() where T : Cache, new()
        {
            return MVCManager.GetInstance().GetCache<T>();
        }

        public virtual void Clear()
        {

        }
    }
}
