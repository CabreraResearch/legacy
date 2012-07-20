using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27270
    /// </summary>
    public class CswUpdateSchemaCase27270 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Not sure how this came to be, but just fix it for now
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update jct_nodes_props 
                                                                         set readonly = '0' 
                                                                       where jctnodepropid in (select jctnodepropid 
                                                                                                 from jct_nodes_props j 
                                                                                                 join nodetype_props p on p.nodetypepropid = j.nodetypepropid
                                                                                                 join field_types f on f.fieldtypeid = p.fieldtypeid
                                                                                                where j.readonly = '1' 
                                                                                                  and f.fieldtype = 'Grid')" );
        }//Update()

    }//class CswUpdateSchemaCase27270

}//namespace ChemSW.Nbt.Schema