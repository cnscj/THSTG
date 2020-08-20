using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GYGame
{
    // 主角遮挡效果
    public class HeroShelterEffect : MonoBehaviour
    {
        public new Camera camera;
        CommandBuffer m_command;
        Material m_material;

        void OnEnable()
        {
            if (m_command == null)
            {
                m_command = new CommandBuffer();
                m_material = new Material(Shader.Find("Hidden/GY/ModelShelter"));             
                SkinnedMeshRenderer[] coms = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var com in coms)
                {
                    var sharedMaterials = com.sharedMaterials;
                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        m_command.DrawRenderer(com, m_material, i);
                    }
                }
            }

            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, m_command);
        }

        void OnDisable()
        {
            if (m_command != null)
            {
                camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, m_command);
            }
        }

        void OnDestroy()
        {
            if (m_material != null)
            {
                Destroy(m_material);
                m_material = null;
            }

            if (m_command != null)
            {
                m_command.Clear();
                m_command.Release();
                m_command = null;
            }
        }

       
    }
}