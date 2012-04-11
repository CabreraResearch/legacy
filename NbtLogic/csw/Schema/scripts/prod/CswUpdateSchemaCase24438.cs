using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24438
    /// </summary>
    public class CswUpdateSchemaCase24438 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass, "folder.gif", false, false );

            // Work Unit - Auditing Enabled
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass,
                                                           CswNbtObjClassWorkUnit.AuditingEnabledPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect );
            
            // Work Unit - Signature Required
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass,
                                                           CswNbtObjClassWorkUnit.SignatureRequiredPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect );

            // User - Default Location
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass,
                                                           CswNbtObjClassUser.DefaultLocationPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Location );

            // User - Work Unit
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass,
                                                           CswNbtObjClassUser.WorkUnitPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                           false,
                                                           false,
                                                           true,
                                                           NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                           WorkUnitOC.ObjectClassId,
                                                           true );

        }//Update()

    }//class CswUpdateSchemaCase24438

}//namespace ChemSW.Nbt.Schema