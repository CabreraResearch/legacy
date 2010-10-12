using System;
using System.Collections;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.TblDn;

namespace ChemSW.Nbt.Sched
{

    public class CswNbtSchdItemUpdateMTBF: CswNbtSchdItem
    {
        private CswNbtResources _CswNbtResources = null;

        public CswNbtSchdItemUpdateMTBF( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            SchedItemName = "UpdateMTBF";
        }//ctor

        public override void reset()
        {
            _Succeeded = true;
            _StatusMessage = string.Empty;
        }//

        /// <summary>
        /// Determines whether this schedule item is due for running
        /// </summary>
        override public bool doesItemRunNow()
        {
            CswTableSelect SchedItemSelect = _CswNbtResources.makeCswTableSelect( "UpdateMTBF_doesItemRunNow_Select", "schedule_items" );
            DataTable SchedItemTable = SchedItemSelect.getTable( "where itemname = '" + this.SchedItemName + "'" );
            return ( SchedItemTable.Rows.Count == 0 ||
                     DateTime.Now.Subtract( CswConvert.ToDateTime( SchedItemTable.Rows[0]["lastrun"] ) ).TotalHours >= 24 );

        }//doesItemRunNow() 


        override public void run()
        {
            try
            {
                // BZ 6779
                // Set all MTBF fields pendingupdate = 1
                Int32 MTBFId = _CswNbtResources.MetaData.getFieldType( ChemSW.Nbt.MetaData.CswNbtMetaDataFieldType.NbtFieldType.MTBF ).FieldTypeId;

                CswTableSelect NTPSelect = _CswNbtResources.makeCswTableSelect( "UpdateMTBF_NTP_Select", "nodetype_props" );
                DataTable NTPTable = NTPSelect.getTable( "fieldtypeid", MTBFId );
                string NTPIds = string.Empty;
                foreach( DataRow NTPRow in NTPTable.Rows )
                {
                    if( NTPIds != string.Empty ) NTPIds += ",";
                    NTPIds += CswConvert.ToInt32( NTPRow["nodetypepropid"] );
                }

                if( NTPIds != string.Empty )
                {
                    CswTableUpdate JNPUpdate = _CswNbtResources.makeCswTableUpdate( "UpdateMTBF_JNP_Update", "jct_nodes_props" );
                    DataTable JNPTable = JNPUpdate.getTable( "where nodetypepropid in (" + NTPIds + ")" );
                    foreach( DataRow JNPRow in JNPTable.Rows )
                    {
                        JNPRow["pendingupdate"] = CswConvert.ToDbVal( true );
                    }
                    JNPUpdate.update( JNPTable );
                }
                
            }//try

            catch( Exception Exception )
            {
                _Succeeded = false;
                _StatusMessage = "Error running Generator " + Name + ": " + Exception.Message;
            }

        }//run()

        override public string Name 
        {
            get
            {
                return "Update MTBF";
            }
        }//Name

        private bool _Succeeded = true;
        override public bool Succeeded
        {
            get
            {
                return( _Succeeded );
            }
        }//Succeeded

        private string _StatusMessage = "";
        override public string StatusMessage
        {
            get
            {
                return( _StatusMessage );
            }
        }//StatusMessage

    }//CswNbtSchdItemUpdateMTBF

}//namespace ChemSW.Nbt.Sched
