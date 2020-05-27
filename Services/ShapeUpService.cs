using Comon;
using Repositories;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class ShapeUpService : IShapeUpService
    {
        private IShapeUpRepository repository;

        public ShapeUpService(IShapeUpRepository repository) => this.repository = repository;

        public Members AddMember(string name, string password, string email) => repository.AddMember(name, password, email);

        public Members GetMember(string name, string password) => repository.GetMember(name, password);

        public Projects AddProject(int id, string name, DateTime oDate, DateTime dDate) => repository.AddProject(id, name, oDate, dDate);

        public Projects GetProject(int id) => repository.GetProject(id);

        public Projects EditProjectTitle(int id, string name, DateTime? o, DateTime? d, bool? s) => repository.EditProjectTitle(id, name, o, d, s);

        public void DeleteProject(int id) => repository.DeleteProject(id);

        public Shapes AddShape(int pid, bool area, int unit, params Point[] coordinates) => repository.AddShape(pid, area, unit, coordinates);

        public Shapes GetShape(int i) => repository.GetShape(i);

        public Shapes EditShape(int id, int cpid, int pid, bool a, int u, params Point[] c) => repository.EditShape(id, cpid, pid, a, u, c);

        public void DeleteShape(int id, int cpid)=>repository.DeleteShape(id, cpid);

        public bool Run(int id)
        {
            Projects p = GetProject(id);
            if (p.Result.Count > 0) return true;
            List<MyShapes> myShapes = new List<MyShapes>();
            MyShapes area = new MyShapes();
            foreach (var item in p.ProjectShapeConn)
            {
                if (item.Shape.Area==true) area = new MyShapes(item.Shape.Id, item.Shape.Unit, item.Shape.Point);
                else myShapes.Add(new MyShapes(item.Shape.Id, item.Shape.Unit, item.Shape.Point));
            }
            Algorithm algorithm = new Algorithm(myShapes, area);
            if (algorithm.Succeeded == true)
            {
                foreach (var item in myShapes)
                {
                    AddResult(item.indexOfPoint.X+item.MinX, item.indexOfPoint.Y+item.MinY, item.PointOnArea.X, item.PointOnArea.Y, item.Id, p.Id);
                }
            }
            return algorithm.Succeeded;
        }

        public List<Result> GetResult(int id) => repository.GetResult(id);

        public void AddResult(double sx, double sy, double ax, double ay, int s, int p)=>repository.AddResult(sx, sy, ax, ay, s, p);
    }
}
