﻿using System;
namespace STGU3D
{
    public static class EntityUtil
    {
        public static EEntityType GetEntityTypeByCode(int code)
        {
            int type = code / 100000;
            return (EEntityType)type;
        }

        public static EHeroType GetHeroTypeByCode(int code)
        {
            int type = code % 100000;
            return (EHeroType)type;
        }
    }

}
