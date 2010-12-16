using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class NodeReport : System.Web.UI.Page
    {
        private CswNbtNode _Node;
        private CswNbtNodeKey _NodeKey;
        private CswAutoTable _Table;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                bool bGetOut = true;
                if( Request.QueryString["nodeid"] != string.Empty )
                {
                    CswPrimaryKey NodeId = new CswPrimaryKey();
                    NodeId.FromString( Request.QueryString["nodeid"] );
                    _Node = Master.CswNbtResources.Nodes[NodeId];
                    _NodeKey = new CswNbtNodeKey( Master.CswNbtResources, null, "", NodeId, _Node.NodeSpecies, _Node.NodeTypeId, _Node.ObjectClassId, "", "" );

                    if( Master.CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, _Node.NodeTypeId, null, null ) )
                    {
                        bGetOut = false;
                    }
                }

                if( bGetOut )
                {
                    //Master.Redirect( "Main.aspx" );
                    Master.GoHome();
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                _Table = new CswAutoTable();
                _Table.ID = "nodereporttable";
                _Table.Width = Unit.Parse( "800px" );
                centerph.Controls.Add( _Table );

                Int32 row = 0;

                TableCell NodeNameCell = _Table.getCell( row, 0 );
                NodeNameCell.ColumnSpan = 2;

                Label NodeNameLabel = new Label();
                NodeNameLabel.Text = _Node.NodeName;
                NodeNameLabel.CssClass = "NodeReport_NodeName";
                NodeNameCell.Controls.Add( NodeNameLabel );
                row++;

                //CswFieldTypeWebControlFactory FTWCFactory = new CswFieldTypeWebControlFactory( Master.CswNbtResources );

                CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( _Node.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeTab Tab in NodeType.NodeTypeTabs )
                {
                    if( Tab.IncludeInNodeReport )
                    {
                        TableCell TabNameCell = _Table.getCell( row, 0 );
                        TabNameCell.ColumnSpan = 2;
                        TabNameCell.CssClass = "NodeReport_TabName";

                        Label TabNameLabel = new Label();
                        TabNameLabel.Text = Tab.TabName;
                        TabNameCell.Controls.Add( TabNameLabel );
                        row++;

                        CswLayoutTable TabTable = new CswLayoutTable( Master.CswNbtResources, Master.AjaxManager );
                        TabTable.ID = "TabTable_" + Tab.TabId;
                        TabTable.CssClass = "NodeReport_TabTable";
                        TabTable.CssClassLabelCell = "NodeReport_LabelCell";
                        TabTable.CssClassValueCell = "NodeReport_ValueCell";
                        TabTable.CssClassEmptyCell = "NodeReport_EmptyCell";
                        _Table.addControl( row, 1, TabTable );
                        foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder )
                        {
                            if( !Prop.hasFilter() || Prop.CheckFilter( _Node ) )
                            {
                                CswPropertyTable.addPropertyToTable( Master.CswNbtResources, TabTable, Prop, _Node, false, false, NodeEditMode.PrintReport, new CswErrorHandler( Master.HandleError ) );
                            }
                        }
                        TabTable.ReinitComponents();
                        row++;
                    }
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

    }//class NodeReport 
}//namespace ChemSW.Nbt.WebPages
