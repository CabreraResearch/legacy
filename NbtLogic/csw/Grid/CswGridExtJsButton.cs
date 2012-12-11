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
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Grid.ExtJs
{
    public class CswGridExtJsButton
    {
        /// <summary>
        /// Unique key to enable Object Class logic
        /// </summary>
        public CswPropIdAttr PropAttr = null;

        /// <summary>
        /// Text to display on Button
        /// </summary>
        public string SelectedText = string.Empty;

        /// <summary>
        /// Menu Options if a menu button
        /// </summary>
        public string MenuOptions = string.Empty;

        /// <summary>
        /// Column name of the Button
        /// </summary>
        public string DataIndex = string.Empty;

        /// <summary>
        /// Row number of button
        /// </summary>
        public Int32 RowNo = Int32.MinValue;

        /// <summary>
        /// 
        /// </summary>
        public JObject ToJson()
        {
            JObject JButton = new JObject();

            if( null != PropAttr && Int32.MinValue != RowNo )
            {
                JButton["propattr"] = PropAttr.ToString();
                JButton["index"] = DataIndex;
                JButton["rowno"] = RowNo;
                if( false == string.IsNullOrEmpty( SelectedText ) )
                {
                    JButton["selectedtext"] = SelectedText;
                }
                if( false == string.IsNullOrEmpty( MenuOptions ) )
                {
                    JButton["menuoptions"] = MenuOptions;
                }

            }
            return JButton;
        } // ToJson()

    } // class CswGridExtJsRow

} // namespace ChemSW.Nbt.Grid.ExtJs
