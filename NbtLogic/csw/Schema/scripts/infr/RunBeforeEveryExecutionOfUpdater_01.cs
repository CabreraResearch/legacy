using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        public override void update()
        {
            #region case 24481

            // Also in 01M-06
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "field_types", "searchable" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "field_types", "searchable", "Whether the field type is searchable", false, true );
            }

            #endregion case 24481

            #region case 25322

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_props", "usenumbering" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class_props", "usenumbering", "Whether the property should be numbered", false, false );
            }

            #endregion case 25322

            #region case 24520

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "audit_transactions", "transactionfirstname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "audit_transactions", "transactionfirstname", "First name of transaction user", false, false, 50 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "audit_transactions", "transactionlastname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "audit_transactions", "transactionlastname", "Last name of transaction user", false, false, 50 );
            }

            #endregion case 24520

            #region case 25518
            // This needs to be run before any execution because it breaks CswNbtObjClassUser

            // Rename 'Quick Launch' views and actions to 'Favorite'
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );

            CswNbtMetaDataObjectClassProp UserQLAProp = UserOC.getObjectClassProp( "Quick Launch Actions" );
            if( UserQLAProp != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UserQLAProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, CswNbtObjClassUser.FavoriteActionsPropertyName );
            }

            CswNbtMetaDataObjectClassProp UserQLVProp = UserOC.getObjectClassProp( "Quick Launch Views" );
            if( UserQLVProp != null )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UserQLVProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, CswNbtObjClassUser.FavoriteViewsPropertyName );
            }
            #endregion case 25518

            #region case 22962

            if( false == _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( "field_types_subfields" ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( "field_types_subfields", "ftsubfieldid" );
                _CswNbtSchemaModTrnsctn.addLongColumn( "field_types_subfields", "fieldtypeid", "FK to field_types", true, true );
                _CswNbtSchemaModTrnsctn.addStringColumn( "field_types_subfields", "propcolname", "name of storage column in jct_node_props", true, true, 20 );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "field_types_subfields", "reportable", "whether to include auto-generated views", true, true );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "field_types_subfields", "is_default", "this field gets no subfield suffix in the view", true, true );
                _CswNbtSchemaModTrnsctn.addStringColumn( "field_types_subfields", "subfieldname", "suffix on this propertyname in the view", true, false, 20 );
            }

            #endregion case 22962


            #region case 24979

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_props", "iscompoundunique" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class_props", "iscompoundunique",
                                                          "all compound unique columns on an instance are validated for uniqueness", false, false );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodetype_props", "iscompoundunique" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetype_props", "iscompoundunique",
                                                          "all compound unique columns on an instance are validated for uniqueness", false, false );
            }

            #endregion case 24979





            // this should be last
            #region case 25635

            _CswNbtSchemaModTrnsctn.makeMissingAuditTablesAndColumns();

            #endregion



        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema


