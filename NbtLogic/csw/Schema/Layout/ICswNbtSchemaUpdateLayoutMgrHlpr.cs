using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using System;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.csw.Schema
{
    internal interface ICswNbtSchemaUpdateLayoutMgrHlpr
    {
        /// <summary>
        /// Copies a property to the tab. If the tab does not exist, this will create the tab.
        /// </summary>
        /// <param name="PropName"></param>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="LayoutType">Default to CurrentLayoutType</param>
        void copyProp( string PropName, Int32 Row, Int32 Column, CswEnumNbtLayoutType LayoutType = null );

        /// <summary>
        /// Moves a property on the "first" tab, which should be the tab of the same name as the NodeType else the first tab by sequence.
        /// </summary>
        void moveProp( string PropName, Int32 Row, Int32 Column, CswEnumNbtLayoutType LayoutType = null );

        /// <summary>
        /// Remove a property from the tab of a specific layout or from all tabs of a layout if the current tab is null.
        /// </summary>
        void removeProp( string PropName, CswEnumNbtLayoutType LayoutType = null );

        void setPermit( CswEnumNbtNodeTypeTabPermission Permission, bool Value, Collection<CswNbtObjClassRole> Roles );
    }
}