using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25226
    /// </summary>
    public class CswUpdateSchemaCase25226 : CswUpdateSchemaTo
    {

        public override void update()
        {

            CswNbtNode cswAdminNode = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
            if( null != cswAdminNode )
            {
                CswNbtObjClassUser cswAdminAsUser = CswNbtNodeCaster.AsUser( cswAdminNode );
                DateTime adate = new DateTime( 2204, 1, 1 );
                cswAdminAsUser.PasswordProperty.ChangedDate = adate;
                cswAdminAsUser.postChanges( true );
            }

        }//Update()

    }//class CswUpdateSchemaCase25226

}//namespace ChemSW.Nbt.Schema