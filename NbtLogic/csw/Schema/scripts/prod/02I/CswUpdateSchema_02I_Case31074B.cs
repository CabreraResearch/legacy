using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31074B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31074; }
        }

        public override string Title
        {
            get { return "Fix GHS Classifications: Nodetypes and nodes"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        private CswNbtMetaDataNodeType GHSClassNT;

        public override void update()
        {
            CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            CswNbtMetaDataObjectClass GHSClassOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClassificationClass );

            // Fix layout of Ghs NodeType
            foreach( CswNbtMetaDataNodeType GhsNT in GhsOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp AddLabelCodesNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.AddLabelCodes );

                Int32 AddDisplayRow = 8;
                if( null != AddLabelCodesNTP.AddLayout && AddLabelCodesNTP.AddLayout.DisplayRow != Int32.MinValue )
                {
                    AddDisplayRow = AddLabelCodesNTP.AddLayout.DisplayRow + 1;
                }
                Int32 EditDisplayRow = 8;
                if( null != AddLabelCodesNTP.FirstEditLayout && AddLabelCodesNTP.FirstEditLayout.DisplayRow != Int32.MinValue )
                {
                    EditDisplayRow = AddLabelCodesNTP.FirstEditLayout.DisplayRow + 1;
                }

                CswNbtMetaDataNodeTypeProp ClassNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Classifications );
                ClassNTP.updateLayout( CswEnumNbtLayoutType.Add, true, Int32.MinValue, AddDisplayRow, 1, "Classification" );
                ClassNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, GhsNT.getFirstNodeTypeTab().TabId, EditDisplayRow, 1, "Classification" );
                EditDisplayRow++;

                CswNbtMetaDataNodeTypeProp ClassGridNTP = GhsNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.ClassificationsGrid );
                ClassGridNTP.removeFromLayout( CswEnumNbtLayoutType.Add );
                ClassGridNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, GhsNT.getFirstNodeTypeTab().TabId, EditDisplayRow, 1, "Classification" );
            }

            // Add new 'GHS Classification' nodetype
            GHSClassNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( GHSClassOC )
                {
                    NodeTypeName = "GHS Classification",
                    Category = "System"
                } );

            GHSClassNT.addNameTemplateText( CswNbtObjClassGHSClassification.PropertyName.English );

            CswNbtMetaDataNodeTypeProp GHSClassCategoryNTP = GHSClassNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHSClassification.PropertyName.Category );
            GHSClassCategoryNTP.updateLayout( CswEnumNbtLayoutType.Add, true, Int32.MinValue, 1, 1 );
            GHSClassCategoryNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, GHSClassNT.getFirstNodeTypeTab().TabId, 1, 1 );


            // Populate 'GHS Classification' nodes
            _addGhsClassNode( "Health", "Acute Toxicity: Oral (Category 1)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Oral (Category 2)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Oral (Category 3)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Oral (Category 4)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Oral (Category 5)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dermal (Category 1)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dermal (Category 2)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dermal (Category 3)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dermal (Category 4)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dermal (Category 5)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Gases (Category 1)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Gases (Category 2)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Gases (Category 3)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Gases (Category 4)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Gases (Category 5)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Vapors (Category 1)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Vapors (Category 2)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Vapors (Category 3)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Vapors (Category 4)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Vapors (Category 5)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dusts & mists (Category 1)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dusts & mists (Category 2)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dusts & mists (Category 3)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dusts & mists (Category 4)" );
            _addGhsClassNode( "Health", "Acute Toxicity: Dusts & mists (Category 5)" );
            _addGhsClassNode( "Environmental", "Aquatic Toxicity (Acute) (Category I)" );
            _addGhsClassNode( "Environmental", "Aquatic Toxicity (Acute) (Category II)" );
            _addGhsClassNode( "Environmental", "Aquatic Toxicity (Acute) (Category III)" );
            _addGhsClassNode( "Environmental", "Aquatic Toxicity (Chronic) (Category I)" );
            _addGhsClassNode( "Environmental", "Aquatic Toxicity (Chronic) (Category II)" );
            _addGhsClassNode( "Environmental", "Aquatic Toxicity (Chronic) (Category III)" );
            _addGhsClassNode( "Environmental", "Aquatic Toxicity (Chronic) (Category IV)" );
            _addGhsClassNode( "Health", "Aspiration Toxicity (Category 1)" );
            _addGhsClassNode( "Health", "Aspiration Toxicity (Category 2)" );
            _addGhsClassNode( "Health", "Carcinogenicity (Category 1A)" );
            _addGhsClassNode( "Health", "Carcinogenicity (Category 1B)" );
            _addGhsClassNode( "Health", "Carcinogenicity (Category 2)" );
            _addGhsClassNode( "Physical", "Corrosive to Metal" );
            _addGhsClassNode( "Physical", "Explosives (Division 1.1)" );
            _addGhsClassNode( "Physical", "Explosives (Division 1.2)" );
            _addGhsClassNode( "Physical", "Explosives (Division 1.3)" );
            _addGhsClassNode( "Physical", "Explosives (Division 1.4)" );
            _addGhsClassNode( "Physical", "Explosives (Division 1.5)" );
            _addGhsClassNode( "Physical", "Explosives (Division 1.6" );
            _addGhsClassNode( "Physical", "Explosives (Unstable Explosives)" );
            _addGhsClassNode( "Physical", "Flammable Aerosols (Category 1)" );
            _addGhsClassNode( "Physical", "Flammable Aerosols (Category 2)" );
            _addGhsClassNode( "Physical", "Flammable Gas (Category 1)" );
            _addGhsClassNode( "Physical", "Flammable Gas (Category 2)" );
            _addGhsClassNode( "Physical", "Flammable Liquids (Category 1)" );
            _addGhsClassNode( "Physical", "Flammable Liquids (Category 2)" );
            _addGhsClassNode( "Physical", "Flammable Liquids (Category 3)" );
            _addGhsClassNode( "Physical", "Flammable Liquids (Category 4)" );
            _addGhsClassNode( "Physical", "Flammable Solids (Category 1)" );
            _addGhsClassNode( "Physical", "Flammable Solids (Category 2)" );
            _addGhsClassNode( "Physical", "Gases Under Pressure (Compressed gas)" );
            _addGhsClassNode( "Physical", "Gases Under Pressure (Dissolved gas)" );
            _addGhsClassNode( "Physical", "Gases Under Pressure (Liquefied gas)" );
            _addGhsClassNode( "Physical", "Gases Under Pressure (Refrigerated liquefied gas)" );
            _addGhsClassNode( "Health", "Germ Cell Mutagenicity (Category 1A)" );
            _addGhsClassNode( "Health", "Germ Cell Mutagenicity (Category 1B)" );
            _addGhsClassNode( "Health", "Germ Cell Mutagenicity (Category 2)" );
            _addGhsClassNode( "Physical", "Organic Peroxides (Type A)" );
            _addGhsClassNode( "Physical", "Organic Peroxides (Type B)" );
            _addGhsClassNode( "Physical", "Organic Peroxides (Type C)" );
            _addGhsClassNode( "Physical", "Organic Peroxides (Type D)" );
            _addGhsClassNode( "Physical", "Organic Peroxides (Type E)" );
            _addGhsClassNode( "Physical", "Organic Peroxides (Type F)" );
            _addGhsClassNode( "Physical", "Organic Peroxides (Type G)" );
            _addGhsClassNode( "Physical", "Oxidizing Gases" );
            _addGhsClassNode( "Physical", "Oxidizing Liquids (Category 1)" );
            _addGhsClassNode( "Physical", "Oxidizing Liquids (Category 2)" );
            _addGhsClassNode( "Physical", "Oxidizing Liquids (Category 3)" );
            _addGhsClassNode( "Physical", "Oxidizing Solids (Category 1)" );
            _addGhsClassNode( "Physical", "Oxidizing Solids (Category 2)" );
            _addGhsClassNode( "Physical", "Oxidizing Solids (Category 3)" );
            _addGhsClassNode( "Physical", "Pyrophoric Liquids" );
            _addGhsClassNode( "Physical", "Pyrophoric Solids" );
            _addGhsClassNode( "Physical", "Self-Heating Substances and Mixtures (Category 1)" );
            _addGhsClassNode( "Physical", "Self-Heating Substances and Mixtures (Category 2)" );
            _addGhsClassNode( "Physical", "Self-Reactive Substances and Mixtures (Type A)" );
            _addGhsClassNode( "Physical", "Self-Reactive Substances and Mixtures (Type B)" );
            _addGhsClassNode( "Physical", "Self-Reactive Substances and Mixtures (Type C)" );
            _addGhsClassNode( "Physical", "Self-Reactive Substances and Mixtures (Type D)" );
            _addGhsClassNode( "Physical", "Self-Reactive Substances and Mixtures (Type E)" );
            _addGhsClassNode( "Physical", "Self-Reactive Substances and Mixtures (Type F)" );
            _addGhsClassNode( "Physical", "Self-Reactive Substances and Mixtures (Type G)" );
            _addGhsClassNode( "Health", "Serious Eye Damage/Eye Irritation (Category 1)" );
            _addGhsClassNode( "Health", "Serious Eye Damage/Eye Irritation (Category 2A)" );
            _addGhsClassNode( "Health", "Serious Eye Damage/Eye Irritation (Category 2B)" );
            _addGhsClassNode( "Health", "Skin Corrosion/Irritation (Category 1A)" );
            _addGhsClassNode( "Health", "Skin Corrosion/Irritation (Category 1B)" );
            _addGhsClassNode( "Health", "Skin Corrosion/Irritation (Category 1C)" );
            _addGhsClassNode( "Health", "Skin Corrosion/Irritation (Category 2)" );
            _addGhsClassNode( "Health", "Skin Corrosion/Irritation (Category 3)" );
            _addGhsClassNode( "Health", "Skin Sensitization" );
            _addGhsClassNode( "Health", "Respiratory Sensitization" );
            _addGhsClassNode( "Health", "Specific Target Organ Toxicity (Repeated Exposure) (Category 1)" );
            _addGhsClassNode( "Health", "Specific Target Organ Toxicity (Repeated Exposure) (Category 2)" );
            _addGhsClassNode( "Health", "Specific Target Organ Toxicity (Single Exposure) (Category 1)" );
            _addGhsClassNode( "Health", "Specific Target Organ Toxicity (Single Exposure) (Category 2)" );
            _addGhsClassNode( "Health", "Specific Target Organ Toxicity (Single Exposure) (Category 3)" );
            _addGhsClassNode( "Physical", "Substances and Mixtures, which on Contact with Water, Emit Flammable Gases (Category 1)" );
            _addGhsClassNode( "Physical", "Substances and Mixtures, which on Contact with Water, Emit Flammable Gases (Category 2)" );
            _addGhsClassNode( "Physical", "Substances and Mixtures, which on Contact with Water, Emit Flammable Gases (Category 3)" );
            _addGhsClassNode( "Health", "Reproductive Toxicity (Category 1A)" );
            _addGhsClassNode( "Health", "Reproductive Toxicity (Category 1B)" );
            _addGhsClassNode( "Health", "Reproductive Toxicity (Category 2)" );
            _addGhsClassNode( "Health", "Reproductive Toxicity (Lactation Effects)" );
        } // update()

        private void _addGhsClassNode( string Category, string Classification )
        {
            _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GHSClassNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassGHSClassification GHSClassNode = NewNode;
                    GHSClassNode.Category.Value = Category;
                    GHSClassNode.English.Text = Classification;
                } );

        } // _addGhsClassNode()

    } // class

}//namespace ChemSW.Nbt.Schema