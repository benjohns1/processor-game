using System;
using System.Linq;

namespace SupplyChain
{
    public readonly struct Packet
    {
        public Shape Shape { get; }
        public int Amount { get; }

        public Packet(Shape shape, int amount)
        {
            Shape = shape;
            Amount = amount;
        }

        public Packet NewAmount(int amount)
        {
            return new Packet(Shape, amount);
        }

        public override string ToString()
        {
            return $"{Amount} {Shape}";
        }
    }

    [Serializable]
    public struct Filter
    {
        private bool allowAll;
        private Shape[] shapes;

        public Filter(bool allowAll, Shape[] shapes)
        {
            this.allowAll = allowAll;
            this.shapes = shapes;
        }

        public static Filter All()
        {
            return new Filter
            {
                allowAll = true,
            };
        }

        public static Filter Shapes(params Shape[] shapes)
        {
            return new Filter
            {
                shapes = shapes,
            };
        }

        public bool MatchShape(Shape shape)
        {
            if (allowAll)
            {
                return true;
            }

            if (shapes.Length == 0)
            {
                return false;
            }

            return shapes.Any(pShape => pShape == shape);
        }
    }
    
    public enum Shape
    {
        Triangle,
        Square
    }
}