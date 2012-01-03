using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-09
    /// </summary>
    public class CswUpdateSchemaTo01L09 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 09 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24574

            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

            CswNbtMetaDataObjectClassProp DueDateOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.NextDueDatePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DueDateOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended, CswNbtNodePropDateTime.DateDisplayMode.DateTime.ToString() );

            #endregion Case 24574

            #region Case 20941

            CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

            //Remove the oppression from existing Inspection nodetypes since they will be locked by default
            foreach( CswNbtMetaDataNodeType NodeType in InspectionDesignOc.NodeTypes )
            {
                if( NodeType.IsLatestVersion )
                {
                    NodeType.IsLocked = false;
                }
            }


            #endregion Case 20941

        }//Update()

    }//class CswUpdateSchemaTo01L09

}//namespace ChemSW.Nbt.Schema


