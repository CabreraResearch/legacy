//using System;
//using ChemSW.Nbt.ObjClasses;

//namespace ChemSW.Nbt
//{
//    public class CswNbtNotificationKey : IEquatable<CswNbtNotificationKey>
//    {
//        public Int32 NodeTypeId;
//        public CswNbtObjClassMailReport.EventOption EventOpt;
//        public string PropName;
//        public string TargetValue;

//        public CswNbtNotificationKey( Int32 inNodeTypeId, CswNbtObjClassMailReport.EventOption inEventOpt, string inPropName, string inTargetValue )
//        {
//            NodeTypeId = inNodeTypeId;
//            EventOpt = inEventOpt;
//            PropName = inPropName;
//            TargetValue = inTargetValue;
//        }

//        #region IEquatable

//        public static bool operator ==( CswNbtNotificationKey k1, CswNbtNotificationKey k2 )
//        {
//            // If both are null, or both are same instance, return true.
//            if( System.Object.ReferenceEquals( k1, k2 ) )
//            {
//                return true;
//            }

//            // If one is null, but not both, return false.
//            if( ( (object) k1 == null ) || ( (object) k2 == null ) )
//            {
//                return false;
//            }

//            // Now we know neither are null.  Compare values.
//            if( k1.NodeTypeId == k2.NodeTypeId &&
//                k1.EventOpt == k2.EventOpt &&
//                k1.PropName == k2.PropName &&
//                k1.TargetValue == k2.TargetValue )
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public static bool operator !=( CswNbtNotificationKey k1, CswNbtNotificationKey k2 )
//        {
//            return !( k1 == k2 );
//        }

//        public override bool Equals( object obj )
//        {
//            if( !( obj is CswNbtNotificationKey ) )
//                return false;
//            return this == (CswNbtNotificationKey) obj;
//        }

//        public bool Equals( CswNbtNotificationKey obj )
//        {
//            return this == (CswNbtNotificationKey) obj;
//        }

//        public override int GetHashCode()
//        {
//            return ( NodeTypeId.ToString() + "_" + EventOpt.ToString() + "_" + PropName + "_" + TargetValue ).GetHashCode();
//        }

//        #endregion IEquatable
//    }
//}
