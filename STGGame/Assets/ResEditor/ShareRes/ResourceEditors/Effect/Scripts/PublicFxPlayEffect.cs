using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace STGGame
{
    public class PublicFxPlayEffect : MonoBehaviour
    {
        public GameObject effectBtnPrefab;
        public GameObject effectList;
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
                foreach (var effect in effectsNode.GetComponentsInChildren<Transform>())
                {
                    if (effect.gameObject == effectsNode)
                    {
                        continue;
                    }

                    effectsList.Add(effect.gameObject);
                }

                //生成UI
                float buttonHeight = (effectBtnPrefab.transform as RectTransform).rect.height;
                int i = 0;
                float contentHeight = effectsList.Count * buttonHeight;
                foreach (var effect in effectsList)
                {
                    var go = Instantiate(effectBtnPrefab, effectList.transform);
                    go.name = effect.name;
                    go.SetActive(true);

                    RectTransform rt = go.transform as RectTransform;
                    rt.anchoredPosition = new Vector2(70, -70-(-i++ * buttonHeight + contentHeight / 2));


                    Text text = go.GetComponentInChildren<Text>();
                    text.text = effect.name;

                    Button btn = go.GetComponent<Button>();
                    btn.onClick.AddListener(() =>
                    {
                        var newFx = Instantiate(effect, m_playNode.transform);
                        Destroy(newFx, 10.0f);
                    });
                }
            }
        }

        private void OnValidate()
        {
            m_playNode = gameObject.transform.Find("PlayTempList").gameObject;
        }
    }

}
