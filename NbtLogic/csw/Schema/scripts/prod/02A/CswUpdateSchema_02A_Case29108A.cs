using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using System.Data;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29108A
    /// </summary>
    public class CswUpdateSchema_02A_Case29108A : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29108; }
        }

        public override void update()
        {

            CswSequenceName assemblySequenceName = new CswSequenceName( "Assembly Barcode" );
            int sequenceId;
            if( false == _CswNbtSchemaModTrnsctn.doesSequenceExist( assemblySequenceName ) )
            {
                sequenceId = _CswNbtSchemaModTrnsctn.makeSequence( assemblySequenceName, "AS", "", 6, 0 );
            }
            else
            {
                sequenceId = CswConvert.ToInt32( _CswNbtSchemaModTrnsctn.getSequence( assemblySequenceName ).Rows[0]["sequenceid"] );
            }

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass );
            foreach( CswNbtMetaDataNodeType assemblyNT in assemblyOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp barcodeNTP = assemblyNT.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipmentAssembly.PropertyName.Barcode );
                if( Int32.MinValue == barcodeNTP.SequenceId )
                {
                    barcodeNTP.setSequence( sequenceId );
                }

                barcodeNTP.removeFromAllLayouts();
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                        assemblyNT.NodeTypeId,
                        barcodeNTP,
                        true,
                        TabId: assemblyNT.getIdentityTab().TabId );
            }


        } // update()

    }//class CswUpdateSchema_02A_Case29108A

}//namespace ChemSW.Nbt.Schema