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

        public static ICswNbtPropertySetPermissionGroup AsPropertySetPermissionGroup( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetPermissionGroup ) )
                    throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetPermissionGroup; Current object class is " + Node.getObjectClass().ObjectClass ) );
                return ( (ICswNbtPropertySetPermissionGroup) Node.ObjClass );
            }
            return null;
        }

        public static ICswNbtPropertySetPermissionTarget AsPropertySetPermissionTarget( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetPermissionTarget ) )
                    throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetPermissionTarget; Current object class is " + Node.getObjectClass().ObjectClass ) );
                return ( (ICswNbtPropertySetPermissionTarget) Node.ObjClass );
            }
            return null;
        }

    } // class CswNbtPropSetCaster
}