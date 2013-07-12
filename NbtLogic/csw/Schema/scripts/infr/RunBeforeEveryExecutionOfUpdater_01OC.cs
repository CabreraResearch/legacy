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
    public class RunBeforeEveryExecutionOfUpdater_01OC : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        #region Blame Logic

        private void _acceptBlame( UnitOfBlame Blame )
        {
            _Author = Blame.Developer;
            _CaseNo = Blame.CaseNumber;
        }

        private void _acceptBlame( CswEnumDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswEnumDeveloper.NBT;
            _CaseNo = 0;
        }

        private CswEnumDeveloper _Author = CswEnumDeveloper.NBT;

        public override CswEnumDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        #endregion Blame Logic

        #region Private helpers

        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswEnumNbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswEnumNbtLayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswEnumNbtLayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        private static string _makeNodeTypePermissionValue( Int32 FirstVersionNodeTypeId, CswEnumNbtNodeTypePermission Permission )
        {
            return "nt_" + FirstVersionNodeTypeId.ToString() + "_" + Permission.ToString();
        }

        #endregion Private helpers

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region DOGWOOD

            _addRegulatoryListListCodeOC( new UnitOfBlame( CswEnumDeveloper.CM, 30008 ) );
            _addRegListLOLIListCodesGrid( new UnitOfBlame( CswEnumDeveloper.CM, 30010 ) );
            _addRegListListModeProp( new UnitOfBlame( CswEnumDeveloper.CM, 30010 ) );
            _addPropFiltertoAddCASNosProp( new UnitOfBlame( CswEnumDeveloper.CM, 30010 ) );

            #endregion DOGWOOD

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        } //Update()

        #region DOGWOOD Methods

        private void _addRegulatoryListListCodeOC( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

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
            _resetBlame();
        } // _addRegulatoryListListCodeOC

        private void _addRegListLOLIListCodesGrid( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

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
            _resetBlame();
        } // _addLoliCodesGrid()

        private void _addRegListListModeProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

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

            _resetBlame();
        }// _addRegListListModeProp()

        private void _addPropFiltertoAddCASNosProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

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

            _resetBlame();
        }

        #endregion DOGWOOD Methods

    }//class RunBeforeEveryExecutionOfUpdater_01OC
}//namespace ChemSW.Nbt.Schema


