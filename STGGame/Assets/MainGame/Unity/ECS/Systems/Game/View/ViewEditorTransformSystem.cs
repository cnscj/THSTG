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
                     GameMatcher.Transform,
                     GameMatcher.EditorEntity
                ));
        }

        public void Execute()
        {

            foreach (var entity in __editorTransformGroup.GetEntities())
            {
                //存在1帧的延误
                if (entity.view.view != null)
                {
                    //因为是使用position做越界判断的,因此,entity的position也要一起变动
                    //影响entity的Trans
                    var pos = entity.view.view.Position;
                    var rot = entity.view.view.Rotation;

                    var transCom = entity.GetComponent(GameComponentsLookup.Transform) as TransformComponent;

                    transCom.position.x = pos.X;
                    transCom.position.y = pos.Y;
                    transCom.position.z = pos.Z;

                    transCom.rotation.x = rot.X;
                    transCom.rotation.y = rot.Y;
                    transCom.rotation.z = rot.Z;

                    entity.ReplaceComponent(GameComponentsLookup.Transform, transCom);
                }
            }
        }
    }
}

