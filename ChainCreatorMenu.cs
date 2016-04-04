using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using VampLamp.Core.EditorTools.ObstaclesTools;
using VampLamp.EditorTools.ObstaclesTools;
[CustomEditor(typeof(ChainCreator))]
public class ChainCreatorMenu : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChainCreator myScript = (ChainCreator)target;
        if (GUILayout.Button("Refresh Chains"))
        {
            Clear();
            CreateNewChains();
        }
        if(GUILayout.Button("Create new Chain"))
        {
            Clear();
            var logMount = GameObject.Instantiate(BuilderProperties.LogMountPrefab);
            var mount    = GameObject.Instantiate(BuilderProperties.MountPrefab);

            var log = myScript.Log.GetComponent<VampLamp.Core.Obstacles.Log>();
            logMount.GetComponent<FixedJoint2D>().connectedBody = log.LogRigidBody;

            logMount.transform.SetParent(log.transform);
            mount.transform.SetParent(myScript.transform);

            logMount.transform.localPosition = Vector3.zero + Vector3.up * 0.1f;
            logMount.transform.localScale    = Vector3.one;
            logMount.name = "LogMount" + (myScript.ObstacleMounts.Count + 1).ToString();
            mount.name    = "Mount"    + (myScript.WallMounts.Count + 1).ToString();

            myScript.WallMounts.Add(mount);
            myScript.ObstacleMounts.Add(logMount);

            CreateNewChains();
        }

    }
    
    private void CreateNewChains()
    {
        ChainCreator myScript = (ChainCreator)target;
        for (int i = 0; i < myScript.WallMounts.Count; i++)
        {
            GameObject root = new GameObject();
            ObstacleBuilder.CreateLogChain(root, myScript.WallMounts[i], myScript.ObstacleMounts[i]);
            ObstacleBuilder.AttachChain(root.GetComponent<VampLamp.Core.Obstacles.Chain.Chain>(),
                myScript.WallMounts[i], myScript.ObstacleMounts[i]);
            root.name = "Chain" + i.ToString();
            root.transform.SetParent(myScript.transform);
            myScript.Chains.Add(root);
        }
    }
    private void Clear()
    {
        ChainCreator myScript = (ChainCreator)target;
        foreach (GameObject chain in myScript.Chains)
        {
            DestroyImmediate(chain);
        }
        myScript.Chains.Clear();

        List<GameObject> tmpList = new List<GameObject>();
        foreach (GameObject mount in myScript.WallMounts)
        {
            
            if (mount != null)
            {
                tmpList.Add(mount);
            }
        }
        myScript.WallMounts = tmpList;

        tmpList = new List<GameObject>();
        foreach (GameObject mount in myScript.ObstacleMounts)
        {

            if (mount != null)
            {
                tmpList.Add(mount);
            }
        }
        myScript.ObstacleMounts = tmpList;
    }
}
