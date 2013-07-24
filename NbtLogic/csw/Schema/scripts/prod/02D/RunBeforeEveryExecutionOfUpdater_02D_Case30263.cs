using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30263
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30263 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30263; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass BatchOpOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BatchOpClass );
            CswNbtMetaDataObjectClassProp BatchOpOpNameOCP = BatchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PropertyName.OpName );

            CswNbtMetaDataFieldType TextFieldType = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Text );

            // Update object_class_props
            CswTableUpdate ObjectClassPropsTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "object_class_props_tbl_update_case_30263", "object_class_props" );
            DataTable OCPDataTable = ObjectClassPropsTableUpdate.getTable( "where objectclassid = " + BatchOpOC.ObjectClassId + " and propname = '" + BatchOpOpNameOCP.PropName + "'" );
            if( OCPDataTable.Rows.Count > 0 )
            {
                OCPDataTable.Rows[0]["fieldtypeid"] = TextFieldType.FieldTypeId;
            }
            ObjectClassPropsTableUpdate.update( OCPDataTable );

            //Update nodetype_props
            foreach( CswNbtMetaDataNodeType BatchOpNT in BatchOpOC.getNodeTypes() )
            {
                CswTableUpdate NodetypePropsTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "nodetype_props_tbl_update_case_30263", "nodetype_props" );
                DataTable NTPDataTable = NodetypePropsTableUpdate.getTable( "where nodetypeid = " + BatchOpNT.NodeTypeId + " and propname = '" + BatchOpOpNameOCP.PropName + "'" );
                if( NTPDataTable.Rows.Count > 0 )
                {
                    NTPDataTable.Rows[0]["fieldtypeid"] = TextFieldType.FieldTypeId;
                }
                NodetypePropsTableUpdate.update( NTPDataTable );
            }

        } // update()

    }//class CswUpdateSchema_02D_Case30263

}//namespace ChemSW.Nbt.Schema