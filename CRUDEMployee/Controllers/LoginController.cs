using CRUDEMployee.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;  // For session

namespace CRUDEMployee.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _client;

        public LoginController()
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
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Show login form
        public IActionResult Login()
        {
            return View();
        }

        // Handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Serialize the login model to JSON
            var loginData = JsonConvert.SerializeObject(login);
            var content = new StringContent(loginData, Encoding.UTF8, "application/json");

            // Send POST request to API for login
            HttpResponseMessage response = await _client.PostAsync("Login", content);

            if (response.IsSuccessStatusCode)
            {
                // Read the JWT token from the response
                var token = await response.Content.ReadAsStringAsync();

                // Store the token in session
                HttpContext.Session.SetString("JWToken", token);

                // Redirect to another page (e.g., Employee list)
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                // If login fails, show an error message
                ViewBag.Message = "Invalid login credentials";
                return View();
            }
        }

        // Logout method
        public IActionResult Logout()
        {
            // Clear the session on logout
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
