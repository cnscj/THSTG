using System;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //某一系列技能的集合,每个技能互相独立
    public partial class SkillData
    {
        private Dictionary<int, SkillBean> _dictById;
        private Dictionary<SkillType, List<SkillBean>> _dictByType;
        //////////////////

        /// <summary>
        /// 根据skillId获取bean
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns>SkillBean</returns>
        public SkillBean GetSkillBean(int skillId)
        {
            var _dictById = GetDictBySkillId();
            if (_dictById == null || _dictById.Count <= 0)
                return default;

            _dictById.TryGetValue(skillId, out var skillBean);
            return skillBean;
        }

        /// <summary>
        /// 根据SkillCastType获取bean
        /// </summary>
        /// <param name="castType"></param>
        /// <returns>SkillBean</returns>
        public SkillBean[] GetSkillBeans(SkillType skillType)
        {
            var dictByType = GetDictByType();
            if (dictByType == null || dictByType.Count <= 0)
                return default;

            dictByType.TryGetValue(skillType, out var skillBeans);
            return skillBeans.ToArray();
        }

        private Dictionary<int, SkillBean> GetDictBySkillId()
        {
            if (_dictById == null)
            {
                _dictById = _dictById ?? new Dictionary<int, SkillBean>();
                foreach (var skillBean in skillBeans)
                {
                    var skillId = skillBean.skillId;
                    if (!_dictById.ContainsKey(skillId))
                    {
                        _dictById[skillId] = skillBean;
                    }
                }
            }

            return _dictById;
        }

        private Dictionary<SkillType, List<SkillBean>> GetDictByType()
        {
            if (_dictByType == null)
            {
                _dictByType = _dictByType ?? new Dictionary<SkillType, List<SkillBean>>();
                foreach(var skillBean in skillBeans)
                {
                    var castType = skillBean.skillType;
                    if (!_dictByType.TryGetValue(castType, out var skillBeanList))
                    {
                        skillBeanList = new List<SkillBean>();
                        _dictByType.Add(castType, skillBeanList);
                    }
                    skillBeanList.Add(skillBean);
                }
            }

            return _dictByType;
        }
    }

}
