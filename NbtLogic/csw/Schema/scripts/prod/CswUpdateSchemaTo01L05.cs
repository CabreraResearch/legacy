using System;
using ChemSW.Audit;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-05
    /// </summary>
    public class CswUpdateSchemaTo01L05 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 05 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24415

            _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.DisableChemSwAdmin, Recurrence.Daily, 1 );

            CswNbtMetaDataObjectClassProp LoginOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass,
                                                                                                    CswNbtObjClassCustomer.LoginPropertyName,
                                                                                                    CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                                                                    false, true, false, CswNbtViewProperty.CswNbtPropType.Unknown, Int32.MinValue, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue,
                                                                                                    CswNbtNodePropButton.ButtonMode.button.ToString(),
                                                                                                    false,
                                                                                                    AuditLevel.NoAudit,
                                                                                                    "Login"
                );

            #endregion Case 24415

        }//Update()

    }//class CswUpdateSchemaTo01L05

}//namespace ChemSW.Nbt.Schema


