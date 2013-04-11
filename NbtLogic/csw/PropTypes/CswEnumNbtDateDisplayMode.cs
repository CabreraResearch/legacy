using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    /// <summary>
    /// Possible display modes for dates
    /// </summary>
    public enum CswEnumNbtDateDisplayMode
    {
        /// <summary>
        /// unknown display mode
        /// </summary>
        Unknown,

        /// <summary>
        /// display date only
        /// </summary>
        Date,

        /// <summary>
        /// display time only
        /// </summary>
        Time,

        /// <summary>
        /// display date and time
        /// </summary>
        DateTime
    };
}//namespace ChemSW.Nbt.PropTypes
