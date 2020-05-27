using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositories
{
    public class ShapeUpRepository:IShapeUpRepository
    {
        private ShapeUp context;
        public ShapeUpRepository(ShapeUp context)
        {
            this.context = context;
        }
        public Members AddMember(string name, string password, string email)
        {
            Members members = new Members { UserName = name, UserPassword = password, Email = email };
            context.Members.Add(members);
            context.SaveChanges();
            return GetMember(name, password);
        }
        public Members GetMember(string name, string password)
        {
            return context.Members.FirstOrDefault(m => m.UserName.Equals(name) && m.UserPassword.Equals(password));
        }
        public Projects AddProject(int id, string name, DateTime oDate, DateTime dDate)
        {
            foreach (var p in context.Projects)
            {
                if (p.ProjectName.Equals(name))
                {
                    return null;
                }
            }
            context.Projects.Add(new Projects { MemberId = id, ProjectName = name, OrderDate = oDate, DueDate = dDate });
            context.SaveChanges();
            return context.Projects.OrderByDescending(q => q.Id).FirstOrDefault();
        }
        public Projects GetProject(int id)
        {
            return context.Projects.FirstOrDefault(p => p.Id == id);

        }
        public Projects EditProjectTitle(int id, string name, DateTime? o, DateTime? d, bool? s)
        {
            context.Projects.FirstOrDefault(p => p.Id == id).ProjectName = name;
            context.Projects.FirstOrDefault(p => p.Id == id).OrderDate = o;
            context.Projects.FirstOrDefault(p => p.Id == id).DueDate = d;
            context.Projects.FirstOrDefault(p => p.Id == id).ProjectStatus = s;
            context.SaveChanges();
            return GetProject(id);
        }
        public void DeleteProject(int id)
        {
            Projects p = GetProject(id);
            foreach (var item in p.Result)
            {
                context.Result.Remove(item);
            }
            foreach (var shape in context.ProjectShapeConn)
            {
                if (shape.ProjectId == p.Id)
                {
                    context.ProjectShapeConn.Remove(shape);
                }
            }
            context.Projects.Remove(p);
            context.SaveChanges();
        }
        public Shapes AddShape(int pid, bool area, int unit, params Point[] coordinates)
        {
            context.Shapes.Add(new Shapes { Area = area, Unit = unit});
            context.ProjectShapeConn.Add(new ProjectShapeConn { ProjectId = pid, ShapeId = context.Shapes.Last().Id });
            foreach(var point in coordinates)
            {
                context.Point.Add(new Point { X = point.X, Y = point.Y, ShapeId = context.Shapes.Last().Id });
            }
            Projects p = GetProject(pid);
            foreach (var item in p.Result)
            {
                context.Result.Remove(item);
            }
            context.SaveChanges();
            return context.Shapes.Last();
        }
        public Shapes GetShape(int i)
        {
            return context.Shapes.FirstOrDefault(s => s.Id == i);
        }
        public Shapes EditShape(int id, int cpid, int pid, bool a, int u, params Point[] c)
        {
            foreach(var s in context.Point)
            {
                if (s.ShapeId == id) context.Point.Remove(s);
            }
            foreach (var s in c)
            {
                context.Point.Add(new Point { ShapeId=id, X=s.X, Y=s.Y});
            }
            if (cpid != pid)
            {
                context.ProjectShapeConn.FirstOrDefault(p => p.ShapeId == id && p.ProjectId == cpid).ProjectId=pid;
            }
            Projects pr = GetProject(pid);
            foreach (var item in pr.Result)
            {
                context.Result.Remove(item);
            }
            context.Shapes.FirstOrDefault(p => p.Id == id).Area = a;
            context.Shapes.FirstOrDefault(p => p.Id == id).Unit = u;
            context.SaveChanges();
            return GetShape(id);
        }
        public void DeleteShape(int id, int cpid)
        {
            context.ProjectShapeConn.Remove(context.ProjectShapeConn.FirstOrDefault(p => p.ShapeId == id && p.ProjectId == cpid));
            Projects pr = GetProject(cpid);
            foreach (var item in pr.Result)
            {
                context.Result.Remove(item);
            }
            context.SaveChanges();
        }

        public List<Result> GetResult(int id)
        {
            List<Result> results = new List<Result>();
            foreach (var item in GetProject(id).Result)
            {
                results.Add(item);
            }
            return results;
        }

        public void AddResult(double sx, double sy, double ax, double ay, int s, int p)
        {
            context.Result.Add(new Result { PointOfShapeX = (decimal)sx, PointOfShapeY = (decimal)sy, PointOnAreaX = (decimal)ax, PointOnAreaY = (decimal)ay, ShapeId = s, ProjectId = p });
            context.SaveChanges();
        }
    }
}
