using ChemSW.Exceptions;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtPropSetCaster
    {
        
        public static ICswNbtPropertySetScheduler AsPropertySetScheduler( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetScheduler ) )
                    throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetScheduler; Current object class is " + Node.getObjectClass().ObjectClass ) );
                return ( (ICswNbtPropertySetScheduler) Node.ObjClass );
            }
            return null;
        }//AsPropertySetScheduler

        public static ICswNbtPropertySetInspectionParent AsPropertySetInspectionParent( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetInspectionParent ) )
                    throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetInspectionParent; Current object class is " + Node.getObjectClass().ObjectClass ) );
                return ( (ICswNbtPropertySetInspectionParent) Node.ObjClass );
            }
            return null;
        }//AsPropertySetInspectionParent

        #region Non-Property Set Interfaces (Object Classes that share behavior, but not properties)

        public static ICswNbtPermissionGroup AsPermissionGroup( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPermissionGroup ) )
                    throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetPermissionGroup; Current object class is " + Node.getObjectClass().ObjectClass ) );
                return ( (ICswNbtPermissionGroup) Node.ObjClass );
            }
            return null;
        }

        public static ICswNbtPermissionTarget AsPermissionTarget( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPermissionTarget ) )
                    throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetPermissionTarget; Current object class is " + Node.getObjectClass().ObjectClass ) );
                return ( (ICswNbtPermissionTarget) Node.ObjClass );
            }
            return null;
        }

        #endregion

    } // class CswNbtPropSetCaster
}