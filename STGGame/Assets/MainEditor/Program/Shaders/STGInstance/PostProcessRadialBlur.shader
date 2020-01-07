Shader "Hidden/STGInstance/PostProcessRadialBlur"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _Range ("Range", Float) = 1
        _Shape ("Shape", Float) = 1
        _Alpha ("Alpha", Float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
	
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
	
            #include "UnityCG.cginc"
	
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
	
            struct v2f
            {
                float2 uv : TEXCOORD0;                
                float4 vertex : SV_POSITION;
            };
	
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = float4(v.vertex.xy, 0.0, 1.0);            
                o.uv = (v.vertex.xy + 1.0) * 0.5;
	
                #if UNITY_UV_STARTS_AT_TOP
                o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
                #endif
                
                return o;
            }
	
            sampler2D _MainTex;
            half _Range;
            half _Shape;
            half _Alpha;
            
            half4 frag (v2f i) : SV_Target
            {
                half2 uvViewPort = i.uv * 2 - 1;                
                half rate = dot(uvViewPort, uvViewPort) * _Range;
                rate = pow(rate, _Shape);
                uvViewPort *= rate;
                
                half4 tex = tex2D(_MainTex, i.uv);
                half4 col = tex;
                col += tex2D(_MainTex, i.uv - uvViewPort * 0.015);
                col += tex2D(_MainTex, i.uv - uvViewPort * 0.02);
                col += tex2D(_MainTex, i.uv - uvViewPort * 0.023);
                col += tex2D(_MainTex, i.uv - uvViewPort * 0.025);
                col *= 0.2;
                return lerp(tex, col, _Alpha * rate);
            }
            ENDCG
        }
    }

    Fallback Off
}
