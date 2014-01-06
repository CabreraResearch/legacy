using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTarget: CswNbtObjClass, ICswNbtPropertySetInspectionParent
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            //public static string LastInspectionDatePropertyName { get { return "Last Inspection Date"; } }
            public const string Status = "Status";
            public const string Location = "Location";
            public const string Description = "Description";
            public const string Barcode = "Barcode";
            public const string InspectionTargetGroup = "Inspection Target Group";
        }

        public string InspectionParentStatusPropertyName { get { return PropertyName.Status; } }


        public CswNbtObjClassInspectionTarget( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionTarget
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionTarget( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTarget ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InspectionTargetClass ) )
            {
                ret = (CswNbtObjClassInspectionTarget) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        /// <summary>
        /// Inspection Target Inspection Status (OK, Deficient)
        /// </summary>
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }

        /// <summary>
        /// Location of Inspection Target
        /// </summary>
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        public CswNbtNodePropText Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropRelationship InspectionTargetGroup
        {
            get { return ( _CswNbtNode.Properties[PropertyName.InspectionTargetGroup] ); }
        }

        #endregion

    }//CswNbtObjClassInspectionTarget

}//namespace ChemSW.Nbt.ObjClasses
