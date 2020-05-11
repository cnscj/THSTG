
using System;
using System.IO;
using DG.Tweening;
using UnityEngine;
using XLibrary.Package;

namespace STGU3D
{
    public class DotweenManager : MonoSingleton<DotweenManager>
    {
        //全局操作
        public int Play(object targetOrId)
        {
            return DOTween.Play(targetOrId);
        }
        public int Pause(object targetOrId)
        {
            return DOTween.Pause(targetOrId);
        }
        public int Kill(object targetOrId, bool complete = false)
        {
            return DOTween.Kill(targetOrId, complete);
        }


        //一个旋转消亡的动画
        public Tween PlayRotatingNarrow(GameEntity entity)
        {
            entity.movement.moveSpeed = Vector3.zero;
            entity.movement.rotationSpeed = Vector3.zero;
            Sequence mySequence = DOTween.Sequence();

            Tween rotationTweener = DOTween.To(() => 
            {
                return entity.transform.localRotation.z;
            }, (val) => 
            {
                var transComp = (TransformComponent)entity.GetComponent(GameComponentsLookup.Transform);
                transComp.localRotation.z = val;
                entity.ReplaceComponent(GameComponentsLookup.Transform, transComp);
            }, 720, 2);
            mySequence.Join(rotationTweener);

            if (entity.hasView && entity.view.view != null)
            {
                Tween scaleTweener = DOTween.To(() =>
                {
                    return entity.view.view.Scale.X;
                }, (val) =>
                {
                    var scale = entity.view.view.Scale;
                    scale.X = val;
                    scale.Y = val;
                    entity.view.view.Scale = scale;
                }, 0f, 2f);

                mySequence.Join(scaleTweener);
            }

            return mySequence;
        }

        /// <summary>
        /// 闪烁动画
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="times">次数</param>
        /// <param name="length">总时长</param>
        /// <returns></returns>
        public Tween PlayFlash(GameEntity entity, int times, float length)
        {
            Sequence mySequence = DOTween.Sequence();
            if (times > 0 && entity.hasView && entity.view.view != null)
            {
                var unityView = (UnityView)entity.view.view;
                float duration = Mathf.Abs(length / times);
                int loops = length < 0 ? -1 : times;
                unityView.viewCtrl.shaderEffectCom.SetAlpha(0f);
                Tween alphaTweener = DOTween.To(() =>
                {
                    return unityView.viewCtrl.shaderEffectCom.GetAlpha();
                }, (val) =>
                {
                    unityView.viewCtrl.shaderEffectCom.SetAlpha(val);
                }, 1f, duration).SetLoops(loops, LoopType.Restart);
                mySequence.Join(alphaTweener);
            }
            
            return mySequence;
        }
    }
}
