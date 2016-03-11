using UnityEngine;
using UnityEditor;
using VampLamp.Core.Builders;

[CustomEditor(typeof(LogObstacleBuilder))]
public class LogObstacleBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LogObstacleBuilder script = (LogObstacleBuilder)target;

        if (GUILayout.Button("Create Chains"))
        {
            script.BuildLogObstacle();
        }
    }
}

