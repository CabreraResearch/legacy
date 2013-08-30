using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case27883: CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 27883"; } }
        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27883; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp AvailableWorkUnitsOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.AvailableWorkUnits );
            if( null == AvailableWorkUnitsOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( UserOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                    {
                        PropName = CswNbtObjClassUser.PropertyName.AvailableWorkUnits,
                        FieldType = CswEnumNbtFieldType.MultiList
                    } );
            }

        }


    }//class RunBeforeEveryExecutionOfUpdater_02F_Case27883
}//namespace ChemSW.Nbt.Schema


