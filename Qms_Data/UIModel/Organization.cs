using System;
using QmsCore.Model;

namespace QmsCore.UIModel
{
    public class Organization : ILoggable
    {
        public int ID { get {return OrgId;}}
        public string Label {get {return OrgLabel;}}  

        public int OrgId { get; set; }
        public int? ParentOrgId { get; set; }
        public string OrgCode { get; set; }
        public string OrgLabel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string POI {get;set;}

        public int? OrgTypeId { get; set; }


        public Organization(SecOrg organization)
        {
            this.OrgId = organization.OrgId;
            this.ParentOrgId = organization.ParentOrgId;
            this.OrgCode = organization.OrgCode;
            this.OrgLabel = organization.OrgLabel;
            this.CreatedAt = organization.CreatedAt;
            this.UpdatedAt = organization.UpdatedAt;
            this.DeletedAt = organization.DeletedAt;
            this.OrgTypeId = organization.OrgtypeId;
  //          this.POI = organization.QmsPersonnelOfficeIdentifier.
        }

        public Organization()
        {}

    }    
}