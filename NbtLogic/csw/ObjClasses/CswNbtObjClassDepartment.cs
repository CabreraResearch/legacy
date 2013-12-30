using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDepartment : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string DepartmentName = "Department Name";
        }

        #region ctor

        public CswNbtObjClassDepartment( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DepartmentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDepartment
        /// </summary>
        public static implicit operator CswNbtObjClassDepartment( CswNbtNode Node )
        {
            CswNbtObjClassDepartment ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DepartmentClass ) )
            {
                ret = (CswNbtObjClassDepartment) Node.ObjClass;
            }
            return ret;
        }

        #endregion ctor

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText DepartmentName { get { return ( _CswNbtNode.Properties[PropertyName.DepartmentName] ); } }

        #endregion

    }//CswNbtObjClassDepartment

}//namespace ChemSW.Nbt.ObjClasses