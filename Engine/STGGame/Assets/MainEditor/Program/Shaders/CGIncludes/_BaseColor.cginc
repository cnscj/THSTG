#ifndef BASE_COLOR_CG_INCLUDED
#define BASE_COLOR_CG_INCLUDED

//灰显
#define COLOR_GRAY(colorRGB) colorRGB = dot(colorRGB.rgb, fixed3(0.222, 0.707, 0.071));
#ifdef GRAY_ON
    half _Gray;
    #define TRY_COLOR_GRAY(colorRGB) if(_Gray > 0) { COLOR_GRAY(colorRGB); }
#else
    #define TRY_COLOR_GRAY(colorRGB)
#endif

#endif
