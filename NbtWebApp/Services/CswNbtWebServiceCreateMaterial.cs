using System;
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

        #endregion Public

    }
}