using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;

namespace NbtWebAppServices.Response
{
    [DataContract]
    [Description( "Returns Inspections submitted for update which require further attention. Completed Inspections are not returned." )]
    public class CswNbtWcfInspectionsResponse : CswNbtWcfResponseBase
    {
        public CswNbtWcfInspectionsResponse( HttpContext Context )
            : base( Context )
        {
            ActionRequired = new Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection>();
            InComplete = new Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection>();
            Failed = new Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection>();
            Completed = new Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection>();
            Cancelled = new Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection>();
            Missed = new Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection>();
        }

        [DataMember]
        [Description( "Inspections which were successfully updated but are now Action Required." )]
        public Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> ActionRequired { get; set; }

        [DataMember]
        [Description( "Inspections which were successfully updated but are not Completed." )]
        public Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> InComplete { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but failed." )]
        public Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> Failed { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but were already completed by another user." )]
        public Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> Completed { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but were cancelled by another user." )]
        public Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> Cancelled { get; set; }

        [DataMember]
        [Description( "Inspections which were submitted for update but were missed." )]
        public Collection<CswNbtWcfInspectionsDataModel.CswNbtInspection> Missed { get; set; }

    }
}