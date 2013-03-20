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
    public class CswUpdateSchema_01V_Case28399 : CswUpdateSchemaTo
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
            // Make sure all Location nodetypes are using the Location sequence

            CswSequenceName LocationSeqName = new CswSequenceName( "Locations" );
            Int32 LocationSeqId;
            if(_CswNbtSchemaModTrnsctn.doesSequenceExist(LocationSeqName ))
            {
                DataTable SeqTable = _CswNbtSchemaModTrnsctn.getSequence( LocationSeqName );
                LocationSeqId = CswConvert.ToInt32( SeqTable.Rows[0]["sequenceid"] );
            }
            else
            {
                LocationSeqId = _CswNbtSchemaModTrnsctn.makeSequence( LocationSeqName, "LS", string.Empty, 6, 1 );
            }

            if(Int32.MinValue != LocationSeqId )
            {
                CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
                {
                    // Set sequence id
                    CswNbtMetaDataNodeTypeProp BarcodeNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Barcode );
                    BarcodeNTP.setSequence( LocationSeqId );
                
                } // foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            } // if(Int32.MinValue != LocationSeqId )

        } //Update()

    }//class CswUpdateSchema_01V_Case28399

}//namespace ChemSW.Nbt.Schema