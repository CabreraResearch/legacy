using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28690
    /// </summary>
    public class CswUpdateSchema_02B_Case28690E : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28690; }
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            //Delete all existing material demo data (incl. sizes)
            foreach( CswNbtMetaDataObjectClass MatOC in MaterialPS.getObjectClasses() )
            {
                foreach( CswNbtNode DemoMaterial in MatOC.getNodes( false, false ) )
                {
                    if( DemoMaterial.IsDemo )
                    {
                        DemoMaterial.delete( true );
                    }
                }
            }

            #region Recreate Default Materials

            CswNbtObjClassVendor DemoVendor = null;
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            CswNbtMetaDataNodeType VendorNT = VendorOC.FirstNodeType;
            foreach( CswNbtObjClassVendor VendorNode in VendorNT.getNodes( false, false ) )
            {
                DemoVendor = VendorNode;
                if( VendorNode.IsDemo )
                {
                    DemoVendor = VendorNode;
                    break;
                }
            }

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtObjClassChemical MaterialNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                MaterialNode.IsDemo = true;
                MaterialNode.TradeName.Text = "Temp " + ChemicalNT.NodeTypeName + " (demo)";
                MaterialNode.PartNumber.Text = "658-35AB";
                MaterialNode.Supplier.RelatedNodeId = DemoVendor.NodeId;
                MaterialNode.ApprovedForReceiving.Checked = CswEnumTristate.True;

                _setMultiListValue( MaterialNode.Node, "NFG (liquified)", "Hazard Classes" );
                _setMultiListValue( MaterialNode.Node, "EHS", "Special Flags" );
                
                MaterialNode.CasNo.Text = "7732-18-5";
                MaterialNode.NFPA.Blue = "0";
                MaterialNode.NFPA.Red = "0";
                MaterialNode.NFPA.Yellow = "0";
                MaterialNode.MeltingPoint.Text = "0 C";
                MaterialNode.BoilingPoint.Text = "100 C";
                MaterialNode.IsTierII.Checked = CswEnumTristate.True;
                CswNbtObjClassUnitOfMeasure YearsUnit = _getUnit( "Unit (Time)", "Years" );
                if( null != YearsUnit )
                {
                    MaterialNode.ExpirationInterval.Quantity = 1;
                    MaterialNode.ExpirationInterval.UnitId = YearsUnit.NodeId;
                }
                MaterialNode.postChanges( false );

                CswNbtObjClassUnitOfMeasure LitersUnit = _getUnit( "Unit (Volume)", "Liters" );
                _createSizeNodeFor( MaterialNode, LitersUnit );
            }

            CswNbtMetaDataObjectClass NonChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass );
            foreach( CswNbtMetaDataNodeType NonChemicalNT in NonChemicalOC.getNodeTypes() )
            {
                CswNbtObjClassNonChemical MaterialNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NonChemicalNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                MaterialNode.IsDemo = true;
                MaterialNode.TradeName.Text = "Temp " + NonChemicalNT.NodeTypeName + " (demo)";
                MaterialNode.PartNumber.Text = "658-35AB";
                MaterialNode.Supplier.RelatedNodeId = DemoVendor.NodeId;
                MaterialNode.ApprovedForReceiving.Checked = CswEnumTristate.True;
                MaterialNode.postChanges( false );

                CswNbtObjClassUnitOfMeasure CasesUnit = _getUnit( "Unit (Each)", "Cases" );
                _createSizeNodeFor( MaterialNode, CasesUnit );
            }
            #endregion Recreate Default Materials
        } // update()

        private CswNbtObjClassUnitOfMeasure _getUnit( String UnitNTName, String UnitName )
        {
            CswNbtObjClassUnitOfMeasure UnitNode = null;
            CswNbtMetaDataNodeType UnitNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( UnitNTName );
            if( null != UnitNT )
            {
                foreach( CswNbtObjClassUnitOfMeasure Unit in UnitNT.getNodes( false, false ) )
                {
                    if( Unit.Name.Text == UnitName )
                    {
                        UnitNode = Unit;
                        break;
                    }
                }
            }
            return UnitNode;
        }

        private void _setMultiListValue( CswNbtNode Node, String MultiListValue, String MultiListPropName )
        {
            CswCommaDelimitedString MultiListString = new CswCommaDelimitedString();
            MultiListString.FromString( MultiListValue );
            CswNbtMetaDataNodeTypeProp MultiListNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( Node.NodeTypeId, MultiListPropName );
            Node.Properties[MultiListNTP].AsMultiList.Value = MultiListString;
        }

        private void _createSizeNodeFor( CswNbtPropertySetMaterial MaterialNode, CswNbtObjClassUnitOfMeasure Unit )
        {
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            CswNbtMetaDataNodeType SizeNT = SizeOC.FirstNodeType;
            CswNbtObjClassSize SizeNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SizeNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
            SizeNode.IsDemo = true;
            SizeNode.Material.RelatedNodeId = MaterialNode.NodeId;
            SizeNode.InitialQuantity.Quantity = 10;
            SizeNode.InitialQuantity.UnitId = Unit.NodeId;
            SizeNode.CatalogNo.Text = "NE-H5/3";
            SizeNode.UnitCount.Value = 1;
            SizeNode.postChanges( false );
        }
    }//class CswUpdateSchema_02B_Case28690E
}//namespace ChemSW.Nbt.Schema