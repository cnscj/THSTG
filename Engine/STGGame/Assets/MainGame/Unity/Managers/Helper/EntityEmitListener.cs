﻿using STGGame;
using THGame;
using UnityEngine;

namespace STGU3D
{
    public class EntityEmitListener : ObjectEmitHandler
    {
        //实例创建
        public override ObjectEmitCreateResult OnCreate(ObjectEmitCreateParams args)
        {
            return base.OnCreate(args);
        }


        //发射
        public override void OnLaunch(ObjectEmitLaunchParams args)
        {
            var gobj = args.createResult.entity as GameObject;
            Transform trans = gobj.GetComponent<Transform>();
            ObjectEmitController ctrl = gobj.GetComponent<ObjectEmitController>();
            if (ctrl == null) ctrl = gobj.AddComponent<ObjectEmitController>();

            trans.position = args.calculateResult.startPosition;
            trans.eulerAngles = args.calculateResult.startEulerAngles;

            ctrl.moveSpeed = args.calculateResult.startMoveSpeed;
            ctrl.rotationSpeed = args.calculateResult.startAngleSpeed;

            //应该是转变为Entity的Movement
            var convertCom = gobj.GetComponent<EntityConverter>();
            if (convertCom != null)
            {
                convertCom.callback += (GameEntity entity) =>
                {
                    if (entity.hasMovement && !entity.isEditorEntity)
                    {
                        entity.movement.moveSpeed = ctrl.moveSpeed;
                        entity.movement.rotationSpeed = ctrl.rotationSpeed;
                        Object.Destroy(ctrl);
                    }
                };
            }

        }
    }
}

