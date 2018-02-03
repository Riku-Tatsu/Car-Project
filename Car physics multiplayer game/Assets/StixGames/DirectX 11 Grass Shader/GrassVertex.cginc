// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef GRASS_VERTEX
#define GRASS_VERTEX

tess_appdata vert(appdata v)
{	
	tess_appdata o;
	UNITY_INITIALIZE_OUTPUT(tess_appdata, o);

	#ifdef GRASS_OBJECT_MODE
		o.objectSpacePos = v.vertex.xyz;
	#endif

	o.vertex = mul(unity_ObjectToWorld, v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _Density);

	#ifdef VERTEX_DENSITY
		o.color = v.color;
	#endif

	#ifdef GRASS_FOLLOW_SURFACE_NORMAL
		o.normal = UnityObjectToWorldNormal(v.normal);
	#endif

	//Camera, or rather renderer pos
	o.cameraPos = getCameraPos();

	return o;
}
#endif