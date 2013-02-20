using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GraphUpdateScene))]
public class GraphUpdateSceneEditor : Editor {
	
	public override void OnInspectorGUI () {
		
		GraphUpdateScene script = target as GraphUpdateScene;
		
		if (script.points == null) script.points = new Vector3[0];
		
		Vector3[] prePoints = script.points;
		DrawDefaultInspector ();
		EditorGUI.indentLevel = 1;
		
		if (prePoints != script.points) { script.RecalcConvex (); HandleUtility.Repaint (); }
		
		bool preConvex = script.convex;
		script.convex = EditorGUILayout.Toggle (new GUIContent ("Convex","Sets if only the convex hull of the points should be used or the whole polygon"),script.convex);
		if (script.convex != preConvex) { script.RecalcConvex (); HandleUtility.Repaint (); }
		
		script.applyOnStart = EditorGUILayout.Toggle ("Apply On Start",script.applyOnStart);
		script.applyOnScan = EditorGUILayout.Toggle ("Apply On Scan",script.applyOnScan);
		
		script.modifyWalkability = EditorGUILayout.Toggle ("Modify walkability",script.modifyWalkability);
		if (script.modifyWalkability) {
			EditorGUI.indentLevel++;
			script.setWalkability = EditorGUILayout.Toggle ("Walkability",script.setWalkability);
			EditorGUI.indentLevel--;
		}
		
		script.penalty = EditorGUILayout.IntField ("Penalty",script.penalty);
		
		script.modifyTag = EditorGUILayout.Toggle (new GUIContent ("Modify Tags","Should the tags of the nodes be modified"),script.modifyTag);
		if (script.modifyTag) {
			EditorGUI.indentLevel++;
			script.setTag = EditorGUILayout.Popup ("Set Tag",script.setTag,AstarPath.FindTagNames ());
			EditorGUI.indentLevel--;
		}
		
		//GUI.color = Color.red;
		if (GUILayout.Button ("Tags can be used to restrict which units can walk on what ground. Click here for more info","HelpBox")) {
			Application.OpenURL (AstarPathEditor.GetURL ("tags"));
		}
		
		//GUI.color = Color.white;
		
		if (GUILayout.Button ("Clear all points")) {
			Undo.RegisterUndo (script,"Removed All Points");
			script.points = new Vector3[0];
			script.RecalcConvex ();
		}
		
		if (GUI.changed) EditorUtility.SetDirty (target);
	}
	
	int selectedPoint = -1;
	
	const float pointGizmosRadius = 0.09F;
	static Color PointColor = new Color (1,0.36F,0,0.6F);
	static Color PointSelectedColor = new Color (1,0.24F,0,1.0F);
	
	public void OnSceneGUI () {
		
		
		GraphUpdateScene script = target as GraphUpdateScene;
		
		if (script.points == null) script.points = new Vector3[0];
		
		if (Event.current.type == EventType.Layout) {
			for (int i=0;i<script.points.Length;i++) {
				HandleUtility.AddControl (-i - 1,HandleUtility.DistanceToLine (script.points[i],script.points[i]));
			}
		}
		HandleUtility.AddDefaultControl (0);
		
		for (int i=0;i<script.points.Length;i++) {
			
			if (i == selectedPoint && Tools.current == Tool.Move) {
				Handles.color = PointSelectedColor;
				Undo.SetSnapshotTarget(script, "Moved Point");
				Handles.SphereCap (-i-1,script.points[i],Quaternion.identity,HandleUtility.GetHandleSize (script.points[i])*pointGizmosRadius*2);
				script.points[i] = Handles.PositionHandle (script.points[i],Quaternion.identity);
			} else {
				Handles.color = PointColor;
				Handles.SphereCap (-i-1,script.points[i],Quaternion.identity,HandleUtility.GetHandleSize (script.points[i])*pointGizmosRadius);
			}
		}
		
		if(Input.GetMouseButtonDown(0)) {
            // Register the undos when we press the Mouse button.
            Undo.CreateSnapshot();
            Undo.RegisterSnapshot();
        }
		
		if (Event.current.type == EventType.MouseDown) {
			int pre = selectedPoint;
			selectedPoint = -(HandleUtility.nearestControl+1);
			if (pre != selectedPoint) GUI.changed = true;
		}
		
		if (Event.current.type == EventType.MouseDown && Event.current.shift && Tools.current == Tool.Move) {
			
			if (((int)Event.current.modifiers & (int)EventModifiers.Alt) != 0) {
				//int nearestControl = -(HandleUtility.nearestControl+1);
				
				if (selectedPoint >= 0 && selectedPoint < script.points.Length) {
					Undo.RegisterUndo (script,"Removed Point");
					List<Vector3> arr = new List<Vector3>(script.points);
					arr.RemoveAt (selectedPoint);
					script.points = arr.ToArray ();
					script.RecalcConvex ();
					GUI.changed = true;
				}
			} else {
				System.Object hit = HandleUtility.RaySnap (HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
				if (hit != null) {
					RaycastHit rayhit = (RaycastHit)hit;
					
					Undo.RegisterUndo (script,"Added Point");
					
					Vector3[] points = new Vector3[script.points.Length+1];
					for (int i=0;i<script.points.Length;i++) {
						points[i] = script.points[i];
					}
					points[points.Length-1] = rayhit.point;
					script.points = points;
					script.RecalcConvex ();
					GUI.changed = true;
				}
			}
			Event.current.Use ();
		}
		
		if (Event.current.shift && Event.current.type == EventType.MouseDrag) {
			//Event.current.Use ();
		}
		
		if (GUI.changed) { HandleUtility.Repaint (); EditorUtility.SetDirty (target); }
	}
}
