using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassWorkUnit : CswNbtObjClass
    {
        public const string AuditingEnabledPropertyName = "Auditing Enabled";
        public const string SignatureRequiredPropertyName = "Signature Required";
        public const string NamePropertyName = "Name";
        
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassWorkUnit( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassWorkUnit
        /// </summary>
        public static implicit operator CswNbtObjClassWorkUnit( CswNbtNode Node )
        {
            CswNbtObjClassWorkUnit ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.WorkUnitClass ) )
            {
                ret = (CswNbtObjClassWorkUnit) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode(bool DeleteAllRequiredRelatedNodes = false)
        {
            _CswNbtObjClassDefault.beforeDeleteNode(DeleteAllRequiredRelatedNodes);

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

        public CswNbtNodePropNodeTypeSelect AuditingEnabled { get { return ( _CswNbtNode.Properties[AuditingEnabledPropertyName] ); } }
        public CswNbtNodePropNodeTypeSelect SignatureRequired { get { return ( _CswNbtNode.Properties[SignatureRequiredPropertyName] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[NamePropertyName] ); } }

        #endregion


    }//CswNbtObjClassWorkUnit

}//namespace ChemSW.Nbt.ObjClasses
