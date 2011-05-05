using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-05
    /// </summary>
    public class CswUpdateSchemaTo01H05 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 


        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 05 ); } }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public CswUpdateSchemaTo01H05( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );

        }

        public void update()
        {
            // BZ 9755
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-05_OCP_Update", "object_class_props" );
            DataTable OCPTable = OCPUpdate.getTable( "where lower(propname) = 'username'" );
            foreach( DataRow OCPRow in OCPTable.Rows )
            {
                OCPRow["isglobalunique"] = CswConvert.ToDbVal( true );
            }
            OCPUpdate.update( OCPTable );


            // BZ 8156
            // Remove 'schedrunner' role and user
            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

            CswNbtMetaDataObjectClassProp RoleNameOCP = RoleOC.getObjectClassProp( CswNbtObjClassRole.NamePropertyName );
            CswNbtMetaDataObjectClassProp UserUsernameOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.UsernamePropertyName );
            CswNbtMetaDataObjectClassProp UserRoleOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.RolePropertyName );

            CswNbtView RolesAndUsersView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship RoleVR = RolesAndUsersView.AddViewRelationship( RoleOC, false );
            CswNbtViewRelationship UserVR = RolesAndUsersView.AddViewRelationship( RoleVR, CswNbtViewRelationship.PropOwnerType.Second, UserRoleOCP, false );
            CswNbtViewProperty RoleNameVP = RolesAndUsersView.AddViewProperty( RoleVR, RoleNameOCP );
            CswNbtViewPropertyFilter RoleNameVPF = RolesAndUsersView.AddViewPropertyFilter( RoleNameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, "ScheduleRunner", false );

            ICswNbtTree RolesAndUsersTree = _CswNbtSchemaModTrnsctn.getTreeFromView( RolesAndUsersView, true );
            for( Int32 r = 0; r < RolesAndUsersTree.getChildNodeCount(); r++ )
            {
                RolesAndUsersTree.goToNthChild( r );
                for( Int32 u = 0; u < RolesAndUsersTree.getChildNodeCount(); u++ )
                {
                    RolesAndUsersTree.goToNthChild( u );
                    CswNbtNode UserNode = RolesAndUsersTree.getNodeForCurrentPosition();
                    UserNode.delete();
                    RolesAndUsersTree.goToParentNode();
                }
                CswNbtNode RoleNode = RolesAndUsersTree.getNodeForCurrentPosition();
                RoleNode.delete();
                RolesAndUsersTree.goToParentNode();
            }

        }//Update()

    }//class CswUpdateSchemaTo01H05

}//namespace ChemSW.Nbt.Schema


