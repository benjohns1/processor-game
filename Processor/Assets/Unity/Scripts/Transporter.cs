using System.Collections.Generic;
using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Unity.Scripts.Mgr;
using Ticker = Unity.Scripts.Mgr.Ticker;

namespace Unity.Scripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class Transporter : MonoBehaviour
    {
        [SerializeField] private Packet packetPrefab;
        [SerializeField] private Rate rate;
        [SerializeField] private int speed = 1000;

        private ITransporter transporter;
        private SupplyChain.Ticker ticker;
        private LineRenderer lr;
        private int length;
        private readonly List<Packet> packets = new List<Packet>();
        private Vector3 velocity;
        private Vector3 fixedVelocity;
        
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float sqrMagnitude;

        private void Awake()
        {
            ticker = FindObjectOfType<Ticker>().ticker;
            lr = GetComponent<LineRenderer>();
        }

        public bool Init(NodeGraph g, INode upstream, Vector3 start, INode downstream, Vector3 end)
        {
            // Create transport connector
            var connector = new Connector(upstream.GetNode(), downstream.GetNode());
            if (!g.AddConnector(connector))
            {
                return false;
            }

            startPosition = start;
            endPosition = end;
            sqrMagnitude = (end - start).sqrMagnitude;
            
            length = Graph.DistanceToConnectorLength(Vector3.Distance(start, end));
            var step = Vector3.Lerp(Vector3.zero, end - start, (float) 1 / length);
            velocity = step * ((float) speed / ticker.UpdatesPerTick);
            transporter = new SupplyChain.Transporter(connector, ticker, length, rate.Get(), speed);
            transporter.PacketsMoved += (sender, args) =>
            {
                fixedVelocity = velocity / Time.fixedDeltaTime;
                
                // Refresh all packet locations
                var i = 0;
                foreach (var newPacket in args.Packets)
                {
                    if (packets.Count <= i)
                    {
                        // Create new packets, if needed
                        packets.Add(Instantiate(packetPrefab));
                    }
                    
                    packets[i].Init(newPacket, start + newPacket.Location * step);
                    i++;
                }

                if (packets.Count <= i) return;
                
                // Delete any packets no longer needed
                for (var j = packets.Count - 1; j > i; j--)
                {
                    Destroy(packets[j].gameObject);
                    packets.Remove(packets[j]);
                }
            };
            
            // Display line
            lr.enabled = false;
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.enabled = true;
            
            return true;
        }

        private void Update()
        {
            var change = Time.deltaTime * fixedVelocity;
            foreach (var packet in packets)
            {
                packet.transform.position += change;
                if (packet.MovingPacket.Location + speed <= length)
                {
                    continue;
                }

                // Ensure packet doesn't overshoot end position
                if ((packet.transform.position - startPosition).sqrMagnitude < sqrMagnitude)
                {
                    continue;
                }
                packet.transform.position = endPosition;
            }
        }
    }
}
