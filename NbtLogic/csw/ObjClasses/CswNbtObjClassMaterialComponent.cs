using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMaterialComponent : CswNbtObjClass
    {
        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMaterialComponent( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass ); }
        }

        public static string PercentagePropertyName { get { return "Percentage"; } }
        public static string MixturePropertyName { get { return "Mixture"; } }
        public static string ConstituentPropertyName { get { return "Constituent"; } }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMaterialComponent
        /// </summary>
        public static implicit operator CswNbtObjClassMaterialComponent( CswNbtNode Node )
        {
            CswNbtObjClassMaterialComponent ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass ) )
            {
                ret = (CswNbtObjClassMaterialComponent) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( null != Mixture.RelatedNodeId && null != Constituent.RelatedNodeId )
            {
                if( Mixture.RelatedNodeId == Constituent.RelatedNodeId )
                {
                    throw new CswDniException( ErrorType.Warning, "Mixture material and Constituent material cannot be the same", "Mixture material and Constituent material cannot be the same" );
                }
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
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

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropNumber Percentage
        {
            get
            {
                return ( _CswNbtNode.Properties[PercentagePropertyName].AsNumber );
            }
        }

        public CswNbtNodePropRelationship Mixture
        {
            get
            {
                return ( _CswNbtNode.Properties[MixturePropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropRelationship Constituent
        {
            get
            {
                return ( _CswNbtNode.Properties[ConstituentPropertyName] );
            }
        }

        #endregion


    }//CswNbtObjClassMaterialComponent

}//namespace ChemSW.Nbt.ObjClasses
