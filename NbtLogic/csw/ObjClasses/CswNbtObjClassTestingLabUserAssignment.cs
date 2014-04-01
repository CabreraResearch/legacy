using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassTestingLabUserAssignment : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string User = "User";
            public const string TestingLab = "Testing Lab";
        }

        public CswNbtObjClassTestingLabUserAssignment( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabUserAssignmentClass); }
        }

        /// <summary>
        /// Convert a CswNbtNode to CswNbtObjClassTestingLabUserAssignment
        /// </summary>
        public static implicit operator CswNbtObjClassTestingLabUserAssignment( CswNbtNode Node )
        {
            CswNbtObjClassTestingLabUserAssignment ret = null;
            if( null !=Node && _Validate(Node, CswEnumNbtObjectClass.TestingLabUserAssignmentClass) )
            {
                ret = (CswNbtObjClassTestingLabUserAssignment)Node.ObjClass;
            }
            return ret;
        }

        #region Object class specific properties

        public CswNbtNodePropRelationship User {
            get { return _CswNbtNode.Properties[PropertyName.User]; }
        }

        public CswNbtNodePropRelationship TestingLab
        {
            get { return _CswNbtNode.Properties[PropertyName.TestingLab]; }
        }

        #endregion

    }//CswNbtObjClassTestingLabUserAssignment

}//namespace ChemSW.Nbt.ObjClasses