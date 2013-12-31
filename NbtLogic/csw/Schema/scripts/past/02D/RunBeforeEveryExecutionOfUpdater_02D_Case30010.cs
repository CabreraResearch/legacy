using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30010 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30010; }
        }

        public override void update()
        {
            _addRegListLOLIListCodesGrid();
            _addRegListListModeProp();
            _addPropFiltertoAddCASNosProp();

        } //Update()

        private void _addRegListLOLIListCodesGrid()
        {
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                // Grid property
                CswNbtMetaDataObjectClassProp RegListLOLIListCodesGridOCP = RegListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.LOLIListCodes );
                if( null == RegListLOLIListCodesGridOCP )
                {
                    RegListLOLIListCodesGridOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListOC )
                    {
                        PropName = CswNbtObjClassRegulatoryList.PropertyName.LOLIListCodes,
                        FieldType = CswEnumNbtFieldType.Grid
                    } );

                    CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
                    if( null != RegListListCodeOC )
                    {
                        CswNbtMetaDataObjectClassProp RegListListCodeRegListOCP = RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList );

                        // Grid View
                        CswNbtView RegListListCodesView = _CswNbtSchemaModTrnsctn.makeView();
                        RegListListCodesView.ViewName = CswNbtObjClassRegulatoryList.PropertyName.LOLIListCodes;
                        RegListListCodesView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                        CswNbtViewRelationship RegListRel = RegListListCodesView.AddViewRelationship( RegListOC, false );
                        CswNbtViewRelationship MemberRel = RegListListCodesView.AddViewRelationship( RegListRel, CswEnumNbtViewPropOwnerType.Second, RegListListCodeRegListOCP, true );
                        CswNbtViewProperty LOLIListNameVP = RegListListCodesView.AddViewProperty( MemberRel, RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListName ), 1 );
                        _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RegListLOLIListCodesGridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, RegListListCodesView.ToString() );
                    }
                } // if( null == RegListListCodesGridOCP )
            } // if( null != RegListOC )
        } // _addLoliCodesGrid()

        private void _addRegListListModeProp()
        {
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                CswNbtMetaDataObjectClassProp FileTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListOC )
                {
                    PropName = CswNbtObjClassRegulatoryList.PropertyName.ListMode,
                    FieldType = CswEnumNbtFieldType.List,
                    ListOptions = CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.Options.ToString(),
                    IsRequired = true,
                    SetValOnAdd = true,

                } );
            }//if (null != RegListOC)
        }// _addRegListListModeProp()

        private void _addPropFiltertoAddCASNosProp()
        {
            CswNbtMetaDataObjectClass RegulatoryListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegulatoryListOC )
            {
                CswNbtMetaDataObjectClassProp ListModeOCP = RegulatoryListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.ListMode );
                CswNbtMetaDataObjectClassProp AddCASNumbersOCP = RegulatoryListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.AddCASNumbers );
                AddCASNumbersOCP.setFilter( FilterProp: ListModeOCP,
                                            SubField: ListModeOCP.getFieldTypeRule().SubFields.Default,
                                            FilterMode: CswEnumNbtFilterMode.Equals,
                                            FilterValue: CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.ManuallyManaged );

            }
        }//_addPropFiltertoAddCASNosProp()

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30010
}//namespace ChemSW.Nbt.Schema


