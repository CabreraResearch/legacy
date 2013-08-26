using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02E_Case30347 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 30347"; } }
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30347; }
        }

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // Add 'ActionByUser' property to ContainerLocation

            CswNbtMetaDataObjectClass ContainerLocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerLocationClass );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );

            CswNbtMetaDataObjectClassProp ActionByUserOCP = ContainerLocationOC.getObjectClassProp( CswNbtObjClassContainerLocation.PropertyName.ActionByUser );
            if( null == ActionByUserOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOC )
                    {
                        PropName = CswNbtObjClassContainerLocation.PropertyName.ActionByUser,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = UserOC.ObjectClassId,
                        ServerManaged = true
                    } );
            }
        } // update()

    }//class RunBeforeEveryExecutionOfUpdater_02E_Case30347
}//namespace ChemSW.Nbt.Schema


