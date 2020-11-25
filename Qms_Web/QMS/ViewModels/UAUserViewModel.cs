using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QmsCore.UIModel;
using QMS.Constants;

namespace QMS.ViewModels
{
    public class UAUserViewModel
    {
        public int      UserId          { get; set; }
        public int?     ManagerId       { get; set; }

        [Required]
        [Display(Name = "Organization")]
        public int? OrgId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string   EmailAddress    { get; set; }


        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Only upper case characters, lower case characters and the hyphen character are allowed.")]
        [Display(Name = "DisplayName")]
        public string   DisplayName     { get; set; }

        public string   DisplayLabel
        {
            get { return $"{this.DisplayName} - [{this.EmailAddress}]"; }
        }
        public List<UARoleViewModel> Roles          { get; } = new List<UARoleViewModel>();
        public List<UARoleViewModel> CheckboxRoles  { get; } = new List<UARoleViewModel>();

        public bool SearchUserSuccessful    { get; set; } = false;
        public bool ShowCreateUserForm      { get; set; } = false;
        public bool ShowUpdateUserForm      { get; set; } = false;
        
        public bool     ShowAlert           { get; set; } = false;
        public string   AlertMessage        { get; set; }
        public string   AlertType           { get; set; }
        public string   AspAction           { get; set; }
        public string   SubmitButtonLabel   { get; set; }
        public string   CardHeader          { get; set; }
        public string   UserAdminModule     { get; set; }
       

        public string UserNavItemNavLink { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_NAVITEM_NAVLINK_VALUE;
        public string UserTabPadFade { get; set; } = UserAdminConstants.UserAdminCssConstants.DEFAULT_TABPANE_FADE_VALUE;

        public string AlertCssClass
        {
            get
            {
                return (UserAdminConstants.AlertTypeConstants.SUCCESS.Equals(this.AlertType)) ? (UserAdminConstants.UserAdminCssConstants.ALERT_CSS_SUCCESS)
                                                                                                : (UserAdminConstants.UserAdminCssConstants.ALERT_CSS_FAILURE);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("UAUserViewModel = {");
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