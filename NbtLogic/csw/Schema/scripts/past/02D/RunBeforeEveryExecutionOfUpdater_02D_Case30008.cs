using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30008 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30008; }
        }

        public override void update()
        {
            _addRegulatoryListListCodeOC();

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
        } // _addRegulatoryListListCodeOC()

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30008
}//namespace ChemSW.Nbt.Schema


