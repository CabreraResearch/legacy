using ChemSW.Nbt.csw.Dev;
using ChemSW.DB;
using ChemSW.Core;
using System.Data;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28752
    /// </summary>
    public class CswUpdateSchema_01Y_Case28752 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28752; }
        }

        public override void update()
        {

            //initialize the includeinquotabar columns to true

            CswTableUpdate object_classTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "includeinquotabar_init_oc", "object_class" );
            DataTable object_class = object_classTU.getTable( "where includeinquotabar is null" );
            foreach( DataRow Row in object_class.Rows )
            {
                Row["includeinquotabar"] = CswConvert.ToDbVal( true );
            }
            object_classTU.update( object_class );

            CswTableUpdate nodetypesTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "includeinquotabar_init_oc", "nodetypes" );
            DataTable nodetypes = nodetypesTU.getTable( "where includeinquotabar is null" );
            foreach( DataRow Row in object_class.Rows )
            {
                Row["includeinquotabar"] = CswConvert.ToDbVal( true );
            }
            nodetypesTU.update( nodetypes );

        } //Update()

    }//class CswUpdateSchema_01Y_Case28752

}//namespace ChemSW.Nbt.Schema