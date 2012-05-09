using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    [Description( "Returns Inspections submitted for update which require further attention. Completed Inspections are not returned." )]
    public class CswNbtWebServiceResponseInspections : CswNbtWebServiceResponseBase
    {
        public CswNbtWebServiceResponseInspections( HttpContext Context )
            : base( Context )
        {
            ActionRequired = new Collection<CswNbtInspectionsDataModel.CswNbtInspection>();
            InComplete = new Collection<CswNbtInspectionsDataModel.CswNbtInspection>();
            Failed = new Collection<CswNbtInspectionsDataModel.CswNbtInspection>();
            Completed = new Collection<CswNbtInspectionsDataModel.CswNbtInspection>();
            Cancelled = new Collection<CswNbtInspectionsDataModel.CswNbtInspection>();
            Missed = new Collection<CswNbtInspectionsDataModel.CswNbtInspection>();
        }

        [DataMember]
        [Description( "Inspections which were successfully updated but are now Action Required." )]
        public Collection<CswNbtInspectionsDataModel.CswNbtInspection> ActionRequired { get; set; }

        [DataMember]
        [Description( "Inspections which were successfully updated but are not Completed." )]
        public Collection<CswNbtInspectionsDataModel.CswNbtInspection> InComplete { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but failed." )]
        public Collection<CswNbtInspectionsDataModel.CswNbtInspection> Failed { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but were already completed by another user." )]
        public Collection<CswNbtInspectionsDataModel.CswNbtInspection> Completed { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but were cancelled by another user." )]
        public Collection<CswNbtInspectionsDataModel.CswNbtInspection> Cancelled { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but were missed." )]
        public Collection<CswNbtInspectionsDataModel.CswNbtInspection> Missed { get; set; }

    }
}