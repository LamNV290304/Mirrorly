using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Mua
{
    public class MuaProfilesModel : PageModel
    {
        private readonly IMuaServices _muaServices;

        public MuaProfilesModel(IMuaServices muaServices)
        {
            _muaServices = muaServices;
        }
        [BindProperty]
        public List<Muaprofile> MuaProfiles { get; set; }
        public void OnGet()
        {
            MuaProfiles = _muaServices.GetAllMUAProfiles();
        }
    }
}
