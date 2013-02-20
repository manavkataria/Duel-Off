using UnityEngine;
using System.Collections;
using Pathfinding;

[AddComponentMenu("Pathfinding/GraphUpdateScene")]
public class GraphUpdateScene : MonoBehaviour {
	
	public void Awake () {
		AstarPath.OnPostScan += Apply;
	}
	
	public void OnDisable () {
		AstarPath.OnPostScan -= Apply;
	}
	
	public void Start () {
		
		//If firstApplied is true, that means the graph was scanned during Awake.
		//So we shouldn't apply it again because then we would end up applying it two times
		if (!firstApplied && applyOnStart) {
			Apply ();
		}
	}
	
	public Vector3[] points;
	Vector3[] convexPoints;
	
	[HideInInspector]
	public bool convex = true;
	[HideInInspector]
	public int penalty = 0;
	[HideInInspector]
	public bool modifyWalkability = false;
	[HideInInspector]
	public bool setWalkability = false;
	[HideInInspector]
	public bool applyOnStart = true;
	[HideInInspector]
	public bool applyOnScan = true;
	
	[HideInInspector]
	public bool modifyTag = false;
	[HideInInspector]
	public int setTag = 0;
	
	private bool firstApplied = false;
	
	public void RecalcConvex () {
		if (convex) convexPoints = Polygon.ConvexHull (points); else convexPoints = null;
	}
	
	public void Apply (AstarPath active) {
		if (applyOnScan) {
			Apply ();
		}
	}
	
	/** Updates graphs with a created GUO.
	 * Creates a Pathfinding::GraphUpdateObject with a Pathfinding::GraphUpdateShape
	 * representing the polygon of this object and update all graphs using AstarPath::UpdateGraphs.
	 * This will not update graphs directly. See AstarPath::UpdateGraph for more info.
	 */
	public void Apply () {
		
		if (AstarPath.active == null) {
			Debug.LogError ("There is no AstarPath object in the scene");
			return;
		}
		
		firstApplied = true;
		
		Pathfinding.GraphUpdateShape shape = new Pathfinding.GraphUpdateShape ();
		shape.convex = convex;
		shape.points = points;
		
		GraphUpdateObject guo = new GraphUpdateObject (shape.GetBounds ());
		guo.shape = shape;
		guo.modifyWalkability = modifyWalkability;
		guo.setWalkability = setWalkability;
		guo.addPenalty = penalty;
		guo.modifyTag = modifyTag;
		guo.setTag = setTag;
		
		AstarPath.active.UpdateGraphs (guo);
	}
	
	public void OnDrawGizmos () {
		OnDrawGizmos (false);
	}
	
	public void OnDrawGizmosSelected () {
		OnDrawGizmos (true);
	}
		
	public void OnDrawGizmos (bool selected) {
		
		if (points == null) return;
		
		Gizmos.color = selected ? new Color (0,0.9F,0,1F) : new Color (0,0.9F,0,0.5F);
		for (int i=0;i<points.Length;i++) {
			Gizmos.DrawLine (points[i],points[(i+1)%points.Length]);
			//Gizmos.DrawRay (points[i],Vector3.down);
		}
		
		
		if (convex) {
			if (convexPoints == null) RecalcConvex ();
			
			Gizmos.color = selected ? new Color (0.9F,0,0,1F) : new Color (0.9F,0,0,0.5F);
			for (int i=0;i<convexPoints.Length;i++) {
				Gizmos.DrawLine (convexPoints[i],convexPoints[(i+1)%convexPoints.Length]);
			}
		}
	}
}