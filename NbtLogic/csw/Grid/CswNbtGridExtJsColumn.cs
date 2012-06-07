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
    public class CswNbtGridExtJsColumn
    {
        /// <summary>
        /// Display name for the column
        /// </summary>
        public string header;
        /// <summary>
        /// Internal column name (matches field definition's dataIndex)
        /// </summary>
        public CswNbtGridExtJsDataIndex dataIndex;
        /// <summary>
        /// Width in pixels (default is 100)
        /// </summary>
        public Int32 width;
        /// <summary>
        /// Whether this column will widen to encompass all extra space in the grid
        /// </summary>
        public bool flex = false;
        /// <summary>
        /// Hide this column (can be manually displayed on client)
        /// </summary>
        public bool hidden = false;
        /// <summary>
        /// Data type for grid column
        /// </summary>
        public extJsXType xtype = extJsXType.Unknown;
        /// <summary>
        /// Date format (if xtype == datecolumn)
        /// </summary>
        public string dateformat;

        public JObject ToJson()
        {
            JObject Jcol = new JObject();
            Jcol["id"] = dataIndex.ToString();
            Jcol["header"] = header;
            Jcol["dataIndex"] = dataIndex.ToString();
            if( width > 0 )
            {
                Jcol["width"] = width;
            }
            if( flex )
            {
                Jcol["flex"] = flex;
            }
            if( hidden )
            {
                Jcol["hidden"] = hidden;
            }
            if( xtype != extJsXType.Unknown )
            {
                Jcol["xtype"] = xtype.ToString();
            }
            if( dateformat != string.Empty )
            {
                Jcol["format"] = dateformat;
            }
            return Jcol;
        } // ToJson()

    } // class CswNbtGridExtJsColumn

} // namespace ChemSW.Nbt.Grid.ExtJs
