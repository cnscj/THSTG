﻿using System;
namespace STGGame
{
    public static class EntityDataUtil
    {
        public static EEntityType GetEntityTypeByCode(int code)
        {
            int type = code / 100000;
            return (EEntityType)type;
        }
    }
}
