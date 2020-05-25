using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts.Mgr
{
    [RequireComponent(typeof(Grid))]
    public class Graph : MonoBehaviour
    {
        private readonly NodeGraph nodeGraph = new NodeGraph();
        private Grid grid;
        
        public static int DistanceToConnectorLength(float distance)
        {
            return Mathf.RoundToInt(distance * 1000f);
        }
        
        public void CreateProcessorNode(Processor processorPrefab, ProcessType process, Vector3 pos)
        {
            if (!grid.Available(pos))
            {
                return;
            }
            
            // Display in game
            var processor = Instantiate(processorPrefab, pos, Quaternion.identity);
            processor.SetProcess(process);
            // ReSharper disable once UseNegatedPatternMatching
            var mNode = processor.GetComponent(typeof(INode)) as INode;
            if (mNode == null || !mNode.Init(nodeGraph) || !grid.Add(processor.transform.position, mNode.GetNode()))
            {
                Destroy(processor);
            }
        }

        public struct DrawConnector
        {
            public GameObject Prefab;
            public INode Upstream;
            public Vector3 Start;
            public INode Downstream;
            public Vector3 End;
            public float Z;
        }

        public void CreateTransportConnector(DrawConnector drawConnector)
        {
            // Display in game
            var go = Instantiate(drawConnector.Prefab, new Vector3(0, 0, drawConnector.Z), Quaternion.identity);
            var monoTransporter = go.GetComponent<Transporter>();
            if (monoTransporter == null || !monoTransporter.Init(nodeGraph, drawConnector.Upstream, drawConnector.Start, drawConnector.Downstream, drawConnector.End))
            {
                Destroy(go);
            }
        }
        
        private void Awake()
        {
            grid = GetComponent<Grid>();
        }
    }
}