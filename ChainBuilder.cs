using UnityEngine;
using System.Collections;

namespace VampLamp.Core.Obstacles.Chain
{
    public class ChainBuilder : MonoBehaviour
    {
        #region constants
        //Временный костыль, так как в спрайты запечены тени и при смене спрайта прийдётся вручную пересчитывать ширину тени
        private const float LINK_ANCOR_X = 0.06f;
        private const float LINK_ANCOR_Y = 0.25f;
        private const float MOUNT_ANCOR_X = 0;
        private const float MOUNT_ANCOR_Y = 0.3f;
        #endregion
        #region private properties
        [SerializeField]
        private GameObject _MountPrefab;
        [SerializeField]
        private GameObject _FirstLinkPrefab;
        [SerializeField]
        private GameObject _SecondLinkPrefab;
        [SerializeField]
        private int _NumLinks;
        #endregion

        public void BuildChain()
        {
            GameObject root = new GameObject();
            root.name = "Chain";
            GameObject instantiatedMount = Instantiate(_MountPrefab);
            instantiatedMount.transform.SetParent(root.transform);
            GameObject instantiatedLink;
            GameObject previousInstantiatedLink = null;
            for(int i = 0; i < _NumLinks; i++)
            {
                instantiatedLink = (i % 2 == 0) ? Instantiate(_FirstLinkPrefab) : Instantiate(_SecondLinkPrefab);

                HingeJoint2D joint = instantiatedLink.GetComponent<HingeJoint2D>();
                joint.connectedBody = previousInstantiatedLink != null ? previousInstantiatedLink.GetComponent<Rigidbody2D>() : instantiatedMount.GetComponent<Rigidbody2D>();
                joint.anchor = new Vector2(LINK_ANCOR_X, LINK_ANCOR_Y);
                if (previousInstantiatedLink != null)
                {
                    joint.connectedAnchor = new Vector2(LINK_ANCOR_X, -LINK_ANCOR_Y);
                }
                else
                {
                    joint.connectedAnchor = new Vector2(MOUNT_ANCOR_X, -MOUNT_ANCOR_Y);
                }

                instantiatedLink.transform.SetParent(root.transform);
                previousInstantiatedLink = instantiatedLink;
            }
        }
 
    }
}
