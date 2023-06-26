using BLL.Interfaces;
using DAL;
using DAL.DAO;
using DAL.DTO;
using System.Linq;
using System.Web.Http;

namespace PersonalTracking.WebApi.Controllers
{
    // Aqui eu preciso fazer o acesso do service
    // O service vai realizar todas as operações e em seguida me retornar os dados
    // TODO: Aplicar -> Injeção de dependência e inversão de controle

    public class DepartmentController : ApiController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public IHttpActionResult GetDepartments()
        {
            var departments = _departmentService.GetAllService();
            if (departments != null)
            {
                return Ok(departments);
            }
            else
            {
                return NotFound();
            }

            //var context = db.GetContext();
            //var departments = context.DEPARTMENTs.ToList();

            //return Ok(departments);
        }

        //public IHttpActionResult GetDepartmentById(int id)
        //{

        //}

        //[HttpPost]
        //public IHttpActionResult CreateDepartment(DEPARTMENT department)
        //{
        //    var context = GetContext();
        //    context.DEPARTMENTs.InsertOnSubmit(department);
        //    context.SubmitChanges();

        //    return Ok();
        //}


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
