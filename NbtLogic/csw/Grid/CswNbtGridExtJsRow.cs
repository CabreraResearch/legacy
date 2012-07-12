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
    public class CswNbtGridExtJsRow
    {
        /// <summary>
        /// Name - value pairs that encompass the row data
        /// </summary>
        public Dictionary<CswNbtGridExtJsDataIndex, string> data = new Dictionary<CswNbtGridExtJsDataIndex, string>();

        /// <summary>
        /// Allow view on this row
        /// </summary>
        public bool canView = true;
        /// <summary>
        /// Allow edit on this row
        /// </summary>
        public bool canEdit = true;
        /// <summary>
        /// Allow delete on this row
        /// </summary>
        public bool canDelete = true;
        /// <summary>
        /// Show that the row is locked
        /// </summary>
        public bool isLocked = false;
        /// <summary>
        /// Show that the row is disabled
        /// </summary>
        public bool isDisabled = false;

        public JObject ToJson()
        {
            JObject Jrow = new JObject();
            foreach( CswNbtGridExtJsDataIndex Key in data.Keys )
            {
                Jrow[Key.ToString()] = data[Key];
            }

            // save on packet size by only including these when they aren't the default:
            if( false == canView )
            {
                Jrow["canview"] = canView;
            }
            if( false == canEdit )
            {
                Jrow["canedit"] = canEdit;
            }
            if( false == canDelete )
            {
                Jrow["candelete"] = canDelete;
            }
            if( isLocked )
            {
                Jrow["islocked"] = isLocked;
            }
            if( isDisabled )
            {
                Jrow["isdisabled"] = isDisabled;
            }
            return Jrow;
        } // ToJson()

    } // class CswNbtGridExtJsRow

} // namespace ChemSW.Nbt.Grid.ExtJs
