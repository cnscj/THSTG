Shader "Hidden/TH/ModelShelter" 
{
    SubShader 
    {
        LOD 200
        Tags { "IgnoreProjector" = "True" "RenderType" = "Opaque" }        

        Pass 
        {
            Blend SrcAlpha One
            ZWrite Off
            ZTest Greater
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"            

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;                
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                half3 normal : NORMAL;                
            };
 
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                return half4(0.4, 0.4, 0.6, 1) * step(i.normal.z, 0.4);
            }

            ENDCG
        }        
    }

    Fallback Off
}
