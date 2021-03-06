﻿using XLibrary.Package;
using System.Collections.Generic;

namespace ASGame
{
    public class EffectLevelManager : Singleton<EffectLevelManager>
    {
        public int Level { get; private set; }
        private HashSet<EffectLevelController> m_allCtrls;
      
        public void AddController(EffectLevelController ctrl)
        {
            if (ctrl == null)
                return;

            m_allCtrls = m_allCtrls ?? new HashSet<EffectLevelController>();
            if (!m_allCtrls.Contains(ctrl))
            {
                m_allCtrls.Add(ctrl);
            }
        }
        public void RemoveController(EffectLevelController ctrl)
        {
            if (ctrl == null)
                return;

            if (m_allCtrls == null)
                return;

            if (m_allCtrls.Contains(ctrl))
            {
                m_allCtrls.Remove(ctrl);
            }
        }
        public List<EffectLevelController> GetControllers()
        {
            if (m_allCtrls == null)
                return null;

            if (m_allCtrls.Count <= 0)
                return null;

            return new List<EffectLevelController>(m_allCtrls);
        }

        ///////////

        /// <summary>
        /// 方式1:显隐的方式
        /// </summary>
        /// <param name="lv"></param>
        public void AdjustControllers(int lv)
        {
            if (m_allCtrls == null || m_allCtrls.Count <= 0)
                return;

            foreach(var ctrl in m_allCtrls)
            {
                if (ctrl == null)
                    continue;

                ctrl.Adjust(lv);
            }
            Level = lv;
        }

        /// <summary>
        /// 方式2:节点的方式
        /// </summary>
        /// <param name="lv"></param>
        public void ChangeControllers(int lv)
        {
            if (m_allCtrls == null || m_allCtrls.Count <= 0)
                return;

            foreach (var ctrl in m_allCtrls)
            {
                if (ctrl == null)
                    continue;

                ctrl.Change(lv, ctrl.gameObject);
            }
            Level = lv;
        }
    }

}

