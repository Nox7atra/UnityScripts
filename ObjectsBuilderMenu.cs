using UnityEngine;
using UnityEditor;
using VampLamp.EditorTools;
using VampLamp.EditorTools.ObstaclesTools;

public class ObjectsBuilderMenu : EditorWindow
{
    private string[] _ObjectType = new string[] {
        "Obstacle",
        "Decoration"
    };

    private int _SelectedButtonIndex = 0;
    [MenuItem("Tools/Objects")]
    internal static void Init()
    {
        // EditorWindow.GetWindow() will return the open instance of the specified window or create a new
        // instance if it can't find one. The second parameter is a flag for creating the window as a
        // Utility window; Utility windows cannot be docked like the Scene and Game view windows.
        var window = (ObjectsBuilderMenu)GetWindow(typeof(ObjectsBuilderMenu), false, "Objects Builder Menu");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }
    internal void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Objects Builder", EditorStyles.boldLabel);
        _SelectedButtonIndex = GUILayout.Toolbar(_SelectedButtonIndex, _ObjectType);
        switch (_SelectedButtonIndex)
        {
            case 0:
                UseObstacleGUI();
                break;
            case 1:
                UseDecorationGUI();
                break;
        }
        EditorGUILayout.EndVertical();
    }

    private void UseObstacleGUI()
    {
        EditorGUILayout.BeginHorizontal(new GUIStyle()
        {
            padding = new RectOffset(0, 0, 10, 0)
        });
        bool isCreateLogObstacle = GUILayout.Button("Create Log Obstacle", EditorGUIStyles.ButtonStyle);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool isCreateLamp        = GUILayout.Button("Create Lamp",         EditorGUIStyles.ButtonStyle);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool isCreateSwitcher    = GUILayout.Button("Create Switcher",     EditorGUIStyles.ButtonStyle);
        EditorGUILayout.EndHorizontal();

        if (isCreateLogObstacle)
        {
            ObstacleBuilder.CreateObstacle(ObstacleBuilder.ObstacleType.Log);
        }
        if (isCreateLamp)
        {
            ObstacleBuilder.CreateObstacle(ObstacleBuilder.ObstacleType.Lamp);
            var eventManager = FindObjectOfType(typeof(VampLamp.Core.Events.ReachedEventManager)) as VampLamp.Core.Events.ReachedEventManager;
            if(eventManager != null)
            {
                eventManager.FindAllLightsInScene();
            }
            else
            {
                Debug.Log("Протерялся ивент менеджер. Верните!");
            }
        }
        if (isCreateSwitcher)
        {
            ObstacleBuilder.CreateObstacle(ObstacleBuilder.ObstacleType.Switcher);
        }
    }
    private void UseDecorationGUI()
    {

    }
}
