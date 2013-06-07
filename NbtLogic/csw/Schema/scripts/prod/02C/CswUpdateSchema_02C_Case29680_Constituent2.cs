using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
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
                        ComponentConstituentNTP.SetFK( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(), ConstituentNT.NodeTypeId );

                        CswNbtView ntConstituentView = _CswNbtSchemaModTrnsctn.restoreView( ComponentConstituentNTP.ViewId );
                        ntConstituentView.Root.ChildRelationships.Clear();
                        CswNbtViewRelationship rel3 = ntConstituentView.AddViewRelationship( ConstituentNT, false );
                        //ntConstituentView.AddViewPropertyAndFilter( rel3,
                        //                                            ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.IsConstituent ),
                        //                                            FilterMode: CswEnumNbtFilterMode.Equals,
                        //                                            Value: CswEnumTristate.True.ToString() );
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

                //// Fix Chemical Components Grid
                //foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalNTs )
                //{
                //    CswNbtMetaDataNodeTypeProp ComponentsGridNTP = ChemicalNT.getNodeTypeProp( "Components" );
                //    if( null != ComponentsGridNTP )
                //    {
                //        CswNbtMetaDataObjectClassProp ComponentPercentOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Percentage );

                //        CswNbtView ComponentsGridView = _CswNbtSchemaModTrnsctn.restoreView( ComponentsGridNTP.ViewId );
                //        ComponentsGridView.Root.ChildRelationships.Clear();
                //        CswNbtViewRelationship ChemRel = ComponentsGridView.AddViewRelationship( ChemicalNT, false );
                //        CswNbtViewRelationship MixRel = ComponentsGridView.AddViewRelationship( ChemRel, CswEnumNbtViewPropOwnerType.Second, ComponentMixtureOCP, false );
                //        ComponentsGridView.AddViewProperty( MixRel, ComponentConstituentOCP, 1 );
                //        ComponentsGridView.AddViewProperty( MixRel, ComponentPercentOCP, 2 );
                //        ComponentsGridView.save();
                //    }
                //} // foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalNTs )


                // 6. Update any views or props that point to the Chemical ObjClass to point to the Chemical NT. 
                // We don't want Constituent Chemicals to show as the target for things like Requests Material or Size Owner.

                string Sql = @"select ocp.propname, oc.objectclass, t.nodetypename, p.propname, p.nodetypepropid, p.fktype, p.fkvalue
                                 from nodetype_props p
                                 join nodetypes t on p.nodetypeid = t.nodetypeid
                                 join object_class oc on t.objectclassid = oc.objectclassid
                                 left outer join object_class_props ocp on p.objectclasspropid = ocp.objectclasspropid
                                 join field_types f on p.fieldtypeid = f.fieldtypeid
                                where (    ( p.fktype = 'PropertySetId' and p.fkvalue = (select propertysetid from property_set where name = 'MaterialSet' ) )
                                        or ( p.fktype = 'ObjectClassId' and p.fkvalue = (select objectclassid from object_class where objectclass = 'ChemicalClass' ) )
                                        or ( p.fktype = 'NodeTypeId' and p.fkvalue = (select nodetypeid from nodetypes where nodetypename = 'Chemical' ) ) 
                                      )
                                  and f.fieldtype = 'Relationship'
                                  and ( p.objectclasspropid is null 
                                        or not (    ( oc.objectclass = 'GHSClass' and ocp.propname = 'Material' )
                                                 or ( oc.objectclass = 'MaterialComponentClass' and ocp.propname = 'Mixture' ) 
                                                 or ( oc.objectclass = 'MaterialComponentClass' and ocp.propname = 'Constituent' ) 
                                                 or ( oc.objectclass = 'DocumentClass' and ocp.propname = 'Owner' ) 
                                               )
                                      )";
                CswArbitrarySelect Select = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "29680_relationship_select", Sql );
                DataTable Table = Select.getTable();

                /*    
                    MaterialSynonymClass                Material	
                    SizeClass                           Material	
                    ContainerClass                      Material	
                    InventoryLevelClass                 Material	
                    CofAMethodTemplateClass             Material	
                    ManufacturerEquivalentPartClass     Material	
                    ReceiptLotClass                     Material	
                    RequestContainerDispenseClass       Material	
                    RequestContainerUpdateClass         Material	
                    RequestMaterialCreateClass          Material	
                    RequestMaterialDispenseClass        Material	
                */

                foreach( DataRow Row in Table.Rows )
                {

                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( CswConvert.ToInt32( Row["nodetypepropid"] ) );
                    CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( Prop.ViewId );
                    if( View.Root.ChildRelationships.Count == 0 )
                    {
                        // The 'Default Filter' will filter out constituents for us
                        if( Row["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() )
                        {
                            View.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswConvert.ToInt32( Row["fkvalue"] ) ), true );
                        }
                        else if( Row["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                        {
                            View.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswConvert.ToInt32( Row["fkvalue"] ) ), true );
                        }
                        else if( Row["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                        {
                            View.AddViewRelationship( _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswConvert.ToInt32( Row["fkvalue"] ) ), true );
                        }
                    }
                    else
                    {
                        foreach( CswNbtViewRelationship viewRel in View.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewRelationship ) )
                        {
                            if( CswConvert.ToInt32( Row["fkvalue"] ) == viewRel.SecondId )
                            {
                                if( Row["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() )
                                {
                                    CswNbtMetaDataNodeType MaterialNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( viewRel.SecondId );
                                    CswNbtViewProperty viewProp = View.AddViewProperty( viewRel, MaterialNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.IsConstituent ) );
                                    viewProp.ShowInGrid = false;
                                    View.AddViewPropertyFilter( viewProp,
                                                                FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                                Value: CswEnumTristate.True.ToString() );
                                }
                                else if( Row["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                                {
                                    CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( viewRel.SecondId );
                                    CswNbtViewProperty viewProp = View.AddViewProperty( viewRel, MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.IsConstituent ) );
                                    viewProp.ShowInGrid = false;
                                    View.AddViewPropertyFilter( viewProp,
                                                                FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                                Value: CswEnumTristate.True.ToString() );
                                }
                                else if( Row["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                                {
                                    CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( viewRel.SecondId );
                                    CswNbtViewProperty viewProp = View.AddViewProperty( viewRel, MaterialPS.getObjectClasses().First().getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.IsConstituent ) );
                                    viewProp.ShowInGrid = false;
                                    View.AddViewPropertyFilter( viewProp,
                                                                FilterMode: CswEnumNbtFilterMode.NotEquals,
                                                                Value: CswEnumTristate.True.ToString() );
                                }
                            }
                        } // foreach( CswNbtViewRelationship viewRel in View.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewRelationship ) )
                    }
                    View.save();
                } // foreach( DataRow relRow in relTable.Rows )

            } // if( null != ConstituentNT && ChemicalNTs.Count > 0 )
        } // update()

    }//class CswUpdateSchema_02C_Case29680_Constituent2

}//namespace ChemSW.Nbt.Schema