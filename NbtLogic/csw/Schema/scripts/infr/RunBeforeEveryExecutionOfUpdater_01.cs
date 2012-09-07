using ChemSW.Core;
using ChemSW.Nbt;
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
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "istemp" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes", "istemp", "Node is temporary", logicaldelete: false, required: true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "sessionid" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodes", "sessionid", "Session ID of temporary node", logicaldelete: false, required: false, datatypesize: 50 );
            }
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update nodes set istemp='0', sessionid=''" );






            // moved from CswUpdateSchemaCase24525 for case 27706
            #region ADD ARCHIVED PROP TO USER
            CswNbtMetaDataFieldType logicalFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Logical );
            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            if( null == userOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Archived ) )
            {
                CswNbtMetaDataObjectClassProp archivedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( userOC )
                {
                    PropName = CswNbtObjClassUser.PropertyName.Archived,
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
                    userNode.Properties[CswNbtObjClassUser.PropertyName.Archived].AsLogical.Checked = Tristate.False;
                    userNode.postChanges( false );
                }
            }
            #endregion

        
        
        
        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


