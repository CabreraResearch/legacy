
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the C3 Module
    /// </summary>
    public class CswNbtModuleRuleChemWatch : CswNbtModuleRule
    {
        public CswNbtModuleRuleChemWatch( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.ChemWatch; } }
        protected override void OnEnable()
        {
            _toggleChemWatchBtn( false );
        }// OnEnabled

        protected override void OnDisable()
        {
            _toggleChemWatchBtn( true );
        } // OnDisable()

        private void _toggleChemWatchBtn( bool Hidden )
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LinkChemWatchNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.LinkChemWatch );
                LinkChemWatchNTP.Hidden = Hidden;
            }
        }

    } // class CswNbtModuleC3
}// namespace ChemSW.Nbt
