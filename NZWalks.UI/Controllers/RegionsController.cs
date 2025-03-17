using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Text;
using System.Text.Json;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Constructor - Inject IHttpClientFactory for making HTTP requests to the Web API
        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // GET: Fetches all regions from the Web API and displays them in the Index view
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();
            try
            {
                var client = httpClientFactory.CreateClient(); // Create an HTTP client instance
                var httpResponseMessage = await client.GetAsync("https://localhost:7144/api/regions"); // Send GET request
                httpResponseMessage.EnsureSuccessStatusCode(); // Ensure request was successful

                // Deserialize JSON response into a list of RegionDto objects
                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
            }
            catch (Exception ex)
            {
                // Log the exception (Can be improved with proper logging mechanism)
                throw;
            }

            return View(response); // Pass the list of regions to the view
        }

        // GET: Displays the Add Region form
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // POST: Adds a new region via API call
        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel model)
        {
            var client = httpClientFactory.CreateClient();

            // Create an HTTP POST request with JSON content
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7144/api/regions"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage); // Send request
            httpResponseMessage.EnsureSuccessStatusCode(); // Ensure successful response
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (response is not null)
            {
                return RedirectToAction("Index", "Regions"); // Redirect to Index on success
            }
            return View(); // Reload form on failure
        }

        // GET: Fetches details of a region by ID for editing
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient();

            // Fetch region details from API
            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7144/api/regions/{id.ToString()}");

            if (response is not null)
            {
                return View(response); // Pass the retrieved region data to the view
            }

            return View(null); // Return empty view if not found
        }

        // POST: Updates a region via API call
        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto request)
        {
            var client = httpClientFactory.CreateClient();

            // Create HTTP PUT request with updated region data
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7144/api/regions/{request.id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage); // Send request
            httpResponseMessage.EnsureSuccessStatusCode(); // Ensure successful response
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (response is not null)
            {
                return RedirectToAction("Edit", "Regions"); // Redirect back to Edit page
            }

            return View();
        }

        // POST: Deletes a region via API call
        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                // Send DELETE request to API
                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7144/api/regions/{request.id}");
                httpResponseMessage.EnsureSuccessStatusCode(); // Ensure success

                return RedirectToAction("Index", "Regions"); // Redirect to Index after deletion
            }
            catch (Exception ex)
            {
                // Handle exception (Can be improved with logging)
            }

            return View("Edit"); // Return to Edit page on failure
        }
    }
}
