using System.Runtime.Serialization;
using ChemSW.Nbt.Actions;
using NbtWebApp.Actions.ChemWatch;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    [DataContract]
    public class CswNbtChemWatchReturn : CswWebSvcReturn
    {
        public CswNbtChemWatchReturn()
        {
            Data = new CswNbtChemWatchRequest();
        }

        [DataMember]
        public CswNbtChemWatchRequest Data;
    }

    public class CswNbtWebServiceChemWatch
    {
        public static void Initialize( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            Return.Data = CswNbtActChemWatch.Initialize( CswResources, Request );
        }

        public static void MaterialSearch( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            Return.Data = CswNbtActChemWatch.MaterialSearch( CswResources, Request );
        }

        public static void SDSDocumentSearch( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            Return.Data = CswNbtActChemWatch.SDSDocumentSearch( CswResources, Request );
        }

        //public static void GetSDSDocument( ICswResources CswResources, string filename )
        //{
        //    Return.Data = CswNbtActChemWatch.GetSDSDocument( CswResources, fileanme );
        //}

        public static void GetMatchingSuppliers( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            Return.Data = CswNbtActChemWatch.GetMatchingSuppliers( CswResources, Request );
        }

        public static void CreateSDSDocuments( ICswResources CswResources, CswNbtChemWatchReturn Return, CswNbtChemWatchRequest Request )
        {
            Return.Data = CswNbtActChemWatch.CreateSDSDocuments( CswResources, Request );
        }

    }
}