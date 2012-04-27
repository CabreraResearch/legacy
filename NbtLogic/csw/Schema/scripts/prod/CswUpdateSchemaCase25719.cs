using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25719
    /// </summary>
    public class CswUpdateSchemaCase25719 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Clean up invalid Welcome content

            CswTableUpdate WelcomeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "25719_welcome_update", "welcome" );
            string WhereClause = @"where nodeviewid not in (select nodeviewid from node_views)
                   or nodetypeid not in (select nodetypeid from nodetypes)
                   or actionid not in (select actionid from actions)
                   or roleid not in
                      (select nodeid
                         from nodes n
                         join nodetypes t on n.nodetypeid = t.nodetypeid
                         join object_class oc on t.objectclassid = oc.objectclassid
                        where oc.objectclass = 'RoleClass')
                   or reportid not in
                      (select nodeid
                         from nodes n
                         join nodetypes t on n.nodetypeid = t.nodetypeid
                         join object_class oc on t.objectclassid = oc.objectclassid
                        where oc.objectclass = 'ReportClass')";
            DataTable WelcomeTable = WelcomeUpdate.getTable( WhereClause );
            foreach( DataRow WelcomeRow in WelcomeTable.Rows )
            {
                WelcomeRow.Delete();
            }
            WelcomeUpdate.update( WelcomeTable );

        }//Update()

    }//class CswUpdateSchemaCase25719

}//namespace ChemSW.Nbt.Schema