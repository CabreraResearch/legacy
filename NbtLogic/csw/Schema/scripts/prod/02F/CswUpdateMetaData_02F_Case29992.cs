using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02F_Case29992 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 29992"; } }

        public override string ScriptName
        {
            get { return "02F_Case29992_MetaData"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29992; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestMaterialCreateOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialCreateClass );
            CswNbtMetaDataObjectClassProp QuantityOCP = RequestMaterialCreateOC.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Quantity );
            if( null == QuantityOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestMaterialCreateOC,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.Quantity,
                        FieldType = CswEnumNbtFieldType.Quantity,
                        IsRequired = true,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.NodeTypeId.ToString()
                    } );
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30281
}//namespace ChemSW.Nbt.Schema


