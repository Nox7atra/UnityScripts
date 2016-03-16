using UnityEngine;
using UnityEditor;
using VampLamp.EditorTools.ObstaclesTools;

public class ObstacleBuilderMenu : EditorWindow
{

    [MenuItem("Tools/Obstacle Builder Menu")]
    internal static void Init()
    {
        // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
        // instance if it can't find one. The second parameter is a flag for creating the window as a
        // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
        var window = (ObstacleBuilderMenu)GetWindow(typeof(ObstacleBuilderMenu), false, "Obstacle Builder Menu");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
        ObstacleBuilder.Init();
    }
    internal void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Obstacle Builder", EditorStyles.boldLabel);

        bool isCreateLogObstacle = GUILayout.Button("Build Log",
            new GUIStyle(GUI.skin.GetStyle("Button"))
            {
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 40f,
                fontSize = 15
            });

        if (isCreateLogObstacle)
        {
            GameObject root = new GameObject();
            root.name = "LogObstacle";
            ObstacleBuilder.CreateLogObstacle(root);
        }

        EditorGUILayout.EndVertical();
    }
}
