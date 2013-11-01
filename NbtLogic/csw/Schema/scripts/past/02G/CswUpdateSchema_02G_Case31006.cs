using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case31006 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31006; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Fix oraviewcolname"; }
        }

        public override void update()
        {
            CswTableSelect select = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "31006_select", "nodetype_props" );
            DataTable tbl = select.getTable( "where oraviewcolname = 'Q12147483648'" );
            foreach( DataRow row in tbl.Rows )
            {
                Int32 PropId = CswConvert.ToInt32( row["nodetypepropid"] );
                CswNbtMetaDataNodeTypeProp nodeTypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( PropId );
                nodeTypeProp.resetDbViewColumnName();
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema