#ifndef GRASS_FRAG
#define GRASS_FRAG

#ifdef UNITY_PASS_FORWARDBASE
half4 frag(FS_INPUT i) : SV_Target
{
	float3 worldPos = i.worldPos;

	#ifdef UNITY_COMPILER_HLSL
		SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
		GrassSurfaceOutput go = (GrassSurfaceOutput)0;
	#else
		SurfaceOutputStandardSpecular o;
		GrassSurfaceOutput go;
	#endif

	o.Albedo = 0.0;
	o.Normal = normalize(i.normal);
	o.Emission = 0.0;
	o.Specular = 0;
	o.Smoothness = 1.0;
	o.Occlusion = 1.0;
	o.Alpha = 0.0;
	go.Subsurface = 0.0;

	surf(i, o, go);

	fixed4 c = 0;

	#if defined(GRASS_UNLIT_LIGHTING)
		c = half4(o.Albedo, 1);
	#else //Not unlit
		fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
		
		UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

		UnityGI gi;
		UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
		gi.indirect.diffuse = 0;
		gi.indirect.specular = 0;
		#if !defined(LIGHTMAP_ON)
			gi.light.color = _LightColor0.rgb;
			gi.light.dir = lightDir;
			gi.light.ndotl = LambertTerm(o.Normal, gi.light.dir) + go.Subsurface * LambertTerm(-o.Normal, gi.light.dir);
		#endif

		// Call GI (lightmaps/SH/reflections) lighting function
		UnityGIInput giInput;
		UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
		giInput.light = gi.light;
		giInput.worldPos = worldPos;
		giInput.worldViewDir = viewDir;
		giInput.atten = atten;
		#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
			giInput.lightmapUV = i.lmap;
		#else
			giInput.lightmapUV = 0.0;
		#endif
		#if UNITY_SHOULD_SAMPLE_SH
			giInput.ambient = i.sh;
		#else
			giInput.ambient.rgb = 0.0;
		#endif
			giInput.probeHDR[0] = unity_SpecCube0_HDR;
			giInput.probeHDR[1] = unity_SpecCube1_HDR;
		#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
			giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
		#endif
		#if UNITY_SPECCUBE_BOX_PROJECTION
			giInput.boxMax[0] = unity_SpecCube0_BoxMax;
			giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
			giInput.boxMax[1] = unity_SpecCube1_BoxMax;
			giInput.boxMin[1] = unity_SpecCube1_BoxMin;
			giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
		#endif
		LightingStandardSpecular_GI(o, giInput, gi);
		
		gi.light.color *= atten;
		c = GrassSpecularLighting(o, viewDir, gi);
	#endif //End not unlit block

	UNITY_APPLY_FOG(i.fogCoord, c); // apply fog
	UNITY_OPAQUE_ALPHA(c.a);
	return c;
}
#endif

#ifdef UNITY_PASS_FORWARDADD
half4 frag(FS_INPUT i) : SV_Target
{
	float3 worldPos = i.worldPos;

	#ifdef UNITY_COMPILER_HLSL
		SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
		GrassSurfaceOutput go = (GrassSurfaceOutput)0;
	#else
		SurfaceOutputStandardSpecular o;
		GrassSurfaceOutput go;
	#endif

	o.Albedo = 0.0;
	o.Normal = normalize(i.normal);;
	o.Emission = 0.0;
	o.Specular = 0;
	o.Smoothness = 0.5;
	o.Occlusion = 1.0;
	o.Alpha = 0.0;
	go.Subsurface = 0.0;

	surf(i, o, go);

	fixed4 c = 0;

	#if !defined(GRASS_UNLIT_LIGHTING)
		fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
		
		UNITY_LIGHT_ATTENUATION(atten, i, worldPos)

		// Setup lighting environment
		UnityGI gi;
		UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
		gi.indirect.diffuse = 0;
		gi.indirect.specular = 0;
		#if !defined(LIGHTMAP_ON)
			gi.light.color = _LightColor0.rgb;
			gi.light.dir = lightDir;
			gi.light.ndotl = LambertTerm (o.Normal, gi.light.dir) + go.Subsurface * LambertTerm(-o.Normal, gi.light.dir);
		#endif
		gi.light.color *= atten;

		c = GrassSpecularLighting(o, viewDir, gi);
	#endif

	c.a = 0.0;

	UNITY_APPLY_FOG(i.fogCoord, c); // apply fog
	UNITY_OPAQUE_ALPHA(c.a);
	return c;
}
#endif

#ifdef UNITY_PASS_SHADOWCASTER
half4 frag(FS_INPUT i) : SV_Target
{
	// prepare and unpack data
	#ifdef UNITY_COMPILER_HLSL
		SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
		GrassSurfaceOutput go = (GrassSurfaceOutput)0;
	#else
		SurfaceOutputStandardSpecular o;
		GrassSurfaceOutput go;
	#endif
	fixed3 normalWorldVertex = fixed3(0, 0, 1);
	o.Albedo = 0.0;
	o.Normal = normalWorldVertex;
	o.Emission = 0.0;
	o.Specular = 0;
	o.Smoothness = 1;
	o.Occlusion = 1.0;
	o.Alpha = 0.0;
	go.Subsurface = 0.0;

	// call surface function
	surf(i, o, go);

	SHADOW_CASTER_FRAGMENT(i)
}
#endif

#ifdef RENDER_NORMAL_DEPTH
half4 frag(FS_INPUT i) : SV_Target
{
	// prepare and unpack data
	#ifdef UNITY_COMPILER_HLSL
		SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
		GrassSurfaceOutput go = (GrassSurfaceOutput)0;
	#else
		SurfaceOutputStandardSpecular o;
		GrassSurfaceOutput go;
	#endif
	fixed3 normalWorldVertex = fixed3(0, 0, 1);
	o.Albedo = 0.0;
	o.Normal = normalWorldVertex;
	o.Emission = 0.0;
	o.Specular = 0;
	o.Smoothness = 1;
	o.Occlusion = 1.0;
	o.Alpha = 0.0;
	go.Subsurface = 0.0;

	// call surface function, here it handles the cutoff
	surf(i, o, go);

	float depth = -(mul(UNITY_MATRIX_V, float4(i.worldPos, 1)).z * _ProjectionParams.w);
	float3 normal = normalize(i.normal);;
	normal.b = 0;
	return EncodeDepthNormal(depth, normal);
}
#endif

#endif