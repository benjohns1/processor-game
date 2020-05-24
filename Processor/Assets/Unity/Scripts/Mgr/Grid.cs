using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SupplyChain.Graph;
using UnityEngine;

namespace Unity.Scripts.Mgr
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private float gridSize = 1f;
        private readonly NodeComparer comparer = new NodeComparer();

        private struct Location : IEqualityComparer<Location>
        {
            public int X;
            public int Y;

            public override string ToString()
            {
                return $"({X},{Y})";
            }

            public bool Equals(Location x, Location y)
            {
                return x.X == y.X && x.Y == y.Y;
            }

            public int GetHashCode(Location obj)
            {
                return obj.X.GetHashCode() ^ obj.Y.GetHashCode() << 2;
            }
        }
        
        private readonly Dictionary<Location, SupplyChain.Graph.INode> nodes = new Dictionary<Location,SupplyChain.Graph.INode>();
            
        private float posRound;
        
        private void Awake()
        {
            posRound = 1 / gridSize;   
        }

        private float OnGrid(float x)
        {
            return Mathf.Round(x * posRound) / posRound;
        }

        public Vector3 Pos(Vector2 xy, float z = 0f)
        {
            return new Vector3(OnGrid(xy.x), OnGrid(xy.y), z);
        }

        public bool Available(Vector2 pos)
        {
            var loc = new Location
            {
                X = Loc(pos.x),
                Y = Loc(pos.y)
            };

            return !nodes.ContainsKey(loc);
        }

        public bool Add(Vector2 pos, SupplyChain.Graph.INode node)
        {
            var loc = new Location
            {
                X = Loc(pos.x),
                Y = Loc(pos.y)
            };

            if (nodes.ContainsKey(loc))
            {
                return false;
            }

            node.Deleted += NodeOnDeleted;
            nodes.Add(loc, node);
            return true;
        }

        private void NodeOnDeleted(object sender, EventArgs e)
        {
            if (!(sender is SupplyChain.Graph.INode node))
            {
                return;
            }
            foreach (var kv in nodes.Where(kv => comparer.Equals(kv.Value, node)))
            {
                nodes.Remove(kv.Key);
                return;
            }
        }

        private int Loc(float pos)
        {
            return Mathf.RoundToInt(pos / gridSize);
        }
    }
}