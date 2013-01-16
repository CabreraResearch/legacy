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

    }//class CswNbtWebServiceCISProNbtMobile

}//namespace NbtWebApp.WebSvc.Logic.CISProNbtMobile
