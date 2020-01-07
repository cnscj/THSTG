Shader "Hidden/STGInstance/PostProcessBloom"
{    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        
        CGINCLUDE
        
        struct Attributes
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };
	
        struct Varyings
        {
            float4 vertex : SV_POSITION;
            half2 texcoord : TEXCOORD0;
            half4 texcoordBox1 : TEXCOORD1;
            half4 texcoordBox2 : TEXCOORD2;
        };
        
        sampler2D _MainTex;
        float4 _MainTex_TexelSize;        
        
        Varyings Vert(Attributes v)
        {
            Varyings o;            
            o.vertex = float4(v.vertex.xy, 0.0, 1.0);
            o.texcoord = (v.vertex.xy + 1.0) * 0.5;
	
            #if UNITY_UV_STARTS_AT_TOP
            o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
            #endif
            
            #if BOX4_SAMPLE
            float sam = 1;
            half4 uvoffset = _MainTex_TexelSize.xyxy * half4(sam, sam, -sam, sam);
            o.texcoordBox1 = o.texcoord.xyxy + uvoffset;
            o.texcoordBox2 = o.texcoord.xyxy - uvoffset;
            #endif
            
            return o;
        }
        
        half4 Prefilter(Varyings i) : SV_Target
        {
            #if BOX4_SAMPLE
            half4 color1 = tex2D(_MainTex, i.texcoordBox1.xy);            
            half4 color2 = tex2D(_MainTex, i.texcoordBox1.zw);
            half4 color3 = tex2D(_MainTex, i.texcoordBox2.xy);
            half4 color4 = tex2D(_MainTex, i.texcoordBox2.zw);            
            half4 color = (color1 + color2 + color3 + color4) * 0.25;            
            #else
            half4 color = tex2D(_MainTex, i.texcoord);
            #endif
            half4 leak = max(0, color - 1);
            half br = dot(leak.rgb, half3(0.15, 0.15, 0.15));
            return leak + br;
        }
        
        half4 DowmSample(Varyings i) : SV_Target
        {
            #if BOX4_SAMPLE            
            half4 color1 = tex2D(_MainTex, i.texcoordBox1.xy);            
            half4 color2 = tex2D(_MainTex, i.texcoordBox1.zw);
            half4 color3 = tex2D(_MainTex, i.texcoordBox2.xy);
            half4 color4 = tex2D(_MainTex, i.texcoordBox2.zw);            
            half4 color = (color1 + color2 + color3 + color4) * 0.25;            
            #else
            half4 color = tex2D(_MainTex, i.texcoord);            
            #endif            
            return color;
        }
      
        sampler2D _BloomTex;
        half4 UpSample(Varyings i) : SV_Target
        {
            #if BOX4_SAMPLE
            half4 color1 = tex2D(_MainTex, i.texcoordBox1.xy);            
            half4 color2 = tex2D(_MainTex, i.texcoordBox1.zw);
            half4 color3 = tex2D(_MainTex, i.texcoordBox2.xy);
            half4 color4 = tex2D(_MainTex, i.texcoordBox2.zw);            
            half4 colorDown = (color1 + color2 + color3 + color4) * 0.25;            
            #else
            half4 colorDown = tex2D(_MainTex, i.texcoord);
            #endif       
            half4 colorUp = tex2D(_BloomTex, i.texcoord);
                        
            return colorDown + colorUp * 0.5;
        }
	
        ENDCG
        
        Pass
        {
            CGPROGRAM
	
            #pragma vertex Vert
            #pragma fragment Prefilter            
            //#pragma multi_compile BOX4_SAMPLE
            
            ENDCG
        }
        
        Pass
        {
            CGPROGRAM
	
            #pragma vertex Vert
            #pragma fragment DowmSample            
            #pragma multi_compile BOX4_SAMPLE
            
            ENDCG
        }
        
        Pass
        {
            CGPROGRAM
	
            #pragma vertex Vert
            #pragma fragment UpSample            
            #pragma multi_compile BOX4_SAMPLE
            
            ENDCG
        }
    }
    
    FallBack Off
}
