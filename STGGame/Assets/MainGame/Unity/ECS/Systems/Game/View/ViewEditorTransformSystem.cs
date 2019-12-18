using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    //TODO:非常消耗性能
    public class ViewEditorTransformSystem : IExecuteSystem
    {
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
                if (entity.view.view != null)
                {
                    if (entity.view.isEditor)
                    {
                        //影响entity的Trans
                        float posX = 0f, posY = 0f, posZ = 0f;
                        float rotX = 0f, rotY = 0f, rotZ = 0f;
                        entity.view.view.GetPosition(ref posX, ref posY, ref posZ);
                        entity.view.view.GetRotation(ref rotX, ref rotY, ref rotZ);

                        var transCom = entity.GetComponent(GameComponentsLookup.Transform) as TransformComponent;

                        transCom.localPosition.x = posX;
                        transCom.localPosition.y = posY;
                        transCom.localPosition.z = posZ;

                        transCom.localRotation.x = rotX;
                        transCom.localRotation.y = rotY;
                        transCom.localRotation.z = rotZ;

                        //entity.ReplaceComponent(GameComponentsLookup.Transform, transCom);
                    }
                }
            }

        }
    }
}

