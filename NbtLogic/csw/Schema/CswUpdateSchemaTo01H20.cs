using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-20
    /// </summary>
    public class CswUpdateSchemaTo01H20 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 20 ); } }
        public CswUpdateSchemaTo01H20( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            var PhysicalInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Physical_Inspection ) );
            var PiSetupTab = PhysicalInspectionNT.getNodeTypeTab( "Setup" );
            if( null != PiSetupTab )
            {
                PiSetupTab.TabName = "Detail";
                PiSetupTab.TabOrder = 3;
            }

        } // update()

    }//class CswUpdateSchemaTo01H20

}//namespace ChemSW.Nbt.Schema


