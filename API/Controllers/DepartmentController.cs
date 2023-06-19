using BLL.Interfaces;
using DAL;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService<DEPARTMENT> _departmentService;

        public DepartmentController(IDepartmentService<DEPARTMENT> departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult GetDepartments()
        {
            var departments = _departmentService.GetAllService();
            return Ok(departments);
        }
    }
}
