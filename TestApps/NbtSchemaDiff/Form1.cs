using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.TreeEvents;
using ChemSW.RscAdo;
using Microsoft.VisualBasic.FileIO;

namespace ChemSW.NbtSchemaDiff
{
    public partial class Form1 : Form
    {

        private DataTable _LeftDbInstances = null;
        private DataTable _RightDbInstances = null;
        private Int32 _currentGridRow = 0;

        private ICswDbCfgInfo _CswDbCfgInfoNbt = null;
        private ICswSetupVbls _CswSetupVblsNbt = null;
        private string _ConfigurationPath;
        private CswNbtResources _CswNbtResourcesLeft = null;
        private CswNbtResources _CswNbtResourcesRight = null;

        private string _LeftAccessId
        {
            get { return LeftSchemaSelectBox.SelectedValue.ToString(); }
        }
        private string _RightAccessId
        {
            get { return RightSchemaSelectBox.SelectedValue.ToString(); }
        }

        private string _ColName_AccessId = "AccessId";
        private string _ColName_ServerType = "Server Type";
        private string _ColName_ServerName = "ServerName";
        private string _ColName_UserName = "UserName";
        private string _ColName_UserCount = "UserCount";
        private string _ColName_Deactivated = "Deactivated";
        private string _ColName_Display = "Display";

        public Form1()
        {
            InitializeComponent();

            _ConfigurationPath = Application.StartupPath + "\\..\\etc";
            if( !FileSystem.DirectoryExists( _ConfigurationPath ) )
            {
                FileSystem.CreateDirectory( _ConfigurationPath );
            }

            _InitSessionResources();

            _LeftDbInstances = new CswDataTable( "LeftDataTable", "" );
            _LeftDbInstances.Columns.Add( _ColName_AccessId, typeof( string ) );
            _LeftDbInstances.Columns.Add( _ColName_ServerType, typeof( string ) );
            _LeftDbInstances.Columns.Add( _ColName_ServerName, typeof( string ) );
            _LeftDbInstances.Columns.Add( _ColName_UserName, typeof( string ) );
            _LeftDbInstances.Columns.Add( _ColName_UserCount, typeof( string ) );
            _LeftDbInstances.Columns.Add( _ColName_Deactivated, typeof( bool ) );
            _LeftDbInstances.Columns.Add( _ColName_Display, typeof( string ) );
            _LeftDbInstances.Rows.Clear();
            foreach( string CurrentAccessId in _CswDbCfgInfoNbt.AccessIds )
            {
                _CswDbCfgInfoNbt.makeConfigurationCurrent( CurrentAccessId );
                DataRow CurrentRow = _LeftDbInstances.NewRow();
                CurrentRow[_ColName_AccessId] = CurrentAccessId.ToString();
                CurrentRow[_ColName_ServerType] = _CswDbCfgInfoNbt.CurrentServerType;
                CurrentRow[_ColName_ServerName] = _CswDbCfgInfoNbt.CurrentServerName;
                CurrentRow[_ColName_UserName] = _CswDbCfgInfoNbt.CurrentUserName;
                CurrentRow[_ColName_UserCount] = _CswDbCfgInfoNbt.CurrentUserCount;
                CurrentRow[_ColName_Deactivated] = _CswDbCfgInfoNbt.CurrentDeactivated;
                CurrentRow[_ColName_Display] = CurrentAccessId + " (" + _CswDbCfgInfoNbt.CurrentUserName + "@" + _CswDbCfgInfoNbt.CurrentServerName + ")";
                _LeftDbInstances.Rows.Add( CurrentRow );
            }

            _RightDbInstances = _LeftDbInstances.Copy();

            LeftSchemaSelectBox.DataSource = _LeftDbInstances;
            LeftSchemaSelectBox.ValueMember = _ColName_AccessId;
            LeftSchemaSelectBox.DisplayMember = _ColName_Display;

            RightSchemaSelectBox.DataSource = _RightDbInstances;
            RightSchemaSelectBox.ValueMember = _ColName_AccessId;
            RightSchemaSelectBox.DisplayMember = _ColName_Display;

            dataGridView1.Columns.Add( "LeftNodeId", "LeftNodeId" );
            dataGridView1.Columns.Add( "LeftNodeName", "LeftNodeName" );
            dataGridView1.Columns.Add( "LeftPropertyId", "LeftPropertyId" );
            dataGridView1.Columns.Add( "LeftPropertyName", "LeftPropertyName" );
            dataGridView1.Columns.Add( "LeftSubFieldName", "LeftSubFieldName" );
            dataGridView1.Columns.Add( "LeftValue", "LeftValue" );
            dataGridView1.Columns.Add( "RightNodeId", "RightNodeId" );
            dataGridView1.Columns.Add( "RightNodeName", "RightNodeName" );
            dataGridView1.Columns.Add( "RightPropertyId", "RightPropertyId" );
            dataGridView1.Columns.Add( "RightPropertyName", "RightPropertyName" );
            dataGridView1.Columns.Add( "RightSubFieldName", "RightSubFieldName" );
            dataGridView1.Columns.Add( "RightValue", "RightValue" );

            dataGridView1.Columns["LeftNodeId"].Width = 40;
            dataGridView1.Columns["LeftPropertyId"].Width = 40;
            dataGridView1.Columns["RightNodeId"].Width = 40;
            dataGridView1.Columns["RightPropertyId"].Width = 40;

            KeyDataGridView.Columns.Add( "No Matching Node", "No Matching Node" );
            KeyDataGridView.Columns.Add( "No Matching Property", "No Matching Property" );
            KeyDataGridView.Columns.Add( "Different Value", "Different Value" );
            KeyDataGridView.Columns.Add( "Equivalent Value", "Equivalent Value" );
            KeyDataGridView.Columns.Add( "Orphan Reference", "Orphan Reference" );
            KeyDataGridView.Rows.Add( 1 );
            KeyDataGridView.Rows[0].Cells[0].Value = "no match";
            KeyDataGridView.Rows[0].Cells[1].Value = "no match";
            KeyDataGridView.Rows[0].Cells[2].Value = "xyz";
            KeyDataGridView.Rows[0].Cells[3].Value = "42";
            KeyDataGridView.Rows[0].Cells[4].Value = "197";
            _makeNoMatchNodeCell( KeyDataGridView.Rows[0].Cells[0] );
            _makeNoMatchPropertyCell( KeyDataGridView.Rows[0].Cells[1] );
            _makeDifferentValueCell( KeyDataGridView.Rows[0].Cells[2] );
            _makeDiffButOkCell( KeyDataGridView.Rows[0].Cells[3] );
            _makeOrphanCell( KeyDataGridView.Rows[0].Cells[4] );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            KeyDataGridView.ClearSelection();
            base.OnPaint( e );
        }

        private void _InitSessionResources()
        {

            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe, IsMobile: false );
            _CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode.NbtExe );

            // Left resources
            //CswNbtObjClassFactory _CswNbtObjClassFactoryLeft = new CswNbtObjClassFactory();
            _CswNbtResourcesLeft = new CswNbtResources( AppType.SchemDiff, _CswSetupVblsNbt, _CswDbCfgInfoNbt, //_CswNbtObjClassFactoryLeft, 
                                                       false, false, null );
            _CswNbtResourcesLeft.SetDbResources( PooledConnectionState.Closed );
            //_CswNbtResources.CswTblFactory = new CswNbtTblFactory( _CswNbtResources );
            //_CswNbtResources.CswTableCaddyFactory = new CswTableCaddyFactoryNbt( _CswNbtResources );

            CswNbtMetaDataEvents _CswNbtMetaDataEventsLeft = new CswNbtMetaDataEvents( _CswNbtResourcesLeft );
            _CswNbtResourcesLeft.OnMakeNewNodeType += new CswNbtResources.NewNodeTypeEventHandler( _CswNbtMetaDataEventsLeft.OnMakeNewNodeType );
            _CswNbtResourcesLeft.OnCopyNodeType += new CswNbtResources.CopyNodeTypeEventHandler( _CswNbtMetaDataEventsLeft.OnCopyNodeType );
            _CswNbtResourcesLeft.OnMakeNewNodeTypeProp += new CswNbtResources.NewNodeTypePropEventHandler( _CswNbtMetaDataEventsLeft.OnMakeNewNodeTypeProp );
            _CswNbtResourcesLeft.OnEditNodeTypePropName += new CswNbtResources.EditPropNameEventHandler( _CswNbtMetaDataEventsLeft.OnEditNodeTypePropName );
            _CswNbtResourcesLeft.OnDeleteNodeTypeProp += new CswNbtResources.DeletePropEventHandler( _CswNbtMetaDataEventsLeft.OnDeleteNodeTypeProp );
            _CswNbtResourcesLeft.OnEditNodeTypeName += new CswNbtResources.EditNodeTypeNameEventHandler( _CswNbtMetaDataEventsLeft.OnEditNodeTypeName );

            // Right resources
            //CswNbtObjClassFactory _CswNbtObjClassFactoryRight = new CswNbtObjClassFactory();
            _CswNbtResourcesRight = new CswNbtResources( AppType.SchemDiff, _CswSetupVblsNbt, _CswDbCfgInfoNbt, //_CswNbtObjClassFactoryRight, 
                                                         false, false, null );
            _CswNbtResourcesRight.SetDbResources( PooledConnectionState.Closed );
            //_CswNbtResources.CswTblFactory = new CswNbtTblFactory( _CswNbtResources );
            //_CswNbtResources.CswTableCaddyFactory = new CswTableCaddyFactoryNbt( _CswNbtResources );

            CswNbtMetaDataEvents _CswNbtMetaDataEventsRight = new CswNbtMetaDataEvents( _CswNbtResourcesRight );
            _CswNbtResourcesRight.OnMakeNewNodeType += new CswNbtResources.NewNodeTypeEventHandler( _CswNbtMetaDataEventsRight.OnMakeNewNodeType );
            _CswNbtResourcesRight.OnCopyNodeType += new CswNbtResources.CopyNodeTypeEventHandler( _CswNbtMetaDataEventsRight.OnCopyNodeType );
            _CswNbtResourcesRight.OnMakeNewNodeTypeProp += new CswNbtResources.NewNodeTypePropEventHandler( _CswNbtMetaDataEventsRight.OnMakeNewNodeTypeProp );
            _CswNbtResourcesRight.OnEditNodeTypePropName += new CswNbtResources.EditPropNameEventHandler( _CswNbtMetaDataEventsRight.OnEditNodeTypePropName );
            _CswNbtResourcesRight.OnDeleteNodeTypeProp += new CswNbtResources.DeletePropEventHandler( _CswNbtMetaDataEventsRight.OnDeleteNodeTypeProp );
            _CswNbtResourcesRight.OnEditNodeTypeName += new CswNbtResources.EditNodeTypeNameEventHandler( _CswNbtMetaDataEventsRight.OnEditNodeTypeName );

            //_CswNbtResources.InitDbResources();

            //_CswLogger = _CswNbtResources.CswLogger;

            //_CswNbtResources.CurrentUser = new CswNbtSchemaUpdaterUser();


        }//_InitSessionResources()

        private void CompareButton_Click( object sender, EventArgs e )
        {
            if( _LeftAccessId != _RightAccessId )
            {
                CompareButton.Enabled = false;
                CompareButton.Text = "Comparing...";
                CompareButton.Refresh();
                dataGridView1.Rows.Clear();

                // get all nodes
                _CswNbtResourcesLeft.AccessId = _LeftAccessId;
                //                _CswNbtResourcesLeft.refreshDataDictionary();
                _CswNbtResourcesLeft.MetaData.refreshAll();
                CswTableSelect LeftNodesSelect = _CswNbtResourcesLeft.makeCswTableSelect( "NbtSchemaDiff_Left_nodes_select", "nodes" );
                DataTable LeftNodesTable = LeftNodesSelect.getTable();
                Collection<CswNbtNode> LeftNodes = new Collection<CswNbtNode>();
                foreach( DataRow NodeRow in LeftNodesTable.Rows )
                {
                    CswPrimaryKey ThisNodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) );
                    CswNbtNode ThisNode = _CswNbtResourcesLeft.Nodes[ThisNodePk];
                    LeftNodes.Add( ThisNode );
                }

                _CswNbtResourcesRight.AccessId = _RightAccessId;
                //                _CswNbtResourcesRight.refreshDataDictionary();
                _CswNbtResourcesRight.MetaData.refreshAll();
                CswTableSelect RightNodesSelect = _CswNbtResourcesRight.makeCswTableSelect( "NbtSchemaDiff_Right_nodes_select", "nodes" );
                DataTable RightNodesTable = RightNodesSelect.getTable();
                Collection<CswNbtNode> RightNodes = new Collection<CswNbtNode>();
                Collection<CswNbtNode> AllRightNodes = new Collection<CswNbtNode>();
                foreach( DataRow NodeRow in RightNodesTable.Rows )
                {
                    CswPrimaryKey ThisNodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) );
                    CswNbtNode ThisNode = _CswNbtResourcesRight.Nodes[ThisNodePk];
                    RightNodes.Add( ThisNode );
                    AllRightNodes.Add( ThisNode );
                }

                Dictionary<CswNbtNode, CswNbtNode> NodeMatches = new Dictionary<CswNbtNode, CswNbtNode>();

                // ---------------------------------------------
                // First Pass
                // Match nodes by name and nodetype name
                Collection<CswNbtNode> UnmatchedLeftNodes = new Collection<CswNbtNode>();
                foreach( CswNbtNode LeftNode in LeftNodes )
                {
                    CswNbtNode MatchingRightNode = null;
                    foreach( CswNbtNode RightNode in RightNodes )
                    {
                        if( LeftNode.NodeName == RightNode.NodeName &&
                            LeftNode.getNodeType().NodeTypeName == RightNode.getNodeType().NodeTypeName )
                        {
                            MatchingRightNode = RightNode;
                            NodeMatches.Add( LeftNode, RightNode );
                            //_CompareNodes( LeftNode, RightNode );
                            break;
                        } // if( LeftNode.NodeName == RightNode.NodeName )
                    } // foreach( CswNbtNode RightNode in RightNodes )

                    if( MatchingRightNode != null )
                        RightNodes.Remove( MatchingRightNode );
                    else
                        UnmatchedLeftNodes.Add( LeftNode );
                } // foreach( CswNbtNode LeftNode in LeftNodes )

                // ---------------------------------------------
                // Next Passes
                // Try to match up the unmatched ones by nodetype and property values
                // We'll match by 10% increments from 90% down to the tolerance specified in the UI
                // This ensures that we get the closest match from what's left.
                Double CurrentTolerance = 0.90;
                Double MinimumTolerance = Convert.ToDouble( ToleranceBox.Text ) / 100;
                while( UnmatchedLeftNodes.Count > 0 && CurrentTolerance > 0 && CurrentTolerance > MinimumTolerance )
                {
                    Collection<CswNbtNode> NextUnmatchedLeftNodes = new Collection<CswNbtNode>();
                    foreach( CswNbtNode LeftNode in UnmatchedLeftNodes )
                    {
                        CswNbtNode MatchingRightNode = null;
                        foreach( CswNbtNode RightNode in RightNodes )
                        {
                            if( LeftNode.getNodeType().NodeTypeName == RightNode.getNodeType().NodeTypeName )
                            {
                                Int32 PropCount = 0;
                                Int32 MatchCount = 0;
                                foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in LeftNode.getNodeType().getNodeTypeProps() )
                                {
                                    PropCount++;
                                    if( LeftNode.Properties[MetaDataProp].Field1 == RightNode.Properties[_CswNbtResourcesRight.MetaData.getNodeTypeProp( RightNode.NodeTypeId, MetaDataProp.PropName )].Field1 )
                                    {
                                        MatchCount++;
                                    }
                                } // foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in LeftNode.NodeType.NodeTypeProps )
                                if( PropCount > 0 &&
                                    ( Convert.ToDouble( MatchCount ) / Convert.ToDouble( PropCount ) ) >= CurrentTolerance )
                                {
                                    MatchingRightNode = RightNode;
                                    NodeMatches.Add( LeftNode, RightNode );
                                    break;
                                }
                            }
                        } // foreach( CswNbtNode RightNode in RightNodes )
                        if( MatchingRightNode != null )
                            RightNodes.Remove( MatchingRightNode );
                        else
                            NextUnmatchedLeftNodes.Add( LeftNode );
                    } // foreach( CswNbtNode LeftNode in UnmatchedLeftNodes )

                    CurrentTolerance -= 0.10;
                    UnmatchedLeftNodes = NextUnmatchedLeftNodes;

                } // while( UnmatchedLeftNodes.Count > 0 && CurrentTolerance > 0 && CurrentTolerance > MinimumTolerance )


                // ---------------------------------------------
                // Fifth Pass
                // If minimumtolerance is set to 0,
                // for the last ones, just try matching on nodetype
                Collection<CswNbtNode> LastUnmatchedLeftNodes = new Collection<CswNbtNode>();
                if( MinimumTolerance <= 0 )
                {
                    foreach( CswNbtNode LeftNode in UnmatchedLeftNodes )
                    {
                        CswNbtNode MatchingRightNode = null;
                        foreach( CswNbtNode RightNode in RightNodes )
                        {
                            if( LeftNode.getNodeType().NodeTypeName == RightNode.getNodeType().NodeTypeName )
                            {

                                MatchingRightNode = RightNode;
                                NodeMatches.Add( LeftNode, RightNode );
                                break;
                            }
                        } // foreach( CswNbtNode RightNode in RightNodes )

                        if( MatchingRightNode != null )
                            RightNodes.Remove( MatchingRightNode );
                        else
                            LastUnmatchedLeftNodes.Add( LeftNode );
                    } // foreach( CswNbtNode LeftNode in UnmatchedLeftNodes )
                }
                else
                {
                    LastUnmatchedLeftNodes = UnmatchedLeftNodes;
                }

                // Now process the matches
                foreach( CswNbtNode LeftNode in NodeMatches.Keys )
                {
                    _CompareNodes( LeftNode, NodeMatches[LeftNode], NodeMatches );
                }

                // Now process the non-matches
                if( ShowUnmatchedCheckBox.Checked )
                {
                    // Leftover non-matched Left nodes
                    foreach( CswNbtNode LeftNode in LastUnmatchedLeftNodes )
                    {
                        Int32 r = dataGridView1.Rows.Add();
                        dataGridView1.Rows[r].Cells["LeftNodeId"].Value = LeftNode.NodeId.PrimaryKey.ToString();
                        dataGridView1.Rows[r].Cells["LeftNodeName"].Value = LeftNode.NodeName;
                        dataGridView1.Rows[r].Cells["RightNodeName"].Value = "no match";
                        _makeNoMatchNodeCell( dataGridView1.Rows[r].Cells["RightNodeId"] );
                        _makeNoMatchNodeCell( dataGridView1.Rows[r].Cells["RightNodeName"] );
                    }

                    // Leftover non-matched Right nodes
                    foreach( CswNbtNode RightNode in RightNodes )
                    {
                        Int32 r = dataGridView1.Rows.Add();
                        dataGridView1.Rows[r].Cells["LeftNodeName"].Value = "no match";
                        _makeNoMatchNodeCell( dataGridView1.Rows[r].Cells["LeftNodeId"] );
                        _makeNoMatchNodeCell( dataGridView1.Rows[r].Cells["LeftNodeName"] );
                        dataGridView1.Rows[r].Cells["RightNodeId"].Value = RightNode.NodeId.PrimaryKey.ToString();
                        dataGridView1.Rows[r].Cells["RightNodeName"].Value = RightNode.NodeName;
                    }
                } // if( ShowUnmatchedCheckBox.Checked )

                CompareButton.Enabled = true;
                CompareButton.Text = "Compare";
                CompareButton.Refresh();
            } // if(_LeftAccessId != _RightAccessId)
        } // CompareButton_Click()


        private void _makeNoMatchNodeCell( DataGridViewCell dataGridViewCell )
        {
            dataGridViewCell.Style.ForeColor = Color.White;
            dataGridViewCell.Style.BackColor = Color.Red;
        }
        private void _makeNoMatchPropertyCell( DataGridViewCell dataGridViewCell )
        {
            dataGridViewCell.Style.ForeColor = Color.White;
            dataGridViewCell.Style.BackColor = Color.IndianRed;
        }
        private void _makeDifferentValueCell( DataGridViewCell dataGridViewCell )
        {
            dataGridViewCell.Style.ForeColor = Color.Red;
            dataGridViewCell.Style.BackColor = Color.Yellow;
        }
        private void _makeDiffButOkCell( DataGridViewCell dataGridViewCell )
        {
            dataGridViewCell.Style.ForeColor = Color.Black;
            dataGridViewCell.Style.BackColor = Color.FromArgb( 200, 255, 200 );
        }

        private void _makeOrphanCell( DataGridViewCell dataGridViewCell )
        {
            dataGridViewCell.Style.ForeColor = Color.Red;
            dataGridViewCell.Style.BackColor = Color.LightBlue;
        }

        private void _addGridEntry( string Source, string TableName, string PkValue, string Message )
        {
            if( _currentGridRow >= dataGridView1.Rows.Count )
                dataGridView1.Rows.Add( 10 );

            dataGridView1.Rows[_currentGridRow].Cells["source"].Value = Source;
            dataGridView1.Rows[_currentGridRow].Cells["table"].Value = TableName;
            dataGridView1.Rows[_currentGridRow].Cells["pk"].Value = PkValue;
            dataGridView1.Rows[_currentGridRow].Cells["message"].Value = Message;

            _currentGridRow++;
        }

        private void _CompareNodes( CswNbtNode LeftNode, CswNbtNode RightNode, Dictionary<CswNbtNode, CswNbtNode> NodeMatches )
        {
            Collection<CswNbtNodePropWrapper> RightProps = new Collection<CswNbtNodePropWrapper>();
            foreach( CswNbtNodePropWrapper RightPropWrapper in RightNode.Properties )
                RightProps.Add( RightPropWrapper );

            foreach( CswNbtNodePropWrapper LeftPropWrapper in LeftNode.Properties )
            {
                CswNbtNodePropWrapper MatchingRightProp = null;
                foreach( CswNbtNodePropWrapper RightPropWrapper in RightProps )
                {
                    if( LeftPropWrapper.PropName == RightPropWrapper.PropName &&
                        LeftPropWrapper.getFieldType().FieldType == RightPropWrapper.getFieldType().FieldType )
                    {
                        MatchingRightProp = RightPropWrapper;

                        //bool SpecialCase = _CompareValue( Subfield.Name, LeftPropWrapper, RightPropWrapper, NodeMatches );
                        CompareValueMatchCase SpecialCase = _CompareValue( LeftPropWrapper.getFieldType().FieldType, LeftPropWrapper, RightPropWrapper, NodeMatches );

                        foreach( CswNbtSubField Subfield in RightPropWrapper.NodeTypeProp.getFieldTypeRule().SubFields )
                        {
                            //string LeftValue = LeftPropWrapper.GetPropRowValue( ( (CswNbtSubField.PropColumn) Enum.Parse( typeof( CswNbtSubField.PropColumn ), Subfield.Column ) ) );
                            //string RightValue = RightPropWrapper.GetPropRowValue( ( (CswNbtSubField.PropColumn) Enum.Parse( typeof( CswNbtSubField.PropColumn ), Subfield.Column ) ) );
                            string LeftValue = LeftPropWrapper.GetPropRowValue( Subfield.Column );
                            string RightValue = RightPropWrapper.GetPropRowValue( Subfield.Column );

                            if( !ShowDiffsOnlyCheckBox.Checked || LeftValue != RightValue )
                            {
                                Int32 r = dataGridView1.Rows.Add();
                                dataGridView1.Rows[r].Cells["LeftNodeId"].Value = LeftNode.NodeId.PrimaryKey.ToString();
                                dataGridView1.Rows[r].Cells["LeftNodeName"].Value = LeftNode.NodeName;
                                dataGridView1.Rows[r].Cells["LeftPropertyId"].Value = LeftPropWrapper.NodeTypePropId.ToString();
                                dataGridView1.Rows[r].Cells["LeftPropertyName"].Value = LeftPropWrapper.PropName;
                                dataGridView1.Rows[r].Cells["LeftSubFieldName"].Value = Subfield.Name.ToString();
                                dataGridView1.Rows[r].Cells["LeftValue"].Value = LeftValue;
                                dataGridView1.Rows[r].Cells["RightNodeId"].Value = RightNode.NodeId.PrimaryKey.ToString();
                                dataGridView1.Rows[r].Cells["RightNodeName"].Value = RightNode.NodeName;
                                dataGridView1.Rows[r].Cells["RightPropertyId"].Value = RightPropWrapper.NodeTypePropId.ToString();
                                dataGridView1.Rows[r].Cells["RightPropertyName"].Value = RightPropWrapper.PropName;
                                dataGridView1.Rows[r].Cells["RightSubFieldName"].Value = Subfield.Name.ToString();
                                dataGridView1.Rows[r].Cells["RightValue"].Value = RightValue;

                                if( LeftNode.NodeName != RightNode.NodeName )
                                {
                                    _makeDifferentValueCell( dataGridView1.Rows[r].Cells["LeftNodeName"] );
                                    _makeDifferentValueCell( dataGridView1.Rows[r].Cells["RightNodeName"] );
                                }

                                //bool SpecialCase = _CompareValue( Subfield.Name, LeftValue, RightValue, r, NodeMatches );
                                switch( SpecialCase )
                                {
                                    case CompareValueMatchCase.Equal:
                                        // nuttin' honey
                                        break;
                                    case CompareValueMatchCase.DifferentValue:
                                        _makeDifferentValueCell( dataGridView1.Rows[r].Cells["LeftValue"] );
                                        _makeDifferentValueCell( dataGridView1.Rows[r].Cells["RightValue"] );
                                        break;
                                    case CompareValueMatchCase.EquivalentValue:
                                        _makeDiffButOkCell( dataGridView1.Rows[r].Cells["LeftValue"] );
                                        _makeDiffButOkCell( dataGridView1.Rows[r].Cells["RightValue"] );
                                        break;
                                    case CompareValueMatchCase.LeftOrphan:
                                        _makeOrphanCell( dataGridView1.Rows[r].Cells["LeftValue"] );
                                        break;
                                    case CompareValueMatchCase.RightOrphan:
                                        _makeOrphanCell( dataGridView1.Rows[r].Cells["RightValue"] );
                                        break;
                                    case CompareValueMatchCase.BothOrphan:
                                        if( ShowAllOrphans.Checked )
                                        {
                                            _makeOrphanCell( dataGridView1.Rows[r].Cells["LeftValue"] );
                                            _makeOrphanCell( dataGridView1.Rows[r].Cells["RightValue"] );
                                        }
                                        break;
                                    case CompareValueMatchCase.Unknown:
                                        if( LeftValue != RightValue )
                                        {
                                            _makeDifferentValueCell( dataGridView1.Rows[r].Cells["LeftValue"] );
                                            _makeDifferentValueCell( dataGridView1.Rows[r].Cells["RightValue"] );
                                        }
                                        break;
                                } // switch( SpecialCase )

                            } // if( !ShowDiffsOnlyCheckBox.Checked || LeftValue != RightValue )
                        } // foreach( CswNbtSubField Subfield in RightPropWrapper.NodeTypeProp.FieldTypeRule.SubFields )
                        break;
                    } // if( LeftPropWrapper.PropName == RightPropWrapper.PropName )
                } // foreach( CswNbtNodePropWrapper RightPropWrapper in RightNode.Properties )

                if( MatchingRightProp != null )
                {
                    RightProps.Remove( MatchingRightProp );
                }
                else
                {
                    // Non-matched Left Property
                    Int32 r = dataGridView1.Rows.Add();
                    dataGridView1.Rows[r].Cells["LeftNodeId"].Value = LeftNode.NodeId.PrimaryKey.ToString();
                    dataGridView1.Rows[r].Cells["LeftNodeName"].Value = LeftNode.NodeName;
                    dataGridView1.Rows[r].Cells["LeftPropertyId"].Value = LeftPropWrapper.NodeTypePropId.ToString();
                    dataGridView1.Rows[r].Cells["LeftPropertyName"].Value = LeftPropWrapper.PropName;
                    dataGridView1.Rows[r].Cells["RightNodeId"].Value = RightNode.NodeId.PrimaryKey.ToString();
                    dataGridView1.Rows[r].Cells["RightNodeName"].Value = RightNode.NodeName;
                    dataGridView1.Rows[r].Cells["RightPropertyName"].Value = "no match";
                    _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["RightPropertyId"] );
                    _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["RightPropertyName"] );
                    _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["RightSubFieldName"] );
                    _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["RightValue"] );
                    if( LeftNode.NodeName != RightNode.NodeName )
                    {
                        _makeDifferentValueCell( dataGridView1.Rows[r].Cells["LeftNodeName"] );
                        _makeDifferentValueCell( dataGridView1.Rows[r].Cells["RightNodeName"] );
                    }
                }
            } // foreach( CswNbtNodePropWrapper LeftPropWrapper in LeftNode.Properties )

            // Leftover non-matched Right props
            foreach( CswNbtNodePropWrapper RightPropWrapper in RightProps )
            {
                Int32 r = dataGridView1.Rows.Add();
                dataGridView1.Rows[r].Cells["LeftNodeId"].Value = LeftNode.NodeId.PrimaryKey.ToString();
                dataGridView1.Rows[r].Cells["LeftNodeName"].Value = LeftNode.NodeName;
                dataGridView1.Rows[r].Cells["LeftPropertyName"].Value = "no match";
                _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["LeftPropertyId"] );
                _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["LeftPropertyName"] );
                _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["LeftSubFieldName"] );
                _makeNoMatchPropertyCell( dataGridView1.Rows[r].Cells["LeftValue"] );
                dataGridView1.Rows[r].Cells["RightNodeId"].Value = RightNode.NodeId.PrimaryKey.ToString();
                dataGridView1.Rows[r].Cells["RightNodeName"].Value = RightNode.NodeName;
                dataGridView1.Rows[r].Cells["RightPropertyId"].Value = RightPropWrapper.NodeTypePropId.ToString();
                dataGridView1.Rows[r].Cells["RightPropertyName"].Value = RightPropWrapper.PropName;
                if( LeftNode.NodeName != RightNode.NodeName )
                {
                    _makeDifferentValueCell( dataGridView1.Rows[r].Cells["LeftNodeName"] );
                    _makeDifferentValueCell( dataGridView1.Rows[r].Cells["RightNodeName"] );
                }
            }
        } // _CompareNodes()


        private CompareValueMatchCase _CompareValue( CswNbtMetaDataFieldType.NbtFieldType FieldType,
                                         CswNbtNodePropWrapper LeftWrapper,
                                         CswNbtNodePropWrapper RightWrapper,
                                         Dictionary<CswNbtNode, CswNbtNode> NodeMatches )
        {
            object LeftObj = null;
            object RightObj = null;
            string LeftValue = string.Empty;
            string RightValue = string.Empty;
            bool Condition = false;
            bool Applies = false;
            switch( FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                    Applies = true;
                    if( LeftWrapper.AsRelationship.RelatedNodeId != null && LeftWrapper.AsRelationship.RelatedNodeId.PrimaryKey != Int32.MinValue )
                    {
                        LeftValue = LeftWrapper.AsRelationship.RelatedNodeId.PrimaryKey.ToString();
                        LeftObj = _CswNbtResourcesLeft.Nodes[LeftWrapper.AsRelationship.RelatedNodeId];
                    }
                    if( RightWrapper.AsRelationship.RelatedNodeId != null && RightWrapper.AsRelationship.RelatedNodeId.PrimaryKey != Int32.MinValue )
                    {
                        RightValue = RightWrapper.AsRelationship.RelatedNodeId.PrimaryKey.ToString();
                        RightObj = _CswNbtResourcesRight.Nodes[RightWrapper.AsRelationship.RelatedNodeId];
                    }
                    Condition = ( LeftObj != null && RightObj != null &&
                                  ( NodeMatches.ContainsKey( (CswNbtNode) LeftObj ) &&
                                    NodeMatches[(CswNbtNode) LeftObj] == (CswNbtNode) RightObj ) );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Location:
                    Applies = true;
                    if( LeftWrapper.AsLocation.SelectedNodeId != null && LeftWrapper.AsLocation.SelectedNodeId.PrimaryKey != Int32.MinValue )
                    {
                        LeftValue = LeftWrapper.AsLocation.SelectedNodeId.PrimaryKey.ToString();
                        LeftObj = _CswNbtResourcesLeft.Nodes[LeftWrapper.AsLocation.SelectedNodeId];
                    }
                    if( RightWrapper.AsLocation.SelectedNodeId != null && RightWrapper.AsLocation.SelectedNodeId.PrimaryKey != Int32.MinValue )
                    {
                        RightValue = RightWrapper.AsLocation.SelectedNodeId.PrimaryKey.ToString();
                        RightObj = _CswNbtResourcesRight.Nodes[RightWrapper.AsLocation.SelectedNodeId];
                    }
                    Condition = ( LeftObj != null && RightObj != null &&
                                  ( NodeMatches.ContainsKey( (CswNbtNode) LeftObj ) &&
                                    NodeMatches[(CswNbtNode) LeftObj] == (CswNbtNode) RightObj ) );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                    Applies = true;
                    if( LeftWrapper.AsNodeTypeSelect.SelectMode == PropertySelectMode.Single &&
                        LeftWrapper.AsNodeTypeSelect.SelectedNodeTypeIds.Count != 0 )
                    {
                        LeftValue = LeftWrapper.AsNodeTypeSelect.SelectedNodeTypeIds.ToString();
                        LeftObj = _CswNbtResourcesLeft.MetaData.getNodeType( CswConvert.ToInt32( LeftWrapper.AsNodeTypeSelect.SelectedNodeTypeIds.ToString() ) );
                    }
                    if( RightWrapper.AsNodeTypeSelect.SelectMode == PropertySelectMode.Single && RightWrapper.AsNodeTypeSelect.SelectedNodeTypeIds.Count != 0 )
                    {
                        RightValue = RightWrapper.AsNodeTypeSelect.SelectedNodeTypeIds.ToString();
                        RightObj = _CswNbtResourcesRight.MetaData.getNodeType( CswConvert.ToInt32( RightWrapper.AsNodeTypeSelect.SelectedNodeTypeIds.ToString() ) );
                    }
                    Condition = ( LeftObj != null && RightObj != null &&
                                  ( (CswNbtMetaDataNodeType) LeftObj ).NodeTypeName == ( (CswNbtMetaDataNodeType) RightObj ).NodeTypeName &&
                                  ( (CswNbtMetaDataNodeType) LeftObj ).getObjectClass().ObjectClass == ( (CswNbtMetaDataNodeType) RightObj ).getObjectClass().ObjectClass );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                    Applies = true;
                    if( LeftWrapper.AsViewPickList.SelectMode == PropertySelectMode.Single && LeftWrapper.AsViewPickList.SelectedViewIds.Count != 0 )
                    {
                        LeftValue = LeftWrapper.AsViewPickList.SelectedViewIds.ToString();
                        LeftObj = _CswNbtResourcesLeft.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( LeftWrapper.AsViewPickList.SelectedViewIds ) ) );
                    }
                    if( RightWrapper.AsViewPickList.SelectMode == PropertySelectMode.Single && RightWrapper.AsViewPickList.SelectedViewIds.Count != 0 )
                    {
                        RightValue = RightWrapper.AsViewPickList.SelectedViewIds.ToString();
                        RightObj = _CswNbtResourcesRight.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( RightWrapper.AsViewPickList.SelectedViewIds ) ) );
                    }
                    Condition = ( LeftObj != null && RightObj != null &&
                                  ( (CswNbtView) LeftObj ).ViewName == ( (CswNbtView) RightObj ).ViewName &&
                                  ( (CswNbtView) LeftObj ).Visibility == ( (CswNbtView) RightObj ).Visibility );
                    break;
            } // switch( SubFieldName )

            CompareValueMatchCase ret = CompareValueMatchCase.Unknown;
            if( Applies )
            {
                if( LeftObj != null && RightObj != null )
                {
                    if( Condition )
                    {
                        // No need to mark if the value and the reference both match
                        if( LeftValue != RightValue )
                            ret = CompareValueMatchCase.EquivalentValue;
                        else
                            ret = CompareValueMatchCase.Equal;
                    }
                    else
                    {
                        // This can happen even if LeftValue == RightValue, which is desired behavior!
                        ret = CompareValueMatchCase.DifferentValue;
                    }
                }
                else
                {
                    if( LeftObj == null && RightObj == null )
                        ret = CompareValueMatchCase.BothOrphan;
                    else if( LeftObj == null )
                        ret = CompareValueMatchCase.LeftOrphan;
                    else if( RightObj == null )
                        ret = CompareValueMatchCase.RightOrphan;
                }
            } // if( Applies )

            return ret;

        } // _CompareValue()

        public enum CompareValueMatchCase
        {
            Equal,
            DifferentValue,
            EquivalentValue,
            LeftOrphan,
            RightOrphan,
            BothOrphan,
            Unknown
        }

    } // class InitializerForm
} // namespace ChemSW.Nbt.Schema
