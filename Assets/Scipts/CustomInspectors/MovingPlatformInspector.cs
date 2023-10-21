using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MovingPlatform movingPlatform = (MovingPlatform)target;
        if (GUILayout.Button("Add Point"))
        {
            movingPlatform.AddPoint();
        }

        if (GUILayout.Button("Remove Point"))
        {
            movingPlatform.RemovePoint();
        }
    }
}
#endif