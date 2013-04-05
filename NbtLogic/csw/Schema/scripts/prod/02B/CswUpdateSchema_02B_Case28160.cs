using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28160
    /// </summary>
    public class CswUpdateSchema_02B_Case28160 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28160; }
        }

        public override void update()
        {
            Dictionary<CswEnumNbtPropertySetName, Int32> PropSetDict = new Dictionary<CswEnumNbtPropertySetName, Int32>();
            {
                // Populate property_set
                CswTableUpdate PropSetUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28160_ps_update", "property_set" );
                DataTable PropSetTable = PropSetUpdate.getEmptyTable();
                foreach( CswEnumNbtPropertySetName PropSetName in CswEnumNbtPropertySetName.All )
                {
                    DataRow Row = PropSetTable.NewRow();
                    Row["name"] = PropSetName;
                    switch( PropSetName )
                    {
                        case CswEnumNbtPropertySetName.GeneratorTargetSet: Row["iconfilename"] = "clipboardcheck.png"; break;
                        case CswEnumNbtPropertySetName.InspectionParentSet: Row["iconfilename"] = "target.png"; break;
                        case CswEnumNbtPropertySetName.RequestItemSet: Row["iconfilename"] = "cart.png"; break;
                        case CswEnumNbtPropertySetName.SchedulerSet: Row["iconfilename"] = "calendar.png"; break;
                    }
                    PropSetTable.Rows.Add( Row );

                    PropSetDict[PropSetName] = CswConvert.ToInt32( Row["propertysetid"] );
                }
                PropSetUpdate.update( PropSetTable );
            }

            Dictionary<NbtObjectClass, CswEnumNbtPropertySetName> Dict = new Dictionary<NbtObjectClass, CswEnumNbtPropertySetName>();
            Dict.Add( NbtObjectClass.InspectionDesignClass, CswEnumNbtPropertySetName.GeneratorTargetSet );
            Dict.Add( NbtObjectClass.TaskClass, CswEnumNbtPropertySetName.GeneratorTargetSet );
            Dict.Add( NbtObjectClass.InspectionTargetClass, CswEnumNbtPropertySetName.InspectionParentSet );
            Dict.Add( NbtObjectClass.RequestContainerDispenseClass, CswEnumNbtPropertySetName.RequestItemSet );
            Dict.Add( NbtObjectClass.RequestContainerUpdateClass, CswEnumNbtPropertySetName.RequestItemSet );
            Dict.Add( NbtObjectClass.RequestMaterialCreateClass, CswEnumNbtPropertySetName.RequestItemSet );
            Dict.Add( NbtObjectClass.RequestMaterialDispenseClass, CswEnumNbtPropertySetName.RequestItemSet );
            Dict.Add( NbtObjectClass.GeneratorClass, CswEnumNbtPropertySetName.SchedulerSet );
            Dict.Add( NbtObjectClass.MailReportClass, CswEnumNbtPropertySetName.SchedulerSet );

            {
                // Populate jct_propertyset_objectclass
                CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28160_jct_update", "jct_propertyset_objectclass" );
                DataTable JctTable = JctUpdate.getEmptyTable();

                foreach( NbtObjectClass oc in Dict.Keys )
                {
                    DataRow NewRow = JctTable.NewRow();
                    NewRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( oc );
                    NewRow["propertysetid"] = CswConvert.ToDbVal( PropSetDict[Dict[oc]] );
                    JctTable.Rows.Add( NewRow );
                }
                JctUpdate.update( JctTable );
            }

            {
                // Populate jct_propertyset_objectclassprop
                CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28160_jct2_update", "jct_propertyset_ocprop" );
                DataTable JctTable = JctUpdate.getEmptyTable();

                foreach( NbtObjectClass oc in Dict.Keys )
                {
                    CswNbtMetaDataObjectClass ObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( oc );
                    foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in ObjectClass.getObjectClassProps() )
                    {
                        bool doInsert = false;
                        switch( Dict[oc] )
                        {
                            case CswEnumNbtPropertySetName.GeneratorTargetSet:
                                doInsert = ( ObjectClassProp.PropName == CswNbtPropertySetGeneratorTarget.PropertyName.CreatedDate ||
                                             ObjectClassProp.PropName == CswNbtPropertySetGeneratorTarget.PropertyName.DueDate ||
                                             ObjectClassProp.PropName == CswNbtPropertySetGeneratorTarget.PropertyName.Generator ||
                                             ObjectClassProp.PropName == CswNbtPropertySetGeneratorTarget.PropertyName.IsFuture );
                                break;

                            case CswEnumNbtPropertySetName.RequestItemSet:
                                doInsert = ( ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.AssignedTo ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Comments ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Description ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.ExternalOrderNumber ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Fulfill ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.InventoryGroup ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Location ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Material ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Name ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.NeededBy ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Number ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Priority ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Request ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.RequestedFor ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Requestor ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Status ||
                                             ObjectClassProp.PropName == CswNbtPropertySetRequestItem.PropertyName.Type );
                                break;

                            case CswEnumNbtPropertySetName.InspectionParentSet:
                                doInsert = ( ObjectClassProp.PropName == "Status" );
                                break;

                            case CswEnumNbtPropertySetName.SchedulerSet:
                                doInsert = ( ObjectClassProp.PropName == "Status" ||
                                             ObjectClassProp.PropName == "Final Due Date" ||
                                             ObjectClassProp.PropName == "Next Due Date" ||
                                             ObjectClassProp.PropName == "Run Status" ||
                                             ObjectClassProp.PropName == "Warning Days" ||
                                             ObjectClassProp.PropName == "Due Date Interval" ||
                                             ObjectClassProp.PropName == "Run Time" ||
                                             ObjectClassProp.PropName == "Enabled" );
                                break;
                        }


                        if( doInsert )
                        {
                            DataRow NewRow = JctTable.NewRow();
                            NewRow["objectclasspropid"] = ObjectClassProp.PropId;
                            NewRow["propertysetid"] = CswConvert.ToDbVal( PropSetDict[Dict[oc]] );
                            JctTable.Rows.Add( NewRow );
                        }
                    } //  foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in ObjectClass.getObjectClassProps() )
                } //  foreach( NbtObjectClass oc in OcPsDict.Keys )
                JctUpdate.update( JctTable );
            }

        } // update()

    }//class CswUpdateSchema_02B_Case28160

}//namespace ChemSW.Nbt.Schema