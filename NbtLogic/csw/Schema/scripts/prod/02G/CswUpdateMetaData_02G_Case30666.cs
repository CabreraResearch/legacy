using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case30666 : CswUpdateSchemaTo
    {
        public override string Title { get { return "User Cost Code Property"; } }

        public override string ScriptName
        {
            get { return "Case_30666_MetaData"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30666; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp CostCodeOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.CostCode ) ??
            _CswNbtSchemaModTrnsctn.createObjectClassProp( UserOC, new CswNbtWcfMetaDataModel.ObjectClassProp( UserOC )
                {
                    PropName = CswNbtObjClassUser.PropertyName.CostCode,
                    FieldType = CswEnumNbtFieldType.Text
                } );
        }
    }
}


