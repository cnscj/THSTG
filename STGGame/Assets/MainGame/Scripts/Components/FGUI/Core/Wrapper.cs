
namespace STGGame.UI
{

    public class Wrapper<T>
    {
        protected T _obj;
        public virtual Wrapper<T> InitWithObj(T obj)
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
