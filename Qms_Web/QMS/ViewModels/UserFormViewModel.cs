using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QmsCore.UIModel;
using QMS.Constants;

namespace QMS.ViewModels
{
    public class UserFormViewModel
    {
        public int UserId { get; set; }
        public int? ManagerId { get; set; }

        [Required]
        [Display(Name = "Organization")]
        public int? OrgId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }


        [Required]
        //[RegularExpression(@"^[a-zA-Z''-.'\s]{1,40}$", ErrorMessage = "Only upper case characters, lower case characters and the hyphen character are allowed.")]
        [Display(Name = "DisplayName")]
        public string DisplayName { get; set; }

        public string DisplayLabel
        {
            get { return $"{this.DisplayName} - [{this.EmailAddress}]"; }
        }

        public List<UARoleViewModel> Roles          { get; } = new List<UARoleViewModel>();
        public List<UARoleViewModel> CheckboxRoles  { get; } = new List<UARoleViewModel>();

        public string AspAction         { get; set; }
        public string SubmitButtonLabel { get; set; }
        public string CardHeader        { get; set; }
        public bool   Deactivatable     { get; set; } = false;
        public bool   Reactivatable     { get; set; } = false;
        public bool   Mutatatable       { get; set; } = false;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("UserFormViewModel = {");
            sb.Append("UserId: ");
            sb.Append(this.UserId);
            sb.Append(", ManagerId: ");
            sb.Append(this.ManagerId);
            sb.Append(", OrgId: ");
            sb.Append(this.OrgId);
            sb.Append(", EmailAddress: ");
            sb.Append(this.EmailAddress);
            sb.Append(", DisplayName: ");
            sb.Append(this.DisplayName);
            sb.Append(", DisplayLabel: ");
            sb.Append(this.DisplayLabel);
            sb.Append("}");
            return sb.ToString();
        }
    }
}