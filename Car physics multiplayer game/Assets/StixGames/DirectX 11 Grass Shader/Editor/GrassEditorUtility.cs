using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace StixGames.GrassShader
{
	public static class GrassEditorUtility
	{
		public static readonly string[] DensityModes = { "UNIFORM_DENSITY", "VERTEX_DENSITY", "" };

		public static DensityMode GetDensityMode(Material mat)
		{
			if (mat.IsKeywordEnabled(DensityModes[0]))
			{
				return DensityMode.Value;
			}

			if (mat.IsKeywordEnabled(DensityModes[1]))
			{
				return DensityMode.Vertex;
			}

			return DensityMode.Texture;
		}

		public static void SetDensityMode(Material mat, DensityMode target)
		{
			var density = GetDensityMode(mat);

			switch (density)
			{
				case DensityMode.Value:
					mat.DisableKeyword(DensityModes[0]);
					break;
				case DensityMode.Vertex:
					mat.DisableKeyword(DensityModes[1]);
					break;
			}

			mat.EnableKeyword(DensityModes[(int)target]);
		}

		public static Texture2D SaveTextureToFile(string path, Texture2D texture)
		{
			File.WriteAllBytes(path, texture.EncodeToPNG());

			//Import and load the new texture
			AssetDatabase.ImportAsset(path);
			var importer = (TextureImporter) AssetImporter.GetAtPath(path);
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.isReadable = true;
			importer.maxTextureSize = Mathf.Max(texture.width, texture.height);
#if UNITY_5_5_OR_NEWER
			importer.sRGBTexture = false;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
			{
				format = TextureImporterFormat.ARGB32,
				overridden = true,
			});
#else
			importer.linearTexture = true;
			importer.textureFormat = TextureImporterFormat.ARGB32;
#endif
			AssetDatabase.ImportAsset(path);

			return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		}
	}

	public enum DensityMode
	{
		Value,
		Vertex,
		Texture
	}
}
