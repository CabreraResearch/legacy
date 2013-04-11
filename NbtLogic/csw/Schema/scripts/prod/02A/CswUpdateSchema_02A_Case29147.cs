using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29147
    /// </summary>
    public class CswUpdateSchema_02A_Case29147: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29147; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp StorageTemperatureOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.StorageTemperature );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StorageTemperatureOCP, CswNbtObjClassContainer.StorageTemperatures.RoomTemperature );
            CswNbtMetaDataObjectClassProp StoragePressureOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.StoragePressure );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StoragePressureOCP, CswNbtObjClassContainer.StoragePressures.Atmospheric );
            CswNbtMetaDataObjectClassProp UseTypeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.UseType );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( UseTypeOCP, CswNbtObjClassContainer.UseTypes.Storage );
        } // update()
    }//class CswUpdateSchema_02A_Case29147
}//namespace ChemSW.Nbt.Schema