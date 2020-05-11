using System.Collections.Generic;
using SupplyChain;
using UnityEngine;

namespace Unity.Scripts
{
    public class MonoGraph : MonoBehaviour
    {
        public static int DistanceToConnectorLength(float distance)
        {
            return Mathf.RoundToInt(distance * 1000f);
        }
        
        public static float ConnectorLengthToDistance(int length)
        {
            return length / 1000f;
        }
        
        public static void CreateNode(MonoGraph g, GameObject prefab, Vector3 pos)
        {
            // Display in game
            var go = Instantiate(prefab, pos, Quaternion.identity);
            
            // Add node to graph
            if (!(go.GetComponent(typeof(IMonoNode)) is IMonoNode mNode))
            {
                return;
            }
            g.AddNode(mNode);
        }

        public struct DrawConnector
        {
            public LineRenderer Prefab;
            public IMonoNode Upstream;
            public Vector3 Start;
            public IMonoNode Downstream;
            public Vector3 End;
        }

        public static void CreateConnector(MonoGraph g, DrawConnector drawConnector)
        {
            var dist = Vector3.Distance(drawConnector.Start, drawConnector.End);
            var length = DistanceToConnectorLength(dist);
            
            // Create connector
            var c = new Connector(drawConnector.Upstream?.GetNode(), drawConnector.Downstream?.GetNode(), length);
            
            // Display in game
            var lr = Instantiate(drawConnector.Prefab, Vector3.zero, Quaternion.identity);
            lr.enabled = false;
            lr.positionCount = 2;
            lr.SetPosition(0, drawConnector.Start);
            lr.SetPosition(1, drawConnector.End);
            
            // Add connector to graph
            if (!g.AddConnector(lr, c))
            {
                Destroy(lr);
                return;
            }
            
            lr.enabled = true;
        }
        
        private Graph graph = new Graph();
        
        private List<LineRenderer> lines = new List<LineRenderer>();
        
        private void Start()
        {
            var sources = FindObjectsOfType<MonoSource>();
            for (var i = sources.Length - 1; i >= 0; i--)
            {
                var node = sources[i].GetNode();
                graph.AddNode(node);
            }
        }

        public Graph GetGraph()
        {
            return graph;
        }

        private void AddNode(IMonoNode mNode)
        {
            var n = mNode.GetNode();
            graph.AddNode(n);
        }

        private bool AddConnector(LineRenderer l, IConnector connector)
        {
            lines.Add(l);
            return graph.AddConnector(connector);
        }
    }
}