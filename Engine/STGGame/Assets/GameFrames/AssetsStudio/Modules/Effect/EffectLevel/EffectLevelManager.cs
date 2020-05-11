using UnityEngine;
using System.Collections;
using System.IO;
using XLibrary.Package;
using System.Collections.Generic;

namespace ASGame
{
    public class EffectLevelManager :Singleton<EffectLevelManager>
    {
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
    }
}

