using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Repositories;
using PersonalTracking.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace PersonalTracking.Web.Api.Controllers
{
    public class DepartmentApiController : ApiController
    {
        private readonly IDepartmentService _departmentService;
        private IDepartmentRepository _departmentRepository;

        public DepartmentApiController()
        {
            _departmentRepository = new DepartmentRepository();
            _departmentService = new DepartmentService(_departmentRepository);
        }

        /// <summary>
        /// Retrieves all departments
        /// </summary>
        /// <returns>A list of departments</returns>
        [HttpGet]
        [Route("api/DepartmentApi")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Success", typeof(List<DepartmentModel>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "No departments found")]
        [SwaggerOperation("GetDepartments")]
        public IHttpActionResult GetDepartments()
        {
            var departments = _departmentService.GetAllModelService();

            if (departments != null)
            {
                return Ok(departments);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Cria um novo departamento.
        /// </summary>
        /// <param name="department">O objeto DepartmentModel contendo os dados do departamento.</param>
        /// <returns>Retorna um código de status HTTP.</returns>
        [HttpPost]
        [Route("api/DepartmentApi")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Department created successfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid department data")]
        [SwaggerOperation("CreateDepartment")]
        public IHttpActionResult CreateDepartment([FromBody] DepartmentModel department)
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
        [Route("api/DepartmentApi/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Success", typeof(DepartmentModel))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "No department found")]
        [SwaggerOperation("GetDepartmentById")]
        public IHttpActionResult GetDepartmentById(int id)
        {
            var department = _departmentService.GetEntityByIdService(id);
            if (department != null)
            {
                return Ok(department);
            }

            return NotFound();
        }

        //public IHttpActionResult Put(DepartmentDTO departmentDTO)
        //{
        //    var context = GetContext();
        //    var updateDepartment = (from depart in context.DEPARTMENTs
        //                            where depart.ID.Equals(departmentDTO.ID)
        //                            select depart).FirstOrDefault();
        //    if (updateDepartment != null)
        //    {
        //        updateDepartment.ID = departmentDTO.ID;
        //        updateDepartment.DepartmentName = departmentDTO.DepartmentName;
        //        context.SubmitChanges();
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }

        //    return Ok();
        //}

        //public IHttpActionResult Delete(int id)
        //{
        //    var context = GetContext();
        //    var deleteDepartment = (from depart in context.DEPARTMENTs
        //                            where depart.ID.Equals(id)
        //                            select depart).FirstOrDefault();

        //    context.DEPARTMENTs.DeleteOnSubmit(deleteDepartment);
        //    context.SubmitChanges();
        //    return Ok();
        //}
    }
}
