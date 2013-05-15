using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for Structure Searching Mols
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class ViewEditor
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "HandleStep" )]
        [Description( "Get the data for a particule View Editor step" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse HandleStep( CswNbtViewEditorData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.HandleStep,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetPreview" )]
        [Description( "Get the data for a particule View Editor step" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse GetPreview( CswNbtViewEditorData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.GetPreview,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "AddFilter" )]
        [Description( "Add a filter to a view" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse AddFilter( CswNbtViewEditorFilterData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorFilterData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.AddFilter,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "RemoveFilter" )]
        [Description( "Remove a filter from a view" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse RemoveFilter( CswNbtViewEditorFilterData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorFilterData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.RemoveFilter,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetFilterProps" )]
        [Description( "Get all properties associated with a relationship" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtViewEditorResponse GetFilterProps( CswNbtViewEditorFilterData Request )
        {
            CswNbtViewEditorResponse Ret = new CswNbtViewEditorResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtViewEditorResponse, CswNbtViewEditorFilterData>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceView.GetFilterProps,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }

    [DataContract]
    public class CswNbtViewEditorResponse: CswWebSvcReturn
    {
        public CswNbtViewEditorResponse()
        {
            Data = new CswNbtViewEditorData();
        }

        [DataMember]
        public CswNbtViewEditorData Data = new CswNbtViewEditorData();
    }

    public class CswNbtViewEditorData
    {
        [DataMember]
        public string ViewId = string.Empty;

        [DataMember]
        public int StepNo = Int32.MinValue;

        [DataMember]
        public string Preview = string.Empty;

        [DataMember]
        public CswNbtView CurrentView = null;

        [DataMember]
        public CswNbtViewEditorStep2 Step2 = new CswNbtViewEditorStep2();

        [DataMember]
        public CswNbtViewEditorStep3 Step3 = new CswNbtViewEditorStep3();

        [DataMember]
        public CswNbtViewEditorStep4 Step4 = new CswNbtViewEditorStep4();
    }

    [DataContract]
    public class CswNbtViewEditorStep2
    {
        [DataMember]
        public Collection<CswNbtViewEditorRelationship> Relationships = new Collection<CswNbtViewEditorRelationship>();
    }

    [DataContract]
    public class CswNbtViewEditorStep3
    {
        [DataMember]
        public Collection<CswNbtViewEditorProperty> Properties = new Collection<CswNbtViewEditorProperty>();
    }

    [DataContract]
    public class CswNbtViewEditorStep4
    {
        [DataMember]
        public string ViewJson = string.Empty;

        [DataMember]
        public Collection<CswNbtViewPropertyFilter> Filters = new Collection<CswNbtViewPropertyFilter>();

        [DataMember]
        public Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

        [DataMember]
        public Collection<CswNbtViewProperty> Properties = new Collection<CswNbtViewProperty>();
    }

    public class CswNbtViewEditorRelationship
    {
        [DataMember]
        public CswNbtViewRelationship Relationship = null;

        [DataMember]
        public bool Checked = false;
    }

    public class CswNbtViewEditorProperty
    {
        [DataMember]
        public CswNbtViewProperty Property = null;

        [DataMember]
        public bool Checked = false;
    }

    public class CswNbtViewEditorFilterData
    {
        [DataMember]
        public CswNbtViewPropertyFilter FilterToRemove;

        [DataMember]
        public CswNbtViewRelationship Relationship;

        [DataMember]
        public CswNbtView CurrentView;

        //For adding to a view
        [DataMember]
        public string FilterConjunction = string.Empty;
        [DataMember]
        public string FilterMode = string.Empty;
        [DataMember]
        public string FilterValue = string.Empty;
        [DataMember]
        public string FilterSubfield = string.Empty;
        [DataMember]
        public string PropArbId = string.Empty;

        [DataMember]
        public CswNbtViewProperty Property;
    }
}