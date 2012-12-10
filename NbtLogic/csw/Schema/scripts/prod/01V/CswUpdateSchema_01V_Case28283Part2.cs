using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28283
    /// </summary>
    public class CswUpdateSchema_01V_Case28283Part2 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28283; }
        }

        private CswPrimaryKey SetNodeId, PoundsId, GallonsId, CubicFeetId, CurieId, MilliCurieId;
        private CswNbtMetaDataNodeType FireClassExemptAmountNT;
        private CswNbtMetaDataObjectClass FireClassExemptAmountOC;

        public override void update()
        {
            #region FireClassExemptAmountSetNT

            CswNbtMetaDataObjectClass FireClassExemptAmountSetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountSetClass );
            if( null != FireClassExemptAmountSetOC )
            {
                CswNbtMetaDataNodeType FireClassExemptAmountSetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Class Exempt Amount Set" );
                if( null == FireClassExemptAmountSetNT )
                {
                    //Create new NodeType
                    FireClassExemptAmountSetNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( FireClassExemptAmountSetOC.ObjectClassId, "Fire Class Exempt Amount Set", "Materials" );
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, FireClassExemptAmountSetNT.NodeTypeId );
                    FireClassExemptAmountSetNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassFireClassExemptAmountSet.PropertyName.SetName ) );                    
                }
                //Create Default Set
                CswNbtObjClassFireClassExemptAmountSet FireClassExemptAmountSetNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( FireClassExemptAmountSetNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                FireClassExemptAmountSetNode.SetName.Text = "Default";
                FireClassExemptAmountSetNode.postChanges( false );
                SetNodeId = FireClassExemptAmountSetNode.NodeId;
            }

            #endregion FireClassExemptAmountSetNT

            #region FireClassExemptAmountNT

            FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass );
            if( null != FireClassExemptAmountOC )
            {
                FireClassExemptAmountNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Fire Class Exempt Amount" );
                if( null == FireClassExemptAmountNT )
                {
                    //Create new NodeType
                    FireClassExemptAmountNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType(FireClassExemptAmountOC.ObjectClassId, "Fire Class Exempt Amount", "Materials");
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, FireClassExemptAmountNT.NodeTypeId );
                    FireClassExemptAmountNT.setNameTemplateText(
                        CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassFireClassExemptAmount.PropertyName.SetName )
                        + " "
                        + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassFireClassExemptAmount.PropertyName.FireHazardClassType ) );
                }

                #region NodeType Edit Layout

                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.SetName, 1, 1, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.SortOrder, 1, 2, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.FireHazardClassType, 1, 3, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.HazardType, 1, 4, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.Material, 1, 5, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptAmount, 1, 6, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptFootnotes, 1, 7, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptAmount, 2, 6, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptFootnotes, 2, 7, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptAmount, 3, 6, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptFootnotes, 3, 7, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptAmount, 1, 8, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptFootnotes, 1, 9, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptAmount, 2, 8, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptFootnotes, 2, 9, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptAmount, 3, 8, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptFootnotes, 3, 9, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptAmount, 1, 10, "Open" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptFootnotes, 1, 11, "Open" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptAmount, 2, 10, "Open" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptFootnotes, 2, 11, "Open" );

                #endregion NodeType Edit Layout

                #region Quantity Views

                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptAmount, false );
                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptAmount, true );
                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptAmount, true );
                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptAmount, false );
                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptAmount, true );
                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptAmount, true );
                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptAmount, false );
                _setQuantityViewId( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptAmount, true );

                #endregion Quantity Views

                #region FireClassExemptAmount Nodes
                
                if( null != SetNodeId && null != FireClassExemptAmountNT )
                {
                    _setUnitNodeIds();
                    _createFireClassExemptAmountNode( 290.1, "Aero-1", "Physical", "Aerosols" );
                    _createFireClassExemptAmountNode( 290.2, "Aero-2", "Physical", "Aerosols" );
                    _createFireClassExemptAmountNode( 290.3, "Aero-3", "Physical", "Aerosols" );
                    _createFireClassExemptAmountNode( 301, "Carc", "Health", "Carcinogenic",
                        SSEF: "9", SLEF: "9", SGEF: "6,9", CSEF: "9", CLEF: "9", CGEF: "6,9", OSEA: "10000", OSEF: "10", OLEF: "10");
                    _createFireClassExemptAmountNode( 211.1, "CF/D (balled)", "Physical", "Combustible Fiber/Dust",
                        SSEA: "-1000", CSEA: "-100", OSEA: "-20" );
                    _createFireClassExemptAmountNode( 211.2, "CF/D (loose)", "Physical", "Combustible Fiber/Dust",
                        SSEA: "-100", CSEA: "-1000", OSEA: "-200" );
                    _createFireClassExemptAmountNode( 210.1, "CL-II", "Physical", "Combustible Liquids",
                        SLEA: "240", SLEF: "10", CLEA: "240", OLEA: "60" );
                    _createFireClassExemptAmountNode( 210.2, "CL-IIIA", "Physical", "Combustible Liquids",
                        SLEA: "660", CLEA: "660", OLEA: "160" );
                    _createFireClassExemptAmountNode( 210.3, "CL-IIIB", "Physical", "Combustible Liquids",
                        SLEF: "10,11", CLEF: "11", OLEF: "11" );
                    _createFireClassExemptAmountNode( 301.1, "Corr", "Health", "Corrosives",
                        SSEA: "10000", SLEA: "1000", SGEA: "1620", SGEF: "6", CSEA: "10000", CLEA: "1000", CGEA: "1620", CGEF: "6", OSEA: "2000", OSEF: "5", OLEA: "200", OLEF: "5" );
                    _createFireClassExemptAmountNode( 212.1, "CRY-FG", "Physical", "Cryogenic",
                        SLEA: "45", CLEA: "45", OLEA: "10" );
                    _createFireClassExemptAmountNode( 212.3, "CRY-OXY", "Physical", "Cryogenic",
                        SLEA: "45", CLEA: "45", OLEA: "10" );
                    _createFireClassExemptAmountNode( 213, "Exp", "Physical", "Explosive",
                        SSEA: "1", SSEF: "10,13", SLEA: "-1", SLEF: "10,13", CSEA: "0.25", CSEF: "12", CLEA: "-0.25", CLEF: "12", OSEA: "0.25", OSEF: "12", OLEA: "-0.25", OLEF: "12" );
                    _createFireClassExemptAmountNode( 214.1, "FG (gaseous)", "Physical", "Flammable Gas",
                        SGEA: "1500", SGEF: "6,10", CGEA: "1500", CGEF: "6,10" );
                    _createFireClassExemptAmountNode( 214.2, "FG (liquified)", "Physical", "Flammable Gas",
                        SLEA: "30", SLEF: "6,10", CLEA: "30", CLEF: "6,10" );
                    _createFireClassExemptAmountNode( 214.3, "FL-1A", "Physical", "Flammable Liquids",
                        SLEA: "60", SLEF: "10", CLEA: "60", OLEA: "20" );
                    _createFireClassExemptAmountNode( 214.4, "FL-1B", "Physical", "Flammable Liquids",
                        SLEA: "120", SLEF: "10", CLEA: "120", OLEA: "30" );
                    _createFireClassExemptAmountNode( 214.5, "FL-1C", "Physical", "Flammable Liquids",
                        SLEA: "180", SLEF: "10", CLEA: "180", OLEA: "40" );
                    _createFireClassExemptAmountNode( 214.6, "FL-Comb", "Physical", "Flammable Liquids",
                        SLEA: "240", SLEF: "10", CLEA: "240", OLEA: "60" );
                    _createFireClassExemptAmountNode( 214, "FS", "Physical", "Flammable Solid",
                        SSEA: "250", SSEF: "6,10", SLEF: "6,10", CSEA: "125", CSEF: "6,10", OSEA: "125", OSEF: "6,10" );
                    _createFireClassExemptAmountNode( 301.3, "H.T.", "Health", "Highly Toxic",
                        SSEA: "20", SLEA: "-20", SGEA: "40", SGEF: "12", CSEA: "20", CLEA: "-20", CGEA: "40", CGEF: "12", OSEA: "6", OSEF: "5", OLEA: "-6", OLEF: "5" );
                    _createFireClassExemptAmountNode( 302, "Irr", "Health", "Irritant",
                        SGEF: "6,9", CGEF: "9", OSEF: "9", OLEF: "9" );
                    _createFireClassExemptAmountNode( 307.5, "OHH", "Health", "Other Health Hazard",
                        SGEF: "6,9", CGEF: "6,9", OSEF: "9", OLEF: "9" );
                    _createFireClassExemptAmountNode( 217.6, "Oxy1", "Physical", "Oxidizer",
                        SSEA: "8000", SSEF: "6,10", SLEA: "-8000", SLEF: "10,12", CSEA: "8000", CSEF: "6", CLEA: "-8000", OSEA: "2000", OSEF: "6", OLEA: "-2000", OLEF: "6" );
                    _createFireClassExemptAmountNode( 217.5, "Oxy2", "Physical", "Oxidizer",
                        SSEA: "500", SSEF: "6,10", SLEA: "-500", SLEF: "6,10", CSEA: "500", CSEF: "6", CLEA: "-500", OSEA: "100", OSEF: "6", OLEA: "-100", OLEF: "6" );
                    _createFireClassExemptAmountNode( 217.4, "Oxy3", "Physical", "Oxidizer",
                        SSEA: "20", SSEF: "6,10", SLEA: "-20", SLEF: "6,10", CSEA: "4", CSEF: "6", CLEA: "-4", OSEA: "4", OSEF: "6", OLEA: "-4", OLEF: "6" );
                    _createFireClassExemptAmountNode( 217.3, "Oxy4", "Physical", "Oxidizer",
                        SSEA: "1", SSEF: "10,12", SLEA: "-1", SLEF: "6,10", CSEA: "0.25", CSEF: "12", CLEA: "-0.25", OSEA: "0.25", OSEF: "12", OLEA: "-0.25", OLEF: "12" );
                    _createFireClassExemptAmountNode( 217.7, "Oxy-Gas", "Physical", "Oxidizer - Gas",
                        SGEA: "3000", CGEA: "3000" );
                    _createFireClassExemptAmountNode( 217.8, "Oxy-Gas (liquid)", "Physical", "Oxidizer - Gas",
                        SLEA: "30", CLEA: "30" );
                    _createFireClassExemptAmountNode( 216.1, "Perox-Det", "Physical", "Organic Peroxide",
                        SSEA: "1", SSEF: "10,12", SLEA: "-1", SLEF: "12,12", CSEA: "0.25", CSEF: "12", CLEA: "-0.25", CLEF: "12", OSEA: "0.25", OSEF: "12", OLEA: "-0.25", OLEF: "12" );
                    _createFireClassExemptAmountNode( 216.2, "Perox-I", "Physical", "Organic Peroxide",
                        SSEA: "10", SSEF: "6,10", SLEA: "-10", SLEF: "6,10", CSEA: "2", CSEF: "6", CLEA: "-2", CLEF: "6", OSEA: "2", OSEF: "6", OLEA: "-2", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.3, "Perox-II", "Physical", "Organic Peroxide",
                        SSEA: "100", SSEF: "6,10", SLEA: "-100", SLEF: "6,10", CSEA: "100", CSEF: "6", CLEA: "-100", CLEF: "6", OSEA: "20", OSEF: "6", OLEA: "-20", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.4, "Perox-III", "Physical", "Organic Peroxide",
                        SSEA: "250", SSEF: "6,10", SLEA: "-250", SLEF: "6,10", CSEA: "250", CSEF: "6", CLEA: "-250", CLEF: "6", OSEA: "50", OSEF: "6", OLEA: "-50", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.5, "Perox-IV", "Physical", "Organic Peroxide",
                        SSEA: "1000", SSEF: "6,10", SLEA: "-1000", SLEF: "6,10", CSEA: "1000", CSEF: "6", CLEA: "-1000", CLEF: "6", OSEA: "200", OSEF: "6", OLEA: "-200", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.6, "Perox-V", "Physical", "Organic Peroxide" );
                    _createFireClassExemptAmountNode( 218, "Pyro", "Physical", "Pyrophoric",
                        SSEA: "4", SSEF: "10,12", SLEA: "-4", SLEF: "10,12", SGEA: "50", SGEF: "10,12", CSEA: "1", CSEF: "12", CLEA: "-1", CLEF: "12", CGEA: "10", OSEA: "0", OLEA: "0" );
                    _createFireClassExemptAmountNode( 304, "RAD-Alpha", "Health", "Radioactive",
                        SSEA: "2", SLEA: "2", SGEA: "0.2", SGEF: "6", CSEA: "2", CLEA: "2", CGEA: "0.2", OSEA: "1", OLEA: "1", isRadioactive: true );
                    _createFireClassExemptAmountNode( 305, "RAD-Beta", "Health", "Radioactive",
                        SSEA: "200", SLEA: "200", SGEA: "20", SGEF: "6", CSEA: "200", CLEA: "200", CGEA: "20", OSEA: "100", OLEA: "100", isRadioactive: true );
                    _createFireClassExemptAmountNode( 306, "RAD-Gamma", "Health", "Radioactive",
                        SSEA: "14", SLEA: "14", SGEA: "1.4", SGEF: "6", CSEA: "14", CLEA: "14", CGEA: "1.4", OSEA: "0.1", OLEA: "0.1", isRadioactive: true );
                    _createFireClassExemptAmountNode( 307, "Sens", "Health", "Sensitizer",
                        SGEF: "6,9", CGEF: "6,9", OSEF: "9", OLEF: "9" );
                    _createFireClassExemptAmountNode( 308, "Tox", "Health", "Toxic",
                        SSEA: "1000", SLEA: "-1000", SGEA: "1620", SGEF: "6", CSEA: "1000", CLEA: "-1000", CGEA: "1620", CGEF: "12", OSEA: "250", OSEF: "5", OLEA: "-250", OLEF: "5" );
                    _createFireClassExemptAmountNode( 219.4, "UR-1", "Physical", "Unstable Reactive",
                        SGEA: "1500", SGEF: "6,10" );
                    _createFireClassExemptAmountNode( 219.3, "UR-2", "Physical", "Unstable Reactive",
                        SSEA: "100", SSEF: "6,10", SLEA: "-100", SLEF: "6,10", SGEA: "500", SGEF: "6,10", CSEA: "100", CSEF: "6", CLEA: "-100", CLEF: "6", CGEA: "500", CGEF: "6,10", OSEA: "20", OSEF: "6", OLEA: "-20", OLEF: "6" );
                    _createFireClassExemptAmountNode( 219.2, "UR-3", "Physical", "Unstable Reactive",
                        SSEA: "10", SSEF: "6,10", SLEA: "-10", SLEF: "6,10", SGEA: "100", SGEF: "6,10", CSEA: "2", CSEF: "6", CLEA: "-2", CLEF: "6", CGEA: "20", CGEF: "6,10", OSEA: "2", OSEF: "6", OLEA: "-2", OLEF: "6" );
                    _createFireClassExemptAmountNode( 219.1, "UR-4", "Physical", "Unstable Reactive",
                        SSEA: "1", SSEF: "10,12", SLEA: "-1", SLEF: "10,13", SGEA: "10", SGEF: "10,12", CSEA: "0.25", CSEF: "12", CLEA: "-0.25", CLEF: "12", CGEA: "2", CGEF: "10,12", OSEA: "0.25", OSEF: "12", OLEA: "-0.25", OLEF: "12" );
                    _createFireClassExemptAmountNode( 220.3, "WR-1", "Physical", "Water Reactive",
                        SSEF: "10,11", SLEF: "10,11", CSEF: "11", OSEF: "11", OLEF: "11" );
                    _createFireClassExemptAmountNode( 220.2, "WR-2", "Physical", "Water Reactive",
                        SSEA: "100", SSEF: "6,10", SLEA: "-100", SLEF: "6,10", CSEA: "100", CSEF: "6", CLEA: "-100", OSEA: "20", OSEF: "6", OLEA: "-20", OLEF: "6" );
                    _createFireClassExemptAmountNode( 220.1, "WR-3", "Physical", "Water Reactive",
                        SSEA: "10", SSEF: "6,10", SLEA: "-10", SLEF: "6,10", CSEA: "10", CSEF: "6", CLEA: "-10", OSEA: "2", OSEF: "6", OLEA: "-2", OLEF: "6" );
                }
                #endregion FireClassExemptAmount Nodes
            }

            #endregion FireClassExemptAmountNT
        }

        #region Private Helper Functions

        private void _createFireClassExemptAmountNode( double SortOrder, string FireHazardClassType, string HazardType, string Material,
            string SSEA = "", string SSEF = "", string SLEA = "", string SLEF = "", string SGEA = "", string SGEF = "",
            string CSEA = "", string CSEF = "", string CLEA = "", string CLEF = "", string CGEA = "", string CGEF = "",
            string OSEA = "", string OSEF = "", string OLEA = "", string OLEF = "", bool isRadioactive = false)
        {
            CswPrimaryKey RadioactiveId = FireHazardClassType == "RAD-Alpha" ? MilliCurieId : CurieId;
            CswPrimaryKey SolidId = isRadioactive ? RadioactiveId : PoundsId;
            CswPrimaryKey LiquidId = isRadioactive ? RadioactiveId : GallonsId;
            CswPrimaryKey GasId = isRadioactive ? RadioactiveId : CubicFeetId;
            CswNbtObjClassFireClassExemptAmount FireClassExemptAmountNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( FireClassExemptAmountNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            FireClassExemptAmountNode.SetName.RelatedNodeId = SetNodeId;
            FireClassExemptAmountNode.SortOrder.Value = SortOrder;
            FireClassExemptAmountNode.FireHazardClassType.Value = FireHazardClassType;
            FireClassExemptAmountNode.HazardType.Value = HazardType;
            FireClassExemptAmountNode.Material.Text = Material;
            if( CswTools.IsDouble( SSEA ) )
            {
                FireClassExemptAmountNode.StorageSolidExemptAmount.Quantity = CswConvert.ToDouble( SSEA );
                FireClassExemptAmountNode.StorageSolidExemptAmount.UnitId = SolidId;
            }
            FireClassExemptAmountNode.StorageSolidExemptFootnotes.Text = SSEF;
            if( CswTools.IsDouble( SLEA ) )
            {
                FireClassExemptAmountNode.StorageLiquidExemptAmount.Quantity = CswConvert.ToDouble( SLEA );
                FireClassExemptAmountNode.StorageLiquidExemptAmount.UnitId = LiquidId;
            }
            FireClassExemptAmountNode.StorageLiquidExemptFootnotes.Text = SLEF;
            if( CswTools.IsDouble( SGEA ) )
            {
                FireClassExemptAmountNode.StorageGasExemptAmount.Quantity = CswConvert.ToDouble( SGEA );
                FireClassExemptAmountNode.StorageGasExemptAmount.UnitId = GasId;
            }            
            FireClassExemptAmountNode.StorageGasExemptFootnotes.Text = SGEF;
            if( CswTools.IsDouble( CSEA ) )
            {
                FireClassExemptAmountNode.ClosedSolidExemptAmount.Quantity = CswConvert.ToDouble( CSEA );
                FireClassExemptAmountNode.ClosedSolidExemptAmount.UnitId = SolidId;
            }
            FireClassExemptAmountNode.ClosedSolidExemptFootnotes.Text = CSEF;
            if( CswTools.IsDouble( CLEA ) )
            {
                FireClassExemptAmountNode.ClosedLiquidExemptAmount.Quantity = CswConvert.ToDouble( CLEA );
                FireClassExemptAmountNode.ClosedLiquidExemptAmount.UnitId = LiquidId;
            }
            FireClassExemptAmountNode.ClosedLiquidExemptFootnotes.Text = CLEF;
            if( CswTools.IsDouble( CGEA ) )
            {
                FireClassExemptAmountNode.ClosedGasExemptAmount.Quantity = CswConvert.ToDouble( CGEA );
                FireClassExemptAmountNode.ClosedGasExemptAmount.UnitId = GasId;
            } 
            FireClassExemptAmountNode.ClosedGasExemptFootnotes.Text = CGEF;
            if( CswTools.IsDouble( OSEA ) )
            {
                FireClassExemptAmountNode.OpenSolidExemptAmount.Quantity = CswConvert.ToDouble( OSEA );
                FireClassExemptAmountNode.OpenSolidExemptAmount.UnitId = SolidId;
            }
            FireClassExemptAmountNode.OpenSolidExemptFootnotes.Text = OSEF;
            if( CswTools.IsDouble( OLEA ) )
            {
                FireClassExemptAmountNode.OpenLiquidExemptAmount.Quantity = CswConvert.ToDouble( OLEA );
                FireClassExemptAmountNode.OpenLiquidExemptAmount.UnitId = LiquidId;
            }
            FireClassExemptAmountNode.OpenLiquidExemptFootnotes.Text = OLEF;
            FireClassExemptAmountNode.postChanges( false );
        }

        private void _setUnitNodeIds()
        {
            CswNbtMetaDataObjectClass UoMClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            foreach( CswNbtObjClassUnitOfMeasure UoMNode in UoMClass.getNodes( false, true ) )
            {
                if( UoMNode.Name.Text == "lb" )
                {
                    PoundsId = UoMNode.NodeId;
                }
                else if( UoMNode.Name.Text == "gal" )
                {
                    GallonsId = UoMNode.NodeId;
                }
                else if( UoMNode.Name.Text == "cu.ft." )
                {
                    CubicFeetId = UoMNode.NodeId;
                }
                else if( UoMNode.Name.Text == "Ci" )
                {
                    CurieId = UoMNode.NodeId;
                }
                else if( UoMNode.Name.Text == "mCi" )
                {
                    MilliCurieId = UoMNode.NodeId;
                }
            }
        }

        private void _setEditLayout( string PropName, int ColNum, int RowNum, string TabGroup = "", bool RemoveFromAdd = true )
        {
            CswNbtMetaDataNodeTypeProp FireClassExemptAmountNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( FireClassExemptAmountNT.NodeTypeId, PropName );
            if( RemoveFromAdd )
                FireClassExemptAmountNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            FireClassExemptAmountNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FireClassExemptAmountNT.getFirstNodeTypeTab().TabId, RowNum, ColNum, TabGroup );
        }

        private void _setQuantityViewId( string PropName, bool UseVolume )
        {
            CswNbtMetaDataNodeTypeProp QuantityProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( FireClassExemptAmountNT.NodeTypeId, PropName );
            CswNbtView QuantityView = _CswNbtSchemaModTrnsctn.makeNewView( QuantityProp.PropName + "_QtyView", NbtViewVisibility.Property );
            CswNbtMetaDataNodeType WeightUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Weight)" );
            if( null != WeightUnitNodeType )
            {
                QuantityView.AddViewRelationship( WeightUnitNodeType, true );
            }
            if( UseVolume )
            {
                CswNbtMetaDataNodeType VolumeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Volume)" );
                if( null != VolumeUnitNodeType )
                {
                    QuantityView.AddViewRelationship( VolumeUnitNodeType, true );
                }
            }
            CswNbtMetaDataNodeType RadiationUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Radiation)" );
            if( null != RadiationUnitNodeType )
            {
                QuantityView.AddViewRelationship( RadiationUnitNodeType, true );
            }
            QuantityView.save();
            QuantityProp.ViewId = QuantityView.ViewId;
        }

        #endregion Private Helper Functions

    }//class CswUpdateSchemaCase_01V_28283

}//namespace ChemSW.Nbt.Schema