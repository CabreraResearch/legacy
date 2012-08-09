using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24525
    /// </summary>
    public class CswUpdateSchemaCase24525 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataFieldType logicalFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Logical );
            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp archivedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( userOC )
            {
                PropName = CswNbtObjClassUser.ArchivedPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsFk = false,
                IsRequired = true,
                ValuePropType = logicalFT.FieldType,
                ValuePropId = logicalFT.FieldTypeId,
            } );
            //set the default val to false - we don't want new users to be archived
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( archivedOCP, archivedOCP.getFieldTypeRule().SubFields.Default.Name, Tristate.False );

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            foreach( CswNbtNode userNode in userOC.getNodes( false, false ) )
            {
                userNode.Properties[CswNbtObjClassUser.ArchivedPropertyName].AsLogical.Checked = Tristate.False;
                userNode.postChanges( false );
            }



        }//Update()

    }//class CswUpdateSchemaCase24525

}//namespace ChemSW.Nbt.Schema