
namespace STGGame.UI
{

    public class FWrapper<T>
    {
        protected T _obj;
        public virtual FWrapper<T> InitWithObj(T obj)
        {
            _obj = obj;
            return this;
        }

        public T GetObject()
        {
            return _obj;
        }
    }

}
