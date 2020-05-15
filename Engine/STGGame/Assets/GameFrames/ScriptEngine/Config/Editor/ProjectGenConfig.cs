using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public static class ProjectGenConfig
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        //dotween的扩展方法在lua中调用
        typeof(DG.Tweening.AutoPlay),
        typeof(DG.Tweening.AxisConstraint),
        typeof(DG.Tweening.Ease),
        typeof(DG.Tweening.LogBehaviour),
        typeof(DG.Tweening.LoopType),
        typeof(DG.Tweening.PathMode),
        typeof(DG.Tweening.PathType),
        typeof(DG.Tweening.RotateMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.TweenType),
        typeof(DG.Tweening.UpdateType),

        typeof(DG.Tweening.DOTween),
        typeof(DG.Tweening.DOVirtual),
        typeof(DG.Tweening.EaseFactory),
        typeof(DG.Tweening.Tweener),
        typeof(DG.Tweening.Tween),
        typeof(DG.Tweening.Sequence),
        typeof(DG.Tweening.TweenParams),
        typeof(DG.Tweening.Core.ABSSequentiable),

        typeof(DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions>),

        typeof(DG.Tweening.TweenExtensions),
        typeof(DG.Tweening.TweenSettingsExtensions),
        typeof(DG.Tweening.ShortcutExtensions),

        typeof(Dictionary<string, object>),
        typeof(Dictionary<string, int>),
        typeof(Dictionary<string, string>),
        typeof(Dictionary<string, bool>),
        typeof(Dictionary<string, UnityEngine.Object>),
        typeof(List<string>),
        typeof(List<int>),
    };

     [CSharpCallLua]
     public static List<Type> CSharpCallLua = new List<Type>() {
        typeof(DG.Tweening.TweenCallback),
        typeof(DG.Tweening.TweenCallback<float>),

        typeof(Action),
        typeof(Action<float>),
        typeof(Action<float, float>),
        typeof(Action<Vector2>),
        typeof(Action<Vector3>),
        typeof(Action<GameObject>),

        typeof(Func<string>),

        typeof(System.Collections.IEnumerator),        
     };
}
    