using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SplatMap_Generator))]
public class SplatMapEditor : Editor 
{
	public Terrain t;
	public SplatMap_Generator myScript;

	public string[] MnuText = new string[] {"Terrain", "Options"}; 
	public int MnuStates;

	public Texture2D[] Icons = new Texture2D[4];
	public string[] dirOptions = new string[] {"X", "Y", "Z","-X", "-Y", "-Z"};
	public bool editTerrain;
	public bool liveMode;
	public string[] gridText = new string[] {"32", "64", "128", "256"};

	private int textDis = 115;
	private int boxDis = 40;
	public void OnEnable()
	{
		myScript = (SplatMap_Generator)target;

		t = myScript.terrain;

		Icons [0] = AssetPreview.GetAssetPreview (Resources.Load ("AngleFlat"));
		Icons [1] = AssetPreview.GetAssetPreview (Resources.Load ("AngleSlope"));
		Icons [2] = AssetPreview.GetAssetPreview (Resources.Load ("HeightLow"));
		Icons [3] = AssetPreview.GetAssetPreview (Resources.Load ("HeightHigh"));

	}

	public override void OnInspectorGUI()
	{

		if (!editTerrain) 
		{
			if(GUILayout.Button("Edit Terrain",GUILayout.Height (30)))
			{
				editTerrain = !editTerrain;
			}

			for (int k = 0; k < t.terrainData.splatPrototypes.Length; k++) 
			{
				//show splatmap
				GUILayout.BeginHorizontal ("box");
				GUILayout.Label (t.terrainData.splatPrototypes [k].texture, GUILayout.MaxWidth (16), GUILayout.MaxHeight (16));
				GUILayout.Label (t.terrainData.splatPrototypes [k].texture.name, GUILayout.Width (100));
				GUILayout.EndHorizontal ();
			}
		}
		else
		{
			// lock terrain
			if(GUILayout.Button("Lock Terrain",GUILayout.Height (30)))
			{
				editTerrain = !editTerrain;
			}
			
			GUILayout.Space(10);

			//counter
			if(myScript.gridCounter < myScript.maxGrid)
			{
				GUILayout.BeginHorizontal ("box");
				GUILayout.Label (myScript.gridCounter.ToString() + " / " + myScript.maxGrid.ToString(), GUILayout.Width (100));
				GUILayout.EndHorizontal ();
			}
			else
			{
				GUILayout.BeginHorizontal ("box");
				GUILayout.Label ("Complete", GUILayout.Width (100));
				GUILayout.EndHorizontal ();
			}

			GUILayout.BeginHorizontal ("label");
			if(GUILayout.Button("Generate Splatmap",GUILayout.Height (80)))
			{
				myScript.UpdateGrid();
				//myScript.GenerateTerrain ();
			}

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ("label");
			GUILayout.Label ("Grid Size", GUILayout.Width (80));
			myScript.gridStates = GUILayout.Toolbar(myScript.gridStates,gridText);
			GUILayout.EndHorizontal ();

			GUILayout.Space(10);

			switch(MnuStates)
			{
			case 0:
				//Splatmaps
				for (int i = 0; i < t.terrainData.splatPrototypes.Length; i++) 
				{
					GUILayout.BeginVertical("box");

					//show splatmap
					GUILayout.BeginHorizontal ("label");
					GUILayout.Label (t.terrainData.splatPrototypes [i].texture, GUILayout.MaxWidth (16), GUILayout.MaxHeight (16));
					GUILayout.Label (t.terrainData.splatPrototypes [i].texture.name, GUILayout.Width (100));
					GUILayout.EndHorizontal ();

					if(i == 0)
					{
						GUILayout.BeginHorizontal ("label");
						GUILayout.Label ("Base Opacity: " + (myScript.SplatMapConstraints [i].threshold*100).ToString("F0") +"%", GUILayout.Width (textDis));

						myScript.SplatMapConstraints [i].t = EditorGUILayout.Slider (myScript.SplatMapConstraints [i].t, 0F, 100.0F, GUILayout.ExpandWidth (true));
						myScript.SplatMapConstraints [i].threshold = myScript.SplatMapConstraints [i].t/100;
						GUILayout.EndHorizontal ();
					}
					else
					{
						//opacity
						GUILayout.BeginHorizontal ("label");
						GUILayout.Label ("Opacity: " + (myScript.SplatMapConstraints[i].opacity* 100).ToString("F0") + "%", GUILayout.Width (textDis));

						myScript.SplatMapConstraints [i].o = EditorGUILayout.Slider (myScript.SplatMapConstraints [i].o, 0F, 100F, GUILayout.ExpandWidth (true));
						myScript.SplatMapConstraints [i].opacity = myScript.SplatMapConstraints [i].o/100;
						GUILayout.EndHorizontal ();

						//buttons
						myScript.SplatMapConstraints [i].terrainState = GUILayout.Toolbar (myScript.SplatMapConstraints [i].terrainState, Icons);


						// angle height
						GUILayout.BeginHorizontal ("label");
						if (myScript.SplatMapConstraints [i].terrainState == 0 || myScript.SplatMapConstraints [i].terrainState == 1) 
						{
							GUILayout.Label ("Angle: " + (90 * myScript.SplatMapConstraints [i].threshold).ToString ("F1") + "º", GUILayout.Width (textDis));

							myScript.SplatMapConstraints [i].t = EditorGUILayout.Slider (myScript.SplatMapConstraints [i].t, 0F, 90F, GUILayout.ExpandWidth (true));
							myScript.SplatMapConstraints [i].threshold = myScript.SplatMapConstraints [i].t/90;
						} 
						else 
						{
							GUILayout.Label ("Height: " + (t.terrainData.size.y * myScript.SplatMapConstraints [i].threshold).ToString ("F1") + "m", GUILayout.Width (textDis));

							myScript.SplatMapConstraints [i].t = EditorGUILayout.Slider (myScript.SplatMapConstraints [i].t, 0F, t.terrainData.size.y, GUILayout.ExpandWidth (true));
							myScript.SplatMapConstraints [i].threshold = myScript.SplatMapConstraints [i].t/t.terrainData.size.y;
						}
				

						GUILayout.EndHorizontal ();

						// offset
						GUILayout.BeginHorizontal ("label");
						if (myScript.SplatMapConstraints [i].terrainState == 0 || myScript.SplatMapConstraints [i].terrainState == 1) 
						{
							GUILayout.Label ("Blur Edge: " + (t.terrainData.size.y * myScript.SplatMapConstraints [i].blurOffset).ToString ("F1") + "º", GUILayout.Width (textDis));

							myScript.SplatMapConstraints [i].b = EditorGUILayout.Slider (myScript.SplatMapConstraints [i].b, 0F, 90F, GUILayout.ExpandWidth (true));
							myScript.SplatMapConstraints [i].blurOffset = myScript.SplatMapConstraints [i].b/90;
						} 
						else 
						{
							GUILayout.Label ("Blur Edge: " + (t.terrainData.size.y * myScript.SplatMapConstraints [i].blurOffset).ToString ("F1") + "m", GUILayout.Width (textDis));

							myScript.SplatMapConstraints [i].b = EditorGUILayout.Slider (myScript.SplatMapConstraints [i].b, 0F, t.terrainData.size.y, GUILayout.ExpandWidth (true));
							myScript.SplatMapConstraints [i].blurOffset = myScript.SplatMapConstraints [i].b/t.terrainData.size.y;
						}
						GUILayout.EndHorizontal ();

						//direction
						GUILayout.BeginHorizontal ("label");
						myScript.SplatMapConstraints [i].direction = GUILayout.Toggle(myScript.SplatMapConstraints [i].direction,"Add Direction",GUILayout.Width(textDis));
						if(myScript.SplatMapConstraints [i].direction)
						{
							myScript.SplatMapConstraints [i].thisDir = EditorGUILayout.Popup(myScript.SplatMapConstraints [i].thisDir,dirOptions);
						} 
						GUILayout.EndHorizontal ();
					}
					GUILayout.EndVertical();
				}
				break;

			case 1:

				break;
			}
		}
	}
}