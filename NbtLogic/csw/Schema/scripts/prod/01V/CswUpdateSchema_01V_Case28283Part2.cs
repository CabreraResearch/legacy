using System;
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

        private CswPrimaryKey SetNodeId;
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
                        CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassFireClassExemptAmount.PropertyName.HazardCategory )
                        + " "
                        + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassFireClassExemptAmount.PropertyName.Class ) );
                }

                CswNbtMetaDataNodeTypeProp SetNameNTP = FireClassExemptAmountNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.SetName );
                SetNameNTP.setIsCompoundUnique( true );
                CswNbtMetaDataNodeTypeProp HazardClassNTP = FireClassExemptAmountNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.HazardClass );
                HazardClassNTP.setIsCompoundUnique( true );
                CswNbtMetaDataNodeTypeProp ClassNTP = FireClassExemptAmountNT.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.Class );
                ClassNTP.removeFromAllLayouts();

                #region NodeType Edit Layout

                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.SetName, 1, 1, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.HazardCategory, 1, 2, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.HazardClass, 1, 3, RemoveFromAdd: false );//TODO - make unique on the node type
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.HazardType, 1, 4, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.CategoryFootnotes, 1, 5, RemoveFromAdd: false );                
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.SortOrder, 1, 6, RemoveFromAdd: false );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptAmount, 1, 7, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptFootnotes, 1, 8, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptAmount, 2, 7, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptFootnotes, 2, 8, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptAmount, 3, 7, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptFootnotes, 3, 8, "Storage" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptAmount, 1, 9, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptFootnotes, 1, 10, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptAmount, 2, 9, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptFootnotes, 2, 10, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptAmount, 3, 9, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptFootnotes, 3, 10, "Closed" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptAmount, 1, 11, "Open" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptFootnotes, 1, 12, "Open" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptAmount, 2, 11, "Open" );
                _setEditLayout( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptFootnotes, 2, 12, "Open" );

                #endregion NodeType Edit Layout

                #region FireClassExemptAmount Nodes
                
                if( null != SetNodeId && null != FireClassExemptAmountNT )
                {
                    _createFireClassExemptAmountNode( 290.1, "Aero-1", "Physical", "Aerosols", Class: "1" );
                    _createFireClassExemptAmountNode( 290.2, "Aero-2", "Physical", "Aerosols", Class: "2" );
                    _createFireClassExemptAmountNode( 290.3, "Aero-3", "Physical", "Aerosols", Class: "3" );
                    _createFireClassExemptAmountNode( 301, "Carc", "Health", "Carcinogenic", "7,8",
                        SSEA: "NL", SSEF: "9", SLEA: "NL", SLEF: "9", SGEA: "NL", SGEF: "6,9", CSEA: "NL", CSEF: "9", CLEA: "NL", CLEF: "9", CGEA: "NL", CGEF: "6,9", OSEA: "10000", OSEF: "10", OLEA: "NL", OLEF: "10" );
                    _createFireClassExemptAmountNode( 211.1, "CF/D (balled)", "Physical", "Combustible Fiber/Dust", Class: "(baled)",
                        SSEA: "(1000)", CSEA: "(100)", OSEA: "(20)" );
                    _createFireClassExemptAmountNode( 211.2, "CF/D (loose)", "Physical", "Combustible Fiber/Dust", Class: "(loose)",
                        SSEA: "(100)", CSEA: "(1000)", OSEA: "(200)" );
                    _createFireClassExemptAmountNode( 210.1, "CL-II", "Physical", "Combustible Liquids", "4,5,6,7,8,9", "II",
                        SLEA: "240", SLEF: "10", CLEA: "240", OLEA: "60" );
                    _createFireClassExemptAmountNode( 210.2, "CL-IIIA", "Physical", "Combustible Liquids", "4,5,6,7,8,9", "III-A",
                        SLEA: "660", CLEA: "660", OLEA: "160" );
                    _createFireClassExemptAmountNode( 210.3, "CL-IIIB", "Physical", "Combustible Liquids", "4,5,6,7,8,9", "III-B",
                        SLEA: "NL", SLEF: "10,11", CLEA: "NL", CLEF: "11", OLEA: "NL", OLEF: "11" );
                    _createFireClassExemptAmountNode( 301.1, "Corr", "Health", "Corrosives",
                        SSEA: "10000", SLEA: "1000", SGEA: "1620", SGEF: "6", CSEA: "10000", CLEA: "1000", CGEA: "1620", CGEF: "6", OSEA: "2000", OSEF: "5", OLEA: "200", OLEF: "5" );
                    _createFireClassExemptAmountNode( 212.1, "CRY-FG", "Physical", "Cryogenic", Class: "FG",
                        SLEA: "45", CLEA: "45", OLEA: "10" );
                    _createFireClassExemptAmountNode( 212.3, "CRY-OXY", "Physical", "Cryogenic", Class: "OXY",
                        SLEA: "45", CLEA: "45", OLEA: "10" );
                    _createFireClassExemptAmountNode( 213, "Exp", "Physical", "Explosive",
                        SSEA: "1", SSEF: "10,13", SLEA: "(1)", SLEF: "10,13", CSEA: "0.25", CSEF: "12", CLEA: "(0.25)", CLEF: "12", OSEA: "0.25", OSEF: "12", OLEA: "(0.25)", OLEF: "12" );
                    _createFireClassExemptAmountNode( 214.1, "FG (gaseous)", "Physical", "Flammable Gas", Class: "(gaseous)",
                        SGEA: "1500", SGEF: "6,10", CGEA: "1500", CGEF: "6,10" );
                    _createFireClassExemptAmountNode( 214.2, "FG (liquified)", "Physical", "Flammable Gas", Class: "(liquified)",
                        SLEA: "30", SLEF: "6,10", CLEA: "30", CLEF: "6,10" );
                    _createFireClassExemptAmountNode( 214.3, "FL-1A", "Physical", "Flammable Liquids", Class: "1A",
                        SLEA: "60", SLEF: "10", CLEA: "60", OLEA: "20" );
                    _createFireClassExemptAmountNode( 214.4, "FL-1B", "Physical", "Flammable Liquids", Class: "1B",
                        SLEA: "120", SLEF: "10", CLEA: "120", OLEA: "30" );
                    _createFireClassExemptAmountNode( 214.5, "FL-1C", "Physical", "Flammable Liquids", Class: "1C",
                        SLEA: "180", SLEF: "10", CLEA: "180", OLEA: "40" );
                    _createFireClassExemptAmountNode( 214.6, "FL-Comb", "Physical", "Flammable Liquids", Class: "1A,1B,1C",
                        SLEA: "240", SLEF: "10", CLEA: "240", OLEA: "60" );
                    _createFireClassExemptAmountNode( 214, "FS", "Physical", "Flammable Solid",
                        SSEA: "250", SSEF: "6,10", SLEF: "6,10", CSEA: "125", CSEF: "6,10", OSEA: "125", OSEF: "6,10" );
                    _createFireClassExemptAmountNode( 301.3, "H.T.", "Health", "Highly Toxic", "11",
                        SSEA: "20", SLEA: "(20)", SGEA: "40", SGEF: "12", CSEA: "20", CLEA: "(20)", CGEA: "40", CGEF: "12", OSEA: "6", OSEF: "5", OLEA: "(6)", OLEF: "5" );
                    _createFireClassExemptAmountNode( 302, "Irr", "Health", "Irritant", "7",
                        SSEA: "NL", SLEA: "NL", SGEA: "NL", SGEF: "6,9", CSEA: "NL", CLEA: "NL", CGEA: "NL", CGEF: "9", OSEA: "NL", OSEF: "9", OLEA: "NL", OLEF: "9" );
                    _createFireClassExemptAmountNode( 307.5, "OHH", "Health", "Other Health Hazard", "7",
                        SSEA: "NL", SLEA: "NL", SGEA: "NL", SGEF: "6,9", CSEA: "NL", CLEA: "NL", CGEA: "NL", CGEF: "6,9", OSEA: "NL", OSEF: "9", OLEA: "NL", OLEF: "9" );
                    _createFireClassExemptAmountNode( 217.6, "Oxy-1", "Physical", "Oxidizer", Class: "1",
                        SSEA: "8000", SSEF: "6,10", SLEA: "(8000)", SLEF: "10,12", CSEA: "8000", CSEF: "6", CLEA: "(8000)", OSEA: "2000", OSEF: "6", OLEA: "(2000)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 217.5, "Oxy-2", "Physical", "Oxidizer", Class: "1",
                        SSEA: "500", SSEF: "6,10", SLEA: "(500)", SLEF: "6,10", CSEA: "500", CSEF: "6", CLEA: "(500)", OSEA: "100", OSEF: "6", OLEA: "(100)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 217.4, "Oxy-3", "Physical", "Oxidizer", Class: "1",
                        SSEA: "20", SSEF: "6,10", SLEA: "(20)", SLEF: "6,10", CSEA: "4", CSEF: "6", CLEA: "(4)", OSEA: "4", OSEF: "6", OLEA: "(4)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 217.3, "Oxy-4", "Physical", "Oxidizer", Class: "1",
                        SSEA: "1", SSEF: "10,12", SLEA: "(1)", SLEF: "6,10", CSEA: "0.25", CSEF: "12", CLEA: "(0.25)", OSEA: "0.25", OSEF: "12", OLEA: "(0.25)", OLEF: "12" );
                    _createFireClassExemptAmountNode( 217.7, "Oxy-Gas", "Physical", "Oxidizer - Gas",
                        SGEA: "3000", CGEA: "3000" );
                    _createFireClassExemptAmountNode( 217.8, "Oxy-Gas (liquid)", "Physical", "Oxidizer - Gas", Class: "(liquified)",
                        SLEA: "30", CLEA: "30" );
                    _createFireClassExemptAmountNode( 216.1, "Perox-Det", "Physical", "Organic Peroxide", Class: "Det",
                        SSEA: "1", SSEF: "10,12", SLEA: "(1)", SLEF: "12,12", CSEA: "0.25", CSEF: "12", CLEA: "(0.25)", CLEF: "12", OSEA: "0.25", OSEF: "12", OLEA: "(0.25)", OLEF: "12" );
                    _createFireClassExemptAmountNode( 216.2, "Perox-I", "Physical", "Organic Peroxide", Class: "I",
                        SSEA: "10", SSEF: "6,10", SLEA: "(10)", SLEF: "6,10", CSEA: "2", CSEF: "6", CLEA: "(2)", CLEF: "6", OSEA: "2", OSEF: "6", OLEA: "(2)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.3, "Perox-II", "Physical", "Organic Peroxide", Class: "II",
                        SSEA: "100", SSEF: "6,10", SLEA: "(100)", SLEF: "6,10", CSEA: "100", CSEF: "6", CLEA: "(100)", CLEF: "6", OSEA: "20", OSEF: "6", OLEA: "(20)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.4, "Perox-III", "Physical", "Organic Peroxide", Class: "III",
                        SSEA: "250", SSEF: "6,10", SLEA: "(250)", SLEF: "6,10", CSEA: "250", CSEF: "6", CLEA: "(250)", CLEF: "6", OSEA: "50", OSEF: "6", OLEA: "(50)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.5, "Perox-IV", "Physical", "Organic Peroxide", Class: "IV",
                        SSEA: "1000", SSEF: "6,10", SLEA: "(1000)", SLEF: "6,10", CSEA: "1000", CSEF: "6", CLEA: "(1000)", CLEF: "6", OSEA: "200", OSEF: "6", OLEA: "(200)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 216.6, "Perox-V", "Physical", "Organic Peroxide", Class: "V",
                        SSEA: "NL", SLEA: "NL", CSEA: "NL", CLEA: "NL", OSEA: "NL", OLEA: "NL" );
                    _createFireClassExemptAmountNode( 218, "Pyro", "Physical", "Pyrophoric",
                        SSEA: "4", SSEF: "10,12", SLEA: "(4)", SLEF: "10,12", SGEA: "50", SGEF: "10,12", CSEA: "1", CSEF: "12", CLEA: "(1)", CLEF: "12", CGEA: "10", OSEA: "0", OLEA: "0" );
                    _createFireClassExemptAmountNode( 304, "RAD-Alpha", "Health", "Radioactive", "13", "Alpha",
                        SSEA: "2", SLEA: "2", SGEA: "0.2", SGEF: "6", CSEA: "2", CLEA: "2", CGEA: "0.2", OSEA: "1", OLEA: "1", isRadioactive: true );
                    _createFireClassExemptAmountNode( 305, "RAD-Beta", "Health", "Radioactive", "13", "Beta",
                        SSEA: "200", SLEA: "200", SGEA: "20", SGEF: "6", CSEA: "200", CLEA: "200", CGEA: "20", OSEA: "100", OLEA: "100", isRadioactive: true );
                    _createFireClassExemptAmountNode( 306, "RAD-Gamma", "Health", "Radioactive", "13", "Gamma",
                        SSEA: "14", SLEA: "14", SGEA: "1.4", SGEF: "6", CSEA: "14", CLEA: "14", CGEA: "1.4", OSEA: "0.1", OLEA: "0.1", isRadioactive: true );
                    _createFireClassExemptAmountNode( 307, "Sens", "Health", "Sensitizer", "7",
                        SSEA: "NL", SLEA: "NL", SGEA: "NL", SGEF: "6,9", CSEA: "NL", CLEA: "NL", CGEA: "NL", CGEF: "6,9", OSEA: "NL", OSEF: "9", OLEA: "NL", OLEF: "9" );
                    _createFireClassExemptAmountNode( 308, "Tox", "Health", "Toxic", "11",
                        SSEA: "1000", SLEA: "(1000)", SGEA: "1620", SGEF: "6", CSEA: "1000", CLEA: "(1000)", CGEA: "1620", CGEF: "12", OSEA: "250", OSEF: "5", OLEA: "(250)", OLEF: "5" );
                    _createFireClassExemptAmountNode( 219.4, "UR-1", "Physical", "Unstable Reactive", Class: "1",
                        SSEA: "NL", SLEA: "(NL)", SGEA: "1500", SGEF: "6,10", CSEA: "NL", CLEA: "(NL)", CGEA: "NL", OSEA: "NL", OLEA: "(NL)" );
                    _createFireClassExemptAmountNode( 219.3, "UR-2", "Physical", "Unstable Reactive", Class: "2",
                        SSEA: "100", SSEF: "6,10", SLEA: "(100)", SLEF: "6,10", SGEA: "500", SGEF: "6,10", CSEA: "100", CSEF: "6", CLEA: "(100)", CLEF: "6", CGEA: "500", CGEF: "6,10", OSEA: "20", OSEF: "6", OLEA: "(20)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 219.2, "UR-3", "Physical", "Unstable Reactive", Class: "3",
                        SSEA: "10", SSEF: "6,10", SLEA: "(10)", SLEF: "6,10", SGEA: "100", SGEF: "6,10", CSEA: "2", CSEF: "6", CLEA: "(2)", CLEF: "6", CGEA: "20", CGEF: "6,10", OSEA: "2", OSEF: "6", OLEA: "(2)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 219.1, "UR-4", "Physical", "Unstable Reactive", Class: "4",
                        SSEA: "1", SSEF: "10,12", SLEA: "(1)", SLEF: "10,13", SGEA: "10", SGEF: "10,12", CSEA: "0.25", CSEF: "12", CLEA: "(0.25)", CLEF: "12", CGEA: "2", CGEF: "10,12", OSEA: "0.25", OSEF: "12", OLEA: "(0.25)", OLEF: "12" );
                    _createFireClassExemptAmountNode( 220.3, "WR-1", "Physical", "Water Reactive", Class: "1",
                        SSEA: "NL", SSEF: "10,11", SLEA: "(NL)", SLEF: "10,11", CSEA: "NL", CSEF: "11", CLEA: "(NL)", OSEA: "NL", OSEF: "11", OLEA: "(NL)", OLEF: "11" );
                    _createFireClassExemptAmountNode( 220.2, "WR-2", "Physical", "Water Reactive", Class: "2",
                        SSEA: "100", SSEF: "6,10", SLEA: "(100)", SLEF: "6,10", CSEA: "100", CSEF: "6", CLEA: "(100)", OSEA: "20", OSEF: "6", OLEA: "(20)", OLEF: "6" );
                    _createFireClassExemptAmountNode( 220.1, "WR-3", "Physical", "Water Reactive", Class: "3",
                        SSEA: "10", SSEF: "6,10", SLEA: "(10)", SLEF: "6,10", CSEA: "10", CSEF: "6", CLEA: "(10)", OSEA: "2", OSEF: "6", OLEA: "(2)", OLEF: "6" );
                }
                #endregion FireClassExemptAmount Nodes
            }

            #endregion FireClassExemptAmountNT
        }

        #region Private Helper Functions

        private void _createFireClassExemptAmountNode( double SortOrder, string HazardClass, string HazardType, string HazardCategory, string CategoryFootnotes = "", string Class = "",
            string SSEA = "", string SSEF = "", string SLEA = "", string SLEF = "", string SGEA = "", string SGEF = "",
            string CSEA = "", string CSEF = "", string CLEA = "", string CLEF = "", string CGEA = "", string CGEF = "",
            string OSEA = "", string OSEF = "", string OLEA = "", string OLEF = "", bool isRadioactive = false)
        {
            String RadioactiveSuffix = HazardClass == "RAD-Alpha" ? " mCi" : " Ci";
            String SolidSuffix = isRadioactive ? RadioactiveSuffix : " lbs";//or ""?
            String LiquidSuffix = isRadioactive ? RadioactiveSuffix : " gal";
            String GasSuffix = isRadioactive ? RadioactiveSuffix : " cu.ft.";
            CswNbtObjClassFireClassExemptAmount FireClassExemptAmountNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( FireClassExemptAmountNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            FireClassExemptAmountNode.SetName.RelatedNodeId = SetNodeId;
            FireClassExemptAmountNode.SortOrder.Value = SortOrder;
            FireClassExemptAmountNode.HazardClass.Value = HazardClass;
            FireClassExemptAmountNode.Class.Text = Class;
            FireClassExemptAmountNode.HazardType.Value = HazardType;
            FireClassExemptAmountNode.HazardCategory.Text = HazardCategory;
            FireClassExemptAmountNode.CategoryFootnotes.Text = CategoryFootnotes;
            FireClassExemptAmountNode.StorageSolidExemptAmount.Text = SSEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( SSEA ) || SSEA.Contains( "NL" ) ? "" : SolidSuffix );
            FireClassExemptAmountNode.StorageSolidExemptFootnotes.Text = SSEF;
            FireClassExemptAmountNode.StorageLiquidExemptAmount.Text = SLEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( SLEA ) || SLEA.Contains( "NL" ) ? "" : LiquidSuffix );
            FireClassExemptAmountNode.StorageLiquidExemptFootnotes.Text = SLEF;
            FireClassExemptAmountNode.StorageGasExemptAmount.Text = SGEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( SGEA ) || SGEA.Contains( "NL" ) ? "" : GasSuffix );
            FireClassExemptAmountNode.StorageGasExemptFootnotes.Text = SGEF;
            FireClassExemptAmountNode.ClosedSolidExemptAmount.Text = CSEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( CSEA ) || CSEA.Contains( "NL" ) ? "" : SolidSuffix );
            FireClassExemptAmountNode.ClosedSolidExemptFootnotes.Text = CSEF;
            FireClassExemptAmountNode.ClosedLiquidExemptAmount.Text = CLEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( CLEA ) || CLEA.Contains( "NL" ) ? "" : LiquidSuffix );
            FireClassExemptAmountNode.ClosedLiquidExemptFootnotes.Text = CLEF;
            FireClassExemptAmountNode.ClosedGasExemptAmount.Text = CGEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( CGEA ) || CGEA.Contains( "NL" ) ? "" : GasSuffix );
            FireClassExemptAmountNode.ClosedGasExemptFootnotes.Text = CGEF;
            FireClassExemptAmountNode.OpenSolidExemptAmount.Text = OSEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( OSEA ) || OSEA.Contains( "NL" ) ? "" : SolidSuffix );
            FireClassExemptAmountNode.OpenSolidExemptFootnotes.Text = OSEF;
            FireClassExemptAmountNode.OpenLiquidExemptAmount.Text = OLEA + ( isRadioactive ? RadioactiveSuffix : "" );// ( String.IsNullOrEmpty( OLEA ) || OLEA.Contains( "NL" ) ? "" : LiquidSuffix );
            FireClassExemptAmountNode.OpenLiquidExemptFootnotes.Text = OLEF;
            FireClassExemptAmountNode.postChanges( false );
        }

        private void _setEditLayout( string PropName, int ColNum, int RowNum, string TabGroup = "", bool RemoveFromAdd = true )
        {
            CswNbtMetaDataNodeTypeProp FireClassExemptAmountNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( FireClassExemptAmountNT.NodeTypeId, PropName );
            if( RemoveFromAdd )
                FireClassExemptAmountNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            FireClassExemptAmountNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, FireClassExemptAmountNT.getFirstNodeTypeTab().TabId, RowNum, ColNum, TabGroup );
        }

        #endregion Private Helper Functions

    }//class CswUpdateSchemaCase_01V_28283

}//namespace ChemSW.Nbt.Schema