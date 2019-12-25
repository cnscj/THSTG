using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    //TODO:非常消耗性能
    public class ViewEditorTransformSystem : IExecuteSystem
    {
        private float posX = 0f, posY = 0f, posZ = 0f;
        private float rotX = 0f, rotY = 0f, rotZ = 0f;
        private IGroup<GameEntity> __editorTransformGroup;
        public ViewEditorTransformSystem(Contexts contexts)
        {
            //移动
            __editorTransformGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.View,
                     GameMatcher.Transform
                ));
        }

        public void Execute()
        {

            foreach (var entity in __editorTransformGroup.GetEntities())
            {
                //存在1帧的延误
                if (entity.view.isEditor)
                {
                    if (entity.view.view != null)
                    {
                        //因为是使用position做越界判断的,因此,entity的position也要一起变动
                        //影响entity的Trans

                        entity.view.view.GetPosition(ref posX, ref posY, ref posZ);
                        entity.view.view.GetRotation(ref rotX, ref rotY, ref rotZ);

                        var transCom = entity.GetComponent(GameComponentsLookup.Transform) as TransformComponent;

                        transCom.position.x = posX;
                        transCom.position.y = posY;
                        transCom.position.z = posZ;

                        transCom.rotation.x = rotX;
                        transCom.rotation.y = rotY;
                        transCom.rotation.z = rotZ;

                        entity.ReplaceComponent(GameComponentsLookup.Transform, transCom);
                    }
                }
            }

        }
    }
}

