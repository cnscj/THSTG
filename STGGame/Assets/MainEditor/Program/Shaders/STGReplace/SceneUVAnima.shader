Shader "STG/Scene/SceneUVAnima"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _CellRowAmount("Cell Row Amount", float) = 1
        _CellColumnAmount("Cell Column Amount", float) = 1
        _Speed("Speed", Range(0.01, 32)) = 12
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4 _MainTex_TexelSize;

        float _CellRowAmount;
        float _CellColumnAmount;
        float _Speed;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 spriteUV = IN.uv_MainTex;

            float cellUVRowPercentage = 1 / _CellRowAmount;
            float cellUVColumnPercentage = 1 / _CellColumnAmount;

            int index = fmod(_Time.y * _Speed, _CellRowAmount * _CellColumnAmount);

            int rowIndex = index / _CellColumnAmount;
            int columnIndex = index - (rowIndex * _CellColumnAmount);//fmod(index, _CellColumnAmount);

            float xValue = spriteUV.x / _CellColumnAmount;
            xValue += columnIndex * cellUVColumnPercentage;
            float yValue = (_CellRowAmount - 1 + spriteUV.y) / _CellRowAmount;
            yValue -= rowIndex * cellUVRowPercentage;

            spriteUV = float2(xValue, yValue);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, spriteUV) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}