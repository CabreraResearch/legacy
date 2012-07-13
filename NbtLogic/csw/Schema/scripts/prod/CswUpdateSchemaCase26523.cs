using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26523
    /// </summary>
    public class CswUpdateSchemaCase26523 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass equipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp assemblyOCP = equipmentOC.getObjectClassProp( CswNbtObjClassEquipment.AssemblyPropertyName );

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );

            if( null != assemblyOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( assemblyOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( assemblyOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( assemblyOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, assemblyOC.ObjectClassId );
            }

        }//Update()

    }//class CswUpdateSchemaCase26523

}//namespace ChemSW.Nbt.Schema