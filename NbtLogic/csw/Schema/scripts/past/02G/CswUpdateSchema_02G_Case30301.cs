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
    public class CswUpdateSchema_02G_Case30301 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Copy Admin Views to ChemSWAdmin"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30301; }
        }

        public override string ScriptName
        {
            get { return "Admin Views"; }
        }

        public override void update()
        {
            Dictionary<string, CswNbtViewId> AdminViews = new Dictionary<string, CswNbtViewId>();
            Dictionary<string, CswNbtViewId> ChemSWAdminViews = new Dictionary<string, CswNbtViewId>();
            DataTable ViewsTable = _CswNbtSchemaModTrnsctn.getAllViews();
            foreach( DataRow Row in ViewsTable.Rows )
            {
                if( CswConvert.ToString( Row["visibility"] ) == CswEnumNbtViewVisibility.Role.ToString() )
                {
                    Int32 RoleId = CswConvert.ToInt32( Row["roleid"] );
                    if( Int32.MinValue != RoleId )
                    {
                        CswNbtObjClassRole Role = _CswNbtSchemaModTrnsctn.Nodes["nodes_" + RoleId];
                        if( null != Role && Role.Administrator.Checked == CswEnumTristate.True )
                        {
                            string ViewName = CswConvert.ToString( Row["viewname"] );
                            CswNbtViewId ViewId = new CswNbtViewId( CswConvert.ToInt32( Row["nodeviewid"] ) );
                            if( Role.Name.Text == CswNbtObjClassRole.ChemSWAdminRoleName )
                            {
                                ChemSWAdminViews.Add( ViewName, ViewId );
                            }
                            else if( false == AdminViews.ContainsKey( ViewName ) )
                            {
                                AdminViews.Add( ViewName, ViewId );
                            }
                        }
                    }
                }
            }

            CswNbtObjClassRole ChemSwAdmin = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            foreach( KeyValuePair<string, CswNbtViewId> KeyValuePair in AdminViews )
            {
                string ViewName = KeyValuePair.Key;
                if( false == ChemSWAdminViews.ContainsKey( ViewName ) )
                {
                    CswNbtViewId ViewId = KeyValuePair.Value;
                    
                    CswNbtView NewCswAdminView = _CswNbtSchemaModTrnsctn.makeView();
                    NewCswAdminView.saveNew(ViewName, CswEnumNbtViewVisibility.Role, ChemSwAdmin.NodeId, CopyViewId: ViewId.get() );
                    
                    NewCswAdminView.save();
                }
            }
        }
    }
}