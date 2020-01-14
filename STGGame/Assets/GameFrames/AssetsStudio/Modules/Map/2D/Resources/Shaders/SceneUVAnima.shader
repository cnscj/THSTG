Shader "AS/Scene/SceneUVAnima"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _ScrollXSpeed ("X Scrll Speed", float) = 2
        _ScrollYSpeed ("Y Scrll Speed", float) = 2
        _ScrollZSpeed ("Z Scrll Speed", float) = 0
	}
	SubShader
	{
 
		Pass
		{
			CGPROGRAM  
			#pragma vertex vert  
			#pragma fragment frag	
			
			#include "UnityCG.cginc"

            float _ScrollXSpeed;
            float _ScrollYSpeed;
			float _ScrollZSpeed;
			sampler2D _MainTex;

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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			
			fixed4 frag (v2f i) : SV_Target	
			{
				float2 tempUV = i.uv;
 
				//平移到原点位置
				tempUV -= float2(0.5, 0.5);
				//旋转
				float angle = _Time.x * _ScrollZSpeed;
				float2 finalUV = 0;
				finalUV.x = tempUV.x * cos(angle) - tempUV.y * sin(angle);
				finalUV.y = tempUV.x * sin(angle) + tempUV.y * cos(angle);

                finalUV.x += _ScrollXSpeed * _Time;
                finalUV.y += _ScrollYSpeed * _Time;
 
				//平移回图片原位置
				finalUV+= float2(0.5, 0.5);
				return tex2D(_MainTex, finalUV);
			}
			ENDCG
		}
	}
}