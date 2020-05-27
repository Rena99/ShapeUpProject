using System.Collections.Generic;
using System;
using Repositories.Models1;
using System.Text;

namespace Commons
{
    public class MyShapes
    {
        public MyShapes()
        {

        }
        public MyShapes(int id, int unit, ICollection<Point> points)
        {
            Id = id;
            Unit = unit;
            Length = setLength(points);
            Width = setWidth(points);
            Vectors = setVectors(points);
            MinX = setMinX(points);
            MinY = setMinY(points);
        }
        public int Id { get; set; }
        public int Unit { get; set; }
        public double Measure { get
            {
                switch (Unit)
                {
                    case 1: return 1 * 1000000;//kilometre
                    case 2: return 1 * 10;//centimetre
                    case 3: return 1 * 1000;//metre
                    case 4: return 1 * 1609344;//mile
                    case 5: return 1 * 914.4000;//yard
                    case 6: return 1 * 304.8000;//foot
                    case 7: return 1 * 25.40000;//inch
                    default: return 1; //millimeter
                }
            }
        }
        public Vector[] Vectors { get; set; }
        public Vector[] setVectors(ICollection<Point> points)
        {int i = 0;
                Vector[] VectorsVectors = new Vector[points.Count];
                foreach (var item in points)
                {
                    VectorsVectors[i]= new Vector((int)(item.X)*Measure-MinX, (int)(item.Y)*Measure-MinY);
                }
                return VectorsVectors;
        }
        public double[][] LinearEquation {
            get
            {
                //y=mx+b
                //0-m, 1=b, 2=vertical
                double[][] equations = new double[Vectors.Length][];
                for (int i = 1; i < Vectors.Length; i++)
                {
                    if (Vectors[i - 1].X == Vectors[i].X)
                    {
                        equations[i - 1][0] = Vectors[i].X;
                        equations[i - 1][1] = 0;
                        equations[i - 1][2] = 1;
                        continue;
                    }
                    equations[i - 1][0] = (Vectors[i-1].Y-Vectors[i].Y)/(Vectors[i-1].X-Vectors[i].X);
                    equations[i - 1][1] =-(equations[i-1][0]*Vectors[i].X)+Vectors[i].Y;
                    equations[i - 1][2] = 0;
                }
                if (Vectors[Vectors.Length - 1].X == Vectors[0].X)
                {
                    equations[Vectors.Length - 1][0] = Vectors[0].X;
                    equations[Vectors.Length - 1][1] = 0;
                    equations[Vectors.Length - 1][2] = 1;
                }
                else
                {
                    equations[Vectors.Length-1][0] = (Vectors[Vectors.Length - 1].Y - Vectors[0].Y) / (Vectors[Vectors.Length - 1].X - Vectors[0].X);
                    equations[Vectors.Length-1][1] = -(equations[Vectors.Length - 1][0] * Vectors[0].X) + Vectors[0].Y;
                    equations[Vectors.Length-1][2] = 0;
                }
                return equations;
            }
        }
        public int MinX { get; set; }
        public int setMinX(ICollection<Point> points)
        {
            decimal minx = -1;
            foreach (var item in points)
            {
                if (minx == -1) { minx = item.X; }
                else if (item.X < minx) minx = item.X;
            }
            return (int)minx * (int)Measure;

        }
        public int MinY { get; set; }
        public int setMinY(ICollection<Point> points)
        {
            decimal miny = -1;
            foreach (var item in points)
            {
                if (miny == -1) { miny = item.Y; }
                else if (item.Y < miny) miny = item.Y;
            }
            return (int)miny * (int)Measure;
        }
        public int Size
        {
            get
            {
                return Length * Width;
            }
        }
        public int Length { get; set; }
        public int setLength(ICollection<Point> points)
        {
            decimal x = System.Int16.MaxValue, x2 = 0;
            foreach (var item in points)
            {
                if (item.X < x) x = item.X;
                if (item.X > x2) x2 = item.X;
            }
            return (int)(x - x2) * (int)Measure;
        }
        public int Width { get; set; }
        public int setWidth(ICollection<Point> points)
        {
            decimal y = System.Int16.MaxValue, y2 = 0;
            foreach (var item in points)
            {
                if (item.Y < y) y = item.Y;
                if (item.Y > y2) y2 = item.Y;
            }
            return (int)(y - y2) * (int)Measure;
        }
        public double AreaOfS
        {
            get
            {
                double one = 0, two = 0;
                for (int i = 0; i < Vectors.Length; i++)
                {
                    if (i + 1 < Vectors.Length)
                    {
                        one += Vectors[i].X * Vectors[i + 1].Y;
                        two += Vectors[i].Y * Vectors[i + 1].X;
                    }
                    else
                    {
                        one += Vectors[i].X * Vectors[0].Y;
                        two += Vectors[i].Y * Vectors[0].X;
                    }
                }
                return (one - two) / 2;
            }
        }
        public Vector PointOnArea { get; set; }
        public int indexOfPoint { get; set; }
    }

}

    

