using CRUDEMployee.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class EmployeeController : Controller
{
    private readonly HttpClient _client;

    public EmployeeController()
    {
        var handler = new HttpClientHandler
        {
            // Disable SSL certificate validation for development only
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        _client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7176/api/")  // Base URL of your Web API
        };
        //_client.DefaultRequestHeaders.Accept.Clear();
        //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));       

    }

    // Action to fetch all employees
    public async Task<IActionResult> Index()
    {
        List<Employees> employees = new List<Employees>();

        // Get the JWT token from the session
        var token = HttpContext.Session.GetString("JWToken");

        if (!string.IsNullOrEmpty(token))
        {
            // Attach the token to the Authorization header
            //_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //_client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Trim('\"', '\\'));
            //_client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // Make the API call to fetch employees
            HttpResponseMessage response = await _client.GetAsync("Employees");

            if (response.IsSuccessStatusCode)
            {
                // Read and deserialize the JSON response
                var jsonData = await response.Content.ReadAsStringAsync();
                employees = JsonConvert.DeserializeObject<List<Employees>>(jsonData);
            }
            else
            {
                ViewBag.ErrorMessage = $"Error fetching data from API: {response.ReasonPhrase} (Status Code: {response.StatusCode})";
            }
        }
        else
        {
            ViewBag.ErrorMessage = "User is not authenticated. Token missing in session.";
        }

        // Pass the employees data to the view
        return View(employees);
    }
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var token = HttpContext.Session.GetString("JWToken");
        _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Trim('\"', '\\'));

        Employees employee = null;


        HttpResponseMessage response = await _client.GetAsync($"Employees/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            employee = JsonConvert.DeserializeObject<Employees>(jsonData);
        }
        else
        {
            ViewBag.ErrorMessage = $"Error fetching employee: {response.ReasonPhrase} (Status Code: {response.StatusCode})";
        }

        return View(employee);
    }

    public IActionResult CreateEmployee()
    {
        return View();
    }

        // POST: Create a new employee
        [HttpPost]
    public async Task<IActionResult> CreateEmployee(Employees employee)
    {
        var token = HttpContext.Session.GetString("JWToken");
        _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Trim('\"', '\\'));
        if (ModelState.IsValid)
        {
            string jsonData = JsonConvert.SerializeObject(employee);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync("Employees", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = $"Error creating employee: {response.ReasonPhrase} (Status Code: {response.StatusCode})";
            }
        }

        return View(employee);
    }
    public async Task<IActionResult> UpdateEmployee(int id)
    {
        var token = HttpContext.Session.GetString("JWToken");
        _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Trim('\"', '\\'));

        Employees employee = null;

        HttpResponseMessage response = await _client.GetAsync($"Employees/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            employee = JsonConvert.DeserializeObject<Employees>(jsonData);
        }
        else
        {
            ViewBag.ErrorMessage = $"Error fetching employee: {response.ReasonPhrase} (Status Code: {response.StatusCode})";
        }

        return View(employee);
    }
    // PUT: Update an employee
    [HttpPost]
    public async Task<IActionResult> UpdateEmployee(int id, Employees employee)
    {
        var token = HttpContext.Session.GetString("JWToken");
        _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Trim('\"', '\\'));

        if (ModelState.IsValid)
        {
            string jsonData = JsonConvert.SerializeObject(employee);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync($"Employees/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = $"Error updating employee: {response.ReasonPhrase} (Status Code: {response.StatusCode})";
            }
        }

        return View(employee);
    }
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var token = HttpContext.Session.GetString("JWToken");
        _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Trim('\"', '\\'));

        Employees employee = null;

        HttpResponseMessage response = await _client.GetAsync($"Employees/{id}");
        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            employee = JsonConvert.DeserializeObject<Employees>(jsonData);
        }
        else
        {
            ViewBag.ErrorMessage = $"Error fetching employee: {response.ReasonPhrase} (Status Code: {response.StatusCode})";
        }

        return View(employee);
    }
    // DELETE: Delete an employee by ID
    public async Task<IActionResult> DeleteEmployeeConfirm(int id)
    {
        var token = HttpContext.Session.GetString("JWToken");
        _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Trim('\"', '\\'));

        HttpResponseMessage response = await _client.DeleteAsync($"Employees/{id}");
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.ErrorMessage = $"Error deleting employee: {response.ReasonPhrase} (Status Code: {response.StatusCode})";
        }

        return RedirectToAction("Index");
    }
}


