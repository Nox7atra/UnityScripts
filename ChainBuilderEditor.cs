using UnityEngine;
using UnityEditor;
using VampLamp.Core.Obstacles.Chain;

[CustomEditor(typeof(ChainBuilder))]
public class ChainBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChainBuilder myScript = (ChainBuilder)target;
        if (GUILayout.Button("Build Chain"))
        {
            myScript.BuildChain();
        }
    }
}
