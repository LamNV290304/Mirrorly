using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Mua
{
    public class DeleteServiceModel : PageModel
    {
        private readonly ISeServices _seServices;
        public DeleteServiceModel(ISeServices seServices)
        {
            _seServices = seServices;
        }

        [BindProperty]
        public Service Service { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            var service = await _seServices.GetServiceByIdAsync(id);
            if (service == null) return NotFound();
            Service = service;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await _seServices.DeleteServiceAsync(Service.ServiceId);
            return RedirectToPage("Services");
        }
    }
}
