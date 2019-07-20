// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TH/BaseVortex2" {
Properties
{
[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
_CloudTex("噪声",2D) ="white"{}
_Color ("Tint", Color) = (1,1,1,1)

_StencilComp ("Stencil Comparison", Float) = 8
_Stencil ("Stencil ID", Float) = 0
_StencilOp ("Stencil Operation", Float) = 0
_StencilWriteMask ("Stencil Write Mask", Float) = 255
_StencilReadMask ("Stencil Read Mask", Float) = 255

_ColorMask ("Color Mask", Float) = 15
}

SubShader
{
Tags
{ 
"Queue"="Transparent" 
"IgnoreProjector"="True" 
"RenderType"="Transparent" 
"PreviewType"="Plane"
"CanUseSpriteAtlas"="True"
}

Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp] 
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Cull Off
Lighting Off
ZWrite Off
ZTest [unity_GUIZTestMode]
Fog { Mode Off }
Blend SrcAlpha OneMinusSrcAlpha
ColorMask [_ColorMask]

Pass
{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
float4 vertex : POSITION;
float4 color : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float4 vertex : SV_POSITION;
fixed4 color : COLOR;
half2 texcoord : TEXCOORD0;
};

fixed4 _Color;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
OUT.color = IN.color * _Color;
return OUT;
}

sampler2D _MainTex;
sampler2D _CloudTex;

fixed4 frag(v2f IN) : SV_Target
{
half2 uv = IN.texcoord;
//uv旋转
half t = _Time.y*2;
t= ( (0.6-length(uv-half2(0.5,0.5)))*2 ) *t;    
uv = uv-half2(0.5,0.5);
half x = uv.x*cos(t)-uv.y*sin(t);
half y = uv.x*sin(t)+uv.y*cos(t);
half2 newUV = half2(x,y)+half2(0.5,0.5);
half4 color = tex2D(_CloudTex,newUV ) * IN.color;

clip (color.a - 0.01);
return color;
}
ENDCG
}
}
}