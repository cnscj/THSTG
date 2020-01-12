using System;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class BossDataComponent : IComponent
    {
        public EBossType bossType;
        public Tween flashTween;
    }

}
