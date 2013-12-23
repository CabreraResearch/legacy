using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02J_Case30825B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30825; }
        }

        public override string Title
        {
            get { return "Add 'Regions' property to Regulatory List"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            CswNbtMetaDataObjectClassProp RegionsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RegListOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRegulatoryList.PropertyName.Regions,
                    FieldType = CswEnumNbtFieldType.MultiList
                } );

            // Update ListModeOCP options
            CswNbtMetaDataObjectClassProp ListModeOCP = RegListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.ListMode );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ListModeOCP, CswEnumNbtObjectClassPropAttributes.listoptions, CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.Options.ToString() );

            // Set conditional filter
            RegionsOCP.setFilter( FilterProp: ListModeOCP,
                                  SubField: ListModeOCP.getFieldTypeRule().SubFields.Default,
                                  FilterMode: CswEnumNbtFilterMode.Equals,
                                  FilterValue: CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.ArielManaged );


        } // update()

    }

}//namespace ChemSW.Nbt.Schema