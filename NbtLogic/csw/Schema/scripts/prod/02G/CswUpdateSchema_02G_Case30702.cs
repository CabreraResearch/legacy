using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30702 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Set nodetype_layout auditlevel"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30702; }
        }

        public override string ScriptName
        {
            get { return "case30702 set auditlevel"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update nodetype_layout l
                                                                          set auditlevel = (select auditlevel 
                                                                                              from nodetype_props p
                                                                                             where p.nodetypepropid = l.nodetypepropid)
                                                                        where auditlevel is null" );
        }
    }
}