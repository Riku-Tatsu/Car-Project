#ifndef GRASS_LIGHTING
#define GRASS_LIGHTING

#if defined(GRASS_UNSHADED_LIGHTING)
inline half4 GrassSpecularLighting(SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
{
	return half4(s.Albedo * (gi.light.color + gi.indirect.diffuse), 1);
}
#elif defined(GRASS_PBR_LIGHTING)
inline half4 GrassSpecularLighting(SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
{
	#if GRASS_IGNORE_GI_SPECULAR
		gi.indirect.specular = half3(0,0,0);
	#endif
	return LightingStandardSpecular(s, viewDir, gi);
}
#else //Use fake lighting
inline half4 GrassSpecularLighting(SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
{
	// energy conservation
	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular(s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

	//Mirror light dir and normal to get specular lighting on the same side as the sun/light. 
	//Just because it's not realistic, doesn't mean it doesn't look nice!
	gi.light.dir = half3(-gi.light.dir.x, gi.light.dir.y, -gi.light.dir.z);
	
	#if !defined(GRASS_RANDOM_DIR)
		//Randomly rotated grass has correct normals (because backfaces are rendered) so the normal doesn't have to be changed
		s.Normal = half3(-s.Normal.x, s.Normal.y, -s.Normal.z);
	#endif

	#if GRASS_IGNORE_GI_SPECULAR
		gi.indirect.specular = half3(0, 0, 0);
	#endif
	half4 c = UNITY_BRDF_PBS(s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
	c.a = 1;
	return c;
}
#endif

#endif //GRASS_LIGHTING