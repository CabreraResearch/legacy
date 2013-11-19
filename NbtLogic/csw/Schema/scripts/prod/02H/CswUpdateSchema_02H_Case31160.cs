using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case31160: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31160; }
        }

        public override string AppendToScriptName()
        {
            return "02H_Case" + CaseNo;
        }

        public override string Title
        {
            get { return "Add prefix to NTPs with no OCP"; }
        }

        public override void update()
        {

            CswTableUpdate ntpTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "Add_prefix_to_NTP_with_no_OCP", "nodetype_props" );
            DataTable ntpDT = ntpTU.getTable( new CswCommaDelimitedString( "oraviewcolname" ), string.Empty, Int32.MinValue, "where objectclasspropid is null", false );

            foreach( DataRow row in ntpDT.Rows )
            {
                row["oraviewcolname"] = CswNbtMetaData.OraViewColNamePrefix + row["oraviewcolname"];
            }
            ntpTU.update( ntpDT );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema