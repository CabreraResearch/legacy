using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
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
            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor

        #region Public

        /// <summary>
        /// Creates a new material, if one does not already exist, and returns the material nodeid
        /// </summary>
        public JObject createMaterial( Int32 NodeTypeId, string SupplierId, string Tradename, string PartNo, string NodeId )
        {
            return _CswNbtActCreateMaterial.createMaterial( NodeTypeId, SupplierId, Tradename, PartNo, NodeId );
        }

        public JObject saveMaterial( Int32 NodeTypeId, string SupplierId, string Suppliername, string Tradename, string PartNo, string NodeId )
        {
            return _CswNbtActCreateMaterial.saveMaterial( NodeTypeId, SupplierId, Suppliername, Tradename, PartNo, NodeId );
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

        public static JObject getMaterialSizes( CswNbtResources CswNbtResources, CswPrimaryKey MaterialId )
        {
            return new JObject(); //CswNbtActCreateMaterial.getMaterialSizes( CswNbtResources, MaterialId );
        }

        public static void initializeCreateMaterial( ICswResources CswResources, MaterialResponse Response, string NodeId )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtActCreateMaterial CreateMaterialAction = new CswNbtActCreateMaterial( NbtResources );

                // Get/Create a node
                CswPrimaryKey NodePk = CreateMaterialAction.makeTemp( NodeId );
                Response.Data.TempNode = new CswNbtNode.Node( NbtResources.getNode( NodePk, DateTime.Now ) );

                // Suppliers view
                CswNbtView SupplierView = CreateMaterialAction.getMaterialSuppliersView();
                if( null != SupplierView )
                {
                    Response.Data.SuppliersView.SessionViewId = SupplierView.SessionViewId;
                }

                //Alert wizard to active modules
                bool ContainersEnabled = NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers );
                Response.Data.ContainersModuleEnabled = ContainersEnabled;
                bool SDSEnabled = NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.SDS ) && 
                    ( CswNbtActReceiving.getSDSDocumentNodeTypeId( NbtResources ) != Int32.MinValue );
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
                    MaterialResponse.WizardStep Sizes = new MaterialResponse.WizardStep()
                    {
                        StepNo = StepNo,
                        StepName = "Size(s)"
                    };
                    Response.Data.Steps.Add( Sizes );
                    StepNo++;
                }
                if( SDSEnabled )
                {
                    MaterialResponse.WizardStep AttachSDS = new MaterialResponse.WizardStep()
                    {
                        StepNo = StepNo,
                        StepName = "Attach SDS"
                    };
                    Response.Data.Steps.Add( AttachSDS );
                }
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
                    CswNbtObjClassMaterial materialNode = NbtResources.Nodes[pk];
                    Response.Data.PhysicalState = materialNode.PhysicalState.Value;
                }
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