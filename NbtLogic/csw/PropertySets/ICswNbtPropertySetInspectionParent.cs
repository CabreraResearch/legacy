using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Nbt.PropTypes;
using ChemSW.TblDn;


namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// This interface defines Object Classes that can be parents of Inspections
    /// </summary>
    public interface ICswNbtPropertySetInspectionParent
    {
        string InspectionParentStatusPropertyName { get; }
        string InspectionParentLastInspectionDatePropertyName { get; }

        CswNbtNodePropList Status { get; }
        CswNbtNodePropDateTime LastInspectionDate { get; }

    }//ICswNbtPropertySetInspectionParent

}//namespace ChemSW.Nbt.ObjClasses
