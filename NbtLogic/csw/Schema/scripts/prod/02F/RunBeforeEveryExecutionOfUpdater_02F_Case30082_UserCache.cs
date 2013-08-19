using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30082_UserCache : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30082";

        public override string ScriptName
        {
            get { return "02F_Case30082_UserCache"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30082; }
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
                PropName = CswNbtObjClassUser.PropertyName.CachedData,
                FieldType = CswEnumNbtFieldType.Memo,
                ServerManaged = true
            } );
        }


    }//class 
}//namespace ChemSW.Nbt.Schema


