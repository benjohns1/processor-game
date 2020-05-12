using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class MonoTransporter : MonoBehaviour, IMonoConnector
    {
        private ITransporter transporter;
        private Ticker ticker;
        private LineRenderer lr;
        [SerializeField] private int rate = 1;
        [SerializeField] private int speed = 1000;

        private void Awake()
        {
            ticker = FindObjectOfType<MonoTicker>().ticker;
            lr = GetComponent<LineRenderer>();
        }

        public bool Init(NodeGraph g, IMonoNode upstream, Vector3 start, IMonoNode downstream, Vector3 end)
        {
            // Create transport connector
            var connector = new Connector(upstream.GetNode(), downstream.GetNode());
            if (!g.AddConnector(connector))
            {
                return false;
            }
            var length = MonoGraph.DistanceToConnectorLength(Vector3.Distance(start, end));
            transporter = new Transporter(connector, ticker, length, rate, speed);
            
            // Display line
            lr.enabled = false;
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.enabled = true;
            
            return true;
        }

        public IConnector GetConnector() => transporter.GetConnector();
    }
}
