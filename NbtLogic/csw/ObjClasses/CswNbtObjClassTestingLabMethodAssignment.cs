using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassTestingLabMethodAssignment : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string TestingLab = "Testing Lab";
            public const string Method = "Method";
            public const string Cost = "Cost";
        }

        public CswNbtObjClassTestingLabMethodAssignment( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabMethodAssignmentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to CswNbtObjClassTestingLabMethodAssignment
        /// </summary>
        public static implicit operator CswNbtObjClassTestingLabMethodAssignment( CswNbtNode Node )
        {
            CswNbtObjClassTestingLabMethodAssignment ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.TestingLabMethodAssignmentClass ) )
            {
                ret = (CswNbtObjClassTestingLabMethodAssignment) Node.ObjClass;
            }
            return ret;
        }

        #region Object class specific properties

        public CswNbtNodePropRelationship TestingLab
        {
            get { return _CswNbtNode.Properties[PropertyName.TestingLab]; }
        }

        public CswNbtNodePropRelationship Method
        {
            get { return _CswNbtNode.Properties[PropertyName.Method]; }
        }

        public CswNbtNodePropRelationship Cost
        {
            get { return _CswNbtNode.Properties[PropertyName.Cost]; }
        }


        #endregion

    }//CswNbtObjClassTestingLabMethodAssignment

}//namespace ChemSW.Nbt.ObjClasses