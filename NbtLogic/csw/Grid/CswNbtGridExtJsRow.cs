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
        public Dictionary<string, string> data = new Dictionary<string, string>();

        public JObject ToJson()
        {
            JObject Jrow = new JObject();
            foreach( string Key in data.Keys )
            {
                Jrow[CswNbtGridExtJsGrid.DataIndexPrefix + Key] = data[Key];
            }
            return Jrow;
        } // ToJson()

    } // class CswNbtGridExtJsRow

} // namespace ChemSW.Nbt.Grid.ExtJs
