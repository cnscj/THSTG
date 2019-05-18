// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TH/BaseVortex" {  
    Properties {  
        //start-------------------------------------------------------------  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
        //end-------------------------------------------------------------  
    }  
  
    CGINCLUDE  
        #include "UnityCG.cginc"  
        #pragma target 3.0    
  
        #define vec2 float2  
        #define vec3 float3  
        #define vec4 float4  
        #define mat2 float2x2  
        #define iGlobalTime _Time.y  
        #define mod fmod  
        #define mix lerp  
        #define atan atan2  
        #define fract frac   
        #define texture2D tex2D  
        // 屏幕分辨率  
        #define iResolution _ScreenParams  
        // 所有像素点（0,屏幕分辨率）为左下的参数。xy/w转为齐次坐标位置，屏幕左下为0，右上为1。  
        #define gl_FragCoord ((_iParam.srcPos.xy/_iParam.srcPos.w)*_ScreenParams.xy)    
        #define x_y iResolution.x / iResolution.y  
        //start-------------------------------------------------------------  
        sampler2D _MainTex;  
        //end-------------------------------------------------------------  
  
        // 顶点输出  
        struct vertOut {        
            float4 pos : SV_POSITION;        
            float4 srcPos : TEXCOORD0;      
        };      
  
        vertOut vert(appdata_base v) {      
            vertOut o;      
            o.pos = UnityObjectToClipPos (v.vertex);  
            o.srcPos = ComputeScreenPos(o.pos);     
            return o;      
        }  
        vec4 main(vec2 fragCoord);  
        fixed4 frag(vertOut _iParam) : COLOR0 {    
            vec2 fragCoord = gl_FragCoord;  
            return main(fragCoord);  
        }    
  
        //start-------------------------------------------------------------  
        vec4 filter(sampler2D tex, vec2 uv, float time)  
        {  
            // 圆形半径  
            float radius = 0.5;  
            // 中心点  
            vec2 center = vec2(0.5 * x_y,0.5);  
            // 将当前UV移到值域为[-1,1]的坐标中  
            vec2 tc = uv - center;  
            // 当前点的长度就是离中心点的距离  
            float dist = length(tc);  
            // 只处理在圆形内的点  
            if (dist < radius)  
            {  
                // 值域[0.5,1] 原点为1，边界为0.5  
                float percent = (radius - dist) / radius;  
                // 让圆内的点做sin曲线运动，效果为【圆形】交替变大变小
                // 乘以系数16是让下面【s型线条】效果更加明显  
                float theta = percent * percent * (sin(time))*16;  
                // s,c为坐标系中相对x轴对称的值  
                float s = sin(theta);  
                float c = cos(theta);  
                // 当前点和当前点对应的sin,cos值求点积
                // 将点积的值作为结果，将【圆形】变成【s型线条】
                tc = vec2(dot(tc, vec2(c, -s)), dot(tc, vec2(s, c)));  
            }  
            // 将当期UV移到值域为[0,1]的屏幕空间  
            tc += center;  
            vec3 color = texture2D(tex, tc).rgb;  
            return vec4(color, 1.0);  
        }  
  
        vec4 main(vec2 fragCoord) {  
            // 转为UV信息  
            vec2 uv = fragCoord.xy / iResolution.xy;  
            uv.x *= x_y;  
            vec4 fragColor = filter(_MainTex, uv, iGlobalTime * 2);  
            return fragColor;    
        }  
        //end-------------------------------------------------------------  
    ENDCG  
  
    SubShader {  
        Pass {  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #pragma fragmentoption ARB_precision_hint_fastest     
            ENDCG  
        }  
    }   
    FallBack "Diffuse"  
}