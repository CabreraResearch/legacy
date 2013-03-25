using System;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRegulatoryList: CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string CASNumbers = "CAS Numbers";
            public const string Name = "Name";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRegulatoryList( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RegulatoryListClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRegulatoryList
        /// </summary>
        public static implicit operator CswNbtObjClassRegulatoryList( CswNbtNode Node )
        {
            CswNbtObjClassRegulatoryList ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RegulatoryListClass ) )
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
                //Case 28838 - remove newline char from CASNos
                CASNumbers.Text = CASNumbers.Text.Replace( "\n", "" ).Replace( "\r", "" );
                CASNumbers.Text = Regex.Replace( CASNumbers.Text, @"\s+", "" );

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

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo CASNumbers { get { return _CswNbtNode.Properties[PropertyName.CASNumbers]; } }
        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }

        #endregion

        #region private helper functions
        private void _removeListFromMaterials()
        {
            string OriginalName = Name.GetOriginalPropRowValue();
            if( false == String.IsNullOrEmpty( OriginalName ) ) //if the original name is blank, it's a new node no materials have this on their reg lists prop
            {
                CswNbtView materialsWithThisList = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp regListsOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.RegulatoryLists );
                CswNbtViewRelationship parent = materialsWithThisList.AddViewRelationship( materialOC, false );
                materialsWithThisList.AddViewPropertyAndFilter( parent, regListsOCP, Value : OriginalName, FilterMode : CswNbtPropFilterSql.PropertyFilterMode.Contains );

                ICswNbtTree materialsWithListTree = _CswNbtResources.Trees.getTreeFromView( materialsWithThisList, false, false, false );
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
        }
        #endregion

    }//CswNbtObjClassRegulatoryList

}//namespace ChemSW.Nbt.ObjClasses
