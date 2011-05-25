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
    /// Updates the schema to version 01H-06
    /// </summary>
    public class CswUpdateSchemaTo01H06 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 06 ); } }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public CswUpdateSchemaTo01H06( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            //<BZ 10319>
            CswTableUpdate FieldTypeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("Question_FieldType_Update", "field_types");
            DataTable NewFieldTypeTable = FieldTypeUpdate.getEmptyTable();
            DataRow NewFieldTypeRow = NewFieldTypeTable.NewRow();
            NewFieldTypeRow["auditflag"] = CswConvert.ToDbVal(false);
            NewFieldTypeRow["datatype"] = "xml";
            NewFieldTypeRow["deleted"] = CswConvert.ToDbVal(false);
            NewFieldTypeRow["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.Question.ToString();
            NewFieldTypeTable.Rows.Add(NewFieldTypeRow);
            FieldTypeUpdate.update(NewFieldTypeTable);

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();
            //</BZ 10319>
        
            // BZ 10094
            // New Notification Object Class
            CswNbtMetaDataObjectClass NotificationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass );

            // Default nodetype
            CswNbtMetaDataNodeType NotificationNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( NotificationOC.ObjectClassId, CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Notification ), "" );

            CswNbtMetaDataNodeTypeProp EventNTP = NotificationNT.getNodeTypeProp( CswNbtObjClassNotification.EventPropertyName );
            CswNbtMetaDataNodeTypeProp TargetTypeNTP = NotificationNT.getNodeTypeProp( CswNbtObjClassNotification.TargetTypePropertyName );
            CswNbtMetaDataNodeTypeProp PropertyNTP = NotificationNT.getNodeTypeProp( CswNbtObjClassNotification.PropertyPropertyName );
            CswNbtMetaDataNodeTypeProp ValueNTP = NotificationNT.getNodeTypeProp( CswNbtObjClassNotification.ValuePropertyName );
            CswNbtMetaDataNodeTypeProp SubscribedUsersNTP = NotificationNT.getNodeTypeProp( CswNbtObjClassNotification.SubscribedUsersPropertyName );
            CswNbtMetaDataNodeTypeProp SubjectNTP = NotificationNT.getNodeTypeProp( CswNbtObjClassNotification.SubjectPropertyName );
            CswNbtMetaDataNodeTypeProp MessageNTP = NotificationNT.getNodeTypeProp( CswNbtObjClassNotification.MessagePropertyName );

            SubjectNTP.DefaultValue.AsText.Text = "ChemSW Live Notification";
            EventNTP.SetValueOnAdd = true;
            TargetTypeNTP.SetValueOnAdd = true;

            TargetTypeNTP.DisplayColumn = 1;
            TargetTypeNTP.DisplayRow = 1;
            TargetTypeNTP.DisplayColAdd = 1;
            TargetTypeNTP.DisplayRowAdd = 1;

            EventNTP.DisplayColumn = 1;
            EventNTP.DisplayRow = 2;
            EventNTP.DisplayColAdd = 1;
            EventNTP.DisplayRowAdd = 2;

            PropertyNTP.DisplayColumn = 1;
            PropertyNTP.DisplayRow = 3;
            PropertyNTP.DisplayColAdd = 1;
            PropertyNTP.DisplayRowAdd = 3;

            ValueNTP.DisplayColumn = 1;
            ValueNTP.DisplayRow = 4;
            ValueNTP.DisplayColAdd = 1;
            ValueNTP.DisplayRowAdd = 4;

            SubscribedUsersNTP.DisplayColumn = 1;
            SubscribedUsersNTP.DisplayRow = 5;
            SubscribedUsersNTP.DisplayColAdd = 1;
            SubscribedUsersNTP.DisplayRowAdd = 5;

            SubjectNTP.DisplayColumn = 1;
            SubjectNTP.DisplayRow = 6;
            SubjectNTP.DisplayColAdd = 1;
            SubjectNTP.DisplayRowAdd = 6;

            MessageNTP.DisplayColumn = 1;
            MessageNTP.DisplayRow = 7;
            MessageNTP.DisplayColAdd = 1;
            MessageNTP.DisplayRowAdd = 7;

            // e.g. Equipment, On Create
            // e.g. Equipment, On Edit
            // e.g. Equipment, On Edit Status
            // e.g. Equipment, On Edit Status Retired
            NotificationNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( TargetTypeNTP.PropName ) +
                                              ", On " + CswNbtMetaData.MakeTemplateEntry( EventNTP.PropName ) +
                                              " " + CswNbtMetaData.MakeTemplateEntry( PropertyNTP.PropName ) +
                                              " " + CswNbtMetaData.MakeTemplateEntry( ValueNTP.PropName );

            // make Property and Value dependent on Event
            PropertyNTP.setFilter( EventNTP.PropId, EventNTP.FieldTypeRule.SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassNotification.EventOption.Edit.ToString() );
            ValueNTP.setFilter( EventNTP.PropId, EventNTP.FieldTypeRule.SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassNotification.EventOption.Edit.ToString() );

            CswNbtNode AdminRoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( AdminRoleNode != null )
            {
                //Grant admin user permissions
                CswNbtNodePropLogicalSet Permissions = ( (CswNbtObjClassRole) CswNbtNodeCaster.AsRole( AdminRoleNode ) ).NodeTypePermissions;
                Permissions.SetValue( NodeTypePermission.Create.ToString(), NotificationNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), NotificationNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), NotificationNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), NotificationNT.NodeTypeId.ToString(), true );
                Permissions.Save();
                AdminRoleNode.postChanges( true );

                // Default view
                CswNbtView NotificationView = _CswNbtSchemaModTrnsctn.makeView();
                NotificationView.makeNew( "Notifications", NbtViewVisibility.Role, AdminRoleNode.NodeId, null, null );
                NotificationView.Category = "System";
                NotificationView.AddViewRelationship( NotificationOC, true );
                NotificationView.save();
            }


            // Email address is now an object class prop on User
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-06_OCP_Update", "object_class_props" );
            DataTable UserOCPTable = OCPUpdate.getEmptyTable();
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( UserOCPTable, UserOC.ObjectClassId, CswNbtObjClassUser.EmailPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            OCPUpdate.update( UserOCPTable );

            // first, for proper placement of new properties
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.NodeTypes )
            {
                if( UserNT.getNodeTypeProp( CswNbtObjClassUser.EmailPropertyName ) == null )
                {
                    CswNbtMetaDataNodeTypeProp LastNameProp = UserNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassUser.LastNamePropertyName );
                    _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( UserNT, LastNameProp, CswNbtMetaDataFieldType.NbtFieldType.Text, CswNbtObjClassUser.EmailPropertyName );
                }
            }

            // after, for synchronization of existing properties
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();


            // BZ 10094 still
            // Changes to Mail Reports:

            CswNbtMetaDataFieldType UserSelectFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.UserSelect );
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );

            // Recipients OCP is now a UserSelect
            DataTable RecipientsOCPTable = OCPUpdate.getTable( "where propname = '" + CswNbtObjClassMailReport.RecipientsPropertyName + "' and objectclassid = '" + MailReportOC.ObjectClassId.ToString() + "'" );
            Int32 RecipientsOCPId = CswConvert.ToInt32( RecipientsOCPTable.Rows[0]["objectclasspropid"] );
            RecipientsOCPTable.Rows[0]["fieldtypeid"] = CswConvert.ToDbVal( UserSelectFT.FieldTypeId );
            OCPUpdate.update( RecipientsOCPTable );

            // Update nodetype props
            CswTableUpdate NTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H-06_NTP_Update", "nodetype_props" );
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.NodeTypes )
            {
                // 1. Create new Recipients UserSelect property
                CswNbtMetaDataNodeTypeProp OldRecipientsNTP = MailReportNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.RecipientsPropertyName );
                CswNbtMetaDataNodeTypeProp NewRecipientsNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( MailReportNT, OldRecipientsNTP, CswNbtMetaDataFieldType.NbtFieldType.UserSelect, CswNbtObjClassMailReport.RecipientsPropertyName + "_TMP" );

                // 2. Reassign the objectclasspropid from the old Recipients Property to the new Recipients Property
                NewRecipientsNTP._DataRow["objectclasspropid"] = CswConvert.ToDbVal( RecipientsOCPId );
                OldRecipientsNTP._DataRow["objectclasspropid"] = DBNull.Value;
                _CswNbtSchemaModTrnsctn.MetaData.refreshAll();  // this will post these changes and refresh

                // 3. Attempt to adapt existing mail report recipients to user accounts for every existing mail report
                //         NOTE: Since we just added the email property above, in order for this to do anything meaningful,
                //         the user would have to have added their own Email property to their User nodetype.
                //         However, if they do this, subscriptions do convey from the old to the new Recipients property.

				//ICswNbtTree UsersTree = _CswNbtSchemaModTrnsctn.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
				//ICswNbtTree MailReportTree = _CswNbtSchemaModTrnsctn.Trees.getTreeFromNodeTypeId( MailReportNT.NodeTypeId );

				//for( Int32 r = 0; r < MailReportTree.getChildNodeCount(); r++ )
				//{
				//    MailReportTree.goToNthChild( r );
				//    CswNbtNode MailReportNode = MailReportTree.getNodeForCurrentPosition();
				foreach(CswNbtNode MailReportNode in MailReportNT.getNodes(false, false))
				{
                    char[] delimiters = { ';', ',', '\n' };
                    string OldRecipientsString = MailReportNode.Properties[OldRecipientsNTP].AsMemo.Text.Replace( "\r", "" );
                    string[] OldRecipientsArray = OldRecipientsString.Split( delimiters, StringSplitOptions.RemoveEmptyEntries );

                    CswCommaDelimitedString NewRecipientsUserIds = new CswCommaDelimitedString();
					//for( Int32 u = 0; u < UsersTree.getChildNodeCount(); u++ )
					//{
					//    UsersTree.goToNthChild( r );
					//    CswNbtNode UserNode = UsersTree.getNodeForCurrentPosition();
					foreach(CswNbtNode UserNode in UserOC.getNodes(false, false))
					{
                        CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) CswNbtNodeCaster.AsUser( UserNode );
                        foreach( string OldRecipientAddress in OldRecipientsArray )
                        {
                            if( UserNodeAsUser.Email.Trim() == OldRecipientAddress.Trim() )
                            {
                                NewRecipientsUserIds.Add( UserNode.NodeId.PrimaryKey.ToString() );
                            }
                        }
                        //UsersTree.goToParentNode();
                    }
                    MailReportNode.Properties[NewRecipientsNTP].AsUserSelect.SelectedUserIds = NewRecipientsUserIds;
                    MailReportNode.postChanges( false );

                    //MailReportTree.goToParentNode();
                }

                // 4. Delete the old Property
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( OldRecipientsNTP );

                // 5. Rename the new Property
                NewRecipientsNTP.PropName = CswNbtObjClassMailReport.RecipientsPropertyName;
            }
            
            //Fix null values to prevent int32.MinVal madness
            String UpdateDisplayRowAdd = "update nodetype_props set display_row_add=display_row where display_row_add is null";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( UpdateDisplayRowAdd );

            String UpdateDisplayColAdd = "update nodetype_props set display_col_add=display_col where display_col_add is null";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( UpdateDisplayColAdd );

            // for synchronization
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();


        }//Update()

    }//class CswUpdateSchemaTo01H06

}//namespace ChemSW.Nbt.Schema


