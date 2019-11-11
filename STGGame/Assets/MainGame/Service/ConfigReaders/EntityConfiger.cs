﻿using STGU3D;
using XLibrary;

namespace STGService
{
    public static class EntityConfiger
    {
        private static CSVTable g_role;
        private static CSVTable g_wingman;
        private static CSVTable g_mob;
        private static CSVTable g_prop;
        private static CSVTable g_bullet;
        private static CSVTable g_boss;

        private static CSVTable GetRoleTable()
        {
            return (g_role = g_role != null ? g_role : AssetSystem.LoadConfig("G_Role.csv"));

        }

        private static CSVTable GetWingmanTable()
        {
            return (g_wingman = g_wingman != null ? g_wingman : AssetSystem.LoadConfig("G_Wingman.csv"));

        }

        private static CSVTable GetMobTable()
        {
            return (g_mob = g_mob != null ? g_mob : AssetSystem.LoadConfig("G_Mob.csv"));

        }
        private static CSVTable GetPropTable()
        {
            return (g_prop = g_prop != null ? g_prop : AssetSystem.LoadConfig("G_Prop.csv"));

        }
        private static CSVTable GetBulletTable()
        {
            return (g_bullet = g_bullet != null ? g_bullet : AssetSystem.LoadConfig("G_Bullet.csv"));

        }

        private static CSVTable GetBossTable()
        {
            return (g_boss = g_boss != null ? g_boss : AssetSystem.LoadConfig("G_Boss.csv"));

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


        public static CSVObject GetEntityInfo(string code)
        {
            //按类型区分
            EEntityType entityType = EntityUtil.GetEntityTypeByCode(int.Parse(code));
            CSVObject obj = null;
            switch(entityType)
            {
                case EEntityType.Hero:
                    obj = GetRoleInfo(code);
                    break;
                case EEntityType.Wingman:
                    obj = GetRoleInfo(code);
                    break;
 
            }
            return obj;
        }

    }
}
