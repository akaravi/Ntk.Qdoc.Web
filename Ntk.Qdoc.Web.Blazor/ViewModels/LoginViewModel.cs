using System.ComponentModel.DataAnnotations;

namespace Ntk.Qdoc.Web.Blazor.ViewModels
{
    public class LoginViewModel{

        [Required, MinLength(1)]
        public string Username{get;set;}
    }
}
