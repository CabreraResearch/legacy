using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;
using ChemSW.Nbt.Security;

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
            CswNbtMetaDataObjectClass regulatoryListOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RegulatoryListClass, "doc.png", true, true );
            CswNbtMetaDataObjectClassProp nameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( regulatoryListOC )
            {
                PropName = CswNbtObjClassRegulatoryList.NamePropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                IsFk = false,
                IsRequired = true,
                IsUnique = true
            } );

            CswNbtMetaDataObjectClassProp casNumbersOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( regulatoryListOC )
            {
                PropName = CswNbtObjClassRegulatoryList.CASNumbersPropertyName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Memo,
                IsFk = false,
                IsRequired = true
            } );
            #endregion

            #region CREATE THE REGULATORY LIST NODETYPE
            CswNbtMetaDataNodeType regulatoryListNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( regulatoryListOC.ObjectClassId, "Regulatory List", "Materials" );
            regulatoryListNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRegulatoryList.NamePropertyName ) ); //set display name
            CswNbtMetaDataNodeTypeProp casNosNTP = regulatoryListNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRegulatoryList.CASNumbersPropertyName );
            casNosNTP.HelpText = "The CASNos property should be a comma delimited set of CASNos in this regulatory list. Example: \"CASNo1,CASNo2,CASNo3\"";
            #endregion

            #region CREATE REGULATORY LISTS VIEW

            CswNbtObjClassRole cisProAdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "CISPro_Admin" );
            if( null != cisProAdminRole )
            {
                CswNbtView regListsView = _CswNbtSchemaModTrnsctn.makeNewView( "Regulatory Lists", NbtViewVisibility.Role, cisProAdminRole.NodeId );
                regListsView.SetViewMode( NbtViewRenderingMode.Tree );
                regListsView.Category = "CISPro Configuration";
                CswNbtViewRelationship parent = regListsView.AddViewRelationship( regulatoryListOC, false );
                regListsView.save();
            }

            #endregion

            #region ADD REGULATORY LIST NODETYPE PERMISSIONS TO CISPro_Admin

            if( null != cisProAdminRole )
            {
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.View, regulatoryListNT, cisProAdminRole, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Create, regulatoryListNT, cisProAdminRole, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Edit, regulatoryListNT, cisProAdminRole, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtPermit.NodeTypePermission.Delete, regulatoryListNT, cisProAdminRole, true );
            }

            #endregion

        }//Update()

    }//class CswUpdateSchemaCase24632

}//namespace ChemSW.Nbt.Schema