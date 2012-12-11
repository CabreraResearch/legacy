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
    public class CswGridExtJsGrid
    {
        /// <summary>
        /// Header Title for Grid
        /// </summary>
        public string title = string.Empty;
        /// <summary>
        /// Field definitions
        /// </summary>
        public Collection<CswGridExtJsField> fields = new Collection<CswGridExtJsField>();
        /// <summary>
        /// Column definitions
        /// </summary>
        public Collection<CswGridExtJsColumn> columns = new Collection<CswGridExtJsColumn>();
        /// <summary>
        /// Row data
        /// </summary>
        public Collection<CswGridExtJsRow> rows = new Collection<CswGridExtJsRow>();
        /// <summary>
        /// Button data
        /// </summary>
        public Collection<CswGridExtJsButton> buttons = new Collection<CswGridExtJsButton>();
        /// <summary>
        /// Page size
        /// </summary>
        public Int32 PageSize = 25;
        /// <summary>
        /// Truncated
        /// </summary>
        public bool Truncated = false;
        /// <summary>
        /// GroupByCol
        /// </summary>
        public string GroupByCol = "";

        public CswGridExtJsGrid( string UniquePrefix )
        {
            // add hidden canview/canedit/candelete columns
            string[] columnNames = new string[] { "canView", "canEdit", "canDelete", "isLocked" };
            foreach( string columnName in columnNames )
            {
                CswGridExtJsDataIndex dataIndex = new CswGridExtJsDataIndex( UniquePrefix, columnName );
                CswGridExtJsField fld = new CswGridExtJsField()
                {
                    dataIndex = dataIndex,
                };
                // we don't need a column definition for these -- we'll never display them
                //CswGridExtJsColumn col = new CswGridExtJsColumn()
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
            foreach( CswGridExtJsColumn col in columns )
            {
                ret = ret || ( col.header.ToLower() == header.ToLower() );
            }
            return ret;
        } // columnsContains()

        public CswGridExtJsColumn getColumn( string header )
        {
            CswGridExtJsColumn ret = null;
            foreach( CswGridExtJsColumn col in columns )
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

            foreach( CswGridExtJsField fld in fields )
            {
                Jfields.Add( fld.ToJson() );
            }
            foreach( CswGridExtJsColumn col in columns )
            {
                Jcolumns.Add( col.ToJson() );
            }
            foreach( CswGridExtJsRow Row in rows )
            {
                Jdataitems.Add( Row.ToJson() );
            }
            foreach( CswGridExtJsButton Button in buttons )
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

            if( false == String.IsNullOrEmpty( GroupByCol ) )
            {
                Jret["grid"]["groupfield"] = GroupByCol.ToLower();
            }

            return Jret;
        } // ToJson()

    } // class CswGridExtJsGrid

} // namespace ChemSW.Nbt.Grid.ExtJs
