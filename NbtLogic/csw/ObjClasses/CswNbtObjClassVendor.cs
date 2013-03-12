using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassVendor : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string VendorName = "Vendor Name";
            public const string CorporateEntityName = "Corporate Entity";
            public const string VendorTypeName = "Vendor Type";
        }

        public sealed class VendorTypes
        {
            public const string Corporate = "Corporate";
            public const string Sales = "Sales";
            public const string Technical = "Technical";
            public const string Manufacturing = "Manufacturing";

            public CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Corporate,
                    Sales,
                    Technical,
                    Manufacturing
                };
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassVendor( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.VendorClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassVendor
        /// </summary>
        public static implicit operator CswNbtObjClassVendor( CswNbtNode Node )
        {
            CswNbtObjClassVendor ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.VendorClass ) )
            {
                ret = (CswNbtObjClassVendor) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( VendorType.WasModified || CorporateIdentity.WasModified )
            {
                //For each Corporate Entity, there can only be one vendortype of Corporate aka Highlander attribute
                if( false == VendorType.Empty && false == CorporateIdentity.Empty )
                {
                    foreach( CswNbtObjClassVendor vendorNode in this.NodeType.getNodes( false, false ) )
                    {
                        if( vendorNode.NodeId != this.NodeId &&
                            vendorNode.CorporateIdentity.Text.Equals( CorporateIdentity.Text ) &&
                            vendorNode.VendorType.Value.Equals( VendorTypes.Corporate ) &&
                            this.VendorType.Value.Equals( VendorTypes.Corporate ) )
                        {
                            throw new CswDniException( ErrorType.Warning,
                                    "Multiple Corporate Entities with a Vendor Type of " + VendorTypes.Corporate + " are not allowed",
                                    "A Vendor with a Corporate Entity of " + vendorNode.CorporateIdentity.Text + " already exists with a Vendor Type of " + vendorNode.VendorType.Value );
                        }
                    }
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

        public CswNbtNodePropText VendorName { get { return ( _CswNbtNode.Properties[PropertyName.VendorName] ); } }
        public CswNbtNodePropText CorporateIdentity { get { return ( _CswNbtNode.Properties[PropertyName.CorporateEntityName] ); } }
        public CswNbtNodePropList VendorType { get { return ( _CswNbtNode.Properties[PropertyName.VendorTypeName] ); } }

        #endregion


    }//CswNbtObjClassVendor

}//namespace ChemSW.Nbt.ObjClasses
