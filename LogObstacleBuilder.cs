using UnityEngine;
using System.Collections;
using VampLamp.Core.Obstacles;

namespace VampLamp.Core.Builders
{
    public class LogObstacleBuilder : MonoBehaviour
    {
        #region constants
        private const float LINK_LENGHT = 0.55f;
        #endregion
        #region private properties
        [SerializeField]
        private GameObject _FirstMount;
        [SerializeField]
        private GameObject _SecondMount;
        [SerializeField]
        private GameObject _Log;
        #endregion
        public void BuildLogObstacle()
        {
            ClearChains();


            //Creating first chain
            Vector3 chainBegin = _FirstMount.transform.position, 
                chainEnd = _Log.GetComponent<Log>().FirstMount.transform.position;
            int firstChainLength = CalcChainLength(chainBegin, chainEnd);
            float firstChainAngle = CalcChainAngle(chainBegin, chainEnd);
            GameObject firstChain = BuildChain("FirstChain", _FirstMount, firstChainLength, firstChainAngle);

            //Creating second chain
            chainBegin = _SecondMount.transform.position; 
            chainEnd = _Log.GetComponent<Log>().SecondMount.transform.position;
            int secondChainLength = CalcChainLength(chainBegin, chainEnd);
            float secondChainAngle = CalcChainAngle(chainBegin, chainEnd);
            GameObject secondChain = BuildChain("SecondChain", _SecondMount, secondChainLength, secondChainAngle);

            _Log.GetComponent<Log>().AttachChains(firstChain, secondChain);
        }

        private GameObject BuildChain(string chainName, GameObject mount, int length, float angle)
        {
            ChainBuilder chainBuilder = GetComponent<ChainBuilder>();
            GameObject ChainRoot = new GameObject();
            chainBuilder.BuildChain(ChainRoot, length, mount);
            ChainRoot.name = chainName;
            ChainRoot.transform.position = mount.transform.position;
            ChainRoot.transform.Rotate(0, 0, angle - 90f);
            ChainRoot.transform.SetParent(mount.transform);
            return ChainRoot;
        }

        private void ClearChains()
        {
            Transform tmp = _FirstMount.transform.FindChild("FirstChain");
            if (tmp != null)
            {
                DestroyImmediate(tmp.gameObject);
            }
            tmp = _SecondMount.transform.FindChild("SecondChain");
            if (tmp != null)
            {
                DestroyImmediate(tmp.gameObject);
            }
        }

        /// <summary>
        /// Calculating num links in chain between 2 points
        /// </summary>
        /// <param name="begin">Point</param>
        /// <param name="end">Point</param>
        /// <returns></returns>
        private int CalcChainLength(Vector3 begin, Vector3 end)
        {
            return (int)((begin - end).magnitude / LINK_LENGHT);
        }
        private float CalcChainAngle(Vector3 begin, Vector3 end)
        {
            return Vector3.Angle((begin - end), Vector3.right);
        }
    }
}