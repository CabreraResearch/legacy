using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30228: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30228; }
        }

        public override void update()
        {

            #region Update all NTPs "Hidden" column to "false"

            CswTableUpdate nodeTypePropsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ntp.setHidden", "nodetype_props" );
            DataTable nodetypePropsDT = nodeTypePropsTU.getTable();
            foreach( DataRow row in nodetypePropsDT.Rows )
            {
                row["hidden"] = CswConvert.ToDbVal( false );
            }
            nodeTypePropsTU.update( nodetypePropsDT );

            #endregion


        } // update()

    }

}//namespace ChemSW.Nbt.Schema