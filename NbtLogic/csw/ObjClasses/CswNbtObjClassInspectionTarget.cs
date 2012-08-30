using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTarget : CswNbtObjClass, ICswNbtPropertySetInspectionParent
    {
        public sealed class PropertyName
        {
            //public static string LastInspectionDatePropertyName { get { return "Last Inspection Date"; } }
            public const string Status = "Status";
            public const string Location = "Location";
            public const string Description = "Description";
            public const string Barcode = "Barcode";
            public const string InspectionTargetGroup = "Inspection Target Group";
        }



        //ICswNbtPropertySetInspectionParent
        public string InspectionParentStatusPropertyName { get { return PropertyName.Status; } }
        //public string InspectionParentLastInspectionDatePropertyName { get { return LastInspectionDatePropertyName; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInspectionTarget( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionTarget
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionTarget( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTarget ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass ) )
            {
                ret = (CswNbtObjClassInspectionTarget) Node.ObjClass;
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
        /*
        /// <summary>
        /// Date of last Inspection
        /// </summary>
        public CswNbtNodePropDateTime LastInspectionDate
        {
            get
            {
                return ( _CswNbtNode.Properties[LastInspectionDatePropertyName].AsDateTime );
            }
        }
        */

        /// <summary>
        /// Inspection Target Inspection Status (OK, Deficient)
        /// </summary>
        public CswNbtNodePropList Status
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Status].AsList );
            }
        }

        /// <summary>
        /// Location of Inspection Target
        /// </summary>
        public CswNbtNodePropLocation Location
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Location].AsLocation );
            }
        }

        public CswNbtNodePropText Description
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Description].AsText );
            }
        }

        public CswNbtNodePropBarcode Barcode
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Barcode].AsBarcode );
            }
        }

        public CswNbtNodePropRelationship InspectionTargetGroup
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.InspectionTargetGroup].AsRelationship );
            }
        }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
