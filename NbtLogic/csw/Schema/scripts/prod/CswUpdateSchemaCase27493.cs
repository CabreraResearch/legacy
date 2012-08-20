using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27493
    /// </summary>
    public class CswUpdateSchemaCase27493 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Reassign nodetype and object class icons

            Dictionary<Int32, string> OCImageDict = new Dictionary<int, string>();

            CswTableUpdate OCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "27493_oc_update", "object_class" );
            DataTable OCTable = OCUpdate.getTable();
            foreach( DataRow OCRow in OCTable.Rows )
            {
                string NewImage = _getNewImageOC( OCRow["objectclass"].ToString() );
                OCRow["iconfilename"] = NewImage;
                OCImageDict[CswConvert.ToInt32( OCRow["objectclassid"] )] = NewImage;
            }
            OCUpdate.update( OCTable );

            CswTableUpdate NTUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "27493_nt_update", "nodetypes" );
            DataTable NTTable = NTUpdate.getTable();
            foreach( DataRow NTRow in NTTable.Rows )
            {
                string NodeTypeName = NTRow["nodetypename"].ToString();

                // NodeType special cases
                string NewImage = _getNewImageNT( NodeTypeName );
                if( NewImage == string.Empty )
                {
                    NewImage = OCImageDict[CswConvert.ToInt32( NTRow["objectclassid"] )];
                }
                NTRow["iconfilename"] = NewImage;
            }
            NTUpdate.update( NTTable );

        } //Update()
        
        public string _getNewImageNT( string NodeTypeName )
        {
            string NewImage = string.Empty;
            switch( NodeTypeName )
            {
                case "Chemical":    NewImage = "atom";          break;
                case "Biological":  NewImage = "DNA";           break;
                case "Supply":      NewImage = "toolbox";       break;
                                                             
                case "Site":        NewImage = "world";         break;
                case "Building":    NewImage = "house";         break;
                case "Floor":       NewImage = "up";            break;
                case "Room":        NewImage = "door";          break;
                case "Cabinet":     NewImage = "cabinet";       break;
                case "Shelf":       NewImage = "minus";         break;
                case "Box":         NewImage = "box";           break;
            }
            if( NewImage != string.Empty )
            {
                NewImage += ".png";
            }
            return NewImage;
        } // _getNewImage(NodeTypeName)

        public string _getNewImageOC(string ObjectClass)
        {
            string NewImage = string.Empty;
            switch( ObjectClass )
            {
                case "AliquotClass":                       NewImage = "flask";            break;
                case "BatchOpClass":                       NewImage = "clock";            break;
                case "BiologicalClass":                    NewImage = "DNA";              break;
                case "ContainerClass":                     NewImage = "barcode";          break;
                case "ContainerDispenseTransactionClass":  NewImage = "barcode";          break;
                case "CustomerClass":                      NewImage = "contact";          break;
                case "DocumentClass":                      NewImage = "doc";              break;
                case "EquipmentAssemblyClass":             NewImage = "gearset";          break;
                case "EquipmentClass":                     NewImage = "gear";             break;
                case "EquipmentTypeClass":                 NewImage = "foldergear";       break;
                case "FeedbackClass":                      NewImage = "questionmark";     break;
                case "GeneratorClass":                     NewImage = "calendar";         break;
                case "GenericClass":                       NewImage = "folder";           break;
                case "InspectionDesignClass":              NewImage = "clipboardcheck";   break;
                case "InspectionRouteClass":               NewImage = "compass";          break;
                case "InspectionTargetClass":              NewImage = "target";           break;
                case "InspectionTargetGroupClass":         NewImage = "targetgroup";      break;
                case "InventoryGroupClass":                NewImage = "smallicons";       break;
                case "InventoryGroupPermissionClass":      NewImage = "check";            break;
                case "InventoryLevelClass":                NewImage = "barchart";         break;
                case "LocationClass":                      NewImage = "world";            break;
                case "MailReportClass":                    NewImage = "envelope";         break;
                case "MaterialClass":                      NewImage = "atom";             break;
                case "MaterialComponentClass":             NewImage = "atom";             break;
                case "MaterialSynonymClass":               NewImage = "atom";             break;
                case "NotificationClass":                  NewImage = "envelope";         break;
                case "ParameterClass":                     NewImage = "square";           break;
                case "PrintLabelClass":                    NewImage = "barcode";          break;
                case "ProblemClass":                       NewImage = "trianglegear";     break;
                case "ReportClass":                        NewImage = "doc";              break;
                case "RequestClass":                       NewImage = "cart";             break;
                case "RequestItemClass":                   NewImage = "cart";             break;
                case "ResultClass":                        NewImage = "options";          break;
                case "RoleClass":                          NewImage = "people";           break;
                case "SampleClass":                        NewImage = "flask";            break;
                case "SizeClass":                          NewImage = "box";              break;
                case "TaskClass":                          NewImage = "clipboardgear";    break;
                case "TestClass":                          NewImage = "square";           break;
                case "UnitOfMeasureClass":                 NewImage = "calculator";       break;
                case "UserClass":                          NewImage = "person";           break;
                case "VendorClass":                        NewImage = "fax";              break;
                case "WorkUnitClass":                      NewImage = "flag";             break;
                    
            } // switch
            if( NewImage == string.Empty )
            {
                NewImage = "square";
            }
            NewImage += ".png";
            return NewImage;
        } // _getNewImage(ObjectClass)



    }//class CswUpdateSchemaCase27493

}//namespace ChemSW.Nbt.Schema