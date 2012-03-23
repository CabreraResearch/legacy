using System;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-01
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DML";

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

        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema


