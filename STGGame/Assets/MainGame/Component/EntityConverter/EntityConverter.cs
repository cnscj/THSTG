using System;
using System.Reflection;
using Entitas;
using STGU3D;
using UnityEngine;

namespace STGGame
{
    public class EntityConverter : MonoBehaviour
    {
        [System.Serializable]
        public class ComponentData
        {
            public string attrName;
            public string attrValue;
        }
        public string entityCode;
        public bool isLink = true;

        public EEntityType entityType;
        public int type;
        public EHeroType heroType;
        public EBossType bossType;
        public EWingmanType wingmanType;

        public ComponentData[] comsList;

        void Start()
        {
            //用其他转换
            if (string.IsNullOrEmpty(entityCode))
            {
                RefreshCode();
            }
            if(!string.IsNullOrEmpty(entityCode))
            {
                var entity = EntityManager.GetInstance().CreateEntity(entityCode);
                if (entity != null)
                {
                    if (entity.hasTransform)
                    {
                        entity.transform.localPosition = gameObject.transform.localPosition;
                        entity.transform.localRotation = gameObject.transform.localEulerAngles;

                    }

                    if (comsList != null)
                    {
                        foreach (var comData in comsList)
                        {
                            ModifyComponent(entity, comData.attrName, comData.attrValue);
                        }
                    }


                    if (isLink)
                    {
                        //直接作为Node节点
                        if (entity.hasView)
                        {
                            var unityView = entity.view.view as UnityView;
                            if (unityView != null)
                            {
                                entity.view.isEditor = true;
                                unityView.node = gameObject;
                                GameObject.Destroy(this);
                            }
                        }
                    }
                }
            }
            if (!isLink)
            {
                GameObject.Destroy(gameObject);
            }
        }

        public void RefreshCode()
        {
            switch (entityType)
            {
                case EEntityType.Hero:
                    entityCode = EntityUtil.GetHeroCode(heroType);
                    break;
                case EEntityType.Boss:
                    entityCode = EntityUtil.GetBossCode(bossType);
                    break;
                case EEntityType.Wingman:
                    entityCode = EntityUtil.GetWingmanCode(wingmanType);
                    break;
                case EEntityType.Mob:
                    entityCode = EntityUtil.GetMobCode(type);
                    break;
                case EEntityType.Bullet:
                    entityCode = EntityUtil.GetBulletCode(type);
                    break;
                case EEntityType.Prop:
                    entityCode = EntityUtil.GetPropCode(type);
                    break;

            }
            
        }

        public object GetModelValue(string fieldPath, object obj)
        {
            try
            {
                string[] fieldNameArray = fieldPath.Split(new char[] { '.' });
                Type ts = obj.GetType();
                PropertyInfo propertyInfo = null;
                FieldInfo fieldInfo = null;
                foreach (var fieldName in fieldNameArray)
                {
                    propertyInfo = ts.GetProperty(fieldName);
                    fieldInfo = ts.GetField(fieldName);
                    ts = propertyInfo == null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                }


                object o = null;

                if (propertyInfo != null) o = propertyInfo.GetValue(obj, null);
                if (fieldInfo != null) o = fieldInfo.GetValue(obj); //这里是值引用,修改不会起效的,要重新赋值

                object value = Convert.ChangeType(o, ts);

                return value;
            }
            catch
            {
                return null;
            }
        }

        private bool SetModelValue(string fieldPath, string value, object obj)
        {
            try
            {
                string[] fieldNameArray = fieldPath.Split(new char[] {'.'});
                object oriObj = obj;
                Type ts = obj.GetType();
                PropertyInfo propertyInfo = null;
                FieldInfo fieldInfo = null;
                for (int i = 0; i < fieldNameArray.Length; i++)
                {
                    string fieldName = fieldNameArray[i];
                    
                    propertyInfo = ts.GetProperty(fieldName);
                    fieldInfo = ts.GetField(fieldName);
                    ts = propertyInfo == null ? fieldInfo.FieldType : propertyInfo.PropertyType;

                    //最后一个不用了
                    if (i < fieldNameArray.Length - 1)
                    {
                        //TODO:不知道这里是不是和传值赋值有关系,因为不起作用
                        obj = GetModelValue(fieldName, obj);
                    }

                }

                object v = Convert.ChangeType(value, ts);

                if (propertyInfo != null) propertyInfo.SetValue(obj, v, null);
                if (fieldInfo != null) fieldInfo.SetValue(obj, v);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ModifyComponent(GameEntity entity, string attrName, string attrValue)
        {
            //获取属性
            if (entity != null)
            {
                SetModelValue(attrName, attrValue, entity);
            }

        }

        private void OnDrawGizmosSelected()
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;                       //为随后绘制的gizmos设置颜色。
            Gizmos.DrawWireSphere(transform.position, .20f);//使用center和radius参数，绘制一个线框球体
        }
    }
}
