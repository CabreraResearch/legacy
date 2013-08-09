using System;
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
            _upgradeDepartmentToObjectClass();
            _upgradeLQNoToObjectClass();
        }

        private void _upgradeDepartmentToObjectClass()
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

        private void _upgradeLQNoToObjectClass()
        {
            CswNbtMetaDataObjectClass LQNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LQNoClass );
            if( null == LQNoOC )
            {
                LQNoOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.LQNoClass, "folder.png", false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( LQNoOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassLQNo.PropertyName.LQNo,
                    FieldType = CswEnumNbtFieldType.Text,
                    IsUnique = true
                } );
                CswNbtMetaDataNodeType WeightNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Weight)" );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( LQNoOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassLQNo.PropertyName.Limit,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    IsRequired = true,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(),
                    FkValue = WeightNt != null ? WeightNt.NodeTypeId : Int32.MinValue
                } );
                CswNbtMetaDataNodeType LQNoNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "LQNo" );
                if( null != LQNoNT )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.ConvertObjectClass( LQNoNT, LQNoOC );
                }
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30251
}//namespace ChemSW.Nbt.Schema


