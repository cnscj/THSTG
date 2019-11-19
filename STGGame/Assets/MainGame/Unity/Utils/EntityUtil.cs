using System;
namespace STGU3D
{
    public static class EntityUtil
    {
        public static EEntityType GetEntityTypeByCode(long code)
        {
            long type = (code / 100000) % 100;
            return (EEntityType)type;
        }

        public static EHeroType GetHeroTypeByCode(long code)
        {
            long type = (code / 1000) % 100;
            return (EHeroType)type;
        }

        public static EEntityType GetEntityTypeByCode(string code)
        {
            return (EEntityType)int.Parse(code.Substring(1, 2));    //直接取前2位,不过必须保证长度
        }

        public static EHeroType GetHeroTypeByCode(string code)
        {
            return GetHeroTypeByCode(long.Parse(code));
        }
    }

}

