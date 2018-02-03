using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace StixGames.GrassShader
{
    public class TerrainConverter : EditorWindow
    {
        private Material newMaterial;
        private bool castShadow = true;
        private bool receiveShadow = true;
    
        [MenuItem("Window/Stix Games/Terrain Converter")]
        public static void Init()
        {
            var window = GetWindow<TerrainConverter>();
            window.Show();
        }

        private void OnGUI()
        {
            newMaterial = (Material) EditorGUILayout.ObjectField(new GUIContent("New Material",
                    "Select the material you want to use for the new terrain-mesh."), newMaterial, 
                typeof(Material), false);

            castShadow = EditorGUILayout.Toggle(new GUIContent("Cast shadows",
                "Sets if the generated meshes should cast shadows. " +
                "This can be changed in the inspector of the submeshes afterwards."), castShadow);
        
            receiveShadow = EditorGUILayout.Toggle(new GUIContent("Receive shadows",
                "Sets if the generated meshes should receive shadows. " +
                "This can be changed in the inspector of the submeshes afterwards."), receiveShadow);
            
            EditorGUILayout.Space();
        
            if (Selection.activeGameObject == null)
            {
                EditorGUILayout.LabelField(new GUIContent("Select a terrain"));
                return;
            }
        
            var terrain = Selection.activeGameObject.GetComponent<Terrain>();

            if (terrain == null)
            {
                EditorGUILayout.LabelField(new GUIContent("Select a terrain"));
                return;
            }

            if (GUILayout.Button(new GUIContent("Convert to mesh", "Converts the currently selected terrain to a mesh.")))
            {
                GenerateTerrainMesh(terrain);
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button(new GUIContent("Extract detail textures", "Extracts the current detail textures for use with the DirectX 11 grass shader.")))
            {
                ExtractDetailTextures(terrain);
            }
        }

        private void ExtractDetailTextures(Terrain terrain)
        {
            var data = terrain.terrainData;

            var detailCount = data.detailPrototypes.Length;

            for (int i = 0; i < detailCount; i += 4)
            {
                string title = "Extract terrain detail textures " + (i+1) + "-" + Mathf.Min(i + 4, detailCount);
                string path = EditorUtility.SaveFilePanelInProject(title, "extractedDensityTexture", 
                    "png", "Choose where to save the extracted density texture");
            
                if (path == null)
                {
                    return;
                }

                var details = new int[4][,];
                for (int j = 0; j < 4; j++)
                {
                    if (i + j < detailCount)
                    {
                        details[j] = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, i+j);
                    }
                    else
                    {
                        details[j] = new int[data.detailWidth, data.detailHeight];
                    }
                }

                var colors = new Color[data.detailWidth * data.detailHeight];
                for (int x = 0; x < data.detailWidth; x++)
                {
                    for (int y = 0; y < data.detailHeight; y++)
                    {
                        //The shader uses different UV's than Unity terrain.. or I fucked up the conversion to terrain...
                        var index = x * data.detailHeight + y;
                        colors[index] =  new Color(details[0][x,y], details[1][x,y], details[2][x,y], details[3][x,y]) / 16;
                    }
                }
                
                var tex = new Texture2D(data.detailWidth, data.detailHeight, TextureFormat.ARGB32, false, true);
                tex.SetPixels(colors);

                GrassEditorUtility.SaveTextureToFile(path, tex);
            }
        }

        private float[,] heights;

        private int width;
        private int length;
        private float height;
        private float sampleWidth;
        private float sampleLength;
    
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<int> triangles = new List<int>();
    
        private Dictionary<Coordinate, int> indexLookup = new Dictionary<Coordinate, int>();

        private void GenerateTerrainMesh(Terrain terrain)
        {
            var terrainData = terrain.terrainData;
            heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

            width = heights.GetLength(0);
            length = heights.GetLength(1);
            sampleWidth = terrainData.size.x / width;
            sampleLength = terrainData.size.z / length;
            height = terrainData.size.y;
        
            var parent = new GameObject(terrain.name + " Mesh").transform;
        
            for (int x = 0; x+1 < width; x++)
            {
                for (int y = 0; y+1 < length; y++)
                {
                    if (vertices.Count + 4 > 65535)
                    {
                        GenerateSubMesh(parent);
                    }
                
                    var v0 = GetOrCreateVertex(x  , y  );
                    var v1 = GetOrCreateVertex(x+1, y  );
                    var v2 = GetOrCreateVertex(x+1, y+1);
                    var v3 = GetOrCreateVertex(x  , y+1);
                
                    triangles.Add(v0);
                    triangles.Add(v1);
                    triangles.Add(v2);
                
                    triangles.Add(v0);
                    triangles.Add(v2);
                    triangles.Add(v3);
                }
            }
        
            GenerateSubMesh(parent);

            var terrainTransform = terrain.transform;
            parent.position = terrainTransform.position;
            parent.rotation = terrainTransform.rotation;
            parent.localScale = terrainTransform.localScale;
            parent.parent = terrainTransform;
        }

        private void GenerateSubMesh(Transform parent)
        {
            var mesh = new Mesh();
            mesh.name = "SubTerrain Mesh";
    #if UNITY_5_2_OR_NEWER
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
    #else
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
    #endif
            ;
            mesh.RecalculateBounds();
            
            var obj = new GameObject("SubTerrain");
            obj.transform.parent = parent;
            obj.AddComponent<MeshFilter>().mesh = mesh;
            var renderer = obj.AddComponent<MeshRenderer>();
            renderer.material = newMaterial;
            renderer.shadowCastingMode = castShadow ? ShadowCastingMode.On : ShadowCastingMode.Off;
            renderer.receiveShadows = receiveShadow;
            
            vertices.Clear();
            uvs.Clear();
            normals.Clear();
            triangles.Clear();
            indexLookup.Clear();
        }

        private int GetOrCreateVertex(int x, int y)
        {
            var coord = new Coordinate(x, y);
            if (indexLookup.ContainsKey(coord))
            {
                return indexLookup[coord];
            }

            var center = GetHeight(x, y);
            vertices.Add(center);
            //I don't know why exactly, but x and y are swapped here... 
            uvs.Add(new Vector2((float)y / length, (float)x / width));
        
            //Calculate normal
            var left =  x > 0        ? GetHeight(x-1, y) : center;
            var right = x+1 < width  ? GetHeight(x+1, y) : center;
            var front = y > 0        ? GetHeight(x, y-1) : center;
            var back =  y+1 < length ? GetHeight(x, y+1) : center;

            var widthDiff = right - left;
            var lengthDiff = front - back;
            normals.Add(Vector3.Cross(lengthDiff, widthDiff));

            int index = vertices.Count - 1;
            indexLookup[coord] = index;
            return index;
        }

        private Vector3 GetHeight(int x, int y)
        {
            //I don't know why exactly, but x and y are swapped here... 
            return new Vector3(y * sampleLength, heights[x, y] * height, x * sampleWidth);
        }

        private struct Coordinate
        {
            public readonly int x, y;

            public Coordinate(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public bool Equals(Coordinate other)
            {
                return x == other.x && y == other.y;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Coordinate && Equals((Coordinate) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (x * 397) ^ y;
                }
            }
        }
    }
}
