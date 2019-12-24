
using UnityEngine;

namespace THGame
{
    public class ObjectEmitListener
    {
        //实例创建
        public virtual ObjectEmitCreateResult OnCreate(ObjectEmitCreateParams args)
        {
            ObjectEmitCreateResult result = new ObjectEmitCreateResult();
            if (args.parent != null)
            {
                var parent = args.parent as GameObject;
                var prefab = args.prefab as GameObject;
                result.entity = Object.Instantiate(prefab, parent.transform, false);
            }
            else
            {
                var prefab = args.prefab as GameObject;
                result.entity = Object.Instantiate(prefab);
            }

            return result;
        }

        //发射
        public virtual void OnLaunch(ObjectEmitLaunchParams args)
        {
            var entity = args.createResult.entity as GameObject;
            Transform trans = entity.GetComponent<Transform>();
            ObjectEmitController ctrl = entity.GetComponent<ObjectEmitController>();
            if (ctrl == null) ctrl = entity.AddComponent<ObjectEmitController>();

            trans.position = args.calculateResult.startPosition;
            trans.eulerAngles = args.calculateResult.startEulerAngles;

            ctrl.moveSpeed = args.calculateResult.startMoveSpeed;
            ctrl.rotationSpeed = args.calculateResult.startAngleSpeed;

            
        }
    }
}

