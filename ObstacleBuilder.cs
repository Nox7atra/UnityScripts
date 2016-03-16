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
        [SerializeField]
        private static GameObject _LogPrefab;
        [SerializeField]
        private static GameObject _MountPrefab;
        [SerializeField]
        private static GameObject _FirstLinkPrefab;
        [SerializeField]
        private static GameObject _SecondLinkPrefab;
       
        private static bool _IsInitialized = false;
        #endregion
        private static bool IsInitialized {
            get
            {
                _IsInitialized = _IsInitialized && _LogPrefab != null &&
                    _MountPrefab != null && _FirstLinkPrefab != null &&
                    _SecondLinkPrefab != null;
                return _IsInitialized;
            }
        }
        public static void Init()
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

            _IsInitialized = true;
        }

        public static void CreateChain(GameObject targetRoot, int length, float angle = 0)
        {
            Init();

            BuildChain(targetRoot, length, angle);
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
        public static void CreateLogObstacle(GameObject targetRoot)
        {
            Init();

            BuildLogObstacle(targetRoot);
        }
        private static void BuildLogObstacle(GameObject targetRoot)
        {
            //Instantiate prefabs
            GameObject firstChain  = new GameObject();
            GameObject secondChain = new GameObject();
            GameObject logObj      = GameObject.Instantiate(_LogPrefab);
            GameObject firstMount  = GameObject.Instantiate(_MountPrefab);
            GameObject secondMount = GameObject.Instantiate(_MountPrefab);

            Log logScript          = logObj.GetComponent<Log>();
            //Rename some 
            firstChain.name  = "FirstChain";
            secondChain.name = "SecondChain";
            firstMount.name  = "FirstMount";
            secondMount.name = "SecondMount";
            //Change posititon of mounts
            firstMount.transform.position  += Vector3.left  * 2f;
            secondMount.transform.position += Vector3.right * 2f;
            //Create chains
            CreateLogChain(firstChain,  firstMount,  logObj.GetComponent<Log>().FirstMount.gameObject);
            CreateLogChain(secondChain, secondMount, logObj.GetComponent<Log>().SecondMount.gameObject);
            //Pack gameobjects into targetRoot gameobject
            firstChain.transform.SetParent(targetRoot.transform);
            secondChain.transform.SetParent(targetRoot.transform);
            firstMount.transform.SetParent(targetRoot.transform);
            secondMount.transform.SetParent(targetRoot.transform);
            logObj.transform.SetParent(targetRoot.transform); 
        }
        private static void CreateLogChain(GameObject targetRoot, GameObject firstMount, GameObject secondMount)
        {
            Vector3 chainBegin = firstMount.transform.position,
            chainEnd = secondMount.transform.position + Vector3.up * LINK_LENGHT / 2;
            BuildChain(targetRoot, CalcChainLength(chainBegin, chainEnd), CalcChainAngle(chainBegin, chainEnd));
            AttachChain(targetRoot.GetComponent<Chain>(), firstMount, secondMount);
            targetRoot.transform.position = firstMount.transform.position;
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
