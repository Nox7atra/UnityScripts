using UnityEngine;
using VampLamp.Core.Obstacles;
using VampLamp.Core.Obstacles.Chain;

namespace VampLamp.EditorTools.ObstaclesTools
{
    //Class created for building in-game obstacles
    //Should be used only in Unity editor
    public static class ObstacleBuilder
    {
        private static Core.Events.TouchEventsManager _TouchEventManager;
        public enum ObstacleType
        {
            Lamp,
            Log,
            Switcher,
            WallSquare,
            WallCircle,
            WoodenBox
        }
        
        public static void  CreateObstacle(ObstacleType type)
        {
            GameObject created = null;
            switch (type)
            {
                case ObstacleType.Log:
                    created = CreateLogObstacle();
                    break;
                case ObstacleType.Lamp:
                    created = CreateLamp();
                    break;
                case ObstacleType.Switcher:
                    created = CreateSwitcher();
                    break;
                case ObstacleType.WallSquare:
                    created = CreateWallSquare();
                    break;
                case ObstacleType.WallCircle:
                    created = CreateWallCircle();
                    break;
                case ObstacleType.WoodenBox:
                    created = CreateWoodenBox();
                    break;
            }
            UnityEditor.Selection.activeObject = created;
        }
        public static void  CreateLogChain(GameObject targetRoot, GameObject beginMount, GameObject endMount)
        {
            _TouchEventManager = MonoBehaviour.FindObjectOfType(typeof(Core.Events.TouchEventsManager)) as Core.Events.TouchEventsManager;
            if (_TouchEventManager == null)
            {
                Debug.Log("На сцене нет event manager. Верните!");
                return;
            }
            Vector3 chainBegin = beginMount.transform.position +
                Vector3.down * BuilderProperties.MOUNT_JOINT_ANCOR_Y + Vector3.left * BuilderProperties.MOUNT_JOINT_ANCOR_X,
             chainEnd = endMount.transform.position + 
                Vector3.up * BuilderProperties.MOUNT_JOINT_ANCOR_Y   + Vector3.left * BuilderProperties.MOUNT_JOINT_ANCOR_X;

            BuildChain(targetRoot, CalcChainLength(chainBegin, chainEnd), CalcChainAngle(chainBegin, chainEnd));

            var chain = targetRoot.GetComponent<Chain>();
            AttachChain(chain, beginMount, endMount);
            chain.BeginMount = beginMount;
            chain.EndMount = endMount;

            targetRoot.transform.position = beginMount.transform.position - Vector3.up 
                * BuilderProperties.MOUNT_JOINT_ANCOR_Y  - Vector3.right * BuilderProperties.MOUNT_JOINT_ANCOR_X;
        }
        private static GameObject CreateLogObstacle()
        {
            GameObject root = new GameObject();
            root.name = "LogObstacle";
            var chainCreator = root.AddComponent<Core.EditorTools.ObstaclesTools.ChainCreator>();
            chainCreator.ObstacleMounts = new System.Collections.Generic.List<GameObject>();
            chainCreator.WallMounts     = new System.Collections.Generic.List<GameObject>();
            chainCreator.Chains         = new System.Collections.Generic.List<GameObject>();
            //Instantiate prefabs
            GameObject logObj = GameObject.Instantiate(BuilderProperties.LogPrefab);
            chainCreator.Log = logObj;
            Log logScript = logObj.GetComponent<Log>();
            logObj.transform.SetParent(root.transform);
            GameObject[] chains = new GameObject[logScript.DefaultMounts.Length];
            GameObject[] mounts = new GameObject[logScript.DefaultMounts.Length];
            for (int i = 0; i < logScript.DefaultMounts.Length; i++)
            {
                mounts[i] = GameObject.Instantiate(BuilderProperties.MountPrefab);
                mounts[i].name = "Mount" + (i + 1).ToString();
                mounts[i].transform.position += Vector3.right * 2f * i;
                mounts[i].transform.SetParent(root.transform);

                chains[i] = new GameObject();
                chains[i].name = "Chain" + (i + 1).ToString();
                CreateLogChain(chains[i], mounts[i], logScript.DefaultMounts[i].gameObject);
                chains[i].transform.SetParent(root.transform);

                chainCreator.ObstacleMounts.Add(logScript.DefaultMounts[i].gameObject);
                chainCreator.WallMounts.Add(mounts[i]);
                chainCreator.Chains.Add(chains[i]);
            }
            _TouchEventManager = null;
            return root;
        }
        private static GameObject CreateLamp()
        {
            return GameObject.Instantiate(BuilderProperties.LampPrefab);
        }
        private static GameObject CreateSwitcher()
        {
            return GameObject.Instantiate(BuilderProperties.SwitcherPrefab);
        }
        private static GameObject CreateWallSquare()
        {
            return GameObject.Instantiate(BuilderProperties.WallSquarePrefab);
        }
        private static GameObject CreateWallCircle()
        {
            return GameObject.Instantiate(BuilderProperties.WallCirclePrefab);
        }
        private static GameObject CreateWoodenBox()
        {
            return GameObject.Instantiate(BuilderProperties.WoodenBox);
        }
        private static void BuildChain(GameObject targetRoot, int length, float angle)
        {
            GameObject instantiatedLink = null;
            GameObject previousInstantiatedLink = null;
            for (int i = 0; i < length; i++)
            {
                //Instantiate Link
                instantiatedLink = (i % 2 == 0) ? GameObject.Instantiate(BuilderProperties.FirstLinkPrefab)
                    : GameObject.Instantiate(BuilderProperties.SecondLinkPrefab);
                //Rename it
                instantiatedLink.name = i.ToString();
                //Setup Hinge2DJoint
                SetupJoint(instantiatedLink, previousInstantiatedLink);
                //Setup information about connected Links
                if (previousInstantiatedLink != null)
                {
                    instantiatedLink.GetComponent<Link>().PreviousLink = previousInstantiatedLink.GetComponent<Link>();
                    previousInstantiatedLink.GetComponent<Link>().NextLink = instantiatedLink.GetComponent<Link>();
                }
                else
                {
                    targetRoot.AddComponent<Chain>().FirtsLink = instantiatedLink;
                }
                instantiatedLink.GetComponent<Link>().TouchEventManager = _TouchEventManager;
                instantiatedLink.transform.SetParent(targetRoot.transform);
                previousInstantiatedLink = instantiatedLink;
            }
            targetRoot.GetComponent<Chain>().LastLink = instantiatedLink;
            targetRoot.transform.Rotate(0, 0, angle);
        }
        private static void SetupJoint(GameObject instantiatedLink, GameObject previousInstantiatedLink)
        {
            HingeJoint2D joint = instantiatedLink.GetComponent<HingeJoint2D>();
            //Connect rigidbody
            joint.connectedBody = previousInstantiatedLink != null ?
                previousInstantiatedLink.GetComponent<Rigidbody2D>() : null;
            //Setup anchors
            joint.anchor = new Vector2(BuilderProperties.LINK_JOINT_ANCOR_X, BuilderProperties.LINK_JOINT_ANCOR_Y);
            if (previousInstantiatedLink != null)
            {
                joint.connectedAnchor = new Vector2(BuilderProperties.LINK_JOINT_ANCOR_X, 
                    -BuilderProperties.LINK_JOINT_ANCOR_Y);
                instantiatedLink.transform.position = new Vector3(previousInstantiatedLink.transform.position.x,
                    previousInstantiatedLink.transform.position.y - BuilderProperties.LINK_LENGHT);
            }
            else
            {
                joint.connectedAnchor = new Vector2(BuilderProperties.MOUNT_JOINT_ANCOR_X, 
                    -BuilderProperties.MOUNT_JOINT_ANCOR_Y);
                instantiatedLink.transform.position = new Vector3(0, -0.2f);
            }
        }
        public static void AttachChain(Chain chain, GameObject firstMount, GameObject secondMount)
        {
            // Attach begin
            HingeJoint2D firstLinkJoint = chain.FirtsLink.GetComponent<HingeJoint2D>();
            firstLinkJoint.connectedBody = firstMount.GetComponent<Rigidbody2D>();
            firstLinkJoint.anchor = new Vector2(BuilderProperties.LINK_JOINT_ANCOR_X,
                BuilderProperties.LINK_JOINT_ANCOR_Y);
            // Attach end
            HingeJoint2D mountJoint = secondMount.GetComponent<HingeJoint2D>();
            mountJoint.connectedBody = chain.LastLink.GetComponent<Rigidbody2D>();
            mountJoint.anchor = new Vector2(BuilderProperties.LOG_MOUNT_JOINT_ANCHOR_X,
                BuilderProperties.LOG_MOUNT_JOINT_ANCHOR_Y);
        }
        private static int CalcChainLength(Vector3 begin, Vector3 end)
        {
            return (int)((begin - end).magnitude / (BuilderProperties.LINK_LENGHT));
        }
        private static float CalcChainAngle(Vector3 begin, Vector3 end)
        {
            if (begin.y < end.y)
            {
                var tmp = begin;
                begin = end;
                end = tmp;
                return Vector3.Angle((begin - end), Vector3.right) + 90f;
            }
            else {
                return Vector3.Angle((begin - end), Vector3.right) - 90f;
            }
        }
    }
}
