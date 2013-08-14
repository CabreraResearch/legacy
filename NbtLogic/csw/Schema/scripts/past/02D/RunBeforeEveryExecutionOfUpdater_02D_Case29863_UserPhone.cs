using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case29863_UserPhone : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 29863: User.Phone";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30194; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( UserOc )
            {
                PropName = CswNbtObjClassUser.PropertyName.Phone,
                FieldType = CswEnumNbtFieldType.Text
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( UserOc )
            {
                PropName = CswNbtObjClassUser.PropertyName.EmployeeId,
                FieldType = CswEnumNbtFieldType.Text,
                IsUnique = true
            } );


        }

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30194
}//namespace ChemSW.Nbt.Schema


