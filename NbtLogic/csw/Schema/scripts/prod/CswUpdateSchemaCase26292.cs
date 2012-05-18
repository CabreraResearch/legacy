using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26292
    /// </summary>
    public class CswUpdateSchemaCase26292 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // link InspectionTargetGroupClass to SI module
            int ocid = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.SI, ocid );


        }//Update()

    }//class CswUpdateSchemaCase26292

}//namespace ChemSW.Nbt.Schema