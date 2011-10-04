using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassPackage : CswNbtObjClass
    {
        public static string ManufacturerIdPropertyName { get { return "Manufacturer ID"; } }
        public static string MaterialIdPropertyName { get { return "Material ID"; } }
        public static string ObsoletePropertyName { get { return "Obsolete"; } }
        public static string DescriptionPropertyName { get { return "Product Description"; } }
        public static string ProductNoPropertyName { get { return "Product No"; } }
        public static string SupplierIdPropertyName { get { return "Supplier ID"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassPackage( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassPackage( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PackageClass ); }
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

        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();
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

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship ManufacturerId { get { return _CswNbtNode.Properties[ManufacturerIdPropertyName].AsRelationship; } }
        public CswNbtNodePropRelationship MaterialId { get { return _CswNbtNode.Properties[MaterialIdPropertyName].AsRelationship; } }
        public CswNbtNodePropLogical Obsolete { get { return _CswNbtNode.Properties[ObsoletePropertyName].AsLogical; } }
        public CswNbtNodePropText Description { get { return _CswNbtNode.Properties[DescriptionPropertyName].AsText; } }
        public CswNbtNodePropText ProductNo { get { return _CswNbtNode.Properties[ProductNoPropertyName].AsText; } }
        public CswNbtNodePropRelationship SupplierId { get { return _CswNbtNode.Properties[SupplierIdPropertyName].AsRelationship; } }

        #endregion

    }//CswNbtObjClassMailReport

}//namespace ChemSW.Nbt.ObjClasses
