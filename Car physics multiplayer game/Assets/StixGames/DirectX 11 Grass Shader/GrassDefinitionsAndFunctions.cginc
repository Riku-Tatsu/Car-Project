#ifndef GRASS_DEFINITIONS
#define GRASS_DEFINITIONS

// ================= PRECOMPILER HELPERS ============
#if defined(UNITY_PASS_SHADOWCASTER)
	#define SHADOWPASS
#endif

#if defined(GRASS_USE_TEXTURE_ATLAS) && !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
	#define GRASS_TEXTURE_ATLAS
#endif

// ================================== VARIABLES ================================
sampler2D _ColorMap;
fixed _EdgeLength;
int _MaxTessellation;

fixed _LODStart;
fixed _LODEnd;
int   _LODMax;

half _GrassFadeStart;
half _GrassFadeEnd;

fixed4 _GrassBottomColor;

fixed4 _BurnColor;
fixed _BurnCutoff;

//Wind
#if defined(GRASS_CALC_GLOBAL_WIND)
fixed4 _WindParams;
fixed _WindRotation;
#endif

#ifdef GRASS_DISPLACEMENT
sampler2D _Displacement;

#ifdef GRASS_RENDERTEXTURE_DISPLACEMENT
sampler2D _GrassRenderTextureDisplacement;

//This is the area the displacement camera is currently rendering. xy is left bottom, wz is width and height.
float4 _GrassRenderTextureArea;
#endif
#endif

//Density for the geom shader. "density" is the sampled texture from _Density.
#ifdef UNIFORM_DENSITY
	fixed4 _DensityValues;
	#define DENSITY00 _DensityValues.x
	#define DENSITY01 _DensityValues.y
	#define DENSITY02 _DensityValues.z
	#define DENSITY03 _DensityValues.w
#else
	#ifndef VERTEX_DENSITY
		sampler2D _Density;
	#endif
	#define DENSITY00 density.r
	#define DENSITY01 density.g
	#define DENSITY02 density.b
	#define DENSITY03 density.a
#endif

#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
float _TextureCutoff;

sampler2D _GrassTex00;
#endif
#ifndef SHADOWPASS
fixed4 _Color00;
fixed4 _SecColor00;
fixed4 _SpecColor00;
fixed _Smoothness00;
fixed _Subsurface00;
#endif
fixed _MaxHeight00;
fixed _Softness00;
fixed _Width00;
fixed _MinHeight00;
#ifdef GRASS_TEXTURE_ATLAS
int _TextureAtlasWidth00;
int _TextureAtlasHeight00;
#endif

#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY) && !defined(ONE_GRASS_TYPE)
sampler2D _GrassTex01;
#ifndef SHADOWPASS
fixed4 _Color01;
fixed4 _SecColor01;
fixed4 _SpecColor01;
fixed _Smoothness01;
fixed _Subsurface01;
#endif
fixed _MaxHeight01;
fixed _Softness01;
fixed _Width01;
fixed _MinHeight01;
#ifdef GRASS_TEXTURE_ATLAS
int _TextureAtlasWidth01;
int _TextureAtlasHeight01;
#endif
#endif

#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY) && !defined(ONE_GRASS_TYPE) && !defined(TWO_GRASS_TYPES)
sampler2D _GrassTex02;
#ifndef SHADOWPASS
fixed4 _Color02;
fixed4 _SecColor02;
fixed4 _SpecColor02;
fixed _Smoothness02;
fixed _Subsurface02;
#endif
fixed _MaxHeight02;
fixed _Softness02;
fixed _Width02;
fixed _MinHeight02;
#ifdef GRASS_TEXTURE_ATLAS
int _TextureAtlasWidth02;
int _TextureAtlasHeight02;
#endif
#endif

#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY) && !defined(ONE_GRASS_TYPE) && !defined(TWO_GRASS_TYPES) && !defined(THREE_GRASS_TYPES)
sampler2D _GrassTex03;
#ifndef SHADOWPASS
fixed4 _Color03;
fixed4 _SecColor03;
fixed4 _SpecColor03;
fixed _Smoothness03;
fixed _Subsurface03;
#endif
fixed _MaxHeight03;
fixed _Softness03;
fixed _Width03;
fixed _MinHeight03;
#ifdef GRASS_TEXTURE_ATLAS
int _TextureAtlasWidth03;
int _TextureAtlasHeight03;
#endif
#endif

half _Disorder;

//Scaling and offset for _Density texture
float4 _Density_ST;

// ================================= STRUCTS ===================================
struct appdata 
{
	float4 vertex : POSITION;

    #ifdef VERTEX_DENSITY
		fixed4 color : COLOR;
	#endif

	#ifdef GRASS_FOLLOW_SURFACE_NORMAL
		float3 normal : NORMAL;
	#endif

	float2 uv : TEXCOORD0;
};

struct tess_appdata 
{
	float4 vertex : INTERNALTESSPOS;

	#ifdef VERTEX_DENSITY
		fixed4 color : COLOR;
	#endif

	#ifdef GRASS_FOLLOW_SURFACE_NORMAL
		float3 normal : NORMAL;
	#endif

	float2 uv : TEXCOORD0;
	float3 cameraPos : TEXCOORD1;

	#ifdef GRASS_OBJECT_MODE
		float3 objectSpacePos : TEXCOORD2;
	#endif
};

struct HS_CONSTANT_OUTPUT
{
	fixed edges[3]  : SV_TessFactor;
	fixed inside : SV_InsideTessFactor;
	fixed realTess : REALTESS;
	fixed mTess : MEANTESS;
};

struct GS_INPUT
{
	float4 position : SV_POSITION;

	#ifdef VERTEX_DENSITY
		fixed4 color : COLOR;
	#endif

	#ifdef GRASS_FOLLOW_SURFACE_NORMAL
		float3 normal : NORMAL;
	#endif

	float2 uv : TEXCOORD0;
	float3 cameraPos : TEXCOORD1;
	fixed smoothing : TANGENT2;

	#ifdef GRASS_OBJECT_MODE
		float3 objectSpacePos : TEXCOORD3;
	#endif
};

struct GS_OUTPUT {
	float4 vertex : SV_POSITION;
	fixed3 normal : NORMAL;
	fixed3 reflectionNormal : NORMAL1;

	#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
		fixed2 uv  : TEXCOORD0;
		int texIndex : TEXCOORD1;

		#ifdef GRASS_TEXTURE_ATLAS
			int textureAtlasIndex : TEXCOORD2;
		#endif
	#endif

	#if !defined(SHADOWPASS)
		fixed4 color : COLOR;
	#endif
};

struct FS_INPUT
{
	float3 worldPos : TEXCOORD7;

	#ifndef SHADOWPASS
		float4  pos : SV_POSITION;
		fixed4 color : COLOR;

		fixed3 normal : NORMAL;

		#if UNITY_SHOULD_SAMPLE_SH
			half3 sh : TANGENT; // SH ???
		#endif
		SHADOW_COORDS(4)
		UNITY_FOG_COORDS(5)
		float4 lmap : TEXCOORD6;
	#else
		V2F_SHADOW_CASTER;
	#endif
		
	#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
		fixed2 uv : TEXCOORD2;
		int texIndex : TEXCOORD3;

		#ifdef GRASS_TEXTURE_ATLAS
			uint textureAtlasIndex : COLOR1;
		#endif
	#endif
};

struct GrassSurfaceOutput
{
	fixed Subsurface;
};


// ========================== HELPER FUNCTIONS ==============================
//Random value from 2D value between 0 and 1
inline float rand(float2 co){
	return frac(sin(dot(co.xy, float2(12.9898,78.233))) * 43758.5453);
}

#if defined(GRASS_CALC_GLOBAL_WIND)
#include "GrassWind.cginc"
#endif

//Get the grass normal from the up direction (or bended up direction) of the grass
inline fixed3 getNormal(fixed3 up, fixed3 right, fixed3 lightDir)
{
	#if defined(GRASS_RANDOM_DIR)
		return normalize(cross(right, up));
	#else
		//Calculate everything as if the grass was perpendicular to the direction of the light, 
		//so rotating the camera does not affect the lighting
		fixed3 grassSegmentRight = cross(up, lightDir);
		return normalize(cross(grassSegmentRight, up));
	#endif
}

inline fixed nextPow2(fixed input)
{
	return pow(2, (ceil(log2(input))));
}

//Seriously expensive operation. You shouldn't use this too much. Unfortunately it's needed for the camera/renderer position.
//From http://answers.unity3d.com/questions/218333/shader-inversefloat4x4-function.html
inline float4x4 inverse(float4x4 input)
{
#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
	//determinant(float3x3(input._22_23_23, input._32_33_34, input._42_43_44))

	float4x4 cofactors = float4x4(
		minor(_22_23_24, _32_33_34, _42_43_44),
		-minor(_21_23_24, _31_33_34, _41_43_44),
		minor(_21_22_24, _31_32_34, _41_42_44),
		-minor(_21_22_23, _31_32_33, _41_42_43),

		-minor(_12_13_14, _32_33_34, _42_43_44),
		minor(_11_13_14, _31_33_34, _41_43_44),
		-minor(_11_12_14, _31_32_34, _41_42_44),
		minor(_11_12_13, _31_32_33, _41_42_43),

		minor(_12_13_14, _22_23_24, _42_43_44),
		-minor(_11_13_14, _21_23_24, _41_43_44),
		minor(_11_12_14, _21_22_24, _41_42_44),
		-minor(_11_12_13, _21_22_23, _41_42_43),

		-minor(_12_13_14, _22_23_24, _32_33_34),
		minor(_11_13_14, _21_23_24, _31_33_34),
		-minor(_11_12_14, _21_22_24, _31_32_34),
		minor(_11_12_13, _21_22_23, _31_32_33)
		);
#undef minor
	return transpose(cofactors) / determinant(input);
}

inline float3 getCameraPos()
{
	#ifdef UNITY_PASS_SHADOWCASTER
		return mul(inverse(UNITY_MATRIX_V), float4(0, 0, 0, 1)).xyz;
	#else
		return _WorldSpaceCameraPos.xyz;
	#endif
}
#endif