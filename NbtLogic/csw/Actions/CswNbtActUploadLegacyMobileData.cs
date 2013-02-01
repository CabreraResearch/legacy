using System;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Mobile;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActUploadLegacyMobileData
    {
        CswNbtResources _CswNbtResources;

        public CswNbtActUploadLegacyMobileData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public CswNbtObjClassBatchOp makeNodesBatch( CswNbtCISProNbtMobileData.MobileRequest LegacyData )
        {
            // Send to background task
            CswNbtObjClassBatchOp BatchNode = null;
            CswNbtBatchOpMobileMultiOpUpdates BatchOp = new CswNbtBatchOpMobileMultiOpUpdates( _CswNbtResources );
            BatchNode = BatchOp.makeBatchOp( LegacyData );

            return BatchNode;
        }

    }//class CswNbtActUploadLegacyMobileData

}//namespace ChemSW.Actions
