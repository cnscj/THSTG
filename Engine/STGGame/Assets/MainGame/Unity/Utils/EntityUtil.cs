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

        public static EBossType GetBossTypeByCode(long code)
        {
            long type = (code / 1000) % 100;
            return (EBossType)type;
        }

        public static EWingmanType GetWingmanTypeByCode(long code)
        {
            long type = (code / 100) % 1000;
            return (EWingmanType)type;
        }
        ///
        public static EEntityType GetEntityTypeByCode(string code)
        {
            return (EEntityType)int.Parse(code.Substring(1, 2));    //直接取前2位,不过必须保证长度
        }

        public static EHeroType GetHeroTypeByCode(string code)
        {
            return GetHeroTypeByCode(long.Parse(code));
        }

        public static EBossType GetBossTypeByCode(string code)
        {
            return GetBossTypeByCode(long.Parse(code));
        }

        public static EWingmanType GetWingmanTypeByCode(string code)
        {
            return GetWingmanTypeByCode(long.Parse(code));
        }

        ///
        public static string GetHeroCode(EHeroType heroType)
        {
            return string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Hero + 1000 * (int)heroType + 1);
        }
        public static string GetBossCode(EBossType bossType)
        {
            return string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Boss + 100 * (int)bossType);
        }
        public static string GetMobCode(int mobType)
        {
            return string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Mob + 100 * (int)mobType);
        }
        public static string GetWingmanCode(EWingmanType wingmanType)
        {
            return string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Wingman + 100 * (int)wingmanType + (int)1);
        }
        public static string GetBulletCode(int bulletType)
        {
            return string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Bullet + (int)bulletType);
        }
        public static string GetPropCode(int propType)
        {
            return string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Prop + (int)propType);
        }
    }
}

