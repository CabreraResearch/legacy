using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
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
                    throw ( new CswDniException( ErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetScheduler; Current object class is " + Node.getObjectClass().ObjectClass.ToString() ) );
                return ( (ICswNbtPropertySetScheduler) Node.ObjClass );
            }
            else
            {
                return null;
            }
        }//AsPropertySetScheduler

        public static ICswNbtPropertySetInspectionParent AsPropertySetInspectionParent( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetInspectionParent ) )
                    throw ( new CswDniException( ErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetInspectionParent; Current object class is " + Node.getObjectClass().ObjectClass.ToString() ) );
                return ( (ICswNbtPropertySetInspectionParent) Node.ObjClass );
            }
            else
            {
                return null;
            }
        }//AsPropertySetInspectionParent

    } // class CswNbtPropSetCaster
}