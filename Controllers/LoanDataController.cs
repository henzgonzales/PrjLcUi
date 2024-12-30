using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Configuration;
using PrjLcUi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PrjLcUi.Controllers
{
    public class LoanDataController : Controller
    {
 #pragma warning disable S1144 // Unused private types or members should be removed
        readonly Uri baseAddress = new("https://localhost:7209/api");
#pragma warning restore S1144 // Unused private types or members should be removed

        private readonly HttpClient _httpClient;

        public LoanDataController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
       
        public IActionResult Index()
        {
             return View();   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("PropertyValue", "LoanAmount")] LoanDataModel loandataModel)
        {
            HttpResponseMessage? response = null;
            string? error;
            try
            {
                response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/LoanDatas/GetLoanData/" + loandataModel.PropertyValue + "/" + loandataModel.LoanAmount);

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    List<LoanDataModel>? loanDataList = JsonConvert.DeserializeObject<List<LoanDataModel>>(data);

                    foreach (var item in loanDataList)
                    {
                        ViewData["PropertyValue"] = item.PropertyValue.ToString("##,###");
                        ViewData["LoanAmount"] = item.LoanAmount.ToString("##,###");
                        ViewData["LVR"] = item.Lvr;
                    }
                }
                else
                {
                    return View("NoAccess");
                }
            }
            catch (Exception ex)
            {
                error = await HandleError(response, ex);
                return View("NoAccess");
            }

            return View();
        }

        private async Task<string> HandleError(HttpResponseMessage response, Exception? ex = null)
        {
            string error = string.Empty;
            // If the API returned a body, get the message from the body.
            // If you know the content type you could cast to xml/json and grab the error property opposed to all of the content.
            if (response != null && response.Content != null)
                error = $"{response.StatusCode} {response.ReasonPhrase} {await response.Content.ReadAsStringAsync()}";
            else if (response != null && string.IsNullOrEmpty(error))
                error = $"{response.StatusCode} {response.ReasonPhrase}"; // Fallback to response status (401 : Unauthorized).
            else if (ex != null && string.IsNullOrEmpty(error))
                error = ex.Message; // Fall back to exception message.
            else
                return "Unhandled";

            return error;
        }

        public IActionResult NoAccess()
        {
            return View();
        }
    }
}
