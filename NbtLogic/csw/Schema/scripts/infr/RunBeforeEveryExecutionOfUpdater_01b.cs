using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01b : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region ROMEO

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

            #endregion ROMEO


            #region SEBASTIAN

            // case 27703 - change containers dispose/dispense buttons to say "Dispose this Container" and "Dispense this Container"
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );

            CswNbtMetaDataObjectClassProp dispenseOCP = containerOC.getObjectClassProp( "Dispense" );
            if( null != dispenseOCP ) //have to null check because property might have already been updated
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( dispenseOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Dispense this Container" );
            }

            CswNbtMetaDataObjectClassProp disposeOCP = containerOC.getObjectClassProp( "Dispose" );
            if( null != disposeOCP ) //have to null check here because property might have been updated
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( disposeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Dispose this Container" );
            }

            #endregion SEBASTIAN

        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01b

}//namespace ChemSW.Nbt.Schema


