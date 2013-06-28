using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Regulatory Lists Module
    /// </summary>
    public class CswNbtModuleRuleRegulatoryLists : CswNbtModuleRule
    {
        public CswNbtModuleRuleRegulatoryLists( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.RegulatoryLists; } }
        protected override void OnEnable()
        {
            //Show the following Chemical properties
            //   Regulatory Lists
            int ChemicalOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.ChemicalClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( ChemicalOC_Id ).Keys )
            {
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassChemical.PropertyName.RegulatoryListsGrid );
            }

        }

        protected override void OnDisable()
        {
            //Hide the following Material properties
            //   Regulatory Lists
            int ChemicalOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.ChemicalClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( ChemicalOC_Id ).Keys )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassChemical.PropertyName.RegulatoryListsGrid );
            }

        } // OnDisable()

    } // class CswNbtModuleRegulatoryLists
}// namespace ChemSW.Nbt
