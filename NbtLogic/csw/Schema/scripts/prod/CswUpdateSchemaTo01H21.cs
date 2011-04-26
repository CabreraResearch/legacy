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
    /// Updates the schema to version 01H-21
    /// </summary>
    public class CswUpdateSchemaTo01H21 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 21 ); } }
        public CswUpdateSchemaTo01H21( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 20998
            CswSequenceName LocationSeqName = new CswSequenceName( "Locations" );
            if( _CswNbtSchemaModTrnsctn.doesSequenceExist( LocationSeqName ) )
            {
                DataTable LocationSeqTable = _CswNbtSchemaModTrnsctn.getSequence( LocationSeqName );


                CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Floor ) );
                if( FloorNT != null )
                {
                    CswNbtMetaDataNodeTypeProp BarcodeNTP = FloorNT.BarcodeProperty;
                    BarcodeNTP.setSequence( CswConvert.ToInt32( LocationSeqTable.Rows[0]["sequenceid"] ) );
                }
            }

        } // update()

    }//class CswUpdateSchemaTo01H21

}//namespace ChemSW.Nbt.Schema


