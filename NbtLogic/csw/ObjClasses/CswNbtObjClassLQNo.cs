using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassLQNo : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string LQNo = "LQNo";
            public const string Limit = "Limit";
        }

        #region ctor

        public CswNbtObjClassLQNo( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LQNoClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassLQNo
        /// </summary>
        public static implicit operator CswNbtObjClassLQNo( CswNbtNode Node )
        {
            CswNbtObjClassLQNo ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.LQNoClass ) )
            {
                ret = (CswNbtObjClassLQNo) Node.ObjClass;
            }
            return ret;
        }

        #endregion ctor

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText LQNo { get { return ( _CswNbtNode.Properties[PropertyName.LQNo] ); } }
        public CswNbtNodePropQuantity Limit { get { return ( _CswNbtNode.Properties[PropertyName.Limit] ); } }

        #endregion

    }//CswNbtObjClassLQNo

}//namespace ChemSW.Nbt.ObjClasses