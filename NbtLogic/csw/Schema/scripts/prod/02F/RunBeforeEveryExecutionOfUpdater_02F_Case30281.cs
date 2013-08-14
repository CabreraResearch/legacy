using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30281 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30281";

        public override string ScriptName
        {
            get { return "02F_Case30281_A"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30281; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataPropertySet MaterialSet = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in MaterialSet.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp ExpirationLockedOCP = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.ContainerExpirationLocked );
                if( null == ExpirationLockedOCP )
                {
                    ExpirationLockedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( MaterialOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtPropertySetMaterial.PropertyName.ContainerExpirationLocked,
                        FieldType = CswEnumNbtFieldType.Logical,
                        SetValOnAdd = false,
                        IsRequired = true
                    } );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ExpirationLockedOCP, CswEnumTristate.True, CswEnumNbtSubFieldName.Checked );
                }
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30281
}//namespace ChemSW.Nbt.Schema


