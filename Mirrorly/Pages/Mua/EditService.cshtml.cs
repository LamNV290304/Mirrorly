using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Mirrorly.Pages.Mua
{
    public class EditServiceModel : PageModel
    {
        private readonly ISeServices _seServices;
        private readonly ICategoryServices _categoryServices;
        private readonly IWebHostEnvironment _env;

        public EditServiceModel(ISeServices seServices, ICategoryServices categoryServices, IWebHostEnvironment env)
        {
            _seServices = seServices;
            _categoryServices = categoryServices;
            _env = env;
        }

        [BindProperty]
        public Service Service { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public SelectList Categories { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            var service = await _seServices.GetServiceByIdAsync(id);
            if (service == null) return NotFound();

            Service = service;
            Categories = new SelectList(_categoryServices.GetAllCategories(), "CategoryId", "CategoryName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                Categories = new SelectList(_categoryServices.GetAllCategories(), "CategoryId", "CategoryName");
                return Page();
            }

            var muaId = HttpContext.Session.GetInt32("UserId");
            if (muaId == null)
            {
                ModelState.AddModelError(string.Empty, "Bạn cần đăng nhập MUA để thêm dịch vụ.");
                return Page();
            }

            var existing = await _seServices.GetServiceByIdAsync(id);
            if (existing == null) return NotFound();

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
            else
            {
                Service.ImageUrl = existing.ImageUrl;
            }

            Service.MuaId = muaId.Value;
            Service.Active = true;
            Service.ServiceId = id;

            await _seServices.UpdateServiceAsync(Service);
            return RedirectToPage("Services");
        }
    }
}
