using System;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtInspectionTargetStatus
    {
        public enum TargetStatus
        {
            /// <summary>
            /// Not yet inspected
            /// </summary>
            Not_Inspected,

            /// <summary>
            /// Last inspection complete and in compliance
            /// </summary>
            OK,

            /// <summary>
            /// Deficient, Out of compliance
            /// </summary>
            Deficient,

            /// <summary>
            /// For unset values
            /// </summary>
            Null
        }

        /// <summary>
        /// Returns Target status as string from TargetStatus Enum
        /// </summary>
        public static string TargetStatusAsString( TargetStatus Status )
        {
            string ret = string.Empty;
            if( Status != TargetStatus.Null )
                ret = Status.ToString().Replace( '_', ' ' );
            return ret;
        }

        /// <summary>
        /// Replaces space with underscore in enum
        /// </summary>
        public static TargetStatus TargetStatusFromString( string Status )
        {
            TargetStatus ret;
            if( !Enum.TryParse( Status.Replace( ' ', '_' ), out ret ) )
                ret = TargetStatus.Null;
            return ret;
        }
    }

}//namespace ChemSW.Nbt.ObjClasses
