using Comon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    //sort
    class CompareSize : IComparer<MyShapes>
    {
        int IComparer<MyShapes>.Compare(MyShapes x, MyShapes y)
        {
            return x.Size.CompareTo(y.Size);
        }
    }
    class CompareLength : IComparer<MyShapes>
    {
        int IComparer<MyShapes>.Compare(MyShapes x, MyShapes y)
        {
            return x.Length.CompareTo(y.Length);
        }
    }
    class CompareWidth : IComparer<MyShapes>
    {
        int IComparer<MyShapes>.Compare(MyShapes x, MyShapes y)
        {
            return (x.Width.CompareTo(y.Width));
        }
    }
    class CompareArea : IComparer<MyShapes>
    {
        int IComparer<MyShapes>.Compare(MyShapes x, MyShapes y)
        {
            return x.AreaOfS.CompareTo(y.AreaOfS);
        }
    }
    class CompareShapes : IComparer<MyShapes>
    {
        int IComparer<MyShapes>.Compare(MyShapes x, MyShapes y)
        {
            return x.PointOnArea.X.CompareTo(y.PointOnArea.X);
        }
    }
}
