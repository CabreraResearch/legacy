using System;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01M : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Modules";

        public void _decoupleImcsFromPrintLabel()
        {
            Int32 ImcsId = _CswNbtSchemaModTrnsctn.getModuleId( CswNbtModuleName.IMCS );
            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
            foreach( CswNbtMetaDataNodeType NodeType in PrintLabelOc.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.removeModuleNodeTypeJunction( ImcsId, NodeType.NodeTypeId );
            }


        }

        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region SEBASTIAN

            _decoupleImcsFromPrintLabel();

            #endregion SEBASTIAN


        }//Update()

    }//class RunBeforeEveryExecutionOfUpdater_01OC

}//namespace ChemSW.Nbt.Schema


