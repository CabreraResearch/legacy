using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    class CswNbtObjClassTestingUserLabAssignment : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string User = "User";
            public const string TestingLab = "Testing Lab";
        }

        public CswNbtObjClassTestingUserLabAssignment( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingUserLabAssignmentClass); }
        }

        /// <summary>
        /// Convert a CswNbtNode to CswNbtObjClassTestingUserLabAssignment
        /// </summary>
        public static implicit operator CswNbtObjClassTestingUserLabAssignment( CswNbtNode Node )
        {
            CswNbtObjClassTestingUserLabAssignment ret = null;
            if( null !=Node && _Validate(Node, CswEnumNbtObjectClass.TestingUserLabAssignmentClass) )
            {
                ret = (CswNbtObjClassTestingUserLabAssignment)Node.ObjClass;
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

    }//CswNbtObjClassTestingUserLabAssignment

}//namespace ChemSW.Nbt.ObjClasses