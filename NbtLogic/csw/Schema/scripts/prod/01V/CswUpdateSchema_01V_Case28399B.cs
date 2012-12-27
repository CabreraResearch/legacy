using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28399
    /// </summary>
    public class CswUpdateSchema_01V_Case28399B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28399; }
        }

        public override void update()
        {
            // Fix existing barcode values
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                foreach( CswNbtObjClassLocation LocationNode in LocationNT.getNodes( false, false ) )
                {
                    if( false == LocationNode.Barcode.Barcode.StartsWith( "LS" ) )
                    {
                        LocationNode.Barcode.setBarcodeValue( OverrideExisting: true );
                    }
                    if( LocationNode.Barcode.Barcode.Length < 8 )
                    {
                        string NewBarcodeValue = "LS" + CswTools.PadInt( CswConvert.ToInt32( LocationNode.Barcode.SequenceNumber ), 6 );
                        LocationNode.Barcode.setBarcodeValueOverride( NewBarcodeValue, false );
                    }
                    LocationNode.postChanges( false );
                } // foreach( CswNbtObjClassLocation LocationNode in LocationNT.getNodes( false, false ) )
            } // foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )

        } //Update()

    }//class CswUpdateSchema_01V_Case28399B

}//namespace ChemSW.Nbt.Schema