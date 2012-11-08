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
                case NbtObjectClass.AliquotClass:
                    ReturnVal = new CswNbtObjClassAliquot( CswNbtResources, Node );
                    break;

                case NbtObjectClass.BiologicalClass:
                    ReturnVal = new CswNbtObjClassBiological( CswNbtResources, Node );
                    break;

                case NbtObjectClass.BatchOpClass:
                    ReturnVal = new CswNbtObjClassBatchOp( CswNbtResources, Node );
                    break;

                case NbtObjectClass.CofAMethodClass:
                    ReturnVal = new CswNbtObjClassCofAMethod( CswNbtResources, Node );
                    break;

                case NbtObjectClass.CofAMethodTemplateClass:
                    ReturnVal = new CswNbtObjClassCofAMethodTemplate( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ContainerClass:
                    ReturnVal = new CswNbtObjClassContainer( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ContainerDispenseTransactionClass:
                    ReturnVal = new CswNbtObjClassContainerDispenseTransaction( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ContainerGroupClass:
                    ReturnVal = new CswNbtObjClassContainerGroup( CswNbtResources, Node );
                    break;

                case NbtObjectClass.CustomerClass:
                    ReturnVal = new CswNbtObjClassCustomer( CswNbtResources, Node );
                    break;

                case NbtObjectClass.DocumentClass:
                    ReturnVal = new CswNbtObjClassDocument( CswNbtResources, Node );
                    break;

                case NbtObjectClass.EnterprisePartClass:
                    ReturnVal = new CswNbtObjClassEnterprisePart( CswNbtResources, Node );
                    break;

                case NbtObjectClass.EquipmentAssemblyClass:
                    ReturnVal = new CswNbtObjClassEquipmentAssembly( CswNbtResources, Node );
                    break;

                case NbtObjectClass.EquipmentClass:
                    ReturnVal = new CswNbtObjClassEquipment( CswNbtResources, Node );
                    break;

                case NbtObjectClass.EquipmentTypeClass:
                    ReturnVal = new CswNbtObjClassEquipmentType( CswNbtResources, Node );
                    break;

                case NbtObjectClass.FeedbackClass:
                    ReturnVal = new CswNbtObjClassFeedback( CswNbtResources, Node );
                    break;

                case NbtObjectClass.GenericClass:
                    ReturnVal = new CswNbtObjClassGeneric( CswNbtResources, Node );
                    break;

                case NbtObjectClass.InspectionDesignClass:
                    ReturnVal = new CswNbtObjClassInspectionDesign( CswNbtResources, Node );
                    break;

                case NbtObjectClass.InspectionRouteClass:
                    ReturnVal = new CswNbtObjClassInspectionRoute( CswNbtResources, Node );
                    break;

                case NbtObjectClass.InspectionTargetClass:

                    ReturnVal = new CswNbtObjClassInspectionTarget( CswNbtResources, Node );
                    break;

                case NbtObjectClass.InspectionTargetGroupClass:
                    ReturnVal = new CswNbtObjClassInspectionTargetGroup( CswNbtResources, Node );
                    break;

                case NbtObjectClass.InventoryGroupClass:

                    ReturnVal = new CswNbtObjClassInventoryGroup( CswNbtResources, Node );
                    break;

                case NbtObjectClass.InventoryGroupPermissionClass:
                    ReturnVal = new CswNbtObjClassInventoryGroupPermission( CswNbtResources, Node );
                    break;

                case NbtObjectClass.InventoryLevelClass:
                    ReturnVal = new CswNbtObjClassInventoryLevel( CswNbtResources, Node );
                    break;

                case NbtObjectClass.JurisdictionClass:
                    ReturnVal = new CswNbtObjClassJurisdiction( CswNbtResources, Node );
                    break;

                case NbtObjectClass.LocationClass:
                    ReturnVal = new CswNbtObjClassLocation( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ManufacturerEquivalentPartClass:
                    ReturnVal = new CswNbtObjClassManufacturerEquivalentPart( CswNbtResources, Node );
                    break;

                case NbtObjectClass.MaterialClass:
                    ReturnVal = new CswNbtObjClassMaterial( CswNbtResources, Node );
                    break;

                case NbtObjectClass.MaterialComponentClass:
                    ReturnVal = new CswNbtObjClassMaterialComponent( CswNbtResources, Node );
                    break;

                case NbtObjectClass.MaterialSynonymClass:
                    ReturnVal = new CswNbtObjClassMaterialSynonym( CswNbtResources, Node );
                    break;

                case NbtObjectClass.MailReportClass:
                    ReturnVal = new CswNbtObjClassMailReport( CswNbtResources, Node );
                    break;

                case NbtObjectClass.MethodClass:
                    ReturnVal = new CswNbtObjClassMethod( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ParameterClass:
                    ReturnVal = new CswNbtObjClassParameter( CswNbtResources, Node );
                    break;

                case NbtObjectClass.PrintLabelClass:
                    ReturnVal = new CswNbtObjClassPrintLabel( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ProblemClass:
                    ReturnVal = new CswNbtObjClassProblem( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ReceiptLotClass:
                    ReturnVal = new CswNbtObjClassReceiptLot( CswNbtResources, Node );
                    break;

                case NbtObjectClass.RegulatoryListClass:
                    ReturnVal = new CswNbtObjClassRegulatoryList( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ReportClass:
                    ReturnVal = new CswNbtObjClassReport( CswNbtResources, Node );
                    break;

                case NbtObjectClass.RequestClass:
                    ReturnVal = new CswNbtObjClassRequest( CswNbtResources, Node );
                    break;

                case NbtObjectClass.RequestContainerDispenseClass:
                    ReturnVal = new CswNbtObjClassRequestContainerDispense( CswNbtResources, Node );
                    break;

                case NbtObjectClass.RequestContainerUpdateClass:
                    ReturnVal = new CswNbtObjClassRequestContainerUpdate( CswNbtResources, Node );
                    break;

                case NbtObjectClass.RequestMaterialCreateClass:
                    ReturnVal = new CswNbtObjClassRequestMaterialCreate( CswNbtResources, Node );
                    break;

                case NbtObjectClass.RequestMaterialDispenseClass:
                    ReturnVal = new CswNbtObjClassRequestMaterialDispense( CswNbtResources, Node );
                    break;

                case NbtObjectClass.ResultClass:
                    ReturnVal = new CswNbtObjClassResult( CswNbtResources, Node );
                    break;

                case NbtObjectClass.RoleClass:
                    ReturnVal = new CswNbtObjClassRole( CswNbtResources, Node );
                    break;

                case NbtObjectClass.SampleClass:
                    ReturnVal = new CswNbtObjClassSample( CswNbtResources, Node );
                    break;

                case NbtObjectClass.GeneratorClass:
                    ReturnVal = new CswNbtObjClassGenerator( CswNbtResources, Node );
                    break;

                case NbtObjectClass.SizeClass:
                    ReturnVal = new CswNbtObjClassSize( CswNbtResources, Node );
                    break;

                case NbtObjectClass.TaskClass:
                    ReturnVal = new CswNbtObjClassTask( CswNbtResources, Node );
                    break;

                case NbtObjectClass.TestClass:
                    ReturnVal = new CswNbtObjClassTest( CswNbtResources, Node );
                    break;

                //case CswNbtMetaDataObjectClassName.NbtObjectClass.TestGroupClass:
                //    ReturnVal = new CswNbtObjClassTestGroup(CswNbtResources, Node);
                //    break;

                case NbtObjectClass.UnitOfMeasureClass:
                    ReturnVal = new CswNbtObjClassUnitOfMeasure( CswNbtResources, Node );
                    break;

                //case CswNbtMetaDataObjectClassName.NbtObjectClass.UnitTypeClass:
                //    ReturnVal = new CswNbtObjClassUnitType( CswNbtResources, Node );
                //    break;//case 7608 - deprecated

                case NbtObjectClass.UserClass:
                    ReturnVal = new CswNbtObjClassUser( CswNbtResources, Node );
                    break;

                case NbtObjectClass.VendorClass:
                    ReturnVal = new CswNbtObjClassVendor( CswNbtResources, Node );
                    break;

                case NbtObjectClass.WorkUnitClass:
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
