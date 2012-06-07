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
    public class CswNbtGridExtJsField
    {
        /// <summary>
        /// Name for the field (matches column definition's dataIndex)
        /// </summary>
        public string name;
        /// <summary>
        /// Data type
        /// string, date, number
        /// </summary>
        public string type = "string";

        public JObject ToJson()
        {
            JObject Jfield = new JObject();
            Jfield["name"] = CswNbtGridExtJsGrid.DataIndexPrefix + name;
            if( type != string.Empty )
            {
                Jfield["type"] = type;
            }
            return Jfield;
        } // ToJson()

    } // class CswNbtGridExtJsField
} // namespace ChemSW.Nbt.Grid.ExtJs
