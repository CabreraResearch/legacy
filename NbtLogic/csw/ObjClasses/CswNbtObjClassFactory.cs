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
                case CswEnumNbtObjectClass.AliquotClass:
                    ReturnVal = new CswNbtObjClassAliquot( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.BiologicalClass:
                    ReturnVal = new CswNbtObjClassBiological( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.BatchOpClass:
                    ReturnVal = new CswNbtObjClassBatchOp( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.CofADocumentClass:
                    ReturnVal = new CswNbtObjClassCofADocument( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.CofAMethodClass:
                    ReturnVal = new CswNbtObjClassCofAMethod( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.CofAMethodTemplateClass:
                    ReturnVal = new CswNbtObjClassCofAMethodTemplate( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ContainerClass:
                    ReturnVal = new CswNbtObjClassContainer( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ContainerDispenseTransactionClass:
                    ReturnVal = new CswNbtObjClassContainerDispenseTransaction( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ContainerGroupClass:
                    ReturnVal = new CswNbtObjClassContainerGroup( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ContainerLocationClass:
                    ReturnVal = new CswNbtObjClassContainerLocation( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.CustomerClass:
                    ReturnVal = new CswNbtObjClassCustomer( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.DesignNodeTypeClass:
                    ReturnVal = new CswNbtObjClassDesignNodeType( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.DesignNodeTypePropClass:
                    ReturnVal = new CswNbtObjClassDesignNodeTypeProp( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.DesignNodeTypeTabClass:
                    ReturnVal = new CswNbtObjClassDesignNodeTypeTab( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.DesignSequenceClass:
                    ReturnVal = new CswNbtObjClassDesignSequence( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.DocumentClass:
                    ReturnVal = new CswNbtObjClassDocument( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.EnterprisePartClass:
                    ReturnVal = new CswNbtObjClassEnterprisePart( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.EquipmentAssemblyClass:
                    ReturnVal = new CswNbtObjClassEquipmentAssembly( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.EquipmentClass:
                    ReturnVal = new CswNbtObjClassEquipment( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.EquipmentTypeClass:
                    ReturnVal = new CswNbtObjClassEquipmentType( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.FeedbackClass:
                    ReturnVal = new CswNbtObjClassFeedback( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.FireClassExemptAmountClass:
                    ReturnVal = new CswNbtObjClassFireClassExemptAmount( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.FireClassExemptAmountSetClass:
                    ReturnVal = new CswNbtObjClassFireClassExemptAmountSet( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.GenericClass:
                    ReturnVal = new CswNbtObjClassGeneric( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.GHSClass:
                    ReturnVal = new CswNbtObjClassGHS( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.GHSPhraseClass:
                    ReturnVal = new CswNbtObjClassGHSPhrase( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.InspectionDesignClass:
                    ReturnVal = new CswNbtObjClassInspectionDesign( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.InspectionRouteClass:
                    ReturnVal = new CswNbtObjClassInspectionRoute( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.InspectionTargetClass:

                    ReturnVal = new CswNbtObjClassInspectionTarget( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.InspectionTargetGroupClass:
                    ReturnVal = new CswNbtObjClassInspectionTargetGroup( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.InventoryGroupClass:

                    ReturnVal = new CswNbtObjClassInventoryGroup( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.InventoryGroupPermissionClass:
                    ReturnVal = new CswNbtObjClassInventoryGroupPermission( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.InventoryLevelClass:
                    ReturnVal = new CswNbtObjClassInventoryLevel( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.JurisdictionClass:
                    ReturnVal = new CswNbtObjClassJurisdiction( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.LocationClass:
                    ReturnVal = new CswNbtObjClassLocation( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ManufacturerEquivalentPartClass:
                    ReturnVal = new CswNbtObjClassManufacturerEquivalentPart( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ChemicalClass:
                    ReturnVal = new CswNbtObjClassChemical( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.MaterialComponentClass:
                    ReturnVal = new CswNbtObjClassMaterialComponent( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.MaterialSynonymClass:
                    ReturnVal = new CswNbtObjClassMaterialSynonym( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.MailReportClass:
                    ReturnVal = new CswNbtObjClassMailReport( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.MailReportGroupClass:
                    ReturnVal = new CswNbtObjClassMailReportGroup( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.MailReportGroupPermissionClass:
                    ReturnVal = new CswNbtObjClassMailReportGroupPermission( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.MethodClass:
                    ReturnVal = new CswNbtObjClassMethod( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.NonChemicalClass:
                    ReturnVal = new CswNbtObjClassNonChemical( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ParameterClass:
                    ReturnVal = new CswNbtObjClassParameter( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.PrinterClass:
                    ReturnVal = new CswNbtObjClassPrinter( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.PrintJobClass:
                    ReturnVal = new CswNbtObjClassPrintJob( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.PrintLabelClass:
                    ReturnVal = new CswNbtObjClassPrintLabel( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ProblemClass:
                    ReturnVal = new CswNbtObjClassProblem( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ReceiptLotClass:
                    ReturnVal = new CswNbtObjClassReceiptLot( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RegulatoryListClass:
                    ReturnVal = new CswNbtObjClassRegulatoryList( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RegulatoryListCasNoClass:
                    ReturnVal = new CswNbtObjClassRegulatoryListCasNo( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RegulatoryListMemberClass:
                    ReturnVal = new CswNbtObjClassRegulatoryListMember( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RegulatoryListListCodeClass:
                    ReturnVal = new CswNbtObjClassRegulatoryListListCode( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ReportClass:
                    ReturnVal = new CswNbtObjClassReport( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ReportGroupClass:
                    ReturnVal = new CswNbtObjClassReportGroup( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ReportGroupPermissionClass:
                    ReturnVal = new CswNbtObjClassReportGroupPermission( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RequestClass:
                    ReturnVal = new CswNbtObjClassRequest( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RequestContainerDispenseClass:
                    ReturnVal = new CswNbtObjClassRequestContainerDispense( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RequestContainerUpdateClass:
                    ReturnVal = new CswNbtObjClassRequestContainerUpdate( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RequestMaterialCreateClass:
                    ReturnVal = new CswNbtObjClassRequestMaterialCreate( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RequestMaterialDispenseClass:
                    ReturnVal = new CswNbtObjClassRequestMaterialDispense( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.ResultClass:
                    ReturnVal = new CswNbtObjClassResult( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.RoleClass:
                    ReturnVal = new CswNbtObjClassRole( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.SampleClass:
                    ReturnVal = new CswNbtObjClassSample( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.SDSDocumentClass:
                    ReturnVal = new CswNbtObjClassSDSDocument( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.GeneratorClass:
                    ReturnVal = new CswNbtObjClassGenerator( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.SizeClass:
                    ReturnVal = new CswNbtObjClassSize( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.TaskClass:
                    ReturnVal = new CswNbtObjClassTask( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.TestClass:
                    ReturnVal = new CswNbtObjClassTest( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.UnitOfMeasureClass:
                    ReturnVal = new CswNbtObjClassUnitOfMeasure( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.UserClass:
                    ReturnVal = new CswNbtObjClassUser( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.VendorClass:
                    ReturnVal = new CswNbtObjClassVendor( CswNbtResources, Node );
                    break;

                case CswEnumNbtObjectClass.WorkUnitClass:
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
