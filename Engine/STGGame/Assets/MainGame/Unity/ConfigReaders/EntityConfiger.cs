using XLibrary;

namespace STGU3D
{
    public static class EntityConfiger
    {
        private static CSVTable g_role;
        private static CSVTable g_wingman;
        private static CSVTable g_mob;
        private static CSVTable g_prop;
        private static CSVTable g_bullet;
        private static CSVTable g_boss;

        private static CSVTable LoadConfig(string code)
        {
            var content = ConfigerManager.GetInstance().LoadConfig(code);
            return new CSVTable(content);
        }

        private static CSVTable GetRoleTable()
        {
            return (g_role = g_role != null ? g_role : LoadConfig("G_Role.csv"));

        }

        private static CSVTable GetWingmanTable()
        {
            return (g_wingman = g_wingman != null ? g_wingman : LoadConfig("G_Wingman.csv"));

        }

        private static CSVTable GetMobTable()
        {
            return (g_mob = g_mob != null ? g_mob : LoadConfig("G_Mob.csv"));

        }
        private static CSVTable GetPropTable()
        {
            return (g_prop = g_prop != null ? g_prop : LoadConfig("G_Prop.csv"));

        }
        private static CSVTable GetBulletTable()
        {
            return (g_bullet = g_bullet != null ? g_bullet : LoadConfig("G_Bullet.csv"));

        }

        private static CSVTable GetBossTable()
        {
            return (g_boss = g_boss != null ? g_boss : LoadConfig("G_Boss.csv"));

        }

        ///取得角色所有数据
        public static CSVObject GetRoleInfo(string key)
        {
            var tb = GetRoleTable();
            return tb[key];

        }

        public static CSVObject GetWingmanInfo(string key)
        {
            var tb = GetWingmanTable();
            return tb[key];

        }

        public static CSVObject GetMobInfo(string key)
        {
            var tb = GetMobTable();
            return tb[key];

        }

        public static CSVObject GetBossInfo(string key)
        {
            var tb = GetBossTable();
            return tb[key];

        }

        public static CSVObject GetBulletInfo(string key)
        {
            var tb = GetBulletTable();
            return tb[key];

        }

        public static CSVObject GetEntityInfo(string code)
        {
            //按类型区分
            EEntityType entityType = EntityUtil.GetEntityTypeByCode(code);
            CSVObject obj = null;
            switch (entityType)
            {
                case EEntityType.Hero:
                    obj = GetRoleInfo(code);
                    break;
                case EEntityType.Wingman:
                    obj = GetWingmanInfo(code);
                    break;
                case EEntityType.Mob:
                    for (int i = 0; i < 2; i++)
                    {
                        code = ReplaceChar(code, code.Length - i, '0');
                        obj = GetMobInfo(code);
                        if (obj != null) break;
                    }
                    break;
                case EEntityType.Boss:
                    obj = GetBossInfo(code);
                    break;
                case EEntityType.Bullet:
                    {
                        for(int i = 0; i < 2; i++)
                        {
                            code = ReplaceChar(code, code.Length - i, '0');
                            obj = GetBulletInfo(code);
                            if (obj != null) break;
                        }
                    }
                    break;

            }
            return obj;
        }

        //将末尾起第bit位置c
        private static string ReplaceChar(string str, int index, char c)
        {
            if (index < 0 || index > str.Length - 1) return str;
            char[] carr = str.ToCharArray();
            carr[index] = c;
            return new string(carr);
        }
    }
}
