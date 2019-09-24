using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ASGame
{
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }

    public class AnimationDemandList : MonoBehaviour
    {
        public string abPath;
        public string animCtrlPath;
        public string otherInfo;
        public List<string> animsList = new List<string>();

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
                        m_animator.runtimeAnimatorController = m_animatorOverrideController;//会崩溃,在lua层这么掉也会
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

        private void Start()
        {

        }

        private Dictionary<string, string> GetAnimMap()
        {
            if (m_animsMap == null)
            {
                if (animsList.Count > 0)
                {
                    m_animsMap = new Dictionary<string, string>();
                    m_animsMap.Clear();
                    foreach (var path in animsList)
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
