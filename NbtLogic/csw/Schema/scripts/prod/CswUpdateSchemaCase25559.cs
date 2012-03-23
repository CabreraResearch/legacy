using System;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25559
    /// </summary>
    public class CswUpdateSchemaCase25559 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Set 'Inspection Design' 'Target' property's fktype and value to 'InspectionTarget' class
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClass InspectionTargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );

            CswNbtMetaDataObjectClassProp InspectionDesignTargetOCP = InspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InspectionDesignTargetOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InspectionDesignTargetOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, InspectionTargetOC.ObjectClassId.ToString() );

        }//Update()

    }//class CswUpdateSchemaCase25559

}//namespace ChemSW.Nbt.Schema