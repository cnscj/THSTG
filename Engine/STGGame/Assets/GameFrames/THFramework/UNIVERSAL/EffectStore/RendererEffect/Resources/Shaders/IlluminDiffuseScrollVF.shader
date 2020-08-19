// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TH/Illumin/IlluminDiffuseScrollVF"   
{  
    Properties   
    {   
       	_Color ("Main Color", Color) = (1,1,1,1)  
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Illum ("Illumin (A)", 2D) = "white" {}
		_Tex("Scroll Tex (RGB)", 2D)= "white" {}
		_ScrollingSpeed("Scrolling speed", Vector) = (0,0,0,0)
    }  
      
    SubShader   
    {  
        
        Tags { "RenderType"="Opaque" }
		LOD 200
       
        pass  
        {         	
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #include "UnityCG.cginc"
            sampler2D _MainTex,_Illum,_Tex;  
            float4 _MainTex_ST,_Illum_ST,_Tex_ST,_Color,_ScrollingSpeed;
              
            struct appdata {  
                float4 vertex : POSITION;  
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;  
            };  
            
            struct v2f  {  
                float4 pos : POSITION;  
                float2 uv_MainTex : TEXCOORD0;
				float2 uv_Illum : TEXCOORD1; 
				float2 uvLM : TEXCOORD2;
				float2 uvScroll : TEXCOORD3;
            };  
            
            v2f vert (appdata v) 
            {  
                v2f o;  
                o.pos = UnityObjectToClipPos(v.vertex)*_Color;  
                o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv_Illum = TRANSFORM_TEX(v.texcoord, _Illum);
                o.uvScroll = TRANSFORM_TEX((v.texcoord.xy+_Time.x*_ScrollingSpeed.xy), _Tex);
                o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;    
                return o;  
            } 
             
            float4 frag (v2f i) : COLOR  
            {  
                float4 texCol = tex2D(_MainTex, i.uv_MainTex);
                float4 IllumTex = tex2D(_Illum,i.uv_Illum);
                float3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));    
                IllumTex.rgb+=tex2D(_Tex,i.uvScroll).rgb;
                IllumTex*=IllumTex.a;
                texCol+=IllumTex;
                texCol.rgb*=lm;
                return texCol;  
            }  
            ENDCG  
        }  
    } 
    FallBack "Diffuse" 
}  