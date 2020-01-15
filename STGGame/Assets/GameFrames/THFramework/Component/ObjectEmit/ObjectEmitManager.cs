using System.Collections.Generic;
using XLibrary.Package;

namespace THGame
{
    public class ObjectEmitManager : MonoSingleton<ObjectEmitManager>
    {
        private static readonly ObjectEmitHandler defaultLaunchListener = new ObjectEmitHandler();

        public int randimRoot;  //随机数种子
        
        private System.Random m_random;
        private ObjectEmitHandler m_launchListener = defaultLaunchListener;
        private HashSet<ObjectEmitter> m_emitterSet;                    //总记录
        private Dictionary<string,List<ObjectEmitter>> m_emittersMap;   //记录那些带key的

        public System.Random GetRandom()
        {
            m_random = m_random ?? new System.Random(randimRoot);
            return m_random;
        }

        public void UpdateRandom(int root = 0)
        {
            randimRoot = root;
            if (m_random != null)
            {
                m_random = new System.Random(root);
            }
        }

        public void SetHandler(ObjectEmitHandler listener)
        {
            m_launchListener = (listener ?? defaultLaunchListener);
        }

        public ObjectEmitHandler GetHandler()
        {
            return m_launchListener;
        }

        public void AddEmitter(ObjectEmitter emitter, string name = null)
        {
            m_emitterSet = m_emitterSet ?? new HashSet<ObjectEmitter>();
            if (m_emitterSet.Contains(emitter))
                return;

            m_emitterSet.Add(emitter);

            AddToMap(emitter, name);
        }

        public void RemoveEmitter(ObjectEmitter emitter)
        {
            if (emitter == null)
                return;

            if (m_emitterSet != null)
            {
                m_emitterSet.Remove(emitter);
            }

            RemoveFromMap(emitter);
        }


        public List<ObjectEmitter> GetEmitters(string name)
        {
            if (string.IsNullOrEmpty(name) && m_emitterSet != null)
            {
                return new List<ObjectEmitter>(m_emitterSet);
            }

            if (m_emittersMap != null)
            {
                if (m_emittersMap.ContainsKey(name))
                {
                    return m_emittersMap[name];
                }
            }

            return null;
        }

        public void PauseAll()
        {
            if (m_emitterSet != null)
            {
                foreach (var emitter in m_emitterSet)
                {
                    emitter.Pause();
                }
            }

        }

        public void ResumeAll()
        {
            if (m_emitterSet != null)
            {
                foreach (var emitter in m_emitterSet)
                {
                    emitter.Resume();
                }
            }
        }

        private void AddToMap(ObjectEmitter emitter, string name)
        {
            if (emitter == null)
                return;

            if (string.IsNullOrEmpty(name))
                return;

            m_emittersMap = m_emittersMap ?? new Dictionary<string, List<ObjectEmitter>>();
            List<ObjectEmitter> list = null;
            if (!m_emittersMap.TryGetValue(name, out list))
            {
                list = new List<ObjectEmitter>();
                m_emittersMap.Add(name,list);
            }
            list.Add(emitter);
        }
        private void RemoveFromMap(ObjectEmitter emitter)
        {
            if (emitter == null)
                return;

            if (string.IsNullOrEmpty(emitter.launchName))
                return;

            if (m_emittersMap == null)
                return;

            if (!m_emittersMap.ContainsKey(emitter.launchName))
                return;

            m_emittersMap[emitter.launchName].Remove(emitter);
        }
    }

}
