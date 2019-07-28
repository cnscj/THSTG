using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame
{
    public class PublicFxPlayEffect : MonoBehaviour
    {
        private GameObject m_playNode;
        private List<GameObject> effectsList = new List<GameObject>();
        void Start()
        {
            Refresh();
        }

       
        void Update()
        {

        }

        void Play()
        {

        }

        void Refresh()
        {
            var effectsNode = PublicFxEditor.GetInstance().effectsNode;
            if (effectsNode)
            {
                effectsList.Clear();
                foreach (var effect in effectsNode.GetComponentsInChildren<Transform>(true))
                {
                    if (effect.gameObject == effectsNode)
                    {
                        continue;
                    }

                    effectsList.Add(effect.gameObject);
                }

                //生成UI
                foreach (var effect in effectsList)
                {

                }
            }
        }

        private void OnValidate()
        {
            m_playNode = gameObject.transform.Find("PlayTempList").gameObject;
        }
    }

}
