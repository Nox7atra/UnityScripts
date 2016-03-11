
using UnityEngine;
using UnityEditor;
using VampLamp.Core.Builders;

[CustomEditor(typeof(BuildersProperties))]
public class BulderPropertiesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BuildersProperties script = (BuildersProperties) target;

        if (GUILayout.Button("Attach Builders"))
        {
            if (script.GetComponent<ChainBuilder>() == null)
            {
                script.gameObject.AddComponent<ChainBuilder>().Properties = script;
            }
        }
    }
}

