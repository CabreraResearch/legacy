using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29489
    /// </summary>
    public class CswUpdateSchema_02C_Case29489 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29489; }
        }

        public override void update()
        {
            // Remove Regulatory Lists Grid header on Chemical
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            if( null != ChemicalOC )
            {
                foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp RegListsGridNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.RegulatoryListsGrid );
                    RegListsGridNTP.Attribute1 = false.ToString();
                }
            }

            // Add Chemical grid on Regulatory List
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
            if( null != RegListOC && null != RegListMemberOC )
            {
                foreach( CswNbtMetaDataNodeType RegListNT in RegListOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeTab ChemTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( RegListNT, "Chemicals", 2 );
                    CswNbtMetaDataNodeTypeProp ChemGrid = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( RegListNT, CswEnumNbtFieldType.Grid, "Chemicals", ChemTab.TabId );
                    CswNbtView ChemView = _CswNbtSchemaModTrnsctn.restoreView( ChemGrid.ViewId );
                    CswNbtViewRelationship regListRel = ChemView.AddViewRelationship( RegListOC, false );
                    CswNbtViewRelationship memberRel = ChemView.AddViewRelationship( regListRel, CswEnumNbtViewPropOwnerType.Second, RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList ), false );
                    CswNbtViewRelationship chemRel = ChemView.AddViewRelationship( memberRel, CswEnumNbtViewPropOwnerType.First, RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.Chemical ), false );
                    ChemView.AddViewProperty( chemRel, ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.TradeName ) );
                    ChemView.AddViewProperty( chemRel, ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.CasNo ) );
                    ChemView.save();
                }
            }
        } // update()

    }//class CswUpdateSchema_02C_Case29489

}//namespace ChemSW.Nbt.Schema