using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Security
{

    /// <summary>
    /// Type of NodeTypePermission on NodeTypeTabs
    /// </summary>
    public enum CswEnumNbtNodeTypeTabPermission
    {
        /// <summary>
        /// NodeTypePermission to view the tab
        /// </summary>
        View,

        /// <summary>
        /// NodeTypePermission to edit property values on this tab
        /// </summary>
        Edit
    };

} // namespace ChemSW.Nbt.Security
