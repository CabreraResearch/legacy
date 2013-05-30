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
    public class CswUpdateSchema_02C_Case29680_Constituent2 : CswUpdateSchemaTo
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
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );

            CswNbtMetaDataNodeType ConstituentNT = null;
            Collection<CswNbtMetaDataNodeType> ChemicalNTs = new Collection<CswNbtMetaDataNodeType>();
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( CswEnumNbtObjectClass.ChemicalClass ) )
            {
                if( NodeType.NodeTypeName == "Constituent" )
                {
                    ConstituentNT = NodeType;
                }
                else
                {
                    ChemicalNTs.Add( NodeType );
                }
            }

            if( null != ConstituentNT && ChemicalNTs.Count > 0 )
            {
                //4. Change Components Consituent property to point to the Constituent NT
                CswNbtMetaDataObjectClass ComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );

                // Only Constituents for 'Constituent' property
                CswNbtMetaDataObjectClassProp ComponentConstituentOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
                {
                    CswNbtView ocConstituentView = _CswNbtSchemaModTrnsctn.makeView();
                    ocConstituentView.ViewName = "Constituent View";
                    CswNbtViewRelationship rel1 = ocConstituentView.AddViewRelationship( ChemicalOC, false );
                    ocConstituentView.AddViewPropertyAndFilter( rel1,
                                                                ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.IsConstituent ),
                                                                FilterMode: CswEnumNbtFilterMode.Equals,
                                                                Value: CswEnumTristate.True.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ComponentConstituentOCP, CswEnumNbtObjectClassPropAttributes.viewxml, ocConstituentView.ToString() );
                }
                // Exclude Constituents from 'Mixture' property
                CswNbtMetaDataObjectClassProp ComponentMixtureOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
                {
                    CswNbtView ocMixtureView = _CswNbtSchemaModTrnsctn.makeView();
                    ocMixtureView.ViewName = "Mixture View";
                    CswNbtViewRelationship rel2 = ocMixtureView.AddViewRelationship( ChemicalOC, false );
                    ocMixtureView.AddViewPropertyAndFilter( rel2,
                                                            ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.IsConstituent ),
                                                            FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                            Value: CswEnumTristate.True.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ComponentMixtureOCP, CswEnumNbtObjectClassPropAttributes.viewxml, ocMixtureView.ToString() );
                }
                // Fix views on existing nodetypes
                foreach( CswNbtMetaDataNodeType ComponentNT in ComponentOC.getNodeTypes() )
                {
                    // Only Constituents for 'Constituent' property
                    {
                        CswNbtMetaDataNodeTypeProp ComponentConstituentNTP = ComponentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
                        CswNbtView ntConstituentView = _CswNbtSchemaModTrnsctn.restoreView( ComponentConstituentNTP.ViewId );
                        ntConstituentView.Root.ChildRelationships.Clear();
                        CswNbtViewRelationship rel3 = ntConstituentView.AddViewRelationship( ChemicalOC, false );
                        ntConstituentView.AddViewPropertyAndFilter( rel3,
                                                                    ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.IsConstituent ),
                                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                                    Value: CswEnumTristate.True.ToString() );
                        ntConstituentView.save();
                    }
                    // Exclude Constituents from 'Mixture' property
                    {
                        CswNbtMetaDataNodeTypeProp ComponentMixtureNTP = ComponentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
                        CswNbtView ntMixtureView = _CswNbtSchemaModTrnsctn.restoreView( ComponentMixtureNTP.ViewId );
                        ntMixtureView.Root.ChildRelationships.Clear();
                        CswNbtViewRelationship rel4 = ntMixtureView.AddViewRelationship( ChemicalOC, false );
                        ntMixtureView.AddViewPropertyAndFilter( rel4,
                                                                ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.IsConstituent ),
                                                                FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                                Value: CswEnumTristate.True.ToString() );
                        ntMixtureView.save();
                    }
                } // foreach( CswNbtMetaDataNodeType ComponentNT in ComponentOC.getNodeTypes() )


                foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalNTs )
                {
                    CswNbtMetaDataNodeTypeProp ComponentsGridNTP = ChemicalNT.getNodeTypeProp( "Components" );
                    if( null != ComponentsGridNTP )
                    {
                        CswNbtMetaDataObjectClassProp ComponentPercentOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Percentage );

                        CswNbtView ComponentsGridView = _CswNbtSchemaModTrnsctn.restoreView( ComponentsGridNTP.ViewId );
                        ComponentsGridView.Root.ChildRelationships.Clear();
                        CswNbtViewRelationship ChemRel = ComponentsGridView.AddViewRelationship( ChemicalNT, false );
                        CswNbtViewRelationship MixRel = ComponentsGridView.AddViewRelationship( ChemRel, CswEnumNbtViewPropOwnerType.Second, ComponentMixtureOCP, false );
                        ComponentsGridView.AddViewProperty( MixRel, ComponentConstituentOCP, 1 );
                        ComponentsGridView.AddViewProperty( MixRel, ComponentPercentOCP, 2 );
                        ComponentsGridView.save();
                    }
                }
            }
        } // update()

    }//class CswUpdateSchema_02C_Case29680_Constituent2

}//namespace ChemSW.Nbt.Schema