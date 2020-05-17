using System;
using System.Collections.Generic;
using System.Linq;
using SupplyChain;
using SupplyChain.Graph;
using UnityEngine;
using Unity.Scripts.Mgr;
using UnityEngine.XR;

namespace Unity.Scripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class MonoTransporter : MonoBehaviour, IMonoConnector
    {
        [SerializeField] private MonoPacket packetPrefab;
        [SerializeField] private int rate = 1;
        [SerializeField] private int speed = 1000;

        private ITransporter transporter;
        private Ticker ticker;
        private LineRenderer lr;
        private int length;
        private readonly List<MonoPacket> packets = new List<MonoPacket>();
        private Vector3 velocity;
        private Vector3 fixedVelocity;
        
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float sqrMagnitude;

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

            startPosition = start;
            endPosition = end;
            sqrMagnitude = (end - start).sqrMagnitude;
            
            length = MonoGraph.DistanceToConnectorLength(Vector3.Distance(start, end));
            var step = Vector3.Lerp(Vector3.zero, end - start, (float) 1 / length);
            velocity = step * ((float) speed / ticker.UpdatesPerTick);
            transporter = new Transporter(connector, ticker, length, rate, speed);
            transporter.PacketsMoved += (sender, args) =>
            {
                fixedVelocity = velocity / Time.fixedDeltaTime;
                
                // Refresh all packet locations
                var i = 0;
                foreach (var newPacket in args.packets)
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
                    if (packets.Remove(packets[j]))
                    {
                        Destroy(packets[j].gameObject);
                    }
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

        public IConnector GetConnector() => transporter.GetConnector();

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
