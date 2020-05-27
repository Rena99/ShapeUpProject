using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Repositories.Models;
using Repositories;
using Comon;
//clean code
//shrinking algorithm
//test bigger
//test not fit
namespace Services
{
    public class Algorithm
    {
        public MyShapes area { get; set; } //area

        public List<MyShapes> shapes { get; set; } //list of shapes

        public bool Succeeded { get; set; } //true if managed to place all shapes

        public int Length { get; set; } //length of all shapes

        //constructer
        public Algorithm(List<MyShapes> myShapes, MyShapes area)
        {
            shapes = myShapes;
            this.area = area;
            Succeeded = true;
            foreach (var item in shapes)
            {
                Length += item.Length;
            }
            OrderShapes();
            Succeeded = SetShapes();
        }

        //sorts shape from hardest to place to easiest to place
        public void OrderShapes()
        {
            CompareSize s = new CompareSize();
            CompareLength l = new CompareLength();
            CompareWidth w = new CompareWidth();
            CompareArea a = new CompareArea();
            shapes.Sort(s);
            shapes.Sort(a);
            if (area.Length > area.Width)
            {
                shapes.Sort(l);
                shapes.Sort(w);
            }
            else
            {
                shapes.Sort(w);
                shapes.Sort(l);
            }
            shapes.Reverse();
        }

        //places shapes on area
        public bool SetShapes()
        {
            double sum = 0;
            bool success = true;
            foreach (var item in shapes)//checks that every shape fits
            {
                if (item.Width > area.Width || item.Length > area.Length) return false;
                sum += item.AreaOfS;
            }
            if (sum > area.AreaOfS) return false; //checks that total area of shapes is less than area of area
            MyShapes t = null;
            List<MyShapes> myShapes = new List<MyShapes>();
            foreach (var item in shapes) //places every shape on area
            {
                success = PlaceShape(item, ref t, myShapes);
                if (!success) return false;
                myShapes.Add(item);
            }
           // ShrinkingAlgorithm(shapes, area);
            foreach (var item in shapes)//checks if shapes fit in area
            {
                if (item.PointOnArea.X + (item.Length - item.indexOfPoint.X) > area.Length) return false;
            }
            return success;
        }

        //shrinks length of area
        private void ShrinkingAlgorithm(List<MyShapes> MyShapes, MyShapes area)
        {
            //start from rightmost shape and try to place shape more in left
            CompareShapes r = new CompareShapes();//sorts shapes by location
            MyShapes.Sort(r);
            int c = MyShapes.Count - 1;
            int i = MyShapes.Count - 1;
            List<MyShapes> done = new List<MyShapes>();
            List<MyShapes> myShapes = new List<MyShapes>();
            bool d = false;
            while (i >= 0 && c > 0)
            {
                for (int k = 0; k < i; k++) myShapes.Add(MyShapes[k]);
                int OptimalO = -1;
                Vector OptimalA = new Vector(Length, 0);
                foreach (var item in done)
                {
                    d = (item == MyShapes[i]) ? true : false;
                }
                if (!d)
                {
                    for (int j = myShapes.Count - 1; j > i; j--)
                    {
                        MyShapes t = shapes[j];
                        GetLocation(MyShapes[i], myShapes[j], ref OptimalO, ref OptimalA, myShapes);
                        if (OptimalO != -1)
                        {
                            PlaceShape(MyShapes[i], ref t, myShapes);
                            MyShapes.Sort(r);
                            c--;
                            done.Add(MyShapes[i]);
                            break;
                        }
                        c--;
                        i++;
                    }
                }
            }
        }

        //places shapes
        public bool PlaceShape(MyShapes shap, ref MyShapes s, List<MyShapes> myShapes)
        {
            //step 1-place first shape
            if (s == null)
            {
                int indexOfF = 0;
                for (int p = 1; p < shap.Vectors.Length; p++)
                {
                    if (shap.Vectors[p].X < shap.Vectors[indexOfF].X) indexOfF = p;
                    else if (shap.Vectors[p].X == shap.Vectors[indexOfF].X && shap.Vectors[p].Y < shap.Vectors[indexOfF].Y) indexOfF = p;
                }
                Vector indexOnA = new Vector(0, area.Width - (area.Width - shap.Vectors[indexOfF].Y));
                PlaceOnArea(shap, indexOfF, indexOnA);
                s = shap;
                return true;
            }
            //step 2-place next shape
            int OptimalO = -1;
            Vector OptimalA = new Vector(Length, 0);
            GetLocation(shap, s, ref OptimalO, ref OptimalA, myShapes);
            if (OptimalO == -1) return false;
            LeftBottom(myShapes, shap, OptimalO, ref OptimalA);
            PlaceOnArea(shap, OptimalO, OptimalA);
            s = shap;
            return true;
        }

        //NTF-get location on which to place shape
        private void GetLocation(MyShapes shap, MyShapes s, ref int OptimalO, ref Vector OptimalA, List<MyShapes> myShapes)
        {
            int i = 1, SIndexOfS = 0;
            for (; i < s.Vectors.Length; i++)
            {
                if (s.Vectors[i].Y < s.Vectors[SIndexOfS].Y) SIndexOfS = i;
            }
            int indexOfS = SIndexOfS, SindexOfO = 0;
            for (i = 1; i < shap.Vectors.Length; i++)
            {
                if (shap.Vectors[i].Y > shap.Vectors[SindexOfO].Y) SindexOfO = i;
            }
            int indexOfO = SindexOfO;
            bool firstMove = true; //condition to stop-returns to original location
            Vector OE = new Vector(-1, -1); //if sliding stopped before vertex of orbiting shape
            Vector SE = new Vector(-1, -1); //if sliding stopped before vertex of stationary shape
            int PCorrectVector = -1;
            int POCorrectVector = -1;
            int[][] PossibleMoves = new int[4][]; //saves all translations
            for (i = 0; i < 4; i++) PossibleMoves[i] = new int[9];
            //0-statinary direction, 1-orbiting direction, 2-orbiting position, 3-translation derived vector
            //4-orbiting vertex change, 5-stationary vertex change, 6-translation vector, 7-stationary line, 8-orbiting line
            while (firstMove == true || !(indexOfS == SIndexOfS && indexOfO == SindexOfO))
            {
                //save optimal location
                Vector staticp = SE.X==-1? (s.PointOnArea - s.indexOfPoint) + s.Vectors[indexOfS]: 
                    new Vector((s.PointOnArea - s.indexOfPoint).X + SE.X, (s.PointOnArea - s.indexOfPoint).Y +SE.Y);
                double below, above, left;
                if (OE.X == -1)
                {
                    below = shap.Vectors[indexOfO].Y;
                    above = shap.Width - shap.Vectors[indexOfO].Y;
                    left =shap.Vectors[indexOfO].X;
                }
                else
                {
                    below = OE.Y;
                    above = shap.Width - OE.Y;
                    left = OE.X;
                }
                if (CanPlace(myShapes, GetVectors(shap, staticp, indexOfO), s.Id, Length) && staticp.Y - below >= 0 && staticp.Y + above < area.Width&&staticp.X-left>=0)
                {
                    if (staticp.X < OptimalA.X)
                    {
                        OptimalA = staticp;
                        OptimalO = indexOfO;
                    }
                    else if (staticp.X == OptimalA.X && staticp.Y < OptimalA.Y)
                    {
                        OptimalA = staticp;
                        OptimalO = indexOfO;
                    }
                }
                //start=0, end=1, right=0, left=1, stationary=0, orbiting=1

                BuildTable(ref PossibleMoves, shap, indexOfO, s, indexOfS, SE, OE);
                int correctTVector;
                if (SE.X != -1 || OE.X != -1)
                {
                    correctTVector = 0;
                    SE = OE = new Vector(-1, -1);
                }
                else
                {
                    EliminatesTransVectors(ref PossibleMoves, s, shap, indexOfO, indexOfS);
                    correctTVector = GetCorrectVector(PossibleMoves, PCorrectVector, POCorrectVector, s, shap);
                    if (correctTVector == -1) break; //no translation possible
                    PCorrectVector = PossibleMoves[correctTVector][7];
                    POCorrectVector = PossibleMoves[correctTVector][8];
                    Vector Trim = TrimTranslation(PossibleMoves[correctTVector], shap, s, indexOfS, indexOfO);
                    if (Trim.X != -1)
                    {
                        if (PossibleMoves[correctTVector][3] == 1) OE = new Vector(shap.Vectors[PossibleMoves[correctTVector][4]].X - Trim.X,
                            shap.Vectors[PossibleMoves[correctTVector][4]].Y - Trim.Y);
                        else SE = new Vector(s.Vectors[PossibleMoves[correctTVector][5]].X - Trim.X,
                            s.Vectors[PossibleMoves[correctTVector][5]].Y - Trim.Y);
                    }
                }
                indexOfO = PossibleMoves[correctTVector][4]; 
                indexOfS = PossibleMoves[correctTVector][5]; 
                firstMove = false;
            }
        }

        private void BuildTable(ref int[][] PossibleMoves, MyShapes shap, int indexOfO, MyShapes s, int indexOfS, Vector SE, Vector OE)
        {
            int i = 0;
            Vector[] ShapMovedVector = GetVectors(shap, s.PlacedVectors[indexOfS], indexOfO);
            int previousO, nextO, previousS, nextS;
            previousS = indexOfS == 0 ? s.Vectors.Length - 1 : indexOfS - 1;
            nextS = indexOfS == s.Vectors.Length - 1 ? 0 : indexOfS + 1;
            previousO = indexOfO == 0 ? shap.Vectors.Length - 1 : indexOfO - 1;
            nextO = indexOfO == shap.Vectors.Length - 1 ? 0 : indexOfO + 1;
            if (SE.X == -1 && OE.X == -1) //get translation options
            {

                PossibleMoves[0][0] = PossibleMoves[0][1] = 1;//e, e
                PossibleMoves[0][2] = ShapMovedVector[previousO].X <= s.PlacedVectors[indexOfS].X ? 1 : 0;
                PossibleMoves[1][0] = 1;//e
                PossibleMoves[1][1] = 0;//s
                PossibleMoves[1][2] = ShapMovedVector[nextO].X <= s.PlacedVectors[indexOfS].X ? 1 : 0;
                PossibleMoves[2][0] = PossibleMoves[2][1] = 0; //s, s
                PossibleMoves[2][2] = ShapMovedVector[nextO].X <= s.PlacedVectors[indexOfS].X ? 1 : 0;
                PossibleMoves[3][0] = 0;//s
                PossibleMoves[3][1] = 1;//e
                PossibleMoves[3][2] = ShapMovedVector[previousO].X <= s.PlacedVectors[indexOfS].X ? 1 : 0;
                for (i = 0; i < 4; i++)//get vector derivision and eliminate vectors
                {
                    if ((PossibleMoves[i][0] == 1 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 0)||
                        (PossibleMoves[i][0] == 1 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 1))
                    {
                        PossibleMoves[i][3] = 1;
                        PossibleMoves[i][7] = indexOfS == 0 ? s.LinearEquation.Length - 1 : indexOfS - 1;
                        PossibleMoves[i][8] = indexOfO == shap.LinearEquation.Length - 1 ? 0 : indexOfO + 1;
                    }
                    else if (PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 1)
                    {
                        PossibleMoves[i][3] = 1;
                        PossibleMoves[i][7] = indexOfS == s.LinearEquation.Length - 1 ? 0 : indexOfS + 1;
                        PossibleMoves[i][8] = indexOfO == shap.LinearEquation.Length - 1 ? 0 : indexOfO + 1;
                    }
                    else if (PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 0)
                    {
                        PossibleMoves[i][3] = 0;
                        PossibleMoves[i][7] = indexOfS == s.LinearEquation.Length - 1 ? 0 : indexOfS + 1;
                        PossibleMoves[i][8] = indexOfO == shap.LinearEquation.Length - 1 ? 0 : indexOfO + 1;
                    }
                    else if ((PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 1 && PossibleMoves[i][2] == 0)||
                        (PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 1 && PossibleMoves[i][2] == 1))
                    {
                        PossibleMoves[i][3] = 0;
                        PossibleMoves[i][7] = indexOfS == s.LinearEquation.Length - 1 ? 0 : indexOfS + 1;
                        PossibleMoves[i][8] = indexOfO == 0 ? shap.LinearEquation.Length - 1 : indexOfO - 1;
                    }
                    else PossibleMoves[i][3] = PossibleMoves[i][7] = PossibleMoves[i][8] = -1;
                }
                for (i = 0; i < 4; i++)//eliminate incorrect translation vector
                {
                    if (PossibleMoves[i][3] == -1) PossibleMoves[i][4] = PossibleMoves[i][5] = -1;
                    else if ((PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 1) ||
                        (PossibleMoves[i][0] == 1 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 0)||
                        (PossibleMoves[i][0] == 1 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 1))
                    {
                        PossibleMoves[i][4] = nextO;
                        PossibleMoves[i][5] = indexOfS;
                        PossibleMoves[i][6] = indexOfO;
                    }
                    else if ((PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 0 && PossibleMoves[i][2] == 0) ||
                         (PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 1 && PossibleMoves[i][2] == 0)||
                         (PossibleMoves[i][0] == 0 && PossibleMoves[i][1] == 1 && PossibleMoves[i][2] == 1))
                    {
                        PossibleMoves[i][4] = indexOfO;
                        PossibleMoves[i][5] = nextS;
                        PossibleMoves[i][6] = indexOfS;
                    }
                }
            }
            else if (SE.X != -1)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int j2 = 0; j2 < 7; j2++)
                    {
                        PossibleMoves[j][j2] = -1;
                    }
                }
                PossibleMoves[0][4] = indexOfO;
                PossibleMoves[0][5] = nextS;
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int j2 = 0; j2 < 6; j2++)
                    {
                        PossibleMoves[j][j2] = -1;
                    }
                }
                PossibleMoves[0][4] = nextO;
                PossibleMoves[0][5] = indexOfS;
            }
        }
        public void EliminatesTransVectors(ref int[][] PossibleMoves, MyShapes s, MyShapes shap, int indexOfO, int indexOfS)
        {
            for (int i = 0; i < 4; i++)
            {
                if (PossibleMoves[i][3] == -1) continue;
                else
                {
                    Vector[] vectors = GetVectors(shap, s.PlacedVectors[indexOfS], indexOfO);
                    Vector distanceTrans = vectors[PossibleMoves[i][4]] - vectors[indexOfO];
                    Vector vector = new Vector();
                    int si;
                    if (PossibleMoves[i][0] == 0)
                    {
                        si = indexOfS == s.Vectors.Length - 1 ? 0 : indexOfS + 1;
                    }
                    else
                    {
                        si = indexOfS == 0 ? s.Vectors.Length - 1 : indexOfS - 1;
                    }
                    int oi;
                    if (PossibleMoves[i][1] == 0)
                    {
                        oi = indexOfO == shap.Vectors.Length - 1 ? 0 : indexOfO + 1;
                    }
                    else
                    {
                        oi = indexOfO == 0 ? shap.Vectors.Length - 1 : indexOfO - 1;
                    }
                    if (CheckBasicVector(vectors, s, shap, PossibleMoves[i][3], indexOfS, si, indexOfO, oi))
                    {
                        PossibleMoves[i][3] = -1;
                        continue;
                    }
                    vectors = GetVectors(shap, s.PlacedVectors[PossibleMoves[i][5]], indexOfO);
                    for (int j = 0; j < vectors.Length; j++) vectors[j] -= distanceTrans;
                    if (PossibleMoves[i][3] == 1)
                    {
                        if ((PossibleMoves[i][4] < indexOfO && indexOfO!=vectors.Length-1) || (PossibleMoves[i][4] == vectors.Length - 1 && indexOfO == 0)) PossibleMoves[i][3] = -1;
                        else if (IsInside(s.PlacedVectors, s.PlacedVectors.Length, vectors[indexOfO], Length + area.Width, vectors[PossibleMoves[i][4]])) PossibleMoves[i][3] = -1;
                        else
                        {
                            for (int j = 0; j < vectors.Length; j++)
                            {
                                int k = j == vectors.Length - 1 ? 0 : j + 1;
                                if (LineSegementsIntersect(vectors[j], vectors[k], s.PlacedVectors[indexOfS], s.PlacedVectors[si], out vector, false) &&
                                    vector.X != s.PlacedVectors[indexOfS].X && vector.Y != s.PlacedVectors[indexOfS].Y &&
                                    vector.X != s.PlacedVectors[si].X && vector.Y != s.PlacedVectors[si].Y)
                                {
                                    PossibleMoves[i][3] = -1;
                                    break;
                                }
                            }
                            for (int j = 0; j < s.PlacedVectors.Length; j++)
                            {
                                int k = j == s.PlacedVectors.Length - 1 ? 0 : j + 1;
                                if (LineSegementsIntersect(s.PlacedVectors[j], s.PlacedVectors[k], vectors[indexOfO], vectors[oi], out vector, false) &&
                                    vector.X != vectors[indexOfO].X && vector.Y != vectors[indexOfO].Y &&
                                    vector.X != vectors[oi].X && vector.Y != vectors[oi].Y)
                                {
                                    PossibleMoves[i][3] = -1;
                                    break;
                                }
                            }
                        }
                    }
                    else if (PossibleMoves[i][3] == 0)
                    {
                        if ((PossibleMoves[i][5] < indexOfS && indexOfS!=s.Vectors.Length-1) || (PossibleMoves[i][5] == s.Vectors.Length - 1 && indexOfS == 0)) PossibleMoves[i][3] = -1;
                        if (IsInside(vectors, vectors.Length, s.PlacedVectors[indexOfS], Length, s.PlacedVectors[PossibleMoves[i][5]])) PossibleMoves[i][3] = -1;
                        else
                        {
                            for (int j = 0; j < s.PlacedVectors.Length; j++)
                            {
                                int k = j == s.PlacedVectors.Length - 1 ? 0 : j + 1;
                                if (LineSegementsIntersect(s.PlacedVectors[j], s.PlacedVectors[k], vectors[indexOfO], vectors[oi], out vector, false) &&
                                    vector.X != vectors[indexOfO].X && vector.Y != vectors[indexOfO].Y&&
                                    vector.X != vectors[oi].X && vector.Y != vectors[oi].Y)
                                {
                                    PossibleMoves[i][3] = -1;
                                    break;
                                }
                            }
                            for (int j = 0; j < vectors.Length; j++)
                            {
                                int k = j == vectors.Length - 1 ? 0 : j + 1;
                                if (LineSegementsIntersect(vectors[j], vectors[k], s.PlacedVectors[indexOfS], s.PlacedVectors[si], out vector, false) &&
                                    vector.X != s.PlacedVectors[indexOfS].X && vector.Y != s.PlacedVectors[indexOfS].Y &&
                                    vector.X != s.PlacedVectors[si].X && vector.Y != s.PlacedVectors[si].Y)
                                {
                                    PossibleMoves[i][3] = -1;
                                    break;
                                }
                            }
                        }
                    }
                    if(PossibleMoves[i][3]!=-1)
                    {
                        int c = 0;
                        for (int j = 0; j < vectors.Length; j++)
                        {
                            if (IsInside(s.PlacedVectors, s.PlacedVectors.Length, vectors[j], Length + area.Width)) c++;
                        }
                        if (c == vectors.Length) PossibleMoves[i][3] = -1;
                    }
                }
            }

        }

        private bool CheckBasicVector(Vector[] vectors, MyShapes s, MyShapes shap, int v, int indexOfS, int si, int indexOfO, int oi)
        {
            double x = 0;
            double y = 0;
            List<MyShapes> shapes = new List<MyShapes>();
            shapes.Add(s);
            if (v == 1)
            {
                if (vectors[oi].X > vectors[indexOfO].X) x = .5;
                else if (vectors[oi].X < vectors[indexOfO].X) x = -.5;
                y = (vectors[oi].Y - vectors[indexOfO].Y) / (vectors[oi].X - vectors[indexOfO].X) * 
                    (vectors[indexOfO].X + x-vectors[oi].X) + vectors[oi].Y;
                y = vectors[indexOfO].Y-y;
            }
            else
            {
                if (s.PlacedVectors[si].X > s.PlacedVectors[indexOfS].X) x = - .5;
                else if (s.PlacedVectors[si].X < s.PlacedVectors[indexOfS].X) x = .5;
                y = (s.PlacedVectors[si].Y - s.PlacedVectors[indexOfS].Y) / (s.PlacedVectors[si].X - s.PlacedVectors[indexOfS].X) *
                   (s.PlacedVectors[indexOfS].X + x-s.PlacedVectors[si].X) + s.PlacedVectors[si].Y;
                y = s.PlacedVectors[indexOfS].Y-y;
            }
            Vector vector = new Vector(x, y*-1);
            for (int i = 0; i < vectors.Length; i++) vectors[i] -= vector;
            if (!CanMove(shapes, vectors)) return true;
            for (int i = 0; i < vectors.Length; i++) vectors[i] += vector;
            return false;
        }

        //places shape on area
        private void PlaceOnArea(MyShapes myShape, int indexOfF, Vector indexOnA)
        {
            myShape.PlacedVectors = GetVectors(myShape, indexOnA, indexOfF);
            myShape.PointOnArea = indexOnA;
            myShape.indexOfPoint = myShape.Vectors[indexOfF];
        }

        //moves every vertex of shape to location on which will be placed
        public Vector[] GetVectors(MyShapes s, Vector indexOnA, int indexOfF)
        {
            double difx = indexOnA.X - s.Vectors[indexOfF].X, diffy = indexOnA.Y - s.Vectors[indexOfF].Y;
            Vector[] vector = new Vector[s.Vectors.Length];
            for (int i = 0; i < vector.Length; i++) vector[i] = new Vector(s.Vectors[i].X + difx, s.Vectors[i].Y + diffy);
            return vector;
        }

        //checks if could places shape on this location
        public bool CanPlace(List<MyShapes> myShapes, Vector[] vectors, int cShape, int INF)
        {
            bool sPoint = false;
            for (int i = 0; i < myShapes.Count; i++)
            {
                if (myShapes[i].Id == cShape) continue;
                for (int j = 0; j < vectors.Length; j++)
                {
                    foreach (var item in myShapes[i].PlacedVectors)
                    {
                        if (vectors[j].X == item.X && vectors[j].Y == item.Y) sPoint = true;
                    }
                    if (!sPoint && IsInside(myShapes[i].PlacedVectors, myShapes[i].PlacedVectors.Length, vectors[j], INF * 2)) return false;
                    sPoint = false;
                }
            }
            return true;
        }

        // Given three colinear points p, q, r, the function checks if point q lies on line segment 'pr' 
        static bool OnSegment(Vector p, Vector q, Vector r)
        {
            if (q.X <= Math.Max(p.X, r.X) &&
                q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) &&
                q.Y >= Math.Min(p.Y, r.Y))
            {
                return true;
            }
            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values: 0 --> p, q and r are colinear, 1 --> Clockwise, 2 --> Counterclockwise 
        static int Orientation(Vector p, Vector q, Vector r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0; // colinear 
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // The function that returns true if line segment 'p1q' and 'p2q2' intersect. 
        static bool DoIntersect(Vector p1, Vector q1, Vector p2, Vector q2)
        {
            // Find the four orientations needed for general and special cases 
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4) return true;

            // Special Cases: 
            //p1, q1 and p2 are colinear and  p2 lies on segment p1q1 
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

            // p1, q1 and p2 are colinear and  q2 lies on segment p1q1 
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            // Doesn't fall in any of the above cases 
            return false;
        }

        // Returns true if the point p lies inside the polygon[] with n vertices 
        static bool IsInside(Vector[] polygon, int n, Vector p, int INF, Vector o = null)
        {
           
            Vector vector = new Vector();
            Vector extreme;
            // There must be at least 3 vertices in polygon[] 
            if (n < 3) return false;

            // Create a point for line segment from p to infinite 
            if (o!=null && o.Y == p.Y) extreme = new Vector(p.X, INF);
            else extreme = new Vector(INF, p.Y);
            foreach (var item in polygon)
            {
                if (item.Y == p.Y)
                {
                    extreme = new Vector(p.X, INF);
                    break;
                }
            }

            // Count intersections of the above line with sides of polygon 
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % n;
                // Check if the line segment from 'p' to 'extreme' intersects with the line segment from 'polygon[i]' to 'polygon[next]' 
                if (DoIntersect(polygon[i], polygon[next], p, extreme))
                {
                    // If the point 'p' is colinear with line segment 'i-next', then check if it lies on segment. If it lies, return true, otherwise false 
                    if (Orientation(polygon[i], p, polygon[next]) == 0) return OnSegment(polygon[i], p, polygon[next]);
                    count++;
                }
                i = next;
            } while (i != 0);

            // Return true if count is odd, false otherwise 
            return (count % 2 == 1);
        }

        //trims translation vector
        private Vector TrimTranslation(int[] v, MyShapes o, MyShapes s, int inds, int indo)
        {
            double difx = s.Vectors[inds].X - o.Vectors[indo].X, dify = s.Vectors[inds].Y - o.Vectors[indo].Y;
            bool sd = v[3] == 0 ? true : false;
            Vector q1 = new Vector();
            Vector q2 = new Vector();
            Vector p1 = new Vector();
            Vector p2 = new Vector();
            Vector intersectPoint = new Vector(-1, -1);
            Vector vector = new Vector();
            Vector realVector = new Vector();
            Vector sPoint = new Vector(s.Vectors[inds].X, s.Vectors[inds].Y);
            Vector ePoint = new Vector();
            Vector[] oVectors;
            if (sd) //if translated on stationary shape
            {
                if (inds == s.Vectors.Length - 1) ePoint = new Vector(s.Vectors[0].X, s.Vectors[0].Y);
                else ePoint = new Vector(s.Vectors[inds + 1].X, s.Vectors[inds + 1].Y);
                oVectors = GetVectors(o, ePoint, v[4]);
            }
            else
            {
                if (indo == o.Vectors.Length - 1) ePoint = new Vector(o.Vectors[0].X + difx, o.Vectors[0].Y + dify);
                else ePoint = new Vector(o.Vectors[indo + 1].X + difx, o.Vectors[indo + 1].Y + dify);
                oVectors = GetVectors(o, sPoint, v[4]);
            }
            
            for (int i = 0; i < o.LinearEquation.Length; i++)
            {
                p1 = oVectors[i];
                if (i == o.LinearEquation.Length - 1) p2 = oVectors[0];
                else p2 = oVectors[i + 1];
                for (int j = 0; j < s.LinearEquation.Length; j++)
                {
                    q1 = s.Vectors[j];
                    q2 = j == s.LinearEquation.Length - 1 ? s.Vectors[0] : s.Vectors[j + 1];
                    if (LineSegementsIntersect(q1, q2, p1, p2, out vector, false) && 
                        ((!sd&&(vector.X != s.Vectors[inds].X && vector.Y != s.Vectors[inds].Y))||
                        (sd&& (vector.X != s.Vectors[v[5]].X && vector.Y != s.Vectors[v[5]].Y))))
                    {
                        if (IsInside(s.Vectors, s.Vectors.Length, p2, Length)) realVector = new Vector(p2.X - vector.X, p2.Y - vector.Y);
                        else realVector = new Vector(q2.X - vector.X, q2.Y - vector.Y);
                        if (intersectPoint.X == -1)
                        {
                            intersectPoint = realVector;
                            continue;
                        }
                        intersectPoint = Math.Sqrt(Math.Pow(ePoint.X - (ePoint.X-realVector.X), 2) + Math.Pow(ePoint.Y - (ePoint.Y - realVector.Y), 2)) >
                            Math.Sqrt(Math.Pow(ePoint.X - (ePoint.X - intersectPoint.X), 2) + Math.Pow(ePoint.Y - (ePoint.Y - intersectPoint.X), 2)) ? 
                            intersectPoint : realVector;
                    }
                }
            }
            return intersectPoint;
        }

        // checks if an intersection point was found
        public static bool LineSegementsIntersect(Vector p, Vector p2, Vector q, Vector q2, out Vector intersection, bool considerCollinearOverlapAsIntersect)
        {
            intersection = new Vector();
            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
            var qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero())
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s)) return true;
                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s then the two lines are collinear but disjoint.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero()) return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)
            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1 the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }
            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        //gets correct translation vector
        private int GetCorrectVector(int[][] PossibleMoves, int PCorrectVector, int POCorrectVector, MyShapes s, MyShapes shap)
        {
            int c = 0, i, corr = 0;
            for (i = 0; i < 4; i++) //checks if there are possibilities
            {
                if (PossibleMoves[i][3] != -1)
                {
                    c++;
                    corr = i;
                }
            }
            if (c == 0) return -1;
            else if (c > 1)
            {
                c = -1;
                for (i = 0; i < 4; i++)
                {
                    if (PossibleMoves[i][3] != -1 && c == -1) //compares all vectors to get optimal one
                    {
                        c = i;
                        if (PCorrectVector == -1) break;
                        continue;
                    }
                    int dp = 0; //distance from previous vector 
                    int dn = 0; //distance from current vector 
                    int comp = 0; //helper variable
                    if (PossibleMoves[i][3] == 0) //stationary translation
                    {
                        int j = PossibleMoves[i][7];
                        while (j != PCorrectVector)
                        {
                            if (j == s.LinearEquation.Length)
                            {
                                j = 0;
                                continue;
                            }
                            dn++;
                            j++;
                        }
                        j = PossibleMoves[c][7];
                        while (j != PCorrectVector)
                        {
                            if (j == 0)
                            {
                                j = s.LinearEquation.Length - 1;
                                continue;
                            }
                            comp++;
                            j--;
                        }
                        dn = comp > dn ? dn : comp;
                        j = PossibleMoves[i][7];
                        while (j != POCorrectVector)
                        {
                            if (j == shap.LinearEquation.Length)
                            {
                                j = 0;
                                continue;
                            }
                            dp++;
                            j++;
                        }
                        j = PossibleMoves[c][7];
                        while (j != POCorrectVector)
                        {
                            if (j == 0)
                            {
                                j = shap.LinearEquation.Length - 1;
                                continue;
                            }
                            comp++;
                            j--;
                        }
                        dp = comp > dn ? dn : comp;
                    }
                    if (PossibleMoves[i][3] == 1) //orbiting translation
                    {
                        int j = PossibleMoves[i][8];
                        while (j != PCorrectVector)
                        {
                            if (j == s.LinearEquation.Length)
                            {
                                j = 0;
                                continue;
                            }
                            dn++;
                            j++;
                        }
                        j = PossibleMoves[c][8];
                        while (j != PCorrectVector)
                        {
                            if (j == 0)
                            {
                                j = s.LinearEquation.Length - 1;
                                continue;
                            }
                            comp++;
                            j--;
                        }
                        dn = comp > dn ? dn : comp;
                        j = PossibleMoves[i][8];
                        while (j != POCorrectVector)
                        {
                            if (j == shap.LinearEquation.Length)
                            {
                                j = 0;
                                break;
                            }
                            dp++;
                            j++;
                        }
                        j = PossibleMoves[c][8];
                        while (j != POCorrectVector)
                        {
                            if (j == 0)
                            {
                                j = shap.LinearEquation.Length - 1;
                                continue;
                            }
                            comp++;
                            j--;
                        }
                        dp = comp > dn ? dn : comp;
                        c = dp > dn ? dp : dn;
                        dp = dn = 0;
                    }
                }
                corr = c;
            }
            return corr;
        }

        //moves shape to left-bottom most location
        private void LeftBottom(List<MyShapes> myShapes, MyShapes s, int indexOfS, ref Vector vector)
        {
            Vector[] vectors = GetVectors(s, vector, indexOfS);
            bool b = true;
            double cx = .1, cy = .1;
            while (b && vector.X - cx >= 0)
            {
                for (int i = 0; i < vectors.Length; i++) vectors[i].X -= .1;
                if (!CanMove(myShapes, vectors))
                {
                    b = false;
                    cx -= .1;
                    for (int j = 0; j < vectors.Length; j++) vectors[j].X += .1;
                    break;
                }
                cx += .1;
            }
            if (b) cx -= .1;
            b = true;
            while (b && vector.Y - cy >= 0)
            {
                for (int i = 0; i < vectors.Length; i++) vectors[i].Y -= .1;
                if (!CanMove(myShapes, vectors))
                {
                    b = false;
                    cy -= .1;
                    for (int j = 0; j < vectors.Length; j++) vectors[j].Y += .1;
                    break;
                }

                cy += .1;
            }
            if (b) cy -= .1;
            vector -= new Vector(Math.Round(cx, 2), Math.Round(cy, 2));
        }

        public bool CanMove(List<MyShapes> myShapes, Vector[] vectors)
        {
            Vector p1, q1, p2, q2, intersect;
            for (int i = 0; i < myShapes.Count; i++)
            {
                for (int j = 0; j < myShapes[i].PlacedVectors.Length; j++)
                {
                    p1 = myShapes[i].PlacedVectors[j];
                    q1 = j == myShapes[i].PlacedVectors.Length - 1 ? 
                        myShapes[i].PlacedVectors[0] : myShapes[i].PlacedVectors[j+1];
                    for (int k = 0; k < vectors.Length; k++)
                    {
                        p2 = vectors[k];
                        q2 = k == vectors.Length - 1 ? 
                            vectors[0] : vectors[k + 1];
                        if (LineSegementsIntersect(p1, q1, p2, q2, out intersect, false))
                        {
                            if(!(intersect.X==p1.X&&intersect.Y==p1.Y)&&!(intersect.X == p2.X && intersect.Y == p2.Y)&&
                                !(Math.Round(intersect.X, 0) == Math.Round(q1.X,0) && 
                                Math.Round(intersect.Y, 0) == Math.Round(q1.Y, 0)) && 
                                !(Math.Round(intersect.X, 0) == Math.Round(q2.X, 0) &&
                                Math.Round(intersect.Y, 0) == Math.Round(q2.Y, 0))&&
                                !(Math.Round(intersect.X, 0) == Math.Round(p1.X, 0) &&
                                Math.Round(intersect.Y, 0) == Math.Round(p1.Y, 0)) && 
                                !(Math.Round(intersect.X, 0) == Math.Round(p2.X, 0) && Math.Round(intersect.Y, 0) == Math.Round(p2.Y, 0)))
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private Vector Intersect(Vector CirclePos, double CircleRad, Vector LineStart, Vector LineEnd)
        {
            //Calculate terms of the linear and quadratic equations
            var M = (LineEnd.Y - LineStart.Y) / (LineEnd.X - LineStart.X);
            var B = LineStart.Y - M * LineStart.X;
            var a = 1 + M * M;
            var b = 2 * (M * B - M * CirclePos.Y - CirclePos.X);
            var c = CirclePos.X * CirclePos.X + B * B + CirclePos.Y * CirclePos.Y -
                    CircleRad * CircleRad - 2 * B * CirclePos.Y;
            // solve quadratic equation
            var sqRtTerm = Math.Sqrt(b * b - 4 * a * c);
            var x = ((-b) + sqRtTerm) / (2 * a);
            // make sure we have the correct root for our line segment
            if ((x < Math.Min(LineStart.X, LineEnd.X) ||
               (x > Math.Max(LineStart.X, LineEnd.X))))
            { x = ((-b) - sqRtTerm) / (2 * a); }
            //solve for the y-component
            var y = M * x + B;
            // Intersection Calculated
            return new Vector(x, y);
        }
            
        public bool GetLineArcIntersection(Vector sPoint, Vector ePoint, double sAngle, double eAngle)
        {
            double dx = ePoint.X - sPoint.X;
            double dy = ePoint.Y - sPoint.Y;
            double al = Math.Tan(dy / dx);
            if (al < sAngle || al > eAngle) return false;
            return true;
        }

        //check if a point lies inside a circle sector. 
        public bool CheckPoint(int radius, double x, double y, float percent, float startAngle)
        {
            // calculate endAngle 
            float endAngle = 360 / percent + startAngle;

            // Calculate polar co-ordinates 
            float polarradius = (float)Math.Sqrt(x * x + y * y);

            float Angle = (float)Math.Atan(y / x);

            // Check whether polarradius is less then radius of circle or not and Angle is between startAngle and endAngle or not 
            if (Angle >= startAngle && Angle <= endAngle && polarradius < radius) return true;
            
            return false;
        }

        public double AngleBetweenLinesInRadians(Vector line1Start, Vector line1End, Vector line2Start, Vector line2End)
        {
            double angle1 = Math.Atan2(line1Start.Y - line1End.Y, line1Start.X - line1End.X);
            double angle2 = Math.Atan2(line2Start.Y - line2End.Y, line2Start.X - line2End.X);
            double result = (angle2 - angle1) * 180 / 3.14;
            if (result < 0)
            {
                result += 360;
            }
            return result;
        }

        // Rotates one point around another
        static Vector RotatePoint(Vector pointToRotate, Vector centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        //gets linear equation of line
        public double[] GetEquation(Vector a, Vector b)
        {
            double[] res = new double[3];
            if (a.X == b.X)
            {
                res[0] = a.X;
                res[1] = 0;
                res[2] = 1;
            }
            else
            {
                res[0] = (b.Y-a.Y) / (b.X-a.X);
                res[1] = -(res[0] * a.X) + a.Y;
                res[2] = 0;
            }
            return res;
        }
    }
}
