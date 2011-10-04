//using System;
//using System.Collections.Generic;
//using System.Collections;
//using System.Text;
//using System.Data;
//using ChemSW.Core;
//using ChemSW.Nbt.PropTypes;
//using ChemSW.Exceptions;
//using ChemSW.Nbt.MetaData;
//using ChemSW.Nbt.PropertySets;


//namespace ChemSW.Nbt.ObjClasses
//{
//    public class CswNbtObjClassVendor : CswNbtObjClass
//    {
//        public static string AccountNoPropertyName { get { return "Account No"; } }
//        public static string CityPropertyName { get { return "City"; } }
//        public static string ContactNamePropertyName { get { return "Contact Name"; } }
//        public static string EmailPropertyName { get { return "Email"; } }
//        public static string FaxPropertyName { get { return "Fax"; } }
//        public static string IsApprovedVendorPropertyName { get { return "Is Approved Vendor"; } }
//        public static string ObsoletePropertyName { get { return "Obsolete"; } }
//        public static string PhonePropertyName { get { return "Phone"; } }
//        public static string StatePropertyName { get { return "State"; } }
//        public static string Street1PropertyName { get { return "Street1"; } }
//        public static string Street2PropertyName { get { return "Street2"; } }
//        public static string VendorNamePropertyName { get { return "Vendor Name"; } }
//        public static string ZipPropertyName { get { return "Zip"; } }
//        public static string DivisionPropertyName { get { return "Division"; } }
//        public static string CountryPropertyName { get { return "Country"; } }


//        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

//        public CswNbtObjClassVendor( CswNbtResources CswNbtResources, CswNbtNode Node )
//            : base( CswNbtResources, Node )
//        {
//            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
//        }//ctor()

//        public CswNbtObjClassVendor( CswNbtResources CswNbtResources )
//            : base( CswNbtResources )
//        {
//            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
//        }//ctor()

//        public override CswNbtMetaDataObjectClass ObjectClass
//        {
//            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass ); }
//        }

//        #region Inherited Events
//        public override void beforeCreateNode( bool OverrideUniqueValidation )
//        {
//            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
//        } // beforeCreateNode()

//        public override void afterCreateNode()
//        {
//            _CswNbtObjClassDefault.afterCreateNode();
//        } // afterCreateNode()

//        public override void beforeWriteNode( bool OverrideUniqueValidation )
//        {
//            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
//        }//beforeWriteNode()

//        public override void afterWriteNode()
//        {
//            _CswNbtObjClassDefault.afterWriteNode();
//        }//afterWriteNode()

//        public override void beforeDeleteNode()
//        {
//            _CswNbtObjClassDefault.beforeDeleteNode();
//        }//beforeDeleteNode()

//        public override void afterDeleteNode()
//        {
//            _CswNbtObjClassDefault.afterDeleteNode();
//        }//afterDeleteNode()        

//        public override void afterPopulateProps()
//        {
//            _CswNbtObjClassDefault.afterPopulateProps();
//        }//afterPopulateProps()

//        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
//        {
//            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
//        }

//        #endregion

//        #region Object class specific properties

//        public CswNbtNodePropText AccountNo { get { return _CswNbtNode.Properties[AccountNoPropertyName].AsText; } }
//        public CswNbtNodePropText City { get { return _CswNbtNode.Properties[CityPropertyName].AsText; } }
//        public CswNbtNodePropText ContactName { get { return _CswNbtNode.Properties[ContactNamePropertyName].AsText; } }
//        public CswNbtNodePropText Email { get { return _CswNbtNode.Properties[EmailPropertyName].AsText; } }
//        public CswNbtNodePropText Fax { get { return _CswNbtNode.Properties[FaxPropertyName].AsText; } }
//        public CswNbtNodePropLogical IsApprovedVendor { get { return _CswNbtNode.Properties[IsApprovedVendorPropertyName].AsLogical; } }
//        public CswNbtNodePropLogical Obsolete { get { return _CswNbtNode.Properties[ObsoletePropertyName].AsLogical; } }
//        public CswNbtNodePropText Phone { get { return _CswNbtNode.Properties[PhonePropertyName].AsText; } }
//        public CswNbtNodePropText State { get { return _CswNbtNode.Properties[StatePropertyName].AsText; } }
//        public CswNbtNodePropText Street1 { get { return _CswNbtNode.Properties[Street1PropertyName].AsText; } }
//        public CswNbtNodePropText Street2 { get { return _CswNbtNode.Properties[Street2PropertyName].AsText; } }
//        public CswNbtNodePropText VendorName { get { return _CswNbtNode.Properties[VendorNamePropertyName].AsText; } }
//        public CswNbtNodePropText Zip { get { return _CswNbtNode.Properties[ZipPropertyName].AsText; } }
//        public CswNbtNodePropText Division { get { return _CswNbtNode.Properties[DivisionPropertyName].AsText; } }
//        public CswNbtNodePropText Country { get { return _CswNbtNode.Properties[CountryPropertyName].AsText; } }


//        #endregion

//    }//CswNbtObjClassMailReport

//}//namespace ChemSW.Nbt.ObjClasses
