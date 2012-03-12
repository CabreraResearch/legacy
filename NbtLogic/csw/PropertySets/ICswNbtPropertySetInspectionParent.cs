using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.PropertySets
{
    /// <summary>
    /// This interface defines Object Classes that can be parents of Inspections
    /// </summary>
    public interface ICswNbtPropertySetInspectionParent
    {
        string InspectionParentStatusPropertyName { get; }
        //string InspectionParentLastInspectionDatePropertyName { get; }

        CswNbtNodePropList Status { get; }
        //CswNbtNodePropDateTime LastInspectionDate { get; }

    }//ICswNbtPropertySetInspectionParent

}//namespace ChemSW.Nbt.ObjClasses
