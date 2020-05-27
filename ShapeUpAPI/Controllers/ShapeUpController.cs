using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShapeUpAPI.Controllers
{
    [Route("api/[controller]")]
    public class ShapeUpController : ControllerBase
    {
        IShapeUpService service;
        public ShapeUpController(IShapeUpService service) => this.service = service;

        [HttpGet("Members/{name}/{password}")]
        public Members GetMembers(string name, string password) => service.GetMember(name, password);
        [HttpPost("Members/{name}/{password}/{email}")]
        public Members AddMember(string name, string password, string email) => service.AddMember(name, password, email);
        [HttpPost("Projects/{id}/{name}/{o}/{d}")]
        public Projects AddProject(int id, string name, DateTime o, DateTime d) => service.AddProject(id, name, o, d);
        [HttpGet("Projects/{id}")]
        public Projects GetProjects(int id) => service.GetProject(id);
        [HttpPost("Projects/{id}/{name}/{o}/{d}/{s}")]
        public Projects EditProjectTitle(int id, string name, DateTime? o, DateTime? d, bool? s) => service.EditProjectTitle(id, name, o, d, s);
        [HttpDelete("Projects/{id}")]
        public void DeleteProject(int id) => service.DeleteProject(id);
        [HttpPost("Shape/{pid}/{area}/{unit}/{r}/{coordinates}")]
        public Shapes AddShape(int pid, bool area, int unit, params Point[] coordinates)=>service.AddShape(pid, area, unit, coordinates);
        [HttpGet("Shape/{id}")]
        public Shapes GetShape(int id) => service.GetShape(id);
        [HttpPost("Shape/{id}/{cpid}/{pid}/{a}/{u}/{r}/{c}")]
        public Shapes EditShape(int id, int cpid, int pid, bool a, int u, params Point[] c) => service.EditShape(id, cpid, pid, a, u, c);
        [HttpDelete("Shape/{id}/{cpid}")]
        public void DeleteShape(int id, int cpid) => service.DeleteShape(id, cpid);
        [HttpGet("{id}")]
        public bool Run(int id) => service.Run(id);
        [HttpGet("Result/{id}")]
        public List<Result> GetResults(int id) => service.GetResult(id);
    }
}
