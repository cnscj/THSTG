using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame
{
    public static class PublicUtil
    {
        public static void SetGameObjecActive(GameObject GObj, bool isActive)
        {
            if (GObj == null)
                return;

            if (GObj.activeInHierarchy == isActive)
                return;

            GObj.SetActive(isActive);
        }


        public static void SetGameObjecttVisible(GameObject obj, bool bActive, int distance = 10000)
        {
            if (obj == null)
                return;

            Animator[] Anis = obj.GetComponentsInChildren<Animator>();
            //没有animator 显影直接SetActive true/fasle
            if (Anis.Length <= 0)
            {
                obj.SetActive(bActive);
                return;
            }

            for (int i = 0, count = Anis.Length; i < count; i++)
            {
                Anis[i].enabled = bActive;
                if (bActive)
                {
                    AnimatorStateInfo anif = Anis[i].GetCurrentAnimatorStateInfo(0);
                    Anis[i].Play(anif.fullPathHash, 0, 0);
                }
            }

            ParticleSystem[] pars = obj.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0, count = pars.Length; i < count; i++)
            {
                if (bActive)
                {
                    pars[i].time = 0;
                    pars[i].Play(false);  // false参数不递归子物体
                }
                else
                {
                    pars[i].Clear(false);
                    pars[i].Stop(false);
                }
            }

            Vector3 pos = obj.transform.localPosition;
            if (bActive)
            {
                if (pos.x > distance)
                {
                    pos.x = pos.x - distance;
                }
            }
            else
            {
                if (pos.x < distance)
                {
                    pos.x = pos.x + distance;
                }
            }

            obj.transform.localPosition = pos;
        }
    }

}
