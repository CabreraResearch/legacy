using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassVendor : CswNbtObjClass
    {

        public const string VendorNamePropertyName = "Vendor Name";
        public const string CorporateEntityPropertyName = "Corporate Entity";
        public const string VendorTypePropertyName = "Vendor Type";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassVendor( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassVendor
        /// </summary>
        public static implicit operator CswNbtObjClassVendor( CswNbtNode Node )
        {
            CswNbtObjClassVendor ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass ) )
            {
                ret = (CswNbtObjClassVendor) Node.ObjClass;
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
            if( VendorType.WasModified || CorporateIdentity.WasModified )
            {
                Dictionary<string, string> corporates = new Dictionary<string, string>();
                foreach( CswNbtObjClassVendor vendorNode in this.NodeType.getNodes( false, false ) )
                {
                    if( vendorNode.VendorType.Value.Equals( "Corporate" ) )
                    {
                        if( corporates.ContainsKey( vendorNode.CorporateIdentity.Text ) )
                        {
                            throw new CswDniException( ErrorType.Warning,
                                "Multiple Corporate Entities with a Vendor Type of Corporate are not allowed",
                                "A Vendor with a Corporate Entity of " + vendorNode.CorporateIdentity.Text + " already exists with a Vendor Type of " + vendorNode.VendorType.Value );
                        }
                        else
                        {
                            corporates.Add( vendorNode.CorporateIdentity.Text, "Corporate" );
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

        public CswNbtNodePropText VendorName { get { return ( _CswNbtNode.Properties[VendorNamePropertyName] ); } }
        public CswNbtNodePropText CorporateIdentity { get { return ( _CswNbtNode.Properties[CorporateEntityPropertyName] ); } }
        public CswNbtNodePropList VendorType { get { return ( _CswNbtNode.Properties[VendorTypePropertyName] ); } }

        #endregion


    }//CswNbtObjClassVendor

}//namespace ChemSW.Nbt.ObjClasses
