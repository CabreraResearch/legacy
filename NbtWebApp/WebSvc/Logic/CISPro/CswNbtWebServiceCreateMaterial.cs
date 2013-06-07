using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using NbtWebApp.Services;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceCreateMaterial
    {
        #region ctor

        private CswNbtResources _CswNbtResources;
        private CswNbtActCreateMaterial _CswNbtActCreateMaterial;
        /// <summary>
        /// Base Constructor
        /// </summary>
        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtActCreateMaterial = new CswNbtActCreateMaterial( CswNbtResources );
            if( false == _CswNbtResources.Permit.can( CswEnumNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor

        #region Public

        /// <summary>
        /// Creates a new material, if one does not already exist, and returns the material nodeid
        /// </summary>
        public JObject createMaterial( Int32 NodeTypeId, string SupplierId, string Tradename, string PartNo, string NodeId )
        {
            return _CswNbtActCreateMaterial.tryCreateTempMaterial( NodeTypeId, SupplierId, Tradename, PartNo, NodeId );
        }

        public JObject saveMaterial( Int32 NodeTypeId, string SupplierId, string Suppliername, string Tradename, string PartNo, string NodeId )
        {
            return _CswNbtActCreateMaterial.initNewTempMaterialNode( NodeTypeId, SupplierId, Suppliername, Tradename, PartNo, NodeId );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode )
        {
            return CswNbtActCreateMaterial.getSizeNodeProps( CswNbtResources, SizeNodeTypeId, SizeDefinition, WriteNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 SizeNodeTypeId, string SizeDefinition, bool WriteNode, out CswNbtNode SizeNode )
        {
            return CswNbtActCreateMaterial.getSizeNodeProps( CswNbtResources, SizeNodeTypeId, SizeDefinition, WriteNode, out SizeNode );
        }

        public static JObject getSizeNodeProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents, Int32 SizeNodeTypeId, JObject SizeObj, bool WriteNode, out CswNbtNode SizeNode )
        {
            return CswNbtActCreateMaterial.getSizeNodeProps( CswNbtResources, SizeNodeTypeId, SizeObj, WriteNode, out SizeNode );
        }

        public static void initializeCreateMaterial( ICswResources CswResources, MaterialResponse Response, string NodeId )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtActCreateMaterial CreateMaterialAction = new CswNbtActCreateMaterial( NbtResources );

                // Get/Create a node
                CswPrimaryKey NodePk = CreateMaterialAction.makeTemp( NodeId );
                CswNbtNode TempNode = NbtResources.getNode( NodePk, DateTime.Now );
                Response.Data.TempNode = new CswNbtNode.Node( TempNode );
                Response.Data.TempNodeObjClassId = CswConvert.ToString( TempNode.getObjectClassId() );

                // Suppliers view
                CswNbtView SupplierView = CreateMaterialAction.getMaterialSuppliersView();
                if( null != SupplierView )
                {
                    Response.Data.SuppliersView.SessionViewId = SupplierView.SessionViewId;
                }

                //Alert wizard to active modules
                bool ContainersEnabled = NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers );
                Response.Data.ContainersModuleEnabled = ContainersEnabled;
                bool SDSEnabled = NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS );
                Response.Data.SDSModuleEnabled = SDSEnabled;

                //Determine the steps
                int StepNo = 1;
                MaterialResponse.WizardStep TypeAndIdentity = new MaterialResponse.WizardStep()
                {
                    StepNo = StepNo,
                    StepName = "Choose Type and Identity"
                };
                Response.Data.Steps.Add( TypeAndIdentity );
                StepNo++;

                MaterialResponse.WizardStep AdditionalProps = new MaterialResponse.WizardStep()
                {
                    StepNo = StepNo,
                    StepName = "Additional Properties"
                };
                Response.Data.Steps.Add( AdditionalProps );
                StepNo++;

                if( ContainersEnabled )
                {
                    bool CanSize = false;
                    CswNbtMetaDataObjectClass SizeOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                    foreach( CswNbtMetaDataNodeType SizeNT in SizeOC.getNodeTypes() )
                    {
                        if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, SizeNT ) )
                        {
                            CanSize = true;
                        }
                    }
                    if( CanSize )
                    {
                        MaterialResponse.WizardStep Sizes = new MaterialResponse.WizardStep()
                            {
                                StepNo = StepNo,
                                StepName = "Size(s)"
                            };
                        Response.Data.Steps.Add( Sizes );
                        StepNo++;
                    }
                }
                if( SDSEnabled )
                {
                    CswNbtMetaDataNodeType SDSNT = NbtResources.MetaData.getNodeType( "SDS Document" );
                    if( null != SDSNT && NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, SDSNT ) )
                    {
                        MaterialResponse.WizardStep AttachSDS = new MaterialResponse.WizardStep()
                            {
                                StepNo = StepNo,
                                StepName = "Attach SDS"
                            };
                        Response.Data.Steps.Add( AttachSDS );
                    }
                }

                // Get the ChemicalObjClassId 
                CswNbtMetaDataObjectClass ChemicalOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
                Response.Data.ChemicalObjClassId = CswConvert.ToString( ChemicalOC.ObjectClassId );
                
                // Determine Constituent NodeTypes
                CswCommaDelimitedString ConstituentNodeTypeIds = new CswCommaDelimitedString();
                foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp IsConstituentNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.IsConstituent );
                    // Yes this is a weird way to know whether a nodetype is a Constituent nodetype, 
                    // but as long as this property remains servermanaged, this will work
                    if( CswEnumTristate.True == IsConstituentNTP.DefaultValue.AsLogical.Checked ) 
                    {
                        ConstituentNodeTypeIds.Add( ChemicalNT.NodeTypeId.ToString() );
                    }
                }
                Response.Data.ConstituentNodeTypeIds = ConstituentNodeTypeIds.ToString();
            }
        }

        public static void getCreateMaterialViews( ICswResources CswResources, MaterialResponse Response, object Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtActCreateMaterial act = new CswNbtActCreateMaterial( NbtResources );
                CswNbtView SupplierView = act.getMaterialSuppliersView();
                if( null != SupplierView )
                {
                    Response.Data.SuppliersView.SessionViewId = SupplierView.SessionViewId;
                }
            }
        }

        public static void getPhysicalState( ICswResources CswResources, MaterialResponse Response, string NodeId )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( NodeId );
                if( CswTools.IsPrimaryKey( pk ) )
                {
                    Response.Data.PhysicalState = "n/a";
                    CswNbtPropertySetMaterial MaterialNode = NbtResources.Nodes[pk];
                    if( MaterialNode.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                    {
                        CswNbtObjClassChemical ChemicalNode = MaterialNode.Node;
                        Response.Data.PhysicalState = ChemicalNode.PhysicalState.Value;
                    }
                }
            }
        }

        public static void saveMaterialProps( ICswResources CswResources, MaterialResponse Response, string PropDefinition )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtActCreateMaterial CreateMaterialAction = new CswNbtActCreateMaterial( NbtResources );

                // Convert PropDefintion to JObject
                JObject PropsObj = CswConvert.ToJObject( PropDefinition );

                if( PropsObj.HasValues )
                {
                    // Convert the nodeid to a primary key
                    CswPrimaryKey NodePk = CswConvert.ToPrimaryKey( CswConvert.ToString( PropsObj["NodeId"] ) );

                    // Convert the Nodetypeid to an Int32
                    Int32 NodeTypeId = CswConvert.ToInt32( CswConvert.ToString( PropsObj["NodeTypeId"] ) );

                    // Properties only
                    JObject Properties = CswConvert.ToJObject( PropsObj["Properties"] );

                    // Save the properties
                    JObject PropValues = CreateMaterialAction.saveMaterialProps( NodePk, Properties, NodeTypeId );

                    // Set the return object
                    if( PropValues.HasValues )
                    {
                        if( null != PropValues.Property( "PhysicalState" ) )
                        {
                            Response.Data.Properties.PhysicalState = CswConvert.ToString( PropValues["PhysicalState"] );
                        }
                    }

                }//if( PropsObj.HasValues )
            }
        }

        /// <summary>
        /// Finalize the new Material
        /// </summary>
        public JObject commitMaterial( string MaterialDefinition )
        {
            return _CswNbtActCreateMaterial.commitMaterial( MaterialDefinition );
        }

        /// <summary>
        /// Get a landing page for a Material
        /// </summary>
        public static JObject getLandingPageData( CswNbtResources NbtResources, CswNbtNode MaterialNode, CswNbtView MaterialNodeView = null )
        {
            return CswNbtActCreateMaterial.getLandingPageData( NbtResources, MaterialNode, MaterialNodeView );
        }

        public static JObject getMaterialUnitsOfMeasure( string PhysicalStateValue, CswNbtResources CswNbtResources )
        {
            return CswNbtActCreateMaterial.getMaterialUnitsOfMeasure( PhysicalStateValue, CswNbtResources );
        }

        public JObject getSizeLogicalsVisibility( int SizeNodeTypeId )
        {
            return _CswNbtActCreateMaterial.getSizeLogicalsVisibility( SizeNodeTypeId );
        }

        #endregion Public
    }
}