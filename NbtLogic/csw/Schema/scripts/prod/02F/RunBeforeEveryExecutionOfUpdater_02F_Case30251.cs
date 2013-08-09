using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30251 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30251";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30251; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass DepartmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DepartmentClass );
            if( null == DepartmentOC )
            {
                DepartmentOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DepartmentClass, "folder.png", false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( DepartmentOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassDepartment.PropertyName.DepartmentName,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsRequired = true,
                    IsUnique = true
                } );
                CswNbtMetaDataNodeType DepartmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Department" );
                if( null != DepartmentNT )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( DepartmentNT, DepartmentOC );
                }
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30281
}//namespace ChemSW.Nbt.Schema


