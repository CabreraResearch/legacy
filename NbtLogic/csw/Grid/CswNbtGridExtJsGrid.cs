using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Grid.ExtJs
{
    public class CswNbtGridExtJsGrid
    {
        /// <summary>
        /// Header Title for Grid
        /// </summary>
        public string title = string.Empty;
        /// <summary>
        /// Field definitions
        /// </summary>
        public Collection<CswNbtGridExtJsField> fields = new Collection<CswNbtGridExtJsField>();
        /// <summary>
        /// Column definitions
        /// </summary>
        public Collection<CswNbtGridExtJsColumn> columns = new Collection<CswNbtGridExtJsColumn>();
        /// <summary>
        /// Row data
        /// </summary>
        public Collection<CswNbtGridExtJsRow> rows = new Collection<CswNbtGridExtJsRow>();
        /// <summary>
        /// Button data
        /// </summary>
        public Collection<CswNbtGridExtJsButton> buttons = new Collection<CswNbtGridExtJsButton>();
        /// <summary>
        /// Page size
        /// </summary>
        public Int32 PageSize = 25;
        /// <summary>
        /// Truncated
        /// </summary>
        public bool Truncated = false;

        public CswNbtGridExtJsGrid( string UniquePrefix )
        {
            // add hidden canview/canedit/candelete columns
            string[] columnNames = new string[] { "canView", "canEdit", "canDelete", "isLocked" };
            foreach( string columnName in columnNames )
            {
                CswNbtGridExtJsDataIndex dataIndex = new CswNbtGridExtJsDataIndex( UniquePrefix, columnName );
                CswNbtGridExtJsField fld = new CswNbtGridExtJsField()
                {
                    dataIndex = dataIndex,
                };
                // we don't need a column definition for these -- we'll never display them
                //CswNbtGridExtJsColumn col = new CswNbtGridExtJsColumn()
                //{
                //    dataIndex = dataIndex,
                //    header = columnName,
                //    hidden = true,
                //    hideable = false 
                //};
                fields.Add( fld );
                //columns.Add( col );
            } // foreach( string columnName in columnNames )
        } // constructor

        public bool columnsContains( string header )
        {
            bool ret = false;
            foreach( CswNbtGridExtJsColumn col in columns )
            {
                ret = ret || ( col.header.ToLower() == header.ToLower() );
            }
            return ret;
        } // columnsContains()

        public CswNbtGridExtJsColumn getColumn( string header )
        {
            CswNbtGridExtJsColumn ret = null;
            foreach( CswNbtGridExtJsColumn col in columns )
            {
                if( col.header.ToLower() == header.ToLower() )
                {
                    ret = col;
                }
            }
            return ret;
        } // getColumn()

        public JObject ToJson()
        {
            JArray Jfields = new JArray();
            JArray Jcolumns = new JArray();
            JArray Jdataitems = new JArray();
            JArray Jdatabuttons = new JArray();

            foreach( CswNbtGridExtJsField fld in fields )
            {
                Jfields.Add( fld.ToJson() );
            }
            foreach( CswNbtGridExtJsColumn col in columns )
            {
                Jcolumns.Add( col.ToJson() );
            }
            foreach( CswNbtGridExtJsRow Row in rows )
            {
                Jdataitems.Add( Row.ToJson() );
            }
            foreach( CswNbtGridExtJsButton Button in buttons )
            {
                Jdatabuttons.Add( Button.ToJson() );
            }
            JObject Jret = new JObject();
            Jret["grid"] = new JObject();
            Jret["grid"]["title"] = title;
            if( Truncated )
            {
                Jret["grid"]["truncated"] = Truncated;
            }
            Jret["grid"]["fields"] = Jfields;
            Jret["grid"]["columns"] = Jcolumns;
            Jret["grid"]["pageSize"] = PageSize;
            Jret["grid"]["data"] = new JObject();
            Jret["grid"]["data"]["items"] = Jdataitems;
            Jret["grid"]["data"]["buttons"] = Jdatabuttons;

            return Jret;
        } // ToJson()

    } // class CswNbtGridExtJsGrid

} // namespace ChemSW.Nbt.Grid.ExtJs
