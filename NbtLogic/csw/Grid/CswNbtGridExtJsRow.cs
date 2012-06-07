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

        public JObject ToJson()
        {
            JObject Jrow = new JObject();
            foreach( CswNbtGridExtJsDataIndex Key in data.Keys )
            {
                Jrow[Key.ToString()] = data[Key];
            }
            return Jrow;
        } // ToJson()

    } // class CswNbtGridExtJsRow

} // namespace ChemSW.Nbt.Grid.ExtJs
