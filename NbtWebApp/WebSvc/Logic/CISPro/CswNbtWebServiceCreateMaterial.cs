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

        public JObject alreadyExists( Int32 NodeTypeId, string SupplierId, string Tradename, string PartNo, string NodeId )
        {
            return _CswNbtActCreateMaterial.alreadyExists( NodeTypeId, SupplierId, Tradename, PartNo, NodeId );
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

        public JObject initializeCreateMaterial( CswNbtResources CswNbtResources, string NodeId )
        {
            JObject Ret = new JObject();

            if( null != CswNbtResources )
            {
                CswNbtActCreateMaterial CreateMaterialAction = new CswNbtActCreateMaterial( CswNbtResources );

                // Get/Create a node
                Ret["nodedata"] = CreateMaterialAction.makeTemp(NodeId);

                // Suppliers view
                CswNbtView SupplierView = CreateMaterialAction.getMaterialSuppliersView();
                if( null != SupplierView )
                {
                    Ret["sessionviewid"] = SupplierView.SessionViewId.ToString();
                }

                // Material sizes (if exist)
                //ICswNbtTree SizesTree = CreateMaterialAction.getMaterialSizes( NbtResources, NodePk );
                //for( int i = 0; i < SizesTree.getChildNodeCount(); i++ )
                //{
                //    SizesTree.goToNthChild( i );
                //    Response.Data.SizeNodes.Add( new CswNbtNode.Node( null )
                //    {
                //        NodeId = SizesTree.getNodeIdForCurrentPosition(),
                //        NodeName = SizesTree.getNodeNameForCurrentPosition()
                //    } );
                //    SizesTree.goToParentNode();
                //}

            }

            return Ret;
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

        public JObject saveMaterial( string state )
        {
            return _CswNbtActCreateMaterial.saveMaterial( _CswNbtResources, state );
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

        public static JObject getMaterialUnitsOfMeasure( string MaterialId, CswNbtResources CswNbtResources )
        {
            return CswNbtActCreateMaterial.getMaterialUnitsOfMeasure( MaterialId, CswNbtResources );
        }

        public JObject getSizeLogicalsVisibility( int SizeNodeTypeId )
        {
            return _CswNbtActCreateMaterial.getSizeLogicalsVisibility( SizeNodeTypeId );
        }

        #endregion Public
    }
}