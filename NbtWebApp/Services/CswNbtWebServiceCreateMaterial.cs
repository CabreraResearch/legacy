using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceCreateMaterial
    {
        #region ctor

        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceCreateMaterial( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            if( false == _CswNbtResources.Permit.can( CswNbtActionName.Create_Material, _CswNbtResources.CurrentNbtUser ) )
            {
                throw new CswDniException( ErrorType.Error, "You do not have permission to use the Create Material wizard.", "Attempted to access the Create Material wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }
        }

        #endregion ctor


        #region Public

        public JObject getMaterial( Int32 NodeTypeId, string Supplier, string Tradename, string PartNo )
        {
            JObject RetObj = new JObject();

            if( Int32.MinValue == NodeTypeId ||
                 string.IsNullOrEmpty( Supplier ) ||
                 string.IsNullOrEmpty( Tradename ) )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get a material without a type, supplier and a tradename.",
                                           "Attempted to call getMaterial with invalid or empty paramters." );
            }
            CswNbtMetaDataNodeType MaterialNt = _CswNbtResources.MetaData.getNodeType( NodeTypeId );

            if( null == MaterialNt ||
                 MaterialNt.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided material type was not a valid material.",
                                           "Attempted to call getMaterial with a NodeType that was not valid." );
            }

            CswNbtView MaterialNodeView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship MaterialRel = MaterialNodeView.AddViewRelationship( MaterialNt, false );
            MaterialNodeView.AddViewPropertyFilter( MaterialRel,
                                                    MaterialNt.getNodeTypePropByObjectClassProp(
                                                        CswNbtObjClassMaterial.TradenamePropName ), Tradename );
            MaterialNodeView.AddViewPropertyFilter( MaterialRel,
                                                    MaterialNt.getNodeTypePropByObjectClassProp(
                                                        CswNbtObjClassMaterial.SupplierPropertyName ), Supplier );
            MaterialNodeView.AddViewPropertyFilter( MaterialRel,
                                                    MaterialNt.getNodeTypePropByObjectClassProp(
                                                        CswNbtObjClassMaterial.PartNumberPropertyName ), Tradename );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( MaterialNodeView, false );
            if( Tree.getChildNodeCount() > 0 )
            {
                Tree.goToNthChild( 0 );
                CswNbtNode Node = Tree.getNodeForCurrentPosition();
                CswNbtObjClassMaterial NodeAsMaterial = CswNbtNodeCaster.AsMaterial( Node );
                RetObj["tradename"] = NodeAsMaterial.TradeName.Text;
                RetObj["partno"] = NodeAsMaterial.PartNumber.Text;
                RetObj["supplier"] = NodeAsMaterial.Supplier.Gestalt;
            }

            RetObj["succeeded"] = true;
            return RetObj;
        }

        public CswNbtView getMaterialSizes( CswPrimaryKey MaterialId )
        {
            CswNbtView RetView;

            if( null == MaterialId )
            {
                throw new CswDniException( ErrorType.Error,
                                           "Cannot get material's sizes without a valid materialid.",
                                           "Attempted to call getMaterialSizes with invalid or empty paramters." );
            }

            CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( MaterialId );

            if( null == MaterialNode ||
                 MaterialNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass )
            {
                throw new CswDniException( ErrorType.Error,
                                           "The provided node was not a valid material.",
                                           "Attempted to call getMaterialSizes with a node that was not valid." );
            }

            RetView = new CswNbtView( _CswNbtResources );  //MaterialNode.getNodeType().CreateDefaultView();
            RetView.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp SizeMaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
            CswNbtViewRelationship SizeRel = RetView.AddViewRelationship( SizeOc, false );

            RetView.AddViewPropertyFilter( SizeRel, SizeMaterialOcp, MaterialId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
            RetView.AddViewPropertyFilter( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName ) );
            RetView.AddViewPropertyFilter( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName ) );
            RetView.AddViewPropertyFilter( SizeRel, SizeOc.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName ) );

            return RetView;
        }

        #endregion Public

    }
}