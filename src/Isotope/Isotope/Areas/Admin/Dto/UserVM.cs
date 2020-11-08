using Isotope.Code.Utils;
using Isotope.Data.Models;
using Mapster;

namespace Isotope.Areas.Admin.Dto
{
    public class UserVM: IMapped
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        
        public void Configure(TypeAdapterConfig config)
        {
            config.NewConfig<AppUser, UserVM>()
                  .Map(x => x.Id, x => x.Id)
                  .Map(x => x.UserName, x => x.UserName)
                  .Map(x => x.IsAdmin, x => x.IsAdmin);
            
            config.NewConfig<UserVM, AppUser>()
                  .Map(x => x.IsAdmin, x => x.IsAdmin);
        }
    }
}