using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGeneric : CswNbtObjClass
    {
        public CswNbtObjClassGeneric( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGeneric
        /// </summary>
        public static implicit operator CswNbtObjClassGeneric( CswNbtNode Node )
        {
            CswNbtObjClassGeneric ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.GenericClass ) )
            {
                ret = (CswNbtObjClassGeneric) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events    

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        // GENERIC SHOULD NOT HAVE ANY!!!

        #endregion

    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
