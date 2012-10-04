using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using System.Collections.Generic;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassVendor : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string VendorName = "Vendor Name";
            public const string CorporateEntityName = "Corporate Entity";
            public const string VendorTypeName = "Vendor Type";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassVendor( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.VendorClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassVendor
        /// </summary>
        public static implicit operator CswNbtObjClassVendor( CswNbtNode Node )
        {
            CswNbtObjClassVendor ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClassName.NbtObjectClass.VendorClass ) )
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
                            vendorNode.VendorType.Value.Equals( "Corporate" ) &&
                            this.VendorType.Value.Equals( "Corporate" ) )
                        {
                            throw new CswDniException( ErrorType.Warning,
                                    "Multiple Corporate Entities with a Vendor Type of \"Corporate\" are not allowed",
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

        public CswNbtNodePropText VendorName { get { return ( _CswNbtNode.Properties[PropertyName.VendorName] ); } }
        public CswNbtNodePropText CorporateIdentity { get { return ( _CswNbtNode.Properties[PropertyName.CorporateEntityName] ); } }
        public CswNbtNodePropList VendorType { get { return ( _CswNbtNode.Properties[PropertyName.VendorTypeName] ); } }

        #endregion


    }//CswNbtObjClassVendor

}//namespace ChemSW.Nbt.ObjClasses
