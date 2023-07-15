using DAL.DTO;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PersonalTracking.Api.Controllers
{
    public class DepartmentViewController : Controller
    {
        private HttpClient _httpClient; // Inicia o http client (mas tenho que usar o dispose no fim)
        private const string DepartmentUri = "https://localhost:44383/api/"; // caminho da Uri
        private const string ApiPath = "DepartmentApi"; // caminho da api

        public DepartmentViewController()
        {
            _httpClient = new HttpClient();
        }

        
        /// <summary>
        /// Get All Departments
        /// </summary>
        /// <returns>A list and a view of all departments</returns>
        public async Task<ActionResult> Index()

        {
            List<DepartmentModel> departments = null; // Inicializa a lista 
            try
            {
                _httpClient.BaseAddress = new Uri(DepartmentUri); // Passa o endereço da API 

                var response = await _httpClient.GetAsync(ApiPath); // Consome a API
                if (response.IsSuccessStatusCode) // Se o status for 200 (Ok) 
                {
                    // Ler os dados de forma asíncrona
                    var displayData = response.Content.ReadAsAsync<List<DepartmentModel>>();
                    displayData.Wait(); // Aguarda a leitura
                    departments = displayData.Result; // Repassa o resultado da leitura (as entidades) para a lista
                }
            }
            catch (Exception ex)
            {
                // Se o status for diferente de 200 vai dar erro interno
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            // Retorna a View da controller com os dados 
            return View(departments);
        }

        /// <summary>
        /// Get details of a department by ID.
        /// </summary>
        /// <param name="id">The ID of the department.</param>
        /// <returns>The details of the department.</returns>
        public async Task<ActionResult> Details(int id)
        {
            DepartmentModel departmentModel = null;
            try
            {
                _httpClient.BaseAddress = new Uri(DepartmentUri);
                var response = await _httpClient.GetAsync($"{ApiPath}?id=" + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var model = await response.Content.ReadAsAsync<DepartmentModel>();
                    departmentModel = model;
                }

                return View(departmentModel);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }


        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(DepartmentModel department)
        {
            try
            {
                _httpClient.BaseAddress = new Uri(DepartmentUri);
                var response = await _httpClient.PostAsJsonAsync(ApiPath, department);

                if (response.IsSuccessStatusCode)
                {
                    var displayData = response.Content.ReadAsAsync<DepartmentModel>();
                    department = displayData.Result;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult> UpdateDepartment(int id)
        {
            DepartmentModel departmentModel = null;
            _httpClient.BaseAddress = new Uri(DepartmentUri);

            var consumeAPI = await _httpClient.GetAsync($"{ApiPath}?id=" + id.ToString());

            if (consumeAPI.IsSuccessStatusCode)
            {
                var displayData = await consumeAPI.Content.ReadAsAsync<DepartmentModel>();
                departmentModel = displayData;
            }

            return View(departmentModel);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateDepartment(DepartmentModel departmentModel)
        {
            _httpClient.BaseAddress = new Uri(DepartmentUri);

            var response = await _httpClient.PutAsJsonAsync(ApiPath, departmentModel);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(departmentModel);
        }

        public async Task<ActionResult> Delete(int id)
        {
            _httpClient.BaseAddress = new Uri(DepartmentUri);

            var consumeAPI = await _httpClient.DeleteAsync($"{ApiPath}/" + id.ToString());

            if (consumeAPI.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View("Index");
        }
    }
}