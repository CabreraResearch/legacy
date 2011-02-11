using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodeCaster
    {
        private static void _Validate( CswNbtNode Node, CswNbtMetaDataObjectClass.NbtObjectClass TargetObjectClass )
        {
            if( Node == null )
                throw new CswDniException( "Invalid node", "CswNbtNodeCaster was given a null node as a parameter" );

            if( !( Node.ObjectClass.ObjectClass == TargetObjectClass ) )
                throw ( new CswDniException( "Invalid cast", "Can't cast current object class as " + TargetObjectClass.ToString() + "; Current object class is " + Node.ObjectClass.ObjectClass.ToString() ) );
        }

        public static CswNbtObjClassAliquot AsAliquot( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass );
            return ( (CswNbtObjClassAliquot) Node.ObjClass );
        }//AsAliquot

        public static CswNbtObjClassBiological AsBiological( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.BiologicalClass );
            return ( (CswNbtObjClassBiological) Node.ObjClass );
        }//AsBiological

        public static CswNbtObjClassCustomer AsCustomer( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
            return ( (CswNbtObjClassCustomer) Node.ObjClass );
        }//AsCustomer

        public static CswNbtObjClassEquipmentAssembly AsEquipmentAssembly( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );
            return ( (CswNbtObjClassEquipmentAssembly) Node.ObjClass );
        }//AsEquipmentAssembly

        public static CswNbtObjClassEquipment AsEquipment( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            return ( (CswNbtObjClassEquipment) Node.ObjClass );
        }//AsEquipment

        public static CswNbtObjClassEquipmentType AsEquipmentType( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentTypeClass );
            return ( (CswNbtObjClassEquipmentType) Node.ObjClass );
        }//AsEquipment

        public static CswNbtObjClassFireExtinguisher AsFireExtinguisher( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.FireExtinguisherClass );
            return ( (CswNbtObjClassFireExtinguisher) Node.ObjClass );
        }//AsFireExtinguisher

        public static CswNbtObjClassGenerator AsGenerator( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            return ( (CswNbtObjClassGenerator) Node.ObjClass );
        }//AsGenerator

        public static CswNbtObjClassInspectionDesign AsInspectionDesign( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            return ( (CswNbtObjClassInspectionDesign) Node.ObjClass );
        }//AsInspectionDesign

        public static CswNbtObjClassInspectionRoute AsInspectionRoute( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );
            return ( (CswNbtObjClassInspectionRoute) Node.ObjClass );
        }//AsInspectionRoute

        public static CswNbtObjClassLocation AsLocation( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            return ( (CswNbtObjClassLocation) Node.ObjClass );
        }//AsLocation

        public static CswNbtObjClassMailReport AsMailReport( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            return ( (CswNbtObjClassMailReport) Node.ObjClass );
        }//AsMailReport

        public static CswNbtObjClassInspectionTarget AsInspectionTarget( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            return ( (CswNbtObjClassInspectionTarget) Node.ObjClass );
        }//AsInspectionTarget

        public static CswNbtObjClassInspectionTargetGroup AsInspectionTargetGroup( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            return ( (CswNbtObjClassInspectionTargetGroup) Node.ObjClass );
        }//AsInspectionTarget
        
        public static CswNbtObjClassNotification AsNotification( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass );
            return ( (CswNbtObjClassNotification) Node.ObjClass );
        }//AsNotification

        public static CswNbtObjClassParameter AsParameter( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ParameterClass );
            return ( (CswNbtObjClassParameter) Node.ObjClass );
        }//AsParameter

        public static CswNbtObjClassPrintLabel AsPrintLabel( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
            return ( (CswNbtObjClassPrintLabel) Node.ObjClass );
        }//AsPrintLabel

        public static CswNbtObjClassReport AsReport( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            return ( (CswNbtObjClassReport) Node.ObjClass );
        }//AsReport

        public static CswNbtObjClassResult AsResult( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ResultClass );
            return ( (CswNbtObjClassResult) Node.ObjClass );
        }//AsResult

        public static CswNbtObjClassRole AsRole( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass );
            return ( (CswNbtObjClassRole) Node.ObjClass );
        }//AsRole

        public static CswNbtObjClassSample AsSample( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.SampleClass );
            return ( (CswNbtObjClassSample) Node.ObjClass );
        }//AsSample

        public static CswNbtObjClassTask AsTask( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );
            return ( (CswNbtObjClassTask) Node.ObjClass );
        }//AsTask

        public static CswNbtObjClassTest AsTest( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );
            return ( (CswNbtObjClassTest) Node.ObjClass );
        }//AsTest

        //public static CswNbtObjClassTestGroup AsTestGroup( CswNbtNode Node )
        //{
        //    _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.TestGroupClass );
        //    return ( (CswNbtObjClassTestGroup) Node.ObjClass );
        //}//AsTestGroup

        public static CswNbtObjClassUnitOfMeasure AsUnitOfMeasure( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );
            return ( (CswNbtObjClassUnitOfMeasure) Node.ObjClass );
        }//AsUnitOfMeasure

        public static CswNbtObjClassUser AsUser( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            return ( (CswNbtObjClassUser) Node.ObjClass );
        }//AsUser


        public static ICswNbtPropertySetGeneratorTarget AsPropertySetGeneratorTarget( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetGeneratorTarget ) )
                    throw ( new CswDniException( "Invalid cast", "Can't cast current object class as ICswNbtPropertySetGeneratorTarget; Current object class is " + Node.ObjectClass.ObjectClass.ToString() ) ); 
                return ( (ICswNbtPropertySetGeneratorTarget) Node.ObjClass );
            }
            else
            {
                return null;
            }
        }//AsPropertySetGeneratorTarget

        public static ICswNbtPropertySetScheduler AsPropertySetScheduler( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetScheduler ) )
                    throw ( new CswDniException( "Invalid cast", "Can't cast current object class as ICswNbtPropertySetScheduler; Current object class is " + Node.ObjectClass.ObjectClass.ToString() ) );
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
                    throw ( new CswDniException( "Invalid cast", "Can't cast current object class as ICswNbtPropertySetInspectionParent; Current object class is " + Node.ObjectClass.ObjectClass.ToString() ) );
                return ( (ICswNbtPropertySetInspectionParent) Node.ObjClass );
            }
            else
            {
                return null;
            }
        }//AsPropertySetScheduler

    }
}