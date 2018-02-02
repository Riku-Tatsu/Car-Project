using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour {

	[System.Serializable]
	public class RaceData
	{
		public string raceName = "Race Name";
		public Transform checkpointHolder;
	}

	public RaceData[] raceList;

	void Start () {
		
	}

	private void OnDrawGizmos()
	{
		if(raceList.Length != 0)
		{
			
			for (int i = 0; i < raceList.Length; i++)
			{
				int checkpointCount = raceList[i].checkpointHolder.childCount;

				for (int j = 0; j < checkpointCount; j++)
				{
					if(j == 0)
					{
						Gizmos.color = Color.green;
					}
					else if(j == checkpointCount - 1)
					{
						Gizmos.color = new Color(1, 0.5f, 0);
					}
					else
					{
						Gizmos.color = Color.cyan;
					}

					Vector3 size = raceList[i].checkpointHolder.GetChild(j).localScale;

					Vector3 pos = raceList[i].checkpointHolder.GetChild(j).position;
					Vector3 left = -raceList[i].checkpointHolder.GetChild(j).right;
					Vector3 fwd = raceList[i].checkpointHolder.GetChild(j).forward;
					Vector3 up = raceList[i].checkpointHolder.GetChild(j).up;

					//Vector3 topleft = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(size.x * 0.5f, size.y, 0));
					//Vector3 topright = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(-size.x * 0.5f, size.y, 0));
					//Vector3 bottomleft = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(size.x * 0.5f, 0, 0));
					//Vector3 bottomright = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(-size.x * 0.5f, 0, 0));

					Vector3 topleft = pos + left * size.x * 0.5f + up * size.y; //(size.x * 0.5f, size.y, 0);
					Vector3 topright = pos + left * -size.x * 0.5f + up * size.y; //(-size.x * 0.5f, size.y, 0);
					Vector3 bottomleft = pos + left * size.x * 0.5f; //(size.x * 0.5f, 0, 0);
					Vector3 bottomright = pos + left * -size.x * 0.5f; //(-size.x * 0.5f, 0, 0);

					//Vector3 topleftInner = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(size.x * 0.5f - 0.05f, size.y - 0.05f, 0));
					//Vector3 toprightInner = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(-size.x * 0.5f + 0.05f, size.y - 0.05f, 0));
					//Vector3 bottomleftInner = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(size.x * 0.5f - 0.05f, 0.05f, 0));
					//Vector3 bottomrightInner = raceList[i].checkpointHolder.GetChild(j).TransformPoint(new Vector3(-size.x * 0.5f + 0.05f, 0.05f, 0));

					Vector3 topleftInner = pos + left * (size.x * 0.5f - 0.05f) + up * (size.y - 0.05f);//(size.x * 0.5f - 0.05f, size.y - 0.05f, 0);
					Vector3 toprightInner = pos + left * (-size.x * 0.5f + 0.05f) + up * (size.y - 0.05f);//(-size.x * 0.5f + 0.05f, size.y - 0.05f, 0);
					Vector3 bottomleftInner = pos + left * (size.x * 0.5f - 0.05f) + up * 0.05f;//(size.x * 0.5f - 0.05f, 0.05f, 0);
					Vector3 bottomrightInner = pos + left * (-size.x * 0.5f + 0.05f) + up * 0.05f;//(-size.x * 0.5f + 0.05f, 0.05f, 0);

					Gizmos.DrawLine(topleft, topright);
					Gizmos.DrawLine(topleft, bottomleft);
					Gizmos.DrawLine(bottomleft, bottomright);
					Gizmos.DrawLine(topright, bottomright);

					Gizmos.DrawLine(topleftInner, toprightInner);
					Gizmos.DrawLine(topleftInner, bottomleftInner);
					Gizmos.DrawLine(bottomleftInner, bottomrightInner);
					Gizmos.DrawLine(toprightInner, bottomrightInner);

					Gizmos.DrawLine(pos, pos + fwd * 2);
					Gizmos.DrawSphere(pos, 0.45f);
					Gizmos.DrawSphere(pos + fwd * 2, 0.225f);

					if(j < checkpointCount - 1 && checkpointCount > 1)
					{
						Debug.DrawLine(raceList[i].checkpointHolder.GetChild(j).position, raceList[i].checkpointHolder.GetChild(j + 1).position);
					}
				}
				
			}
		}
	}

	void Update ()
	{

	}
}
