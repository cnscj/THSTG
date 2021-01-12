using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class SkillParametersTrigger
    {
        public static bool VerifyTo(SkillTriggerParameters parameters, SkillTriggableConditions conditions)
        {
            if (parameters == null)
                return false;

            if (conditions == null)
                return false;

            if (conditions.list == null || conditions.list.Count <= 0)
                return false;

            bool ret = true;
            foreach (var keyValue in conditions.list)
            {
                var param = parameters.GetParam(keyValue.key);
                var value = param?.value ?? 0;
                ret &= keyValue.condition.Verify(value);
            }

            return ret;
        }
        //触发器类型的,判断完所有后下一帧直接置false
        private SkillTriggerParameters _parameters;
        private Queue<SkillTriggerParameter> _triggerReleaseList;

        public void AddParam(string key, SkillTriggableParameterType type)
        {
            GetParameters().AddParam(key, new SkillTriggerParameter() { type = type });
        }

        public void RemoveParam(string key)
        {
            GetParameters().RemoveParam(key);
        }

        public void SetInt(string key, int value)
        {
            var param = GetParameters().GetParam(key);
            if (param != null)
            {
                param.SetInt(value);
            }
        }

        public void SetFloat(string key, float value)
        {
            var param = GetParameters().GetParam(key);
            if (param != null)
            {
                param.SetFloat(value);
            }
        }

        public void SetBool(string key, bool value)
        {
            var param = GetParameters().GetParam(key);
            if (param != null)
            {
                param.SetBool(value);
            }
        }

        public void Trigge(string key)
        {
            var param = GetParameters().GetParam(key);
            if (param != null)
            {
                param.SetBool(true);
                GetTriggerReleaseList().Enqueue(param);
            }
        }

        public bool Verify(SkillTriggableConditions conditions)
        {
            return VerifyTo(_parameters, conditions);
        }

        public void UpdateLate()
        {
            Purge();
        }

        //帧后处理
        private void Purge()
        {
            if (_triggerReleaseList == null || _triggerReleaseList.Count <= 0)
                return;

            while (_triggerReleaseList.Count > 0)
            {
                var param = _triggerReleaseList.Dequeue();
                param.SetBool(false);
            }
        }

        private SkillTriggerParameters GetParameters()
        {
            _parameters = _parameters ?? new SkillTriggerParameters();
            return _parameters;
        }

        private Queue<SkillTriggerParameter> GetTriggerReleaseList()
        {
            _triggerReleaseList = _triggerReleaseList ?? new Queue<SkillTriggerParameter>();
            return _triggerReleaseList;
        }
    }  
}
