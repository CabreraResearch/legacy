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
    /// Type of NodeTypePermission on NodeTypes
    /// </summary>
    public enum CswEnumNbtNodeTypePermission
    {
        /// <summary>
        /// NodeTypePermission to view nodes of this type
        /// </summary>
        View,
        /// <summary>
        /// NodeTypePermission to create new nodes of this type
        /// </summary>
        Create,
        /// <summary>
        /// NodeTypePermission to delete nodes of this type
        /// </summary>
        Delete,
        /// <summary>
        /// NodeTypePermission to edit property values of nodes of this type
        /// </summary>
        Edit
    }

} // namespace ChemSW.Nbt.Security
