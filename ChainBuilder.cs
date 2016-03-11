using UnityEngine;
using System.Collections;
using VampLamp.Core.Obstacles.Chain;

namespace VampLamp.Core.Builders
{
    public class ChainBuilder : MonoBehaviour
    {
        #region constants
        //Временный костыль, так как в спрайты запечены тени и при смене спрайта прийдётся вручную пересчитывать ширину тени
        private const float LINK_ANCOR_X = 0.06f;
        private const float LINK_ANCOR_Y = 0.25f;
        private const float MOUNT_ANCOR_X = 0.06f;
        private const float MOUNT_ANCOR_Y = 0.3f;
        #endregion
        #region private properties
        [SerializeField]
        private int _NumLinks;
        [SerializeField]
        private BuildersProperties _Properties;
        #endregion

        public BuildersProperties Properties
        {
            set
            {
                _Properties = value;
            }
        }

        public void BuildChain()
        {
            GameObject root = new GameObject();
            root.name = "Chain";
            CreateLinks(root, _NumLinks);
        }
        public void BuildChain(GameObject targetRoot, int numLinks, GameObject mount = null)
        {
            CreateLinks(targetRoot, numLinks, mount);
        }

        private void CreateLinks(GameObject targetRoot, int numLinks, GameObject mount = null)
        {
            GameObject instantiatedLink = null;
            GameObject previousInstantiatedLink = null;
            for (int i = 0; i < numLinks; i++)
            {
                instantiatedLink = (i % 2 == 0) ? Instantiate(_Properties.FirstLinkPrefab) 
                    : Instantiate(_Properties.SecondLinkPrefab);
                instantiatedLink.name = i.ToString();

                SetupJoint(mount, instantiatedLink, previousInstantiatedLink);
                instantiatedLink.GetComponent<Link>().PreviousLink = previousInstantiatedLink;
                instantiatedLink.GetComponent<Link>().TouchEventManager = _Properties.TouchEventManager;
                instantiatedLink.transform.SetParent(targetRoot.transform);
                previousInstantiatedLink = instantiatedLink;
            }
            targetRoot.AddComponent<Chain>().LastLink = instantiatedLink;
        }

        private void SetupJoint(GameObject mount, GameObject instantiatedLink, GameObject previousInstantiatedLink)
        {
            HingeJoint2D joint = instantiatedLink.GetComponent<HingeJoint2D>();

            //Connect rigidbody
            if (mount != null)
            {
                joint.connectedBody = previousInstantiatedLink != null ?
                    previousInstantiatedLink.GetComponent<Rigidbody2D>() : mount.GetComponent<Rigidbody2D>();
            }
            else
            {
                joint.connectedBody = previousInstantiatedLink != null ?
                    previousInstantiatedLink.GetComponent<Rigidbody2D>() : null;
            }

            //Setup anchors
            joint.anchor = new Vector2(LINK_ANCOR_X, LINK_ANCOR_Y);
            if (previousInstantiatedLink != null)
            {
                joint.connectedAnchor = new Vector2(LINK_ANCOR_X, -LINK_ANCOR_Y);
                previousInstantiatedLink.GetComponent<Link>().NextLink = instantiatedLink;
                instantiatedLink.transform.position = new Vector3(previousInstantiatedLink.transform.position.x,
                    previousInstantiatedLink.transform.position.y - LINK_ANCOR_Y * 2);
            }
            else
            {
                joint.connectedAnchor = new Vector2(MOUNT_ANCOR_X, -MOUNT_ANCOR_Y);
                instantiatedLink.transform.position = new Vector3(-MOUNT_ANCOR_X, -MOUNT_ANCOR_Y * 2);
            }
        }
    }
}
