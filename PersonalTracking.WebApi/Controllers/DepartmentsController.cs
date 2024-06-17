using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTracking.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTracking.WebApi.Controllers
{
    [Route("departments")]
    public class DepartmentsController : ControllerBase
    {
        public IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IList<DepartmentDto>>> GetAll()
        {
            try
            {
                var departments = await _departmentService.GetDepartments();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        //[HttpGet]
        //[Route("{id:int}")]
        //public async Task<ActionResult<Department>> GetById(int id, [FromServices] DataContext context)
        //{
        //    var department = await context.Departments.AsNoTracking().FirstOrDefaultAsync(dpt => dpt.Id == id);
        //    return department;
        //}

        //[HttpPost]
        //public async Task<ActionResult<Department>> Create([FromBody] Department model, [FromServices] DataContext context)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        context.Departments.Add(model);
        //        await context.SaveChangesAsync();

        //        return Ok(model);
        //    }
        //    catch
        //    {
        //        return BadRequest(new { message = "Error. Not possible to save the department. Verify your information and try again." });
        //    }
        //}

        //[HttpPut]
        //[Route("{id:int}")]
        //public async Task<ActionResult<IList<Department>>> Update(int id,
        //                                                               [FromBody] Department model,
        //                                                               [FromServices] DataContext context)
        //{
        //    if (model.Id != id)
        //    {
        //        return NotFound(new { message = "Department not found." });
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        context.Entry(model).State = EntityState.Modified;
        //        await context.SaveChangesAsync();
        //        return Ok(model);
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {

        //        return BadRequest(new { message = "No possible to update the department. Verify your information and try again." });
        //    }
        //}

        //[HttpDelete]
        //[Route("{id:int}")]
        //public async Task<ActionResult<IList<Department>>> Remove(int id, [FromServices] DataContext context)
        //{
        //    var department = await context.Departments.FirstOrDefaultAsync(dpt => dpt.Id == id);
        //    if (department == null)
        //    {
        //        return NotFound(new { message = "Department not found." });
        //    }

        //    try
        //    {
        //        context.Departments.Remove(department);
        //        await context.SaveChangesAsync();
        //        return Ok(department);
        //    }
        //    catch (Exception)
        //    {

        //        return BadRequest(new { message = "No possible to delete the department. Verify your information and try again." });
        //    }
        
    }
}
