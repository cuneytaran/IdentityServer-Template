using Microsoft.AspNetCore.Identity;

namespace IdentityAPI.AuthServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //user tablosunu oluştururken ekstra tablo oluşturmak istiyorsak aşağıdakini uyguluyoruz.
        public string City { get; set; }
        public string IkinciMeslegi { get; set; }
    }
}
