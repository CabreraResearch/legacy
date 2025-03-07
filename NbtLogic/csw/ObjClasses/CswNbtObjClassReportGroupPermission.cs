using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReportGroupPermission : CswNbtPropertySetPermission
    {
        #region Properties

        public new sealed class PropertyName : CswNbtPropertySetPermission.PropertyName
        {
            //Add ObjectClass-specific properties here
        }

        #endregion Properties

        #region Base

        public CswNbtObjClassReportGroupPermission( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupPermissionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReportGroupPermission
        /// </summary>
        public static implicit operator CswNbtObjClassReportGroupPermission( CswNbtNode Node )
        {
            CswNbtObjClassReportGroupPermission ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ReportGroupPermissionClass ) )
            {
                ret = (CswNbtObjClassReportGroupPermission) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Permission PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassReportGroupPermission fromPropertySet( CswNbtPropertySetPermission PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetPermission toPropertySet( CswNbtObjClassReportGroupPermission ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Inherited Events

        //Extend CswNbtPropertySetPermission events here

        #endregion Inherited Events

        #region Public Static Functions

        

        #endregion Public Static Functions

        #region Object class specific properties

        //Add ObjectClass-specific properties here

        #endregion Object class specific properties

    }//CswNbtObjClassReportGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
