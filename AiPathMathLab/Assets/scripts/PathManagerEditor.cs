using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(pathManager))]
public class PathManagerEditor : Editor
{
    [SerializeField]
    pathManager pathManager;

    [SerializeField]
    List<wayPoint> thePath;
    List<int> toDelete;

    wayPoint selectedPoint = null;
    bool doRepaint = true;

    private void OnSceneGUI()
    {
        thePath = pathManager.GetPath();
        DrawPath(thePath);
    }

    private void OnEnable()
    {
        pathManager = target as pathManager;
        toDelete = new List<int>();
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        thePath = pathManager.GetPath();

        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Path");

        DrawGUIForPoints();

        //button for adding point to the path
        if (GUILayout.Button("Add Point To Path"))
        {
            pathManager.CreateAddPoint();
        }

        EditorGUILayout.EndVertical();
        SceneView.RepaintAll();

    }

    void DrawGUIForPoints()
    {
        if (thePath != null && thePath.Count >0)
        {
            for (int i = 0; i < thePath.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                wayPoint p = thePath[i];

                Color c = GUI.color;
                if (selectedPoint == p) GUI.color = Color.green;

                Vector3 oldPos = p.GetPos();
                Vector3 newPos = EditorGUILayout.Vector3Field("", oldPos);

                if (EditorGUI.EndChangeCheck()) p.SetPos(newPos);

                //delete button
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    toDelete.Add(i); //adds index to our delete list
                }

                GUI.color = c;
                EditorGUILayout.EndHorizontal();
            }
        }

        if (toDelete.Count > 0)
        {
            foreach (int i in toDelete)
                thePath.RemoveAt(i); // remove from path
            toDelete.Clear();//clear the delete list for the next time
        }
    }

    public void DrawPath(List<wayPoint> path)
    {
        if (path != null)
        {
            int current = 0;
            foreach (wayPoint wp in path)
            {
                //draw current point
                doRepaint = DrawPoint(wp);
                int next = (current + +1) % path.Count;
                wayPoint wpnext = path[next];

                DrawPathLine(wp, wpnext);

                //advance counter
                current += 1;
            }
        }

        if (doRepaint) Repaint();
    }

    public bool DrawPoint(wayPoint p)
    {
        bool isChanged = false;
        if (selectedPoint == p)
        {
            Color c = Handles.color;
            Handles.color = Color.green;

            EditorGUI.BeginChangeCheck();
            Vector3 oldPos = p.GetPos();
            Vector3 newPos = Handles.PositionHandle(oldPos, Quaternion.identity);

            float handleSize = HandleUtility.GetHandleSize(newPos);

            Handles.SphereHandleCap(-1, newPos, Quaternion.identity, 0.25f * handleSize, EventType.Repaint);
            if (EditorGUI.EndChangeCheck())
            {
                p.SetPos(newPos);
            }

            Handles.color = c;
        
        }
        else
        {
            Vector3 currPos = p.GetPos();
            float handleSize = HandleUtility.GetHandleSize(currPos);
            if (Handles.Button(currPos, Quaternion.identity, 0.25f* handleSize, 0.25f*handleSize, Handles.SphereHandleCap))
            {
                isChanged = true;
                selectedPoint = p;
            }
        }
        
        return isChanged;

    }

    public void DrawPathLine (wayPoint p1, wayPoint p2)
    {
        //draws line between current and next point
        Color c = Handles.color;
        Handles.color = Color.gray;
        Handles.DrawLine(p1.GetPos(), p2.GetPos());
        Handles.color = c;
    }

}
