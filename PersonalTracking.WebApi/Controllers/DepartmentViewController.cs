using DAL;
using DAL.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;

namespace PersonalTracking.WebApi.Controllers
{
    public class DepartmentViewController : Controller
    {
        // GET: DepartmentView
        public ActionResult Index()
        {
            IEnumerable<DEPARTMENT> department = null;
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44315/api/Department");

            var consumeAPI = httpClient.GetAsync("Department");
            consumeAPI.Wait();

            var readData = consumeAPI.Result;
            if (readData.IsSuccessStatusCode)
            {
                var displayData = readData.Content.ReadAsAsync<IList<DEPARTMENT>>();
                displayData.Wait();

                department = displayData.Result;
            }

            return View(department);
        }


        public ActionResult Details(int id)
        {
            DepartmentDTO departmentDTO = null;
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44315/api/");

            var consumeAPI = httpClient.GetAsync("Department?id=" + id.ToString());
            consumeAPI.Wait();

            var readData = consumeAPI.Result;
            if (readData.IsSuccessStatusCode)
            {
                var displayData = readData.Content.ReadAsAsync<DepartmentDTO>();
                displayData.Wait();
                departmentDTO = displayData.Result;
            }

            return View(departmentDTO);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DepartmentDTO departmentDTO)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44315/api/");

            var insertData = httpClient.PostAsJsonAsync<DepartmentDTO>("Department", departmentDTO);
            insertData.Wait();

            var saveData = insertData.Result;

            if (saveData.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View("Create");
        }

        public ActionResult UpdateDepartment(int id)
        {
            DepartmentDTO departmentDTO = null;
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44315/api/");

            var consumeAPI = httpClient.GetAsync("Department?id=" + id.ToString());
            consumeAPI.Wait();

            var readData = consumeAPI.Result;
            if (readData.IsSuccessStatusCode)
            {
                var displayData = readData.Content.ReadAsAsync<DepartmentDTO>();
                displayData.Wait();
                departmentDTO = displayData.Result;
            }

            return View(departmentDTO);
        }

        [HttpPost]
        public ActionResult UpdateDepartment(DepartmentDTO departmentDTO)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44315/api/Department");

            var insertData = httpClient.PutAsJsonAsync("Department", departmentDTO);
            insertData.Wait();

            var saveData = insertData.Result;

            if (saveData.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(departmentDTO);
        }

        public ActionResult Delete(int id)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:44315/api/Department");

            var consumeAPI = httpClient.DeleteAsync("Department/" + id.ToString());
            consumeAPI.Wait();

            var deleteData = consumeAPI.Result;
            if (deleteData.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View("Index");
        }
    }
}