
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27632
    /// </summary>
    public class CswUpdateSchemaCase27632 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtObjClassUser DispenserNode = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( "dispenser" );
            if( null != DispenserNode )
            {
                DispenserNode.UsernameProperty.Text = "cispro_dispenser";
                DispenserNode.IsDemo = true;
                DispenserNode.postChanges( false );
            }

            _markRoleAsDemoData( "CISPro_View_Only" );
            _markRoleAsDemoData( "CISPro_Request_Fulfiller" );
            _markRoleAsDemoData( "CISPro_Receiver" );
            _markRoleAsDemoData( "CISPro_General" );
            _markRoleAsDemoData( "CISPro_Admin" );
            _markRoleAsDemoData( "CISPro_Dispenser" );

            _markUserAsDemoData( "cispro_view_only" );
            _markUserAsDemoData( "cispro_request_fulfiller" );
            _markUserAsDemoData( "cispro_receiver" );
            _markUserAsDemoData( "cispro_general" );
            _markUserAsDemoData( "cispro_admin" );

            _markViewAsDemoData( "Action Required Lab Safety Inspections (demo)" );
            _markViewAsDemoData( "Completed Lab Safety Inspections (demo)" );
            _markViewAsDemoData( "Due Lab Safety Inspections (demo)" );
            _markViewAsDemoData( "Groups, Lab Safety Checklist: Lab Safety (demo)" );
            _markViewAsDemoData( "Inspections, Lab Safety Checklist: Lab Safety (demo)" );
            _markViewAsDemoData( "Lab Safety By Location (demo)" );
            _markViewAsDemoData( "Scheduling, Lab Safety Checklist: Lab Safety (demo)" );
        }//Update()

        private void _markRoleAsDemoData( string RoleName )
        {
            CswNbtObjClassRole RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( RoleName );
            if( null != RoleNode )
            {
                RoleNode.IsDemo = true;
                RoleNode.postChanges( false );
            }
        }

        private void _markUserAsDemoData( string UserName )
        {
            CswNbtObjClassUser UserNode = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( UserName );
            if( null != UserNode )
            {
                UserNode.IsDemo = true;
                UserNode.postChanges( false );
            }
        }

        private void _markViewAsDemoData( string ViewName )
        {
            CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( ViewName );
            if( null != View )
            {
                View.IsDemo = true;
                View.save();
            }
        }

    }

}//namespace ChemSW.Nbt.Schema