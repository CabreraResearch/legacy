using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the SDS Module
    /// </summary>
    public class CswNbtModuleRuleSDS : CswNbtModuleRule
    {
        public CswNbtModuleRuleSDS( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.SDS; } }

        protected override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.CISPro );
            }

            //Show the following Material properties...
            //   Assigned SDS
            //   View SDS
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.ShowProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.ViewSDS );
                _CswNbtResources.Modules.ShowProp( ChemicalNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.AssignedSDS );
            }

            //Show the following Container properties...
            //   View SDS
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(CswEnumNbtObjectClass.ContainerClass);
                foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
                {
                    _CswNbtResources.Modules.ShowProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ViewSDS );
                }
            }
        }

        protected override void OnDisable()
        {
            //Hide the following Material properties...
            //   Assigned SDS
            //   View SDS
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( int ChemicalNTId in ChemicalOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( ChemicalNTId, CswNbtObjClassChemical.PropertyName.AssignedSDS );
                _CswNbtResources.Modules.HideProp( ChemicalNTId, CswNbtObjClassChemical.PropertyName.ViewSDS );
            }

            //Hide the following Container properties...
            //   ViewSDS
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(CswEnumNbtObjectClass.ContainerClass);
                foreach( int ContainerNTId in ContainerOC.getNodeTypeIds() )
                {
                    _CswNbtResources.Modules.HideProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.ViewSDS );
                }
            }
        } // OnDisable()
    } // class CswNbtModuleSDS
}// namespace ChemSW.Nbt
