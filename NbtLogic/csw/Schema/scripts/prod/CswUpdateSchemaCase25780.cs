using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25780
    /// </summary>
    public class CswUpdateSchemaCase25780 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // populate gestaltsearch
            // yes, this might take awhile.  85 seconds on cabot.
            
            string Sql = @"update jct_nodes_props
                              set gestaltsearch = SUBSTR(gestalt, 0, 512)
                            where gestalt is not null 
                              and gestaltsearch is null";

            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( Sql );


        }//Update()

    }//class CswUpdateSchemaCase25780

}//namespace ChemSW.Nbt.Schema