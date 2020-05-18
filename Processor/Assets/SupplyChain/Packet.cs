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

        public Packet NewShape(Shape shape)
        {
            return new Packet(shape, Amount);
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
        private Shape[] includeShapes;
        private Shape[] excludeShapes;

        public Filter(bool allowAll, Shape[] includeShapes, Shape[] excludeShapes)
        {
            this.allowAll = allowAll;
            this.includeShapes = includeShapes;
            this.excludeShapes = excludeShapes;
        }

        public static Filter AllowAll()
        {
            return new Filter
            {
                allowAll = true,
            };
        }

        public static Filter AllowShapes(params Shape[] shapes)
        {
            return new Filter
            {
                includeShapes = shapes,
            };
        }

        public static Filter ExcludeShapes(params Shape[] shapes)
        {
            return new Filter
            {
                allowAll = true,
                excludeShapes = shapes,
            };
        }

        public bool MatchShape(Shape shape)
        {
            if (allowAll)
            {
                if (excludeShapes.Length == 0)
                {
                    return true;   
                }

                return excludeShapes.All(pShape => pShape != shape);
            }

            if (includeShapes.Length == 0)
            {
                return false;
            }

            return includeShapes.Any(pShape => pShape == shape);
        }
    }
    
    public enum Shape
    {
        Triangle,
        Square,
        Pentagon,
        Hexagon
    }
}