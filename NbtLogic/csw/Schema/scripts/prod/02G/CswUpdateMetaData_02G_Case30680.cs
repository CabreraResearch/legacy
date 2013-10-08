using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case30680 : CswUpdateSchemaTo
    {
        public override string Title { get { return "GHS Add Codes Properties"; } }

        public override string ScriptName
        {
            get { return "Case_30680OC"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30680; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass GHSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( GHSOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassGHS.PropertyName.AddClassCodes,
                FieldType = CswEnumNbtFieldType.Memo,
                SetValOnAdd = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( GHSOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassGHS.PropertyName.AddLabelCodes,
                FieldType = CswEnumNbtFieldType.Memo,
                SetValOnAdd = true
            } );
        }
    }
}


