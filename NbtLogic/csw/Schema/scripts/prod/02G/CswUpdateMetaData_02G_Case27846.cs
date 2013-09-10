using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for MetaData changes
    /// </summary>
    public class CswUpdateMetaData_02G_Case27846 : CswUpdateSchemaTo
    {
        public override string Title { get { return "UserSelect FieldType Field1 to ClobData"; } }

        public override string ScriptName
        {
            get { return "Case_27846"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27846; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataFieldType UserSelectFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.UserSelect );
            CswTableUpdate FTSubUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "UserSelectUpdate", "field_types_subfields" );
            DataTable FTSubTable = FTSubUpdate.getTable( "where fieldtypeid = " + UserSelectFT.FieldTypeId + " and propcolname = '" + CswEnumNbtPropColumn.Field1 + "'" );
            if( FTSubTable.Rows.Count > 0 )
            {
                FTSubTable.Rows[0]["propcolname"] = CswEnumNbtPropColumn.ClobData;
                FTSubUpdate.update( FTSubTable );
            }

            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"
                update jct_nodes_props jnp
                    set clobdata = field1, 
                        field1 = null
                    where jnp.nodetypepropid in 
                    (select nodetypepropid 
                        from nodetype_props ntp 
                        where ntp.fieldtypeid = " + UserSelectFT.FieldTypeId + ")" );
        }
    }
}


