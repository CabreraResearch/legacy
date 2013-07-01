
using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the LOLI Sync Module
    /// </summary>
    public class CswNbtModuleRuleLOLISync : CswNbtModuleRule
    {
        public CswNbtModuleRuleLOLISync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.LOLISync; } }
        protected override void OnEnable()
        {
            // Clear the C3SyncDate property of all Chemicals where C3SyncDate is not null.
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "ClearC3SyncDateOfChemicals";
            CswNbtViewRelationship ParentRelationship = View.AddViewRelationship( ChemicalOC, true );
            View.AddViewPropertyAndFilter( ParentViewRelationship: ParentRelationship,
                                           MetaDataProp: ChemicalOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.C3SyncDate ),
                                           SubFieldName: CswEnumNbtSubFieldName.Value,
                                           FilterMode: CswEnumNbtFilterMode.NotNull );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, true, true );
            for( int i = 0; i < Tree.getChildNodeCount(); i++ )
            {
                Tree.goToNthChild( i );

                CswNbtObjClassChemical CurrentChemicalNode = Tree.getCurrentNode();
                // Setting this to DateTime.MinValue is like setting the value to null.
                CurrentChemicalNode.C3SyncDate.DateTimeValue = DateTime.MinValue;
                CurrentChemicalNode.postChanges( false );

                Tree.goToParentNode();
            }

        }// OnEnabled

        protected override void OnDisable()
        {
        } // OnDisable()

    } // class CswNbtModuleLOLISync
}// namespace ChemSW.Nbt
