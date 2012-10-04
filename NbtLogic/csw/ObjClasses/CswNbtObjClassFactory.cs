using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Creates CswNbtObjClass instances
    /// </summary>
    public class CswNbtObjClassFactory
    {
        /// <summary>
        /// Create an ObjClass instance for given ObjectClassId and Node
        /// </summary>
        public static CswNbtObjClass makeObjClass( CswNbtResources CswNbtResources, Int32 ObjectClassId, CswNbtNode Node = null )
        {
            return makeObjClass( CswNbtResources, CswNbtResources.MetaData.getObjectClass( ObjectClassId ), Node );
        }

        /// <summary>
        /// Create an ObjClass instance for given ObjectClass and Node
        /// </summary>
        public static CswNbtObjClass makeObjClass( CswNbtResources CswNbtResources, CswNbtMetaDataObjectClass ObjectClass, CswNbtNode Node = null )
        {
            CswNbtObjClass ReturnVal = null;

            switch( ObjectClass.ObjectClass )
            {
                case CswNbtMetaDataObjectClassName.NbtObjectClass.AliquotClass:
                    ReturnVal = new CswNbtObjClassAliquot( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.BiologicalClass:
                    ReturnVal = new CswNbtObjClassBiological( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.BatchOpClass:
                    ReturnVal = new CswNbtObjClassBatchOp( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.CertMethodClass:
                    ReturnVal = new CswNbtObjClassCertMethod( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.CertMethodTemplateClass:
                    ReturnVal = new CswNbtObjClassCertMethodTemplate( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.ContainerClass:
                    ReturnVal = new CswNbtObjClassContainer( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.ContainerDispenseTransactionClass:
                    ReturnVal = new CswNbtObjClassContainerDispenseTransaction( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.CustomerClass:
                    ReturnVal = new CswNbtObjClassCustomer( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.DocumentClass:
                    ReturnVal = new CswNbtObjClassDocument( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentAssemblyClass:
                    ReturnVal = new CswNbtObjClassEquipmentAssembly( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentClass:
                    ReturnVal = new CswNbtObjClassEquipment( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.EquipmentTypeClass:
                    ReturnVal = new CswNbtObjClassEquipmentType( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.FeedbackClass:
                    ReturnVal = new CswNbtObjClassFeedback( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.GenericClass:
                    ReturnVal = new CswNbtObjClassGeneric( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.InspectionDesignClass:
                    ReturnVal = new CswNbtObjClassInspectionDesign( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.InspectionRouteClass:
                    ReturnVal = new CswNbtObjClassInspectionRoute( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.InventoryGroupClass:
                    ReturnVal = new CswNbtObjClassInventoryGroup( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.InventoryGroupPermissionClass:
                    ReturnVal = new CswNbtObjClassInventoryGroupPermission( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.InventoryLevelClass:
                    ReturnVal = new CswNbtObjClassInventoryLevel( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.LocationClass:
                    ReturnVal = new CswNbtObjClassLocation( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.MaterialClass:
                    ReturnVal = new CswNbtObjClassMaterial( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.MaterialComponentClass:
                    ReturnVal = new CswNbtObjClassMaterialComponent( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.MaterialSynonymClass:
                    ReturnVal = new CswNbtObjClassMaterialSynonym( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.MailReportClass:
                    ReturnVal = new CswNbtObjClassMailReport( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.InspectionTargetClass:
                    ReturnVal = new CswNbtObjClassInspectionTarget( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.InspectionTargetGroupClass:
                    ReturnVal = new CswNbtObjClassInspectionTargetGroup( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.NotificationClass:
                    ReturnVal = new CswNbtObjClassNotification( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.ParameterClass:
                    ReturnVal = new CswNbtObjClassParameter( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.PrintLabelClass:
                    ReturnVal = new CswNbtObjClassPrintLabel( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.ProblemClass:
                    ReturnVal = new CswNbtObjClassProblem( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.RegulatoryListClass:
                    ReturnVal = new CswNbtObjClassRegulatoryList( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.ReportClass:
                    ReturnVal = new CswNbtObjClassReport( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.RequestClass:
                    ReturnVal = new CswNbtObjClassRequest( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.RequestItemClass:
                    ReturnVal = new CswNbtObjClassRequestItem( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.ResultClass:
                    ReturnVal = new CswNbtObjClassResult( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.RoleClass:
                    ReturnVal = new CswNbtObjClassRole( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.SampleClass:
                    ReturnVal = new CswNbtObjClassSample( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.GeneratorClass:
                    ReturnVal = new CswNbtObjClassGenerator( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.SizeClass:
                    ReturnVal = new CswNbtObjClassSize( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.TaskClass:
                    ReturnVal = new CswNbtObjClassTask( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.TestClass:
                    ReturnVal = new CswNbtObjClassTest( CswNbtResources, Node );
                    break;

                //case CswNbtMetaDataObjectClassName.NbtObjectClass.TestGroupClass:
                //    ReturnVal = new CswNbtObjClassTestGroup(CswNbtResources, Node);
                //    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.UnitOfMeasureClass:
                    ReturnVal = new CswNbtObjClassUnitOfMeasure( CswNbtResources, Node );
                    break;

                //case CswNbtMetaDataObjectClassName.NbtObjectClass.UnitTypeClass:
                //    ReturnVal = new CswNbtObjClassUnitType( CswNbtResources, Node );
                //    break;//case 7608 - deprecated

                case CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass:
                    ReturnVal = new CswNbtObjClassUser( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.VendorClass:
                    ReturnVal = new CswNbtObjClassVendor( CswNbtResources, Node );
                    break;

                case CswNbtMetaDataObjectClassName.NbtObjectClass.WorkUnitClass:
                    ReturnVal = new CswNbtObjClassWorkUnit( CswNbtResources, Node );
                    break;

                default:
                    //ReturnVal = new CswNbtObjClassDefault( CswNbtResources, _CswNbtNode );
                    throw ( new CswDniException( "There is no NbtObjClass derivative for object class " + ObjectClass.ObjectClass.ToString() ) );

            }//switch


            return ( ReturnVal );

        }//makeObjClass()

    }//CswNbtObjClassFactory

}//namespace ChemSW.Nbt.ObjClasses
