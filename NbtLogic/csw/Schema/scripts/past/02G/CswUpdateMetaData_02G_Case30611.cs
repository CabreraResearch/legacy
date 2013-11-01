using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case30611 : CswUpdateSchemaTo
    {
        public override string Title { get { return "ContainerDispenseTransaction Dispenser Property"; } }

        public override string ScriptName
        {
            get { return "Case_30611"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30611; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass CDTOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerDispenseTransactionClass );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CDTOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassContainerDispenseTransaction.PropertyName.Dispenser,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOC.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );
        }
    }
}


