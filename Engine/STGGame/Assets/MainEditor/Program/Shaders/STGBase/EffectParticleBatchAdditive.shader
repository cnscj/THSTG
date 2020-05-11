Shader "STG/EffectParticleBatchAdditive"
{
    Properties 
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Bright ("Bright", Range(1, 10)) = 1
        _BloomLimit ("Bloom Limit", Range(1, 10)) = 1
        
    }

    SubShader 
    {
        Tags 
        { 
            "Queue" = "Transparent+100" 
            "IgnoreProjector" = "True" 
            "RenderType" = "Transparent" 
            "PreviewType" = "Plane" 
        }
        Blend SrcAlpha One
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
    
            #include "UnityCG.cginc"
    
            sampler2D _MainTex;    
            half _Bright;        
            half _BloomLimit;
            
            struct appdata_t 
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
    
            struct v2f 
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;                
                UNITY_VERTEX_OUTPUT_STEREO
            };
    
            float4 _MainTex_ST;
    
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);                
    
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half4 col = 2.0f * i.color * tex2D(_MainTex, i.texcoord);
                col.rgb *= _Bright;
                return min(half4(_BloomLimit, _BloomLimit, _BloomLimit, 1), col);                
            }
            
            ENDCG
        }
    }

    Fallback Off
}
