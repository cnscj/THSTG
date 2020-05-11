using System;
using System.Reflection;
using Entitas;
using STGU3D;
using UnityEngine;

namespace STGGame
{
    public class EntityConverter : MonoBehaviour
    {
        public delegate void Callback(GameEntity entity);
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
        public Callback callback;

        public ComponentData[] comsList;

        void Start()
        {
            //用其他转换
            if (string.IsNullOrEmpty(entityCode))
            {
                RefreshCode();
            }
            if( !string.IsNullOrEmpty(entityCode) )
            {
                var entity = EntityManager.GetInstance().CreateEntity(entityCode);
                if (entity != null)
                {
                    var transCom = (TransformComponent)entity.GetComponent(GameComponentsLookup.Transform);
                    if (transCom != null)
                    {
                        transCom.localPosition = gameObject.transform.position;
                        transCom.localRotation = gameObject.transform.eulerAngles;

                        entity.ReplaceComponent(GameComponentsLookup.Transform, transCom);
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
                        //标记为编辑器生成的
                        var editorEntity = entity.CreateComponent<EditorEntityComponent>(GameComponentsLookup.EditorEntity);
                        entity.AddComponent(GameComponentsLookup.EditorEntity, editorEntity);

                        //直接作为Node节点
                        if (entity.hasView)
                        {
                            var unityView = entity.view.view as UnityView;
                            if (unityView != null)
                            {
                                unityView.node = gameObject;    //因此node在此前必须为null
                            }
                        }
                    }

                    if (callback != null)
                    {
                        callback.Invoke(entity);
                        callback = null;
                    }
                }

                
            }
            if (!isLink)
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                GameObject.Destroy(this);
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
                object oriObj = obj;
                Type ts = obj.GetType();
                PropertyInfo propertyInfo = null;
                FieldInfo fieldInfo = null;
                for (int i = 0; i < fieldNameArray.Length; i++)
                {
                    string fieldName = fieldNameArray[i];

                    propertyInfo = ts.GetRuntimeProperty(fieldName);
                    fieldInfo = ts.GetRuntimeField(fieldName);
                    ts = propertyInfo == null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                    if (i < fieldNameArray.Length - 1) obj = propertyInfo == null ? fieldInfo.GetValue(obj) : propertyInfo.GetValue(obj, null);
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
                    
                    propertyInfo = ts.GetRuntimeProperty(fieldName);
                    fieldInfo = ts.GetRuntimeField(fieldName);
                    ts = propertyInfo == null ? fieldInfo.FieldType : propertyInfo.PropertyType;

                    if (i < fieldNameArray.Length - 1)
                    {
                        
                        if (propertyInfo == null)
                        {
                            //TODO:在这里就改失败了,因为执行obj = newObj是传值赋值,不在是修改原来的了
                            //obj = fieldInfo.GetValueDirect(__makeref(obj));                   //TODO:传值完蛋,这里就不行了
                            obj = fieldInfo.GetValue(obj);
                        }
                        else
                        {
                            obj = propertyInfo.GetValue(obj, null);
                        }
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
            if (!enabled) return;

            Gizmos.color = Color.red;                       //为随后绘制的gizmos设置颜色。
            Gizmos.DrawWireSphere(transform.position, .20f);//使用center和radius参数，绘制一个线框球体
        }
    }
}
