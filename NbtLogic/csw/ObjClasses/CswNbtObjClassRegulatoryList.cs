using System;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRegulatoryList : CswNbtObjClass
    {
        public const string CASNumbersPropertyName = "CAS Numbers";
        public const string NamePropertyName = "Name";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRegulatoryList( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RegulatoryListClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRegulatoryList
        /// </summary>
        public static implicit operator CswNbtObjClassRegulatoryList( CswNbtNode Node )
        {
            CswNbtObjClassRegulatoryList ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RegulatoryListClass ) )
            {
                ret = (CswNbtObjClassRegulatoryList) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( CASNumbers.WasModified || Name.WasModified )
            {
                //remove this list from all material nodes
                _removeListFromMaterials();

                //start the batch operation to update
                CswNbtBatchOpUpdateRegulatoryLists BatchOp = new CswNbtBatchOpUpdateRegulatoryLists( _CswNbtResources );
                CswCommaDelimitedString CASNosAsCommaString = new CswCommaDelimitedString();
                CASNosAsCommaString.FromString( CASNumbers.Text );
                BatchOp.makeBatchOp( Name.Text, CASNosAsCommaString );
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _removeListFromMaterials();
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo CASNumbers { get { return _CswNbtNode.Properties[CASNumbersPropertyName]; } }
        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[NamePropertyName]; } }

        #endregion

        #region private helper functions
        private void _removeListFromMaterials()
        {
            CswNbtView materialsWithThisList = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp regListsOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.RegulatoryLists );
            CswNbtViewRelationship parent = materialsWithThisList.AddViewRelationship( materialOC, false );
            string OriginalName = Name.GetOriginalPropRowValue();
            materialsWithThisList.AddViewPropertyAndFilter( parent, regListsOCP, Value: OriginalName, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );

            ICswNbtTree materialsWithListTree = _CswNbtResources.Trees.getTreeFromView( materialsWithThisList, false );
            int nodeCount = materialsWithListTree.getChildNodeCount();
            for( int i = 0; i < nodeCount; i++ )
            {
                materialsWithListTree.goToNthChild( i );
                CswNbtObjClassMaterial nodeAsMaterial = (CswNbtObjClassMaterial) materialsWithListTree.getNodeForCurrentPosition();
                CswCommaDelimitedString regLists = new CswCommaDelimitedString();
                regLists.FromString( nodeAsMaterial.RegulatoryLists.StaticText );
                regLists.Remove( OriginalName );
                nodeAsMaterial.RegulatoryLists.StaticText = regLists.ToString();
                nodeAsMaterial.postChanges( false );
                materialsWithListTree.goToParentNode();
            }
        }
        #endregion

    }//CswNbtObjClassRegulatoryList

}//namespace ChemSW.Nbt.ObjClasses
