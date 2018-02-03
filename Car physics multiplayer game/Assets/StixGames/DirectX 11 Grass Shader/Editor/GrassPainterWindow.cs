using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StixGames.GrassShader
{
	public class GrassPainterWindow : EditorWindow
	{
		private static readonly string[] TargetTextureLabels =
		{
			"Color Height",
			"Density"
		};
		
		private static readonly string[] ColorHeightChannelLabels = {
			"Color",
			"Height",
			"Both"
		};
		
		private static readonly string[] DensityChannelLabels = {
			"1",
			"2",
			"3",
			"4"
		};

		private static readonly string[] PaintModeLabels = {
			"Blend",
			"Add",
			"Remove",
		};

		private static bool useUndo = true;
		private static bool searchRoot = true;

		private static GrassPainterWindow window;

		private GameObject grassObject;
		private Material grassMaterial;
		
		private List<Collider> tempColliders = new List<Collider>();

		private int textureSize = 512;

		private bool mouseDown;
		private Vector3 lastPaintPos;
		private bool didDraw;
		private bool showCloseMessage;
		private bool showTargetSwitchMessage;
		public GrassPainter grassPainter = new GrassPainter();

		[MenuItem("Window/Stix Games/Grass Painter")]
		public static void OpenWindow()
		{
			window = GetWindow<GrassPainterWindow>();
			window.Show();
		}

		private void RecreateWindow()
		{
			window = Instantiate(this);
			window.Show();
		}

		private void OnGUI()
		{
			TextureSettings();

			BrushSettings();

			PainterSettings();
		}

		private void TextureSettings()
		{
			if (grassObject == null)
			{
				EditorGUILayout.LabelField("No grass object selected. Select a grass object in the scene Hierarchy.");
				return;
			}

			EditorGUILayout.LabelField("Current object: " + grassObject.name);
			EditorGUILayout.Space();

			if (GrassEditorUtility.GetDensityMode(grassMaterial) != DensityMode.Texture)
			{
				EditorGUILayout.LabelField("The grass material is not in texture density mode.", EditorStyles.boldLabel);
				if (GUILayout.Button("Change material to texture density"))
				{
					GrassEditorUtility.SetDensityMode(grassMaterial, DensityMode.Texture);
				}

				EditorGUILayout.Space();
			}

			if (grassMaterial.GetTexture("_ColorMap") == null || grassMaterial.GetTexture("_Density") == null)
			{
				EditorGUILayout.LabelField("Create new texture", EditorStyles.boldLabel);
				EditorGUILayout.Space();

				textureSize = EditorGUILayout.IntField("Texture Size", textureSize);

				if (grassMaterial.GetTexture("_ColorMap") == null && GUILayout.Button("Create color height texture"))
				{
					//Select the path and return on cancel
					string path = EditorUtility.SaveFilePanelInProject("Create color height texture", 
						"newColorHeightTexture", "png", "Choose where to save the new color height texture");

					if (path == null)
					{
						return;
					}

					CreateTexture(path, TextureTarget.ColorHeight);
				}
				
				if (grassMaterial.GetTexture("_Density") == null && GUILayout.Button("Create density texture"))
				{
					//Select the path and return on cancel
					string path = EditorUtility.SaveFilePanelInProject("Create density texture", "newDensityTexture", 
						"png", "Choose where to save the new density texture");

					if (path == null)
					{
						return;
					}

					CreateTexture(path, TextureTarget.Density);
				}

				EditorGUILayout.Space();
			}
		}
		
		private void BrushSettings()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Brush settings", EditorStyles.boldLabel);

			//Target texture
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Target");
			grassPainter.Target = 
				(TextureTarget) GUILayout.SelectionGrid((int) grassPainter.Target, TargetTextureLabels, 2);
			EditorGUILayout.EndHorizontal();

			switch (grassPainter.Target)
			{
				case TextureTarget.ColorHeight:
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Channel");
					grassPainter.ColorHeightChannel = 
						(ColorHeightChannel) GUILayout.SelectionGrid((int) grassPainter.ColorHeightChannel, 
							ColorHeightChannelLabels, 3);
					EditorGUILayout.EndHorizontal();
					break;
				case TextureTarget.Density:
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Channel");
					grassPainter.DensityChannel = 
						(DensityChannel) GUILayout.SelectionGrid((int) grassPainter.DensityChannel, 
							DensityChannelLabels, 4);
					EditorGUILayout.EndHorizontal();
					break;
			}

			EditorGUILayout.Space();

			//Paint mode
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Mode");
			grassPainter.Brush = (BrushMode) GUILayout.SelectionGrid((int) grassPainter.Brush, PaintModeLabels, 3);
			EditorGUILayout.EndHorizontal();

			//TODO: Make slider max value dynamically changeable by writing into field.
			//For now, just change the right value if you want more or less size
			if (grassPainter.Target == TextureTarget.ColorHeight)
			{
				grassPainter.PaintColor = EditorGUILayout.ColorField(new GUIContent("Color (RGB) Height(A)",
					"The color that will be used for painting. The alpha channel (Transparency) represents the height."), 
					grassPainter.PaintColor);
			}
			if (grassPainter.Target == TextureTarget.Density)
			{
				grassPainter.PaintDensity = EditorGUILayout.Slider(new GUIContent("Blend Density",
					"The target density used when using the blend mode. In Add and Subtract, only the strength is used"), 
					grassPainter.PaintDensity, 0.0f, 1.0f);
			}
			grassPainter.Strength = EditorGUILayout.Slider("Strength", grassPainter.Strength, 0, 1);
			grassPainter.Size = EditorGUILayout.Slider("Size", grassPainter.Size, 0.01f, 50);
			grassPainter.Softness = EditorGUILayout.Slider("Softness", grassPainter.Softness, 0, 1);
			grassPainter.Spacing = EditorGUILayout.Slider("Spacing", grassPainter.Spacing, 0, 2);
			//rotation = EditorGUILayout.Slider("Rotation", rotation, 0, 360);

			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetInt("StixGames.Painter.Target", (int) grassPainter.Target);
				EditorPrefs.SetInt("StixGames.Painter.ColorHeightChannel", (int) grassPainter.ColorHeightChannel);
				EditorPrefs.SetInt("StixGames.Painter.DensityChannel", (int) grassPainter.DensityChannel);
				EditorPrefs.SetInt("StixGames.Painter.BrushMode", (int) grassPainter.Brush);
				EditorPrefs.SetFloat("StixGames.Painter.BrushMode.R", grassPainter.PaintColor.r);
				EditorPrefs.SetFloat("StixGames.Painter.BrushMode.G", grassPainter.PaintColor.g);
				EditorPrefs.SetFloat("StixGames.Painter.BrushMode.B", grassPainter.PaintColor.b);
				EditorPrefs.SetFloat("StixGames.Painter.BrushMode.A", grassPainter.PaintColor.a);
				EditorPrefs.SetFloat("StixGames.Painter.PaintDensity", grassPainter.PaintDensity);
				EditorPrefs.SetFloat("StixGames.Painter.Strength", grassPainter.Strength);
				EditorPrefs.SetFloat("StixGames.Painter.Size", grassPainter.Size);
				EditorPrefs.SetFloat("StixGames.Painter.Softness", grassPainter.Softness);
				EditorPrefs.SetFloat("StixGames.Painter.Spacing", grassPainter.Spacing);
				EditorPrefs.SetFloat("StixGames.Painter.Rotation", grassPainter.Rotation);
			}

			EditorGUILayout.Space();
		}
		
		private void PainterSettings()
		{
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Painter settings", EditorStyles.boldLabel);

			useUndo = EditorGUILayout.ToggleLeft("Save Undo/Redo data (may cause lag)", useUndo);
			searchRoot = EditorGUILayout.ToggleLeft(new GUIContent("Search root for grass", 
				"Searches all children of the root object for grass objects. " +
				"This can take a while, but will make it possible to paint on tiled objects with the same material."), 
				searchRoot);

			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool("StixGames.Painter.UseUndo", useUndo);
				EditorPrefs.SetBool("StixGames.Painter.SearchRoot", searchRoot);

				if (!useUndo && grassPainter.DensityTexture != null)
				{
					Undo.ClearUndo(grassPainter.DensityTexture);
				}
			}
		}
		
		void OnFocus()
		{
			grassPainter.Target = (TextureTarget) EditorPrefs.GetInt("StixGames.Painter.Target", 
				(int) grassPainter.Target);
			grassPainter.ColorHeightChannel = (ColorHeightChannel) EditorPrefs.GetInt("StixGames.Painter.ColorHeightChannel", 
				(int) grassPainter.ColorHeightChannel);
			grassPainter.DensityChannel = (DensityChannel) EditorPrefs.GetInt("StixGames.Painter.DensityChannel", 
				(int) grassPainter.DensityChannel);
			grassPainter.Brush = (BrushMode) EditorPrefs.GetInt("StixGames.Painter.BrushMode", (int) grassPainter.Brush);
			grassPainter.PaintColor.r = EditorPrefs.GetFloat("StixGames.Painter.BrushMode.R", grassPainter.PaintColor.r);
			grassPainter.PaintColor.g = EditorPrefs.GetFloat("StixGames.Painter.BrushMode.G", grassPainter.PaintColor.g);
			grassPainter.PaintColor.b = EditorPrefs.GetFloat("StixGames.Painter.BrushMode.B", grassPainter.PaintColor.b);
			grassPainter.PaintColor.a = EditorPrefs.GetFloat("StixGames.Painter.BrushMode.A", grassPainter.PaintColor.a);
			grassPainter.PaintDensity = EditorPrefs.GetFloat("StixGames.Painter.PaintDensity", grassPainter.PaintDensity);
			grassPainter.Strength = EditorPrefs.GetFloat("StixGames.Painter.Strength", grassPainter.Strength);
			grassPainter.Size = EditorPrefs.GetFloat("StixGames.Painter.Size", grassPainter.Size);
			grassPainter.Softness = EditorPrefs.GetFloat("StixGames.Painter.Softness", grassPainter.Softness);
			grassPainter.Spacing = EditorPrefs.GetFloat("StixGames.Painter.Spacing", grassPainter.Spacing);
			grassPainter.Rotation = EditorPrefs.GetFloat("StixGames.Painter.Rotation", grassPainter.Rotation);
			showCloseMessage = EditorPrefs.GetBool("StixGames.Painter.ShowCloseMessage", true);
			showTargetSwitchMessage = EditorPrefs.GetBool("StixGames.Painter.ShowTargetSwitchMessage", true);
			useUndo = EditorPrefs.GetBool("StixGames.Painter.UseUndo", true);
			searchRoot = EditorPrefs.GetBool("StixGames.Painter.SearchRoot", true);

			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			SceneView.onSceneGUIDelegate += OnSceneGUI;

			Undo.undoRedoPerformed -= SaveTexture;
			Undo.undoRedoPerformed += SaveTexture;
		}

		void OnDestroy()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			Undo.undoRedoPerformed -= SaveTexture;
			ResetRenderer(true);

			if (ShowUndoLossWarning(true))
			{
				Undo.ClearUndo(grassPainter.DensityTexture);
			}
			else
			{
				RecreateWindow();
			}
		}

		private bool ShowUndoLossWarning(bool isWindowClose)
		{
			if (isWindowClose && showCloseMessage || !isWindowClose && showTargetSwitchMessage)
			{
				string message = isWindowClose
					? "After closing the painter changes will be permanent, undo will no longer be possible."
					: "After switching grass object changes will be permanent, undo will no longer be possible.";

				int result = EditorUtility.DisplayDialogComplex("Make changes permanent?",
					message, isWindowClose ? "Close" : "Switch", "Cancel",
					isWindowClose ? "Close and don't show again" : "Switch and don't show again");

				if (result == 1)
				{
					return false;
				}

				if (result == 2)
				{
					if (showCloseMessage)
					{
						showCloseMessage = false;
						EditorPrefs.SetBool("StixGames.Painter.ShowCloseMessage", false);
					}
					else
					{
						showTargetSwitchMessage = false;
						EditorPrefs.SetBool("StixGames.Painter.ShowTargetSwitchMessage", false);
					}
				}
			}

			//Always accept if message is hidden
			return true;
		}

		private void CreateTexture(string path, TextureTarget target)
		{
			//Create the new texture and save it at the selected path
			grassPainter.CurrentTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false, true);

			Color[] colors = new Color[textureSize * textureSize];
			var initColor = target == TextureTarget.Density ? new Color(0, 0, 0, 0) : Color.white;
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = initColor;
			}

			grassPainter.CurrentTexture.SetPixels(colors);
			grassPainter.CurrentTexture.Apply();

			var workingTex = grassPainter.CurrentTexture;
			var texture = GrassEditorUtility.SaveTextureToFile(path, workingTex);
			Destroy(workingTex);
			grassPainter.CurrentTexture = texture;
			
			switch (target)
			{
				case TextureTarget.ColorHeight:
					grassMaterial.SetTexture("_ColorMap", texture);
					break;
				case TextureTarget.Density:
					grassMaterial.SetTexture("_Density", texture);
					break;
				default:
					throw new ArgumentOutOfRangeException("target", target, null);
			}
		}

		void OnSceneGUI(SceneView sceneView)
		{
			BlockSceneSelection();

			UpdateInput();

			//Update grass renderer
			UpdateSelectedRenderer();

			if (grassObject == null)
			{
				return;
			}

			//Calculate ray from mouse cursor
			var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

			//Check if grass was hit
			RaycastHit hit = new RaycastHit();
			if (!grassPainter.RaycastColliders(ray, out hit))
			{
				return;
			}

			Handles.color = new Color(1, 0, 0, 1);
			Handles.CircleCap(0, hit.point, Quaternion.LookRotation(Vector3.up), grassPainter.Size);

			//Paint
			if (mouseDown)
			{
				float newDist = Vector3.Distance(lastPaintPos, hit.point);

				//Check draw spacing
				if (!didDraw || newDist > grassPainter.Spacing * grassPainter.Size)
				{
					//Draw brush
					grassPainter.ApplyBrush(hit.point);

					lastPaintPos = hit.point;
				}

				didDraw = true;
			}

			SceneView.RepaintAll();
		}

		private void BlockSceneSelection()
		{
			//Only block when a grass object is selected
			if (grassObject == null)
			{
				return;
			}

			//Disable selection in editor view, only painting will be accepted as input
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		}

		private void UpdateInput()
		{
			if (grassPainter.DensityTexture != null && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				mouseDown = true;

				if (useUndo)
				{
					Undo.RegisterCompleteObjectUndo(grassPainter.DensityTexture, "Texture paint");
				}
			}
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
			{
				mouseDown = false;

				if (didDraw)
				{
					SaveTexture();
					didDraw = false;
				}
			}
		}

		private void UpdateSelectedRenderer()
		{
			//Return if no new object was selected
			if (Selection.activeGameObject == null)
			{
				return;
			}

			//Return if the new object is not a grass object
			var newGrassObject = Selection.activeGameObject;
			Selection.activeGameObject = null;

			if (newGrassObject == grassObject)
			{
				return;
			}

			//Get the grass material, if non was found, return.
			var newGrassMaterial = GetGrassMaterial(newGrassObject);
			if (newGrassMaterial == null)
			{
				return;
			}

			//A new object was selected. If another object was selected before, tell the user that this will make changes permanent.
			if (grassObject != null && !ShowUndoLossWarning(false))
			{
				return;
			}

			//Clear undo history for current texture
			Undo.ClearUndo(grassPainter.DensityTexture);

			//Reset the previously selected grass object
			ResetRenderer(false);

			//Assign new grass object
			grassObject = newGrassObject;
			grassMaterial = newGrassMaterial;
			grassPainter.ColorHeightTexture = grassMaterial.GetTexture("_ColorMap") as Texture2D;
			grassPainter.DensityTexture = grassMaterial.GetTexture("_Density") as Texture2D;

			if (searchRoot)
			{
				grassPainter.GrassColliders = GetGrassObjectsInChildren(grassObject.transform.root.gameObject)
					.Select(x => GetColliderOrAddTemp(x))
					.ToArray();
			}
			else
			{
				var collider = GetColliderOrAddTemp(grassObject);

				grassPainter.GrassColliders = new[] { collider };
			}
		}

		private Collider GetColliderOrAddTemp(GameObject x)
		{
			var collider = x.GetComponent<Collider>();
			
			//In Unity null is not actually null........
			// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
			if (collider == null)
			{
				collider = AddTempCollider(x);
			}
			return collider;
		}

		private Collider AddTempCollider(GameObject obj)
		{
			if (obj.GetComponent<Renderer>() != null)
			{
				var c = obj.AddComponent<MeshCollider>();
				tempColliders.Add(c);
				return c;
			}

			if (obj.GetComponent<Terrain>() != null)
			{
				var c = obj.AddComponent<TerrainCollider>();
				tempColliders.Add(c);
				return c;
			}

			throw new ArgumentException("obj doesn't have a Renderer or Terrain component");
		}

		private void ResetRenderer(bool reselectPrevious)
		{
			if (reselectPrevious && grassObject != null)
			{
				Selection.activeGameObject = grassObject;
			}

			foreach (var collider in tempColliders)
			{
				DestroyImmediate(collider);
			}
			tempColliders.Clear();
			
			grassObject = null;
			grassMaterial = null;
			grassPainter.GrassColliders = new Collider[0];
		}

		private GameObject[] GetGrassObjectsInChildren(GameObject newGrassObject)
		{
			var renderers = newGrassObject.GetComponentsInChildren<Renderer>()
				.Where(x => x.sharedMaterial != null && x.sharedMaterial.shader != null
							&& x.sharedMaterial.shader.name == "Stix Games/Grass")
				.Select(x => x.gameObject);
			

			var terrains = newGrassObject.GetComponentsInChildren<Terrain>()
				.Where(x => x.materialTemplate != null && x.materialTemplate.shader != null
							&& x.materialTemplate.shader.name == "Stix Games/Grass")
				.Select(x => x.gameObject);
			
			return renderers.Concat(terrains).ToArray();
		}
		
		private Material GetGrassMaterial(GameObject newGrassObject)
		{
			var renderer = newGrassObject.GetComponent<Renderer>();
			if (renderer != null && renderer.sharedMaterial != null 
				&& renderer.sharedMaterial.shader != null 
				&& renderer.sharedMaterial.shader.name == "Stix Games/Grass")
			{
				return renderer.sharedMaterial;
			}

			var terrain = newGrassObject.GetComponent<Terrain>();
			if (terrain != null && terrain.materialTemplate != null
				&& terrain.materialTemplate.shader != null
				&& terrain.materialTemplate.shader.name == "Stix Games/Grass")
			{
				return terrain.materialTemplate;
			}

			return null;
		}
		
		private void SaveTexture()
		{
			if (grassPainter.CurrentTexture == null)
			{
				return;
			}
			
			string path = AssetDatabase.GetAssetPath(grassPainter.CurrentTexture);
			File.WriteAllBytes(path, grassPainter.CurrentTexture.EncodeToPNG());
		}
	}
}
