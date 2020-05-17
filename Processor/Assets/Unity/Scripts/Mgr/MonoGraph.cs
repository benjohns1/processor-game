using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts.Mgr
{
    public class MonoGraph : MonoBehaviour
    {
        public static int DistanceToConnectorLength(float distance)
        {
            return Mathf.RoundToInt(distance * 1000f);
        }
        
        public void CreateNode(GameObject prefab, Vector3 pos)
        {
            // Display in game
            var go = Instantiate(prefab, pos, Quaternion.identity);
            var mNode = go.GetComponent(typeof(IMonoNode)) as IMonoNode;
            if (mNode == null || !mNode.Init(nodeGraph))
            {
                Destroy(go);
            }
        }

        public struct DrawConnector
        {
            public GameObject Prefab;
            public IMonoNode Upstream;
            public Vector3 Start;
            public IMonoNode Downstream;
            public Vector3 End;
        }

        public void CreateTransportConnector(DrawConnector drawConnector)
        {
            // Display in game
            var go = Instantiate(drawConnector.Prefab, Vector3.zero, Quaternion.identity);
            var monoTransporter = go.GetComponent<MonoTransporter>();
            if (monoTransporter == null || !monoTransporter.Init(nodeGraph, drawConnector.Upstream, drawConnector.Start, drawConnector.Downstream, drawConnector.End))
            {
                Destroy(go);
            }
        }
        
        private NodeGraph nodeGraph = new NodeGraph();
        
        private void Start()
        {
            var sources = FindObjectsOfType<MonoSource>();
            for (var i = sources.Length - 1; i >= 0; i--)
            {
                var node = sources[i].GetNode();
                nodeGraph.AddNode(node);
            }
        }
    }
}