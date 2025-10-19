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
        private readonly IWebHostEnvironment _env;

        public CreateServicesModel(ISeServices seServices, ICategoryServices cateServices, IWebHostEnvironment env)
        {
            _seServices = seServices;
            _cateServices = cateServices;
            _env = env;
        }
        public List<Category> CategoryList { get; set; }
        [BindProperty]
        public Service Service { get; set; }
        [BindProperty]
        public IFormFile? ImageFile { get; set; }

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
            if (ImageFile != null)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                string uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "services");
                Directory.CreateDirectory(uploadFolder);
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                Service.ImageUrl = $"/uploads/services/{fileName}";
            }
            Service.MuaId = muaId.Value;
            Service.Active = true; 
            await _seServices.addServiceAsync(Service);

            return RedirectToPage("Services");
        }
    }
}
