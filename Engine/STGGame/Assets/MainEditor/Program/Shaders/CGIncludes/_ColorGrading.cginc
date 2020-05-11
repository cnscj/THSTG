#ifndef GY_COLOR_GRADING_INCLUDED
#define GY_COLOR_GRADING_INCLUDED

// 
float _LightmapInBlack;
float _LightmapOutBlack;        
float _LightmapInWhite;
float _LightmapOutWhite;
float _LightmapGamma;
float4 _LightmapColorScale;
float4 _LightmapColorAdd;
float4 NeutralToneMapping(float4 color)
{
    color = (pow(((color) - _LightmapInBlack) / (_LightmapInWhite - _LightmapInBlack), _LightmapGamma) * (_LightmapOutWhite - _LightmapOutBlack) + _LightmapOutBlack);
    color = color * _LightmapColorScale + _LightmapColorAdd - 0.5;
    color = max(0, color);
    return color;            
}

#ifdef _GY_LIGHTMAPS_COLOR_GRADING
    // only in editor
    #define BAKED_COLOR_GRADING(color) color = NeutralToneMapping(color)
#else
    // do nothing in game
    #define BAKED_COLOR_GRADING(color)
#endif

half3 _EnvColorScale;
half3 _EnvColorAdd;

#ifdef _GY_ENV_COLOR_GRADING    
    #define ENV_COLOR_GRADING(colorRGB) colorRGB = colorRGB * _EnvColorScale + _EnvColorAdd;
#else    
    #define ENV_COLOR_GRADING(colorRGB)
#endif

#endif
