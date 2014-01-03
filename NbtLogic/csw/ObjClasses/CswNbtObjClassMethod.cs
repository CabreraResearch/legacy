using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMethod : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string MethodNo = "Method No";
            public const string MethodDescription = "Method Description";
        }

        public CswNbtObjClassMethod( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MethodClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMethod
        /// </summary>
        public static implicit operator CswNbtObjClassMethod( CswNbtNode Node )
        {
            CswNbtObjClassMethod ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MethodClass ) )
            {
                ret = (CswNbtObjClassMethod) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText MethodNo { get { return _CswNbtNode.Properties[PropertyName.MethodNo]; } }
        public CswNbtNodePropText MethodDescription { get { return _CswNbtNode.Properties[PropertyName.MethodDescription]; } }

        #endregion

    }//CswNbtObjClassMethod

}//namespace ChemSW.Nbt.ObjClasses
