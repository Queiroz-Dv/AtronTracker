using BLL.Interfaces;
using PersonalTracking.Models;
using System.Web.Http;

namespace PersonalTracking.Api.Controllers
{
    public class DepartmentApiController : ApiController
    {
        // Interfaces para serem usadas no processo
        private readonly IDepartmentService _departmentService;

        public DepartmentApiController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Retrieves all departments
        /// </summary>
        /// <returns>A list of departments</returns>
        [HttpGet]
        public IHttpActionResult GetDepartments()
        {
            //Chama o service para obter todos os dados 
            var departments = _departmentService.GetAllService();
            // Se estiver preenchido retorna Ok e as entidades
            if (departments != null)
            {
                return Ok(departments);
            }
            else
            {
                // Senão retorna que não encontrou 
                return NotFound();
            }
        }

        /// <summary>
        /// Cria um novo departamento.
        /// </summary>
        /// <param name="department">O objeto DepartmentModel contendo os dados do departamento.</param>
        /// <returns>Retorna um código de status HTTP.</returns>
        [HttpPost]
        public IHttpActionResult CreateDepartment([FromBody] Department department)
        {
            if (department != null)
            {
                _departmentService.CreateEntityService(department);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult GetDepartmentById(int id)
        {
            var department = _departmentService.GetEntityByIdService(id);
            if (department != null)
            {
                return Ok(department);
            }

            return NotFound();
        }

        [HttpPut]
        public IHttpActionResult Put(Department departmentModel)
        {
            if (departmentModel != null)
            {
                _departmentService.UpdateEntityService(departmentModel);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public IHttpActionResult Delete(int id)
        {
            _departmentService.RemoveEntityService(id);
            return Ok();
        }
    }
}
