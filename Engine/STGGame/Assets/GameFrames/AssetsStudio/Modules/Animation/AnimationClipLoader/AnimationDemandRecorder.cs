using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ASGame
{
    public class AnimationDemandRecorder : MonoBehaviour
    {

        public AnimationDemandList infoObj;
        private Dictionary<string, string> m_animsMap;

        [HideInInspector] Animator m_animator;
        [HideInInspector] AnimatorOverrideController m_animatorOverrideController;
        [HideInInspector] AnimationClipOverrides m_clipOverrides;


        public string GetAnimPath(string name)
        {
            var animaMap = GetAnimMap();
            if (animaMap != null)
            {
                if (animaMap.ContainsKey(name))
                {
                    return animaMap[name];
                }
            }
            return null;
        }

        public bool isHaveAnim(string name)
        {
            if (m_clipOverrides != null)
            {
                return m_clipOverrides[name] != null;
            }
            return false;
        }

        public bool SetupOverrideCtrl(AnimatorOverrideController animatorOverrideController, bool isSetup)
        {
            if (animatorOverrideController != null)
            {
                m_animatorOverrideController = animatorOverrideController;
                m_clipOverrides = new AnimationClipOverrides(m_animatorOverrideController.overridesCount);
                m_animatorOverrideController.GetOverrides(m_clipOverrides);
                if (isSetup)
                {
                    if (m_animator != null)
                    {
                        m_animator.runtimeAnimatorController = m_animatorOverrideController;
                    }
                }
            }
            return false;
        }


        public AnimatorOverrideController SetupCtrl(RuntimeAnimatorController runtimeAnimatorController, bool isSetup)
        {
            if (runtimeAnimatorController != null)
            {
                var animatorOverrideController = new AnimatorOverrideController(runtimeAnimatorController);
                SetupOverrideCtrl(animatorOverrideController, isSetup);

                return m_animatorOverrideController;
            }
            return null;
        }

        public bool SetupAnim(AnimationClip clip, bool isForce = true)
        {
            if (clip && m_animatorOverrideController && m_clipOverrides != null)
            {
                if ((m_clipOverrides[clip.name] != null) && !isForce)
                {
                    return false;
                }

                m_animatorOverrideController.GetOverrides(m_clipOverrides);
                m_clipOverrides[clip.name] = clip;
                m_animatorOverrideController.ApplyOverrides(m_clipOverrides);

            }
            return false;
        }

        private void Awake()
        {

            m_animator = gameObject.GetComponent<Animator>();
        }

        private Dictionary<string, string> GetAnimMap()
        {
            if (m_animsMap == null)
            {
                if (infoObj.animsList.Count > 0)
                {
                    m_animsMap = new Dictionary<string, string>();
                    m_animsMap.Clear();
                    foreach (var path in infoObj.animsList)
                    {
                        string animName = Path.GetFileNameWithoutExtension(path);
                        if (!m_animsMap.ContainsKey(animName))
                        {
                            m_animsMap.Add(animName, path);
                        }
                        else
                        {
                            m_animsMap[animName] = path;
                        }
                    }

                }
            }
            return m_animsMap;
        }
    }
}
