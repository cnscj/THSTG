
using System;
using System.IO;
using DG.Tweening;
using UnityEngine;
using XLibrary.Package;

namespace STGU3D
{
    public class DotweenManager : MonoSingleton<DotweenManager>
    {
        //一个旋转消亡的动画
        public Tween PlayRotatingNarrow(GameEntity entity, Action onCompleted = null)
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
                }, 0, 2);
                mySequence.Join(scaleTweener);
            }
 
            mySequence.onComplete = () =>
            {
                onCompleted?.Invoke();
            };

            return mySequence;
        }

        //TODO:一个闪烁的动画
        public void PlayFlash(GameEntity entity, Action onCompleted = null)
        {

        }
    }
}
