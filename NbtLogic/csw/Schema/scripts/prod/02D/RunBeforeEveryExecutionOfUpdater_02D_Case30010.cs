using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

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
            // case 30008
            _addRegulatoryListListCodeOC();
            // case 30010
            _addRegListLOLIListCodesGrid();
            _addRegListListModeProp();
            _addPropFiltertoAddCASNosProp();
            // case 30126
            _removeC3SyncDateMPSProp();

        } //Update()

        private void _addRegulatoryListListCodeOC()
        {
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
                if( null == RegListListCodeOC )
                {
                    // Create the object class
                    RegListListCodeOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass, "doc.png", false );

                    // Create the properties
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListListCodeOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = RegListOC.ObjectClassId,
                        IsCompoundUnique = true,
                        ReadOnly = true,
                        DisplayRowAdd = 1,
                        DisplayColAdd = 1
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListListCodeOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListName,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = "",
                        SetValOnAdd = true,
                        DisplayRowAdd = 2,
                        DisplayColAdd = 1
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListListCodeOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListCode,
                        FieldType = CswEnumNbtFieldType.Number,
                        ServerManaged = true
                    } );

                    // Tie to the Regulatory Lists module
                    _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.RegulatoryLists, RegListListCodeOC.ObjectClassId );

                } // if( null == RegListCasListCode )
            } // if( null != RegListOC )
        } // _addRegulatoryListListCodeOC

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
        }

        private void _removeC3SyncDateMPSProp()
        {
            // Remove the C3SyncDate Property from the Material Property Set
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass CurrentObjectClass in MaterialPS.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp C3SyncDateOCP = CurrentObjectClass.getObjectClassProp( "C3SyncDate" );
                if( null != C3SyncDateOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteJctPropertySetOcPropRow( C3SyncDateOCP );
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( C3SyncDateOCP, true );
                }
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30010
}//namespace ChemSW.Nbt.Schema


