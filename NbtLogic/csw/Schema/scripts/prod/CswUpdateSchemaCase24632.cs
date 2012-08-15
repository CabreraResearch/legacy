using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24632
    /// </summary>
    public class CswUpdateSchemaCase24632 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //get field types used for this class
            CswNbtMetaDataFieldType memoFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Memo );
            CswNbtMetaDataFieldType logicalFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Logical );
            CswNbtMetaDataFieldType textFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Text );

            #region CREATE THE REGULATORY LIST OC AND IT'S PROPS
            CswNbtMetaDataObjectClass regulatoryListOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RegulatoryListClass, "report.gif", true, true );
            CswNbtMetaDataObjectClassProp casNumbersOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( regulatoryListOC )
            {
                PropName = CswNbtObjClassRegulatoryList.CASNumbersPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Memo,
                IsFk = false,
                IsRequired = true,
                ValuePropType = memoFT.FieldType,
                ValuePropId = memoFT.FieldTypeId
            } );

            CswNbtMetaDataObjectClassProp nameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( regulatoryListOC )
            {
                PropName = CswNbtObjClassRegulatoryList.NamePropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                IsFk = false,
                IsRequired = true,
                ValuePropType = textFT.FieldType,
                ValuePropId = textFT.FieldTypeId
            } );
            #endregion

            #region CREATE THE REGULATORY LIST NODETYPE
            CswNbtMetaDataNodeType regulatoryListNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( regulatoryListOC.ObjectClassId, "Regulatory List", "Materials" );
            regulatoryListNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryList.NamePropertyName ) ); //set display name
            #endregion

        }//Update()

    }//class CswUpdateSchemaCase24632

}//namespace ChemSW.Nbt.Schema