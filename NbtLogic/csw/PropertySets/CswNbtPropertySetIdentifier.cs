//using System;
//using System.Collections.Generic;
//using System.Text;
//using ChemSW.Nbt;
//using ChemSW.Nbt.MetaData;

//namespace ChemSW.Nbt.PropertySets
//{
//    /// <summary>
//    /// This class is responsible for telling if a nodetype or object class implements a given property set
//    /// </summary>
//    public class CswNbtPropertySetIdentifier
//    {
//        /// <summary>
//        /// Property Sets
//        /// </summary>
//        public enum CswNbtPropertySet
//        {
//            /// <summary>
//            /// NodeTypes and Object Classes that the Scheduler can use
//            /// </summary>
//            Scheduler,
//            /// <summary>
//            /// NodeTypes that can be the target of a Generator class
//            /// </summary>
//            GeneratorTarget
//        }

//        public static bool DoesNodeTypeImplementPropertySet( CswNbtResources CswNbtResources, CswNbtMetaDataNodeType NodeType, CswNbtPropertySet PropertySet )
//        {
//            bool ret = false;
//            switch( PropertySet )
//            {
//                case CswNbtPropertySet.GeneratorTarget:
//                    ret = ( NodeType.ObjectClass is ICswNbtPropertySetGeneratorTarget );
//                    break;
//                case CswNbtPropertySet.Scheduler:
//                    ret = ( NodeType.ObjectClass is ICswNbtPropertySetScheduler );
//                    break;
//            }
//            return ret;
//        }


//        //public static Int32 GetNodeTypePropIdForPropertySetProp(CswNbtMetaDataNodeType NodeType, CswNbtPropertySet PropertySet, string PropertySetPropName)
//        //{
//        //    switch( PropertySet )
//        //    {
//        //        case CswNbtPropertySet.GeneratorTarget:
                    
//        //            switch(PropertySetPropName)
//        //                case "GeneratedDate":
//        //                    return ();


//        //            break;
//        //        case CswNbtPropertySet.Scheduler:
//        //            ret = ( NodeType.ObjectClass is ICswNbtPropertySetScheduler );
//        //            break;
//        //    }
//        //}


//    }
//}
