using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27071RequestItem2
    /// </summary>
    public class CswUpdateSchemaCase27071RequestItem2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtObjClassRole NodeAsRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Dispenser" );
            if( null != NodeAsRole )
            {
                NodeAsRole.Name.Text = "CISPro_Dispenser";
                NodeAsRole.postChanges( true );
            }

        }//Update()

    }//class CswUpdateSchemaCase27071RequestItem2

}//namespace ChemSW.Nbt.Schema