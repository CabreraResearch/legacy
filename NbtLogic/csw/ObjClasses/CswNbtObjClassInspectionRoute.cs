using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionRoute : CswNbtObjClass
    {
        public CswNbtObjClassInspectionRoute( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionRouteClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionRoute
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionRoute( CswNbtNode Node )
        {
            CswNbtObjClassInspectionRoute ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InspectionRouteClass ) )
            {
                ret = (CswNbtObjClassInspectionRoute) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        //Extend CswNbtObjClass events here

        #endregion

        #region Object class specific properties

        //Add ObjectClass-specific properties here

        #endregion

    }//CswNbtObjClassInspectionRoute

}//namespace ChemSW.Nbt.ObjClasses
