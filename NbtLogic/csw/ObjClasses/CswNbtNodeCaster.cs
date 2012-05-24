using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodeCaster
    {
        private static void _Validate( CswNbtNode Node, CswNbtMetaDataObjectClass.NbtObjectClass TargetObjectClass )
        {
            if( Node == null )
                throw new CswDniException( ErrorType.Error, "Invalid node", "CswNbtNodeCaster was given a null node as a parameter" );

            if( !( Node.getObjectClass().ObjectClass == TargetObjectClass ) )
                throw ( new CswDniException( ErrorType.Error, "Invalid cast", "Can't cast current object class as " + TargetObjectClass.ToString() + "; Current object class is " + Node.getObjectClass().ObjectClass.ToString() ) );
        }

        public static CswNbtObjClassAliquot AsAliquot( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass );
            return ( (CswNbtObjClassAliquot) Node.ObjClass );
        }//AsAliquot


        // This is the beginning of the end of CswNbtNodeCaster.
        // Use this instead:
        //  CswNbtNode Node = _CswNbtResources.Nodes[BatchId];
        //  CswNbtObjClassBatchOp BatchNode = (CswNbtObjClassBatchOp) Node;

        //public static CswNbtObjClassBatchOp AsBatchOp( CswNbtNode Node )
        //{
        //    _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.BatchOpClass );
        //    return ( (CswNbtObjClassBatchOp) Node.ObjClass );
        //}//AsBatchOp

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

        public static CswNbtObjClassDocument AsDocument( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass );
            return ( (CswNbtObjClassDocument) Node.ObjClass );
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

        public static CswNbtObjClassInspectionTarget AsInspectionTarget( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            return ( (CswNbtObjClassInspectionTarget) Node.ObjClass );
        }//AsInspectionTarget

        public static CswNbtObjClassInspectionTargetGroup AsInspectionTargetGroup( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            return ( (CswNbtObjClassInspectionTargetGroup) Node.ObjClass );
        }//AsInspectionTargetGroup

        public static CswNbtObjClassInventoryGroup AsInventoryGroup( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            return ( (CswNbtObjClassInventoryGroup) Node.ObjClass );
        }//AsInventoryGroup

        public static CswNbtObjClassInventoryGroupPermission AsInventoryGroupPermission( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass );
            return ( (CswNbtObjClassInventoryGroupPermission) Node.ObjClass );
        }//AsInventoryGroupPermission

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

        public static CswNbtObjClassMaterial AsMaterial( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            return ( (CswNbtObjClassMaterial) Node.ObjClass );
        }//AsMaterial

        public static CswNbtObjClassMaterialSynonym AsMaterialSynonym( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass );
            return ( (CswNbtObjClassMaterialSynonym) Node.ObjClass );
        }//AsMaterialSynonym

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

        public static CswNbtObjClassRequest AsRequest( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            return ( (CswNbtObjClassRequest) Node.ObjClass );
        }//AsResult

        public static CswNbtObjClassRequestItem AsRequestItem( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            return ( (CswNbtObjClassRequestItem) Node.ObjClass );
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

        public static CswNbtObjClassSize AsSize( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            return ( (CswNbtObjClassSize) Node.ObjClass );
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

        public static CswNbtObjClassUnitType AsUnitType( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.UnitTypeClass );
            return ( (CswNbtObjClassUnitType) Node.ObjClass );
        }//AsUnitType

        public static CswNbtObjClassUser AsUser( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            return ( (CswNbtObjClassUser) Node.ObjClass );
        }//AsUser


        public static CswNbtObjClassVendor AsVendor( CswNbtNode Node )
        {
            _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass );
            return ( (CswNbtObjClassVendor) Node.ObjClass );
        }//AsVendor


        public static ICswNbtPropertySetGeneratorTarget AsPropertySetGeneratorTarget( CswNbtNode Node )
        {
            if( Node != null )
            {
                if( !( Node.ObjClass is ICswNbtPropertySetGeneratorTarget ) )
                    throw ( new CswDniException( ErrorType.Error, "Invalid cast", "Can't cast current object class as ICswNbtPropertySetGeneratorTarget; Current object class is " + Node.getObjectClass().ObjectClass.ToString() ) );
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
        }//AsPropertySetScheduler

    }
}