using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Container Dispense Transaction Object Class
    /// </summary>
    public class CswNbtObjClassContainerDispenseTransaction : CswNbtObjClass
    {
        #region Static Properties

        public static string SourceContainerPropertyName { get { return "Source Container"; } }
        public static string DestinationContainerPropertyName { get { return "Destination Container"; } }
        public static string QuantityDispensedPropertyName { get { return "Quantity Dispensed"; } }
        public static string TypePropertyName { get { return "Dispense Type"; } }
        public static string DispensedDatePropertyName { get { return "Dispensed Date"; } }
        public static string RemainingSourceContainerQuantityPropertyName { get { return "Remaining Source Container Quantity"; } }
        public static string RequestItemPropertyName { get { return "Request Item"; } }

        public sealed class DispenseType : CswEnum<DispenseType>
        {
            private DispenseType( string Name ) : base( Name ) { }
            public static IEnumerable<DispenseType> _All { get { return All; } }
            public static implicit operator DispenseType( string str )
            {
                DispenseType ret = Parse( str );
                return ret ?? Unknown;
            }
            public static readonly DispenseType Unknown = new DispenseType( "Unknown" );
            /// <summary>
            /// Add new (child) containers with material specified in existing source container (no parent container)
            /// </summary>
            public static readonly DispenseType Receive = new DispenseType( "Receive" );
            /// <summary>
            /// Transfer material from a source (parent) container to zero or more destination (child) containers
            /// </summary>
            public static readonly DispenseType Dispense = new DispenseType( "Dispense" );
            /// <summary>
            /// Transfer material from a source (parent) container to an undocumented location (no child containers)
            /// </summary>
            public static readonly DispenseType Waste = new DispenseType( "Waste" );
            /// <summary>
            /// Empty material from a source (parent) container and mark as disposed (no child containers)
            /// </summary>
            public static readonly DispenseType Dispose = new DispenseType( "Dispose" );
            /// <summary>
            /// Add material to an existing source container (no parent container, no child containers)
            /// </summary>
            public static readonly DispenseType Add = new DispenseType( "Add" );
        }

        #endregion

        #region ctor

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainerDispenseTransaction( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainerDispenseTransaction
        /// </summary>
        public static implicit operator CswNbtObjClassContainerDispenseTransaction( CswNbtNode Node )
        {
            CswNbtObjClassContainerDispenseTransaction ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ContainerDispenseTransactionClass ) )
            {
                ret = (CswNbtObjClassContainerDispenseTransaction) Node.ObjClass;
            }
            return ret;
        }

        #endregion

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
            _CswNbtNode.ReadOnly = true;//case 24508 - is this what we want?

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

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            
            
            
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship SourceContainer
        {
            get { return _CswNbtNode.Properties[SourceContainerPropertyName]; }
        }
        public CswNbtNodePropRelationship DestinationContainer
        {
            get { return _CswNbtNode.Properties[DestinationContainerPropertyName]; }
        }
        public CswNbtNodePropQuantity QuantityDispensed
        {
            get { return _CswNbtNode.Properties[QuantityDispensedPropertyName]; }
        }
        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[TypePropertyName]; }
        }
        public CswNbtNodePropDateTime DispensedDate
        {
            get { return _CswNbtNode.Properties[DispensedDatePropertyName]; }
        }
        public CswNbtNodePropQuantity RemainingSourceContainerQuantity
        {
            get { return _CswNbtNode.Properties[RemainingSourceContainerQuantityPropertyName]; }
        }
        public CswNbtNodePropRelationship Request
        {
            get { return _CswNbtNode.Properties[RequestItemPropertyName]; }
        }

        #endregion

    }//CswNbtObjClassContainerDispenseTransaction

}//namespace ChemSW.Nbt.ObjClasses

