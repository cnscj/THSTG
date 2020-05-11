Shader "STG/EffectBase"
{
    Properties 
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _MainColor ("MainColor", Color) = (1,1,1,1)
        _Bright ("Bright", Range(1, 10)) = 1
        _BloomLimit ("Bloom Limit", Range(1, 10)) = 1
        
        //_RenderMode ("", Float) = 0.0
        //_SrcBlend ("", Float) = 0.0
        //_DstBlend ("", Float) = 1.0
       //_ZWrite ("", Float) = 0.0      
    }

    SubShader 
    {
        LOD 200
        Tags 
        { 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
            "RenderType" = "Transparent" 
            "PreviewType" = "Plane" 
        }
       //Blend [_SrcBlend] [_DstBlend]
        //ZWrite [_ZWrite]
        //Cull [_RenderMode]
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        ZWrite Off
        ColorMask RGB
        Lighting Off
        
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
    
            uniform sampler2D _MainTex; 
            uniform float4 _MainTex_ST;   
            uniform half4 _MainColor;
            uniform half _Bright;        
            uniform half _BloomLimit;
            
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
       
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _MainColor;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);                
    
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half4 col = i.color * tex2D(_MainTex, i.texcoord);
                col.rgb *= _Bright;
                return min(half4(_BloomLimit, _BloomLimit, _BloomLimit, 1), col);                
            }
            
            ENDCG
        }
    }
}
