using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Mua
{
    public class CreateServicesModel : PageModel
    {
        private readonly ISeServices _seServices;
        private readonly ICategoryServices _cateServices;

        public CreateServicesModel(ISeServices seServices, ICategoryServices cateServices)
        {
            _seServices = seServices;
            _cateServices = cateServices;
        }
        public List<Category> CategoryList { get; set; }
        [BindProperty]
        public Service Service { get; set; }

        public SelectList Categories { get; set; }
        public void OnGet()
        {
            var category = _cateServices.GetAllCategories();
            Categories = new SelectList(category, "CategoryId", "CategoryName");
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var category = _cateServices.GetAllCategories();
                Categories = new SelectList(category, "CategoryId", "CategoryName");
                return Page();
            }

            var muaId = HttpContext.Session.GetInt32("UserId");
            if (muaId == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn cần đăng nhập MUA để thêm dịch vụ.");
                return Page();
            }

            Service.MuaId = muaId.Value;
            Service.Active = true; 
            await _seServices.addServiceAsync(Service);

            return RedirectToPage("Services");
        }
    }
}
