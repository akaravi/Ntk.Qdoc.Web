using System.ComponentModel.DataAnnotations;

namespace Ntk.Qdoc.Web.Blazor.ViewModels
{
    public class PostMessageViewModel
    {
        [Required, MinLength(1)]
        public string Text { get; set; }
    }
}
