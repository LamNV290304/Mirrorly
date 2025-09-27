using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Mua
{
    public class ServicesModel : PageModel
    {
        private readonly ISeServices _seServices;

        public ServicesModel(ISeServices seServices)
        {
            _seServices = seServices;
        }
        [BindProperty]
        public List<Service> Services { get; set; }
        public void OnGet(int? id)
        {
            id = HttpContext.Session.GetInt32("RoleId");
            if (id.HasValue)
            {
                Services = _seServices.getServicesByMuaId(id.Value);
            }
        }
    }
}
