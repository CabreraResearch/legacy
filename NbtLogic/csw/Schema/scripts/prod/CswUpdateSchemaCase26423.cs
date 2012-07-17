using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Welcome;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26423
    /// </summary>
    public class CswUpdateSchemaCase26423 : CswUpdateSchemaTo
    {
        public override void update()
        {


            CswNbtWelcomeTable welcomeTable = _CswNbtSchemaModTrnsctn.getWelcomeTable();

            //get the role id of chemsw_admin
            CswNbtMetaDataNodeType roleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" );
            string strRoleId = null;
            if( null != roleNT )
            {
                foreach( CswNbtNode node in roleNT.getNodes( false, true ) )
                {
                    if( node.NodeName.Equals( "chemsw_admin_role" ) )
                    {
                        strRoleId = node.NodeId.ToString();
                    }
                }
            }

            if( null != strRoleId )
            {
                Int32 designActId = _CswNbtSchemaModTrnsctn.Actions[Actions.CswNbtActionName.Design].ActionId;
                Int32 createInsId = _CswNbtSchemaModTrnsctn.Actions[Actions.CswNbtActionName.Create_Inspection].ActionId;
                CswNbtWelcomeTable.WelcomeComponentType linkType = CswNbtWelcomeTable.WelcomeComponentType.Link;

                welcomeTable.AddWelcomeItem( linkType, CswNbtView.ViewType.Action, designActId.ToString(), Int32.MinValue, "Design Mode", Int32.MinValue, Int32.MinValue, "wrench.gif", strRoleId );
                welcomeTable.AddWelcomeItem( linkType, CswNbtView.ViewType.Action, createInsId.ToString(), Int32.MinValue, "Create Inspection", Int32.MinValue, Int32.MinValue, "createinspection.png", strRoleId );
            }

        }//Update()

    }//class CswUpdateSchemaCase26423

}//namespace ChemSW.Nbt.Schema