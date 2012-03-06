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
    /// Updates the schema to version case25226
    /// </summary>
    public class CswUpdateSchemaToCase25226 : CswUpdateSchemaTo
    {

        public override void update()
        {

            CswNbtNode cswAdminNode = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername );
            if( null != cswAdminNode )
            {
                CswNbtObjClassUser cswAdminAsUser = CswNbtNodeCaster.AsUser( cswAdminNode );
                DateTime adate = new DateTime( 2204, 1, 1 );
                cswAdminAsUser.PasswordProperty.ChangedDate = adate;
            }

        }//Update()

    }//class CswUpdateSchemaTo01M13

}//namespace ChemSW.Nbt.Schema