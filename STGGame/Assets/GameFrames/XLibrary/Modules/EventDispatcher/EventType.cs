using UnityEngine;

namespace XLibGame
{
    public class EventType
    {
        //XXX:尽管可以兼容两种情况,然而并不能保证没有冲突
        private int m_type;

        public static bool operator ==(EventType c1, EventType c2)
        {
            if (Equals(c1, null) || Equals(c2, null))
            {
                return Equals(c1, c2);
            }

            return c1.m_type == c2.m_type;
        }

        public static bool operator !=(EventType c1, EventType c2)
        {
            if (Equals(c1, null) || Equals(c2, null))
            {
                return !Equals(c1, c2);
            }

            return c1.m_type != c2.m_type;
        }
        //隐式转换
        public static implicit operator EventType(string v)
        {
            return new EventType(v);
        }

        public static implicit operator EventType(int v)
        {
            return new EventType(v);
        }

        public EventType(int t)
        {
            m_type = t;
        }

        public EventType(string t)
        {
            m_type = t.GetHashCode();
        }

        //容器对比函数重载
        public override bool Equals(object obj)
        {
            var objType = (EventType)obj;

            return base.Equals(obj) || m_type == objType.m_type;
        }

        public override int GetHashCode()
        {
            return m_type.GetHashCode();
        }

        public override string ToString()
        {
            return m_type.ToString();
        }

    }
}