using System;
using System.Collections.Generic;
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
            List<CswPrimaryKey> MaterialPKs = new List<CswPrimaryKey>();

            //Rename all existing material demo data
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MatOC in MaterialPS.getObjectClasses() )
            {
                foreach( CswNbtPropertySetMaterial DemoMaterial in MatOC.getNodes( false, false ) )
                {
                    if( DemoMaterial.IsDemo )
                    {
                        MaterialPKs.Add( DemoMaterial.NodeId );
                        DemoMaterial.TradeName.Text = DemoMaterial.TradeName.Text.Replace( "Default", "(demo)" );
                        DemoMaterial.ApprovedForReceiving.Checked = CswEnumTristate.True;
                        if( DemoMaterial.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                        {
                            DemoMaterial.Node.Properties[CswNbtObjClassChemical.PropertyName.PhysicalState].AsList.Value = "liquid";
                        }
                        DemoMaterial.postChanges( false );
                    }
                }
            }

            //Fix existing demo material sizes
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            foreach( CswNbtObjClassSize DemoSize in SizeOC.getNodes( false, false ) )
            {
                if( DemoSize.IsDemo && MaterialPKs.Contains( DemoSize.Material.RelatedNodeId ) )
                {
                    DemoSize.InitialQuantity.Quantity = 10;
                    CswNbtPropertySetMaterial DemoMaterial = _CswNbtSchemaModTrnsctn.Nodes[DemoSize.Material.RelatedNodeId];
                    if( DemoMaterial.ObjectClass.ObjectClass == CswEnumNbtObjectClass.NonChemicalClass )
                    {
                        CswNbtObjClassUnitOfMeasure CasesUnit = _getUnit( "Unit (Each)", "Cases" );
                        DemoSize.InitialQuantity.UnitId = CasesUnit.NodeId;
                        DemoSize.InitialQuantity.View.Root.ChildRelationships.Clear();
                        DemoSize.InitialQuantity.View.AddViewRelationship( CasesUnit.NodeType, true );
                        DemoSize.InitialQuantity.View.save();
                        DemoSize.postChanges( false );
                    }
                    else if( DemoMaterial.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                    {
                        CswNbtObjClassUnitOfMeasure KGUnit = _getUnit( "Unit (Weight)", "kg" );
                        CswNbtObjClassUnitOfMeasure LitersUnit = _getUnit( "Unit (Volume)", "Liters" );
                        DemoSize.InitialQuantity.UnitId = LitersUnit.NodeId;
                        DemoSize.InitialQuantity.View.Root.ChildRelationships.Clear();
                        DemoSize.InitialQuantity.View.AddViewRelationship( KGUnit.NodeType, true );
                        DemoSize.InitialQuantity.View.AddViewRelationship( LitersUnit.NodeType, true );
                        DemoSize.InitialQuantity.View.save();
                    }
                    DemoSize.postChanges( false );
                }
            }
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
    }//class CswUpdateSchema_02B_Case28690E
}//namespace ChemSW.Nbt.Schema