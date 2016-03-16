using UnityEngine;
using VampLamp.Core.Obstacles;
using VampLamp.Core.Obstacles.Chain;

namespace VampLamp.EditorTools.ObstaclesTools
{
    //Class created for building in-game obstacles
    //Should be used only in Unity editor
    public static class ObstacleBuilder
    {
        #region constants
        //Временный костыль, так как в спрайты запечены тени и при смене спрайта прийдётся вручную пересчитывать ширину тени
        private const float LINK_JOINT_ANCOR_X = 0.06f;
        private const float LINK_JOINT_ANCOR_Y = 0.25f;
        private const float LINK_LENGHT = 0.55f;
        private const float MOUNT_JOINT_ANCOR_X = 0.06f;
        private const float MOUNT_JOINT_ANCOR_Y = 0.3f;
        private const float LOG_MOUNT_JOINT_ANCHOR_X = 0;
        private const float LOG_MOUNT_JOINT_ANCHOR_Y = 0.9f;
        #endregion
        #region private properties
        private static GameObject _LogPrefab;
        private static GameObject _MountPrefab;
        private static GameObject _FirstLinkPrefab;
        private static GameObject _SecondLinkPrefab;
        private static GameObject _LampPrefab;
        private static GameObject _SwitcherPrefab;

        private static Core.Events.TouchEventsManager _TouchEventManager;
        private static bool _IsInitialized = false;
     
        private static bool IsInitialized {
            get
            {
                _IsInitialized = _IsInitialized && _LogPrefab != null &&
                    _MountPrefab != null && _FirstLinkPrefab != null &&
                    _SecondLinkPrefab != null && _LampPrefab != null &&
                    _SwitcherPrefab != null;
                return _IsInitialized;
            }
        }
        #endregion
        public enum ObstacleType
        {
            Lamp,
            Log,
            Switcher
        }
        private static void Init()
        {
            if (IsInitialized)
            {
                return;
            }
            //Loading prefabs
            _LogPrefab        = Resources.Load(EditorPathConstants.LogPrefabPath)        as GameObject;
            _MountPrefab      = Resources.Load(EditorPathConstants.MountPrefabPath)      as GameObject;
            _FirstLinkPrefab  = Resources.Load(EditorPathConstants.FirstLinkPrefabPath)  as GameObject;
            _SecondLinkPrefab = Resources.Load(EditorPathConstants.SecondLinkPrefabPath) as GameObject;
            _LampPrefab       = Resources.Load(EditorPathConstants.LampPrefabPath)       as GameObject;
            _SwitcherPrefab   = Resources.Load(EditorPathConstants.SwitcherPrefabPath)   as GameObject;
            _IsInitialized = true;
        }

        public static void CreateObstacle(ObstacleType type)
        {
            Init();
            switch (type)
            {
                case ObstacleType.Log:
                    CreateLogObstacle();
                    break;
                case ObstacleType.Lamp:
                    CreateLamp();
                    break;
                case ObstacleType.Switcher:
                    CreateSwitcher();
                    break;
            }
        }
        private static void CreateLogObstacle()
        {
            _TouchEventManager = MonoBehaviour.FindObjectOfType(typeof(Core.Events.TouchEventsManager)) as Core.Events.TouchEventsManager;
            if(_TouchEventManager == null)
            {
                Debug.Log("На сцене нет event manager. Верните!");
                return;
            }
            GameObject root = new GameObject();
            root.name = "LogObstacle";
            //Instantiate prefabs
            GameObject logObj      = GameObject.Instantiate(_LogPrefab);
            Log logScript          = logObj.GetComponent<Log>();
            logObj.transform.SetParent(root.transform);

            GameObject[] chains = new GameObject[logScript.Mounts.Length];
            GameObject[] mounts = new GameObject[logScript.Mounts.Length];
            for(int i = 0; i < logScript.Mounts.Length; i++)
            {
                mounts[i] = GameObject.Instantiate(_MountPrefab);
                mounts[i].name = "Mount" + (i + 1).ToString();
                mounts[i].transform.position += Vector3.right * 2f * i;
                mounts[i].transform.SetParent(root.transform);

                chains[i] = new GameObject();
                chains[i].name = "Chain" + (i + 1).ToString();
                CreateLogChain(chains[i], mounts[i], logScript.Mounts[i].gameObject);
                chains[i].transform.SetParent(root.transform);
            }
            _TouchEventManager = null;
        }
        private static void CreateLamp()
        {
            GameObject.Instantiate(_LampPrefab);
        }
        private static void CreateSwitcher()
        {
            GameObject.Instantiate(_SwitcherPrefab);
        }
        private static void CreateLogChain(GameObject targetRoot, GameObject beginMount, GameObject endMount)
        {
            Vector3 chainBegin = beginMount.transform.position,
             chainEnd = endMount.transform.position + Vector3.up * LINK_LENGHT / 2;

            BuildChain(targetRoot, CalcChainLength(chainBegin, chainEnd), CalcChainAngle(chainBegin, chainEnd));

            var chain = targetRoot.GetComponent<Chain>();
            AttachChain(chain, beginMount, endMount);
            chain.BeginMount = beginMount;
            chain.EndMount = endMount;

            targetRoot.transform.position = beginMount.transform.position;
        }
        private static void BuildChain(GameObject targetRoot, int length, float angle)
        {
            GameObject instantiatedLink = null;
            GameObject previousInstantiatedLink = null;
            for (int i = 0; i < length; i++)
            {
                //Instantiate Link
                instantiatedLink = (i % 2 == 0) ? GameObject.Instantiate(_FirstLinkPrefab)
                    : GameObject.Instantiate(_SecondLinkPrefab);
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
            joint.anchor = new Vector2(LINK_JOINT_ANCOR_X, LINK_JOINT_ANCOR_Y);
            if (previousInstantiatedLink != null)
            {
                joint.connectedAnchor = new Vector2(LINK_JOINT_ANCOR_X, -LINK_JOINT_ANCOR_Y);
                instantiatedLink.transform.position = new Vector3(previousInstantiatedLink.transform.position.x,
                    previousInstantiatedLink.transform.position.y - LINK_JOINT_ANCOR_Y * 2);
            }
            else
            {
                joint.connectedAnchor = new Vector2(MOUNT_JOINT_ANCOR_X, -MOUNT_JOINT_ANCOR_Y);
                instantiatedLink.transform.position = new Vector3(-MOUNT_JOINT_ANCOR_X, -MOUNT_JOINT_ANCOR_Y * 2);
            }
        }
        public static void AttachChain(Chain chain, GameObject firstMount, GameObject secondMount)
        {
            // Attach begin
            HingeJoint2D firstLinkJoint = chain.FirtsLink.GetComponent<HingeJoint2D>();
            firstLinkJoint.connectedBody = firstMount.GetComponent<Rigidbody2D>();
            firstLinkJoint.anchor = new Vector2(LINK_JOINT_ANCOR_X, LINK_JOINT_ANCOR_Y);
            // Attach end
            HingeJoint2D mountJoint = secondMount.GetComponent<HingeJoint2D>();
            mountJoint.connectedBody = chain.LastLink.GetComponent<Rigidbody2D>();
            mountJoint.anchor = new Vector2(LOG_MOUNT_JOINT_ANCHOR_X, LOG_MOUNT_JOINT_ANCHOR_Y);
        }
        private static int CalcChainLength(Vector3 begin, Vector3 end)
        {
            return (int)((begin - end).magnitude / LINK_LENGHT);
        }
        private static float CalcChainAngle(Vector3 begin, Vector3 end)
        {
            return Vector3.Angle((begin - end), Vector3.right) - 90f;
        }
    }
}
