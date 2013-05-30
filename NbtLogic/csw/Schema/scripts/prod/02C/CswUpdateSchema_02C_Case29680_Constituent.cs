using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29680
    /// </summary>
    public class CswUpdateSchema_02C_Case29680_Constituent : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29680; }
        }

        public override void update()
        {
            // New 'IsConstituent' property
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in _CswNbtSchemaModTrnsctn.MetaData.getObjectClassesByPropertySetId( MaterialPS.PropertySetId ) )
            {
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp IsConstituentNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.IsConstituent );
                    IsConstituentNTP.DefaultValue.AsLogical.Checked = CswEnumTristate.False;
                    IsConstituentNTP.removeFromAllLayouts();

                    foreach( CswNbtPropertySetMaterial MaterialNode in MaterialNT.getNodes( false, true ) )
                    {
                        MaterialNode.IsConstituent.Checked = CswEnumTristate.False;
                        MaterialNode.postChanges( false );
                    }
                }
            }

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            if( null != ChemicalOC.FirstNodeType )
            {
                //1. Create a new NodeType (of ObjClassChemical) called "Constituent"
                CswNbtMetaDataNodeType ConstituentNT = _CswNbtSchemaModTrnsctn.MetaData.CopyNodeType( ChemicalOC.FirstNodeType, "Constituent" );

                CswNbtMetaDataNodeTypeProp TradeNameNTP = ConstituentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.TradeName );
                ConstituentNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( TradeNameNTP.PropName ) );

                // IsConstituent is true here
                CswNbtMetaDataNodeTypeProp IsConstituentNTP = ConstituentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.IsConstituent );
                IsConstituentNTP.DefaultValue.AsLogical.Checked = CswEnumTristate.True;

                //2. Set Allow Receiving prop to "false" by default
                CswNbtMetaDataNodeTypeProp ApprovedReceivingNTP = ConstituentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.ApprovedForReceiving );
                ApprovedReceivingNTP.DefaultValue.AsLogical.Checked = CswEnumTristate.False;
                ApprovedReceivingNTP.ServerManaged = true;

                Collection<CswNbtMetaDataNodeTypeProp> PropertiesToDelete = new Collection<CswNbtMetaDataNodeTypeProp>();
                PropertiesToDelete.Add( ConstituentNT.getNodeTypeProp( "Chemical Sizes" ) );
                PropertiesToDelete.Add( ConstituentNT.getNodeTypeProp( "Inventory Levels" ) );
                PropertiesToDelete.Add( ConstituentNT.getNodeTypeProp( "Chemical Containers" ) );
                PropertiesToDelete.Add( ConstituentNT.getNodeTypeProp( "Components" ) );

                foreach( CswNbtMetaDataNodeTypeProp DoomedProp in PropertiesToDelete )
                {
                    if( null != DoomedProp )
                    {
                        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( DoomedProp );
                    }
                }
            } // if( null != ChemicalOC.FirstNodeType )
        } // update()

    }//class CswUpdateSchema_02C_Case29680_Constituent

}//namespace ChemSW.Nbt.Schema