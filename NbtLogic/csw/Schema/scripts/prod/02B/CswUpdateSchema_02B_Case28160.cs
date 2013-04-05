using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
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
                        case CswEnumNbtPropertySetName.GeneratorTargetSet:  Row["iconfilename"] = "clipboardcheck.png"; break;
                        case CswEnumNbtPropertySetName.InspectionParentSet: Row["iconfilename"] = "target.png";         break;
                        case CswEnumNbtPropertySetName.RequestItemSet:      Row["iconfilename"] = "cart.png";           break;
                        case CswEnumNbtPropertySetName.SchedulerSet:        Row["iconfilename"] = "calendar.png";       break;
                    }
                    PropSetTable.Rows.Add( Row );

                    PropSetDict[PropSetName] = CswConvert.ToInt32( Row["propertysetid"] );
                }
                PropSetUpdate.update( PropSetTable );
            }

            {
                // Populate jct_propertyset_objectclass
                CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28160_jct_update", "jct_propertyset_objectclass" );
                DataTable JctTable = JctUpdate.getEmptyTable();
                
                DataRow Row1 = JctTable.NewRow();
                Row1["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.InspectionDesignClass );
                Row1["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.GeneratorTargetSet] );
                JctTable.Rows.Add( Row1 );

                DataRow Row2 = JctTable.NewRow();
                Row2["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.TaskClass );
                Row2["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.GeneratorTargetSet] );
                JctTable.Rows.Add( Row2 );

                DataRow Row3 = JctTable.NewRow();
                Row3["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.InspectionTargetClass );
                Row3["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.InspectionParentSet] );
                JctTable.Rows.Add( Row3 );

                DataRow Row4 = JctTable.NewRow();
                Row4["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestContainerDispenseClass );
                Row4["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.RequestItemSet] );
                JctTable.Rows.Add( Row4 );

                DataRow Row5 = JctTable.NewRow();
                Row5["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestContainerUpdateClass );
                Row5["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.RequestItemSet] );
                JctTable.Rows.Add( Row5 );

                DataRow Row6 = JctTable.NewRow();
                Row6["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestMaterialCreateClass );
                Row6["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.RequestItemSet] );
                JctTable.Rows.Add( Row6 );

                DataRow Row7 = JctTable.NewRow();
                Row7["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.RequestMaterialDispenseClass );
                Row7["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.RequestItemSet] );
                JctTable.Rows.Add( Row7 );

                DataRow Row8 = JctTable.NewRow();
                Row8["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.GeneratorClass );
                Row8["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.SchedulerSet] );
                JctTable.Rows.Add( Row8 );

                DataRow Row9 = JctTable.NewRow();
                Row9["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( NbtObjectClass.MailReportClass );
                Row9["propertysetid"] = CswConvert.ToDbVal( PropSetDict[CswEnumNbtPropertySetName.SchedulerSet] );
                JctTable.Rows.Add( Row9 );

                JctUpdate.update( JctTable );
            }
        } // update()

    }//class CswUpdateSchema_02B_Case28160

}//namespace ChemSW.Nbt.Schema