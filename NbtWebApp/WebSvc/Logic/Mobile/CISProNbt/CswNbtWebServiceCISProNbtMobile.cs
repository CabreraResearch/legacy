using System.Runtime.Serialization;
using ChemSW;
using ChemSW.Nbt;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.csw.Mobile;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Mobile.CISProNbt
{
    public class CswNbtWebServiceCISProNbtMobile
    {
        [DataContract]
        public class CswNbtMobileReturn : CswWebSvcReturn
        {
            public CswNbtMobileReturn()
            {
                Data = new CswNbtCISProNbtMobileData.MobileResponse();
            }

            [DataMember]
            public CswNbtCISProNbtMobileData.MobileResponse Data;
        }

        public static void saveOperations( ICswResources CswResources, CswNbtWebServiceCISProNbtMobile.CswNbtMobileReturn returnobj, CswNbtCISProNbtMobileData.MobileRequest MobileRequest )
        {
            //Shelve this to a batch operation
            CswNbtBatchOpMobileMultiOpUpdates op = new CswNbtBatchOpMobileMultiOpUpdates( (CswNbtResources) CswResources );
            CswNbtObjClassBatchOp BatchNode = op.makeBatchOp( MobileRequest );

        }

        /// <summary>
        /// Saves a given CSV string of RapidLoader records to the temp directory and emails a link to the user
        /// </summary>
        public static void RLSaveData( ICswResources CswResources, CswNbtMobileReturn Return, RapidLoaderData.RapidLoaderDataRequest Request )
        {
            CswNbtResources _CswNbtResources = ( CswNbtResources ) CswResources;
            Request.EmailAddress = _CswNbtResources.CurrentNbtUser.Email.Trim();
            CswNbtMobileRapidLoader _CswNbtMobileRapidLoader = new CswNbtMobileRapidLoader( _CswNbtResources );
            _CswNbtMobileRapidLoader.saveRapidLoaderData( Request );
        }

    }//class CswNbtWebServiceCISProNbtMobile

}//namespace NbtWebApp.WebSvc.Logic.CISProNbtMobile
