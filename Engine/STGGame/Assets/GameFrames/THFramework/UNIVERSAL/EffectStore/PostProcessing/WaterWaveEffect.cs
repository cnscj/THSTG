﻿using UnityEngine;

namespace THGame
{
    public class WaterWaveEffect : BasePostEffect
    {
        //距离系数
        public float distanceFactor = 60.0f;
        //时间系数
        public float timeFactor = -30.0f;
        //sin函数结果系数
        public float totalFactor = 1.0f;

        //波纹宽度
        public float waveWidth = 0.3f;
        //波纹扩散的速度
        public float waveSpeed = 0.3f;
        //波纹开始位置
        private Vector4 startPos = new Vector4(0.5f, 0.5f, 0, 0);

        private float waveStartTime;

        protected override Shader OnShader()
        {
            return Shader.Find("Hidden/TH/PostProcessWaterWave");
        }

        public void Wave(float x,float y)
        {
            waveStartTime = Time.time;
            startPos.x = x;
            startPos.y = y;
        }
        public void WaveCenter()
        {
            Wave(0.5f, 0.5f);
        }
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Material)
            {
                //计算波纹移动的距离，根据enable到目前的时间*速度求解
                float curWaveDistance = (Time.time - waveStartTime) * waveSpeed;
                //设置一系列参数
                Material.SetFloat("_distanceFactor", distanceFactor);
                Material.SetFloat("_timeFactor", timeFactor);
                Material.SetFloat("_totalFactor", totalFactor);
                Material.SetFloat("_waveWidth", waveWidth);
                Material.SetFloat("_curWaveDis", curWaveDistance);
                Material.SetVector("_startPos", startPos);
                Graphics.Blit(source, destination, Material);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}
