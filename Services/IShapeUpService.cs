using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IShapeUpService
    {
        Members AddMember(string name, string password, string email);
        Members GetMember(string name, string password);
        Projects AddProject(int id, string name, DateTime oDate, DateTime dDate);
        Projects GetProject(int id);
        Projects EditProjectTitle(int id, string name, DateTime? o, DateTime? d, bool? s);
        void DeleteProject(int id);
        Shapes AddShape(int pid, bool area, int unit, params Point[] coordinates);
        Shapes GetShape(int i);
        Shapes EditShape(int id, int cpid, int pid, bool a, int u, params Point[] c);
        void DeleteShape(int id, int cpid);
        bool Run(int id);
        List<Result> GetResult(int id);
        void AddResult(double sx, double sy, double ax, double ay, int s, int p);
    }
}