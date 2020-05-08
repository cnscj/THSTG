using UnityEngine;

namespace XLibGame
{
    public class Timebound<T> : Timechecker
    {
        public T obj;
      
        public Timebound(T o)
        {
            obj = o;
        }
    }

}
