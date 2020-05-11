using System.Collections.Generic;
using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts
{
    public class MonoGraph : MonoBehaviour
    {
        public static int DistanceToConnectorLength(float distance)
        {
            return Mathf.RoundToInt(distance * 1000f);
        }
        
        public static void CreateNode(MonoGraph g, GameObject prefab, Vector3 pos)
        {
            // Display in game
            var go = Instantiate(prefab, pos, Quaternion.identity);
            
            // Add node to graph
            if (!(go.GetComponent(typeof(IMonoNode)) is IMonoNode mNode))
            {
                Destroy(go);
                return;
            }

            if (!g.AddNode(mNode))
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

        public static void CreateTransportConnector(MonoGraph g, DrawConnector drawConnector)
        {
            // Display in game
            var go = Instantiate(drawConnector.Prefab, Vector3.zero, Quaternion.identity);
            var monoTransporter = go.GetComponent<MonoTransporter>();
            if (monoTransporter == null || !monoTransporter.Init(drawConnector.Upstream, drawConnector.Start, drawConnector.Downstream, drawConnector.End))
            {
                Destroy(go);
                return;
            }
            
            // Add connector to graph
            if (!g.AddConnector(monoTransporter))
            {
                Destroy(go);
                return;
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

        public NodeGraph GetGraph()
        {
            return nodeGraph;
        }

        private bool AddNode(IMonoNode mNode)
        {
            var n = mNode.GetNode();
            return nodeGraph.AddNode(n);
        }

        private bool AddConnector(IMonoConnector mConnector)
        {
            var c = mConnector.GetConnector();
            return nodeGraph.AddConnector(c);
        }
    }
}