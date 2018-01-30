using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq; // used for Sum of array

[ExecuteInEditMode]
public class SplatMap_Generator : MonoBehaviour 
{
	// Get the attached terrain component
	public Terrain terrain;
	public TerrainData terrain_data;
	
	// texture class
	[System.Serializable]
	public class TextureOptions
	{
		public int terrainState;

		public float opacity = 0.5f;
		public float threshold = 0.005f;
		public float blurOffset = 0.005f;

		public float o = 50f;
		public float t = 0.1f;
		public float b = 0.1f;
		
		public bool direction;
		public int thisDir;
	}

	public TextureOptions[] SplatMapConstraints = new TextureOptions[50];

	// Assigning Splatmap
	public float splatSum;
	public float[,,] splatmap_data;
	public float[] splatWeights;
	
	// live rendering
	public bool[] gridCheck;
	
	public int gridStates;
	public int gridSize = 32; 

	public int gridX;
	public int gridY;

	public int buildX;
	public int buildY;

	public int maxGrid;
	public int gridCounter;
	

	void OnEnable()
	{
		terrain = GetComponent<Terrain>();
		EditorApplication.update();
	}

	// activates the grid process
	public void UpdateGrid()
	{
		terrain = this.GetComponent<Terrain>();
		terrain_data = terrain.terrainData;

		splatWeights = new float[terrain_data.alphamapLayers];
		splatmap_data = new float[terrain_data.alphamapWidth, terrain_data.alphamapHeight, terrain_data.alphamapLayers];

		switch (gridStates)
		{
		case 0:
			gridSize = 32;
			break;
		case 1:
			gridSize = 64;
			break;
		case 2:
			gridSize = 128;
			break;
		case 3:
			gridSize = 256;
			break;
		}
		
		gridX = (int) terrain_data.alphamapHeight / gridSize;
		gridY = (int) terrain_data.alphamapHeight / gridSize;

		maxGrid = gridX * gridY;
		gridCounter = 0;
		buildY = 0;

		EditorApplication.update = GridManager;
		GridManager();
	}

	void GridManager()
	{
		buildX = gridCounter - Mathf.FloorToInt(gridCounter/gridX) * gridX;
		buildY = Mathf.FloorToInt(gridCounter/gridX);

		BuildGrid(buildX,buildY);
		gridCounter++;	

		terrain_data.SetAlphamaps(0, 0, splatmap_data);
		
		if(gridCounter >= maxGrid)
		{
			EditorApplication.update = null;
		}
	}

	// for each section of the grid, generate each pixel.
	void BuildGrid(int xg, int yg)
	{
		for (int y = 0; y < gridSize; y++)
		{
			for (int x = 0; x < gridSize; x++)
			{
				GeneratePixel(xg * gridSize + x, yg * gridSize + y);
			}
		}
	} 

	public void GeneratePixel (int x, int y) 
	{
		float yPos;		
		float xPos;
		
		int xDis;
		int yDis;

		float height;
		Vector3 normal;
		float steepness;
		
		// get location and terrain data
		yPos = (float)y/(float)terrain_data.alphamapHeight;
		xPos = (float)x/(float)terrain_data.alphamapWidth;
		
		xDis = Mathf.RoundToInt(xPos * terrain_data.heightmapWidth);
		yDis = Mathf.RoundToInt(yPos * terrain_data.heightmapHeight);

		if(yDis > terrain_data.heightmapHeight || xDis > terrain_data.heightmapWidth)
		{
			return;
		}

		height = terrain_data.GetHeight(yDis,xDis) / terrain_data.size.y;
		normal = terrain_data.GetInterpolatedNormal(yPos,xPos);
		steepness = terrain_data.GetSteepness(yPos,xPos)/90;

		//Set Textures
		splatWeights[0] = SplatMapConstraints[0].threshold;

		for(int j = 1; j < terrain_data.splatPrototypes.Length; j++)
		{
			if(SplatMapConstraints[j].terrainState == 0 || SplatMapConstraints[j].terrainState == 1)
			{
				splatWeights[j] = CalcWeight(j,SplatMapConstraints[j].terrainState,steepness,normal,SplatMapConstraints [j].opacity);
			}
			else
			{
				splatWeights[j] = CalcWeight(j,SplatMapConstraints[j].terrainState,height,normal,SplatMapConstraints [j].opacity);
			}
		}

		//Assigning Textures
		splatSum = splatWeights.Sum();

		for(int i = 0; i < terrain_data.alphamapLayers; i++)
		{	
			// Normalize texture weights keeping base layer and assigns point to splatmap array
			splatWeights[i] /= splatSum;
			splatmap_data[x, y, i] = splatWeights[i];
		}
	}

	//math for the calculations
	float CalcWeight(int t, int s, float h, Vector3 n,float o)
	{
		float val = 0f;

		float st = SplatMapConstraints [t].threshold;
		float sb = SplatMapConstraints [t].blurOffset;
		
		switch (s) 
		{
		case 0:
			val = Mathf.Clamp01(1-(h-st)/sb)*o;
			break;

		case 1:
			val = Mathf.Clamp01(((h-st-sb)/sb))*o;
			break;

		case 2:
			val = Mathf.Clamp01(1-(h-st)/sb)*o;
			break;

		case 3:
			val = Mathf.Clamp01(((h-st-sb)/sb))*o;
			break;
		}

		if (SplatMapConstraints [t].direction) 
		{
			switch (SplatMapConstraints [t].thisDir) 
			{
			case 0:
				val *= Mathf.Clamp01 (n.x);
				break;
			case 1:
				val *= Mathf.Clamp01 (n.y);
				break;
			case 2:
				val *= Mathf.Clamp01 (n.z);
				break;
			case 3:
				val *= Mathf.Clamp01 (-n.x);
				break;
			case 4:
				val *= Mathf.Clamp01 (-n.y);
				break;
			case 5:
				val *= Mathf.Clamp01 (-n.z);
				break;
			}
		}
		return val;
	}

	/*public void GenerateTerrain () 
	{
		// Get a reference to the terrain data
		terrain = this.GetComponent<Terrain>();
		TerrainData terrain_data = terrain.terrainData;
		
		float yPos;
		float xPos;
		
		float height;
		Vector3 normal;
		float steepness;
		
		float splatSum;
		
		float[, ,] splatmap_data = new float[terrain_data.alphamapWidth, terrain_data.alphamapHeight, terrain_data.alphamapLayers];
		
		for (int y = 0; y < terrain_data.alphamapHeight; y++)
		{
			for (int x = 0; x < terrain_data.alphamapWidth; x++)
			{
				// GET TERRAIN DATA
				yPos = (float)y/(float)terrain_data.alphamapHeight;
				xPos = (float)x/(float)terrain_data.alphamapWidth;
				
				height = terrain_data.GetHeight(Mathf.RoundToInt(yPos * terrain_data.heightmapHeight),Mathf.RoundToInt(xPos * terrain_data.heightmapWidth)) / terrain_data.size.y;
				normal = terrain_data.GetInterpolatedNormal(yPos,xPos);
				steepness = terrain_data.GetSteepness(yPos,xPos)/90;

				float[] splatWeights = new float[terrain_data.alphamapLayers];

				//Set Textures
				splatWeights[0] = SplatMapConstraints[0].threshold;
				for(int j = 1; j < terrain_data.splatPrototypes.Length; j++)
				{
					if(SplatMapConstraints[j].terrainState == 0 || SplatMapConstraints[j].terrainState == 1)
					{
						splatWeights[j] = CalcWeight(j,SplatMapConstraints[j].terrainState,steepness,normal,SplatMapConstraints [j].opacity);
					}
					else
					{
						splatWeights[j] = CalcWeight(j,SplatMapConstraints[j].terrainState,height,normal,SplatMapConstraints [j].opacity);
					}
				}

				// ASSIGN TEXTURES
				splatSum = splatWeights.Sum();

				for(int i = 0; i < terrain_data.alphamapLayers; i++)
				{	
					// Normalize texture weights keeping base layer and assigns point to splatmap array
					splatWeights[i] /= splatSum;
					splatmap_data[x, y, i] = splatWeights[i];
				}
			}
		}
		terrain_data.SetAlphamaps(0, 0, splatmap_data);
	}*/
}
