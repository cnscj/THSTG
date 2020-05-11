using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillPlayable : MonoBehaviour
    {
        public Animator animator;
        public SkillPlayableItemData[] skillList;
        private Dictionary<string, SkillPlayableItemData> m_skillDic;
        
        public void Awake()
        {
            Init();
        }

        //施展技能
        public void Cast(string skill)
        {
            if (animator = null) return;
            
        }

        //中断技能
        public void Break()
        {
            if (animator = null) return;

            
        }

        public void Load()
        {

        }

        //
        private void Init()
        {
            if (skillList != null)
            {
                if (skillList.Length > 0)
                {
                    m_skillDic = m_skillDic ?? new Dictionary<string, SkillPlayableItemData>();
                    m_skillDic.Clear();
                    foreach (var item in skillList)
                    {
                        m_skillDic[item.skillName] = item;
                    }
                }
            }

            animator = animator ?? GetComponentInChildren<Animator>();
        }
    }

}
