
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
        //TODO：缺少缩放
        public void PlayRotatingNarrow(GameEntity entity, Action onCompleted = null)
        {
            entity.movement.moveSpeed = Vector3.zero;
            entity.movement.rotationSpeed = Vector3.zero;
            Tween rotationTweener = DOTween.To(() => {
                return entity.transform.localRotation.z;
            }, (val) => {
                var transComp = (TransformComponent)entity.GetComponent(GameComponentsLookup.Transform);
                transComp.localRotation.z = val;
                entity.ReplaceComponent(GameComponentsLookup.Transform, transComp);
            }, 720, 2);

            Tween scaleTweener = DOTween.To(() => {
                return entity.transform.localRotation.z;
            }, (val) => {
                var transComp = (TransformComponent)entity.GetComponent(GameComponentsLookup.Transform);
                transComp.localRotation.z = val;
                entity.ReplaceComponent(GameComponentsLookup.Transform, transComp);
            }, 720, 2);

            Sequence mySequence = DOTween.Sequence();
            mySequence.Join(rotationTweener);
            //mySequence.Join(scaleTweener);
            mySequence.onComplete = () =>
            {
                onCompleted?.Invoke();
            };

        }

        //TODO:一个闪烁的动画
        public void PlayFlash(GameEntity entity, Action onCompleted = null)
        {

        }
    }
}
