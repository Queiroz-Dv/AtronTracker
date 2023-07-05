namespace PersonalTracking.Api.Controllers
{
    //public class DepartmentViewController : Controller
    //{
    //    private HttpClient _httpClient;
    //    private const string DepartmentUri = "DepartmentApi";

    //    public DepartmentViewController()
    //    {
    //        _httpClient = new HttpClient();
    //        _httpClient.BaseAddress = ApiEndpointBuilder.BuildDepartmentEndpoint();
    //    }


    //    // GET: DepartmentView
    //    public async Task<ActionResult> Index()
    //    {
    //        List<DepartmentModel> departments;
    //        try
    //        {
    //            var response = await _httpClient.GetAsync(DepartmentUri);
    //            response.EnsureSuccessStatusCode();
    //            departments = await response.Content.ReadAsAsync<List<DepartmentModel>>();
    //        }
    //        catch (Exception ex)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
    //        }

    //        return View(departments);
    //    }

    //    /// <summary>
    //    /// Get details of a department by ID.
    //    /// </summary>
    //    /// <param name="id">The ID of the department.</param>
    //    /// <returns>The details of the department.</returns>
    //    public async Task<ActionResult> Details(int id)
    //    {
    //        DepartmentModel departmentModel = null;
    //        try
    //        {
    //            var response = await _httpClient.GetAsync($"{DepartmentUri}?id=" + id.ToString());
    //            if (response.IsSuccessStatusCode)
    //            {
    //                var model = await response.Content.ReadAsAsync<DepartmentModel>();
    //                departmentModel = model;
    //            }

    //            return View(departmentModel);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        //DepartmentDTO departmentDTO = null;
    //        //HttpClient httpClient = new HttpClient();
    //        //httpClient.BaseAddress = new Uri("https://localhost:44315/api/");

    //        //var consumeAPI = httpClient.GetAsync("Department?id=" + id.ToString());
    //        //consumeAPI.Wait();

    //        //var readData = consumeAPI.Result;
    //        //if (readData.IsSuccessStatusCode)
    //        //{
    //        //    var displayData = readData.Content.ReadAsAsync<DepartmentDTO>();
    //        //    displayData.Wait();
    //        //    departmentDTO = displayData.Result;
    //        //}

    //        //return View(departmentDTO);
    //    }

    //    public ActionResult Create()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public async Task<ActionResult> Create(DepartmentModel department)
    //    {
    //        try
    //        {
    //            var response = await _httpClient.PostAsJsonAsync(DepartmentUri, department);
    //            response.EnsureSuccessStatusCode();
    //            return RedirectToAction("Index");
    //        }
    //        catch (Exception ex)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
    //        }
    //    }

    //    public ActionResult UpdateDepartment(int id)
    //    {
    //        DepartmentDTO departmentDTO = null;
    //        HttpClient httpClient = new HttpClient();
    //        httpClient.BaseAddress = new Uri("https://localhost:44315/api/");

    //        var consumeAPI = httpClient.GetAsync("Department?id=" + id.ToString());
    //        consumeAPI.Wait();

    //        var readData = consumeAPI.Result;
    //        if (readData.IsSuccessStatusCode)
    //        {
    //            var displayData = readData.Content.ReadAsAsync<DepartmentDTO>();
    //            displayData.Wait();
    //            departmentDTO = displayData.Result;
    //        }

    //        return View(departmentDTO);
    //    }

    //    [HttpPost]
    //    public ActionResult UpdateDepartment(DepartmentDTO departmentDTO)
    //    {
    //        HttpClient httpClient = new HttpClient();
    //        httpClient.BaseAddress = new Uri("https://localhost:44315/api/Department");

    //        var insertData = httpClient.PutAsJsonAsync("Department", departmentDTO);
    //        insertData.Wait();

    //        var saveData = insertData.Result;

    //        if (saveData.IsSuccessStatusCode)
    //        {
    //            return RedirectToAction("Index");
    //        }

    //        return View(departmentDTO);
    //    }

    //    public ActionResult Delete(int id)
    //    {
    //        HttpClient httpClient = new HttpClient();
    //        httpClient.BaseAddress = new Uri("https://localhost:44315/api/Department");

    //        var consumeAPI = httpClient.DeleteAsync("Department/" + id.ToString());
    //        consumeAPI.Wait();

    //        var deleteData = consumeAPI.Result;
    //        if (deleteData.IsSuccessStatusCode)
    //        {
    //            return RedirectToAction("Index");
    //        }

    //        return View("Index");
    //    }
    //}
}