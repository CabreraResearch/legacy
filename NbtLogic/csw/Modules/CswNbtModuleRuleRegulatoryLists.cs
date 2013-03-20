using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.DB;
using ChemSW.Core;

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
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.RegulatoryLists; } }
        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
            }

            //Show the following Material properties
            //   Regulatory Lists
            int MaterialOC_Id = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.MaterialClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( MaterialOC_Id ) )
            {
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassMaterial.PropertyName.RegulatoryLists );
            }

        }

        public override void OnDisable()
        {
            //Hide the following Material properties
            //   Regulatory Lists
            int MaterialOC_Id = _CswNbtResources.MetaData.getObjectClassId( NbtObjectClass.MaterialClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( MaterialOC_Id ) )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassMaterial.PropertyName.RegulatoryLists );
            }

        } // OnDisable()

    } // class CswNbtModuleRegulatoryLists
}// namespace ChemSW.Nbt
