using System;
using System.Data;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data.OleDb;
using System.Xml;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.DB;
using ChemSW.Nbt.ImportExport;

namespace ChemSW.Nbt.WebPages
{
    public partial class Act_ImportData : System.Web.UI.Page
    {
        #region Page Lifecycle

        protected void Page_Init( object sender, EventArgs e )
        {
            try
            {
                EnsureChildControls();
                initNodeTypesCBA();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }
    
        private CswAutoTable Table;
        private Literal HHFileUploadLiteral;
        private FileUpload _HHFileUpload;
        private Literal ExcelFileUploadLiteral;
        private FileUpload _ExcelFileUpload;
        private Literal ModeLiteral;
        private DropDownList _ModeList;
        private Button ProcessButton;
        private Literal AllResultsLiteral;
        private Literal ResultLiteral;
        private TextBox ErrorLogMemo;
        private LinkButton LoadViewLink;
        private HiddenField LoadViewXml;
        private LinkButton LoadNodeTypeLink;
        private HiddenField LoadNodeTypeId;
        private CswCheckBoxArray NodeTypesCBA;

        protected override void CreateChildControls()
        {
            Table = new CswAutoTable();
            Table.ID = "importtable";
            Table.FirstCellRightAlign = true;
            ph.Controls.Add( Table );

            Literal DownloadLiteral = new Literal();
            DownloadLiteral.Text = "Download:";

            Literal NodeTypesCBALiteral = new Literal();
            NodeTypesCBALiteral.Text = "Select nodetypes to include in template";
            
            NodeTypesCBA = new CswCheckBoxArray(Master.CswNbtResources);
            NodeTypesCBA.ID = "NodeTypesCBA";

            Button DownloadButton = new Button();
            DownloadButton.ID = "DownloadButton";
            DownloadButton.Click += new EventHandler( DownloadButton_Click );
            DownloadButton.CssClass = "Button";
            DownloadButton.Text = "Download Excel Template";
            
            Literal ImportLiteral = new Literal();
            ImportLiteral.Text = "Import:";

            HHFileUploadLiteral = new Literal();
            HHFileUploadLiteral.Text = "Mobile XML File:";

            _HHFileUpload = new FileUpload();
            _HHFileUpload.EnableViewState = true;

            if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.NBTManager ) )
            {
                ModeLiteral = new Literal();
                ModeLiteral.Text = "Import Mode:";

                _ModeList = new DropDownList();
                _ModeList.ID = "ModeList";
                _ModeList.CssClass = "selectinput";
                foreach( CswNbtImportExport.ImportMode Mode in Enum.GetValues( typeof( CswNbtImportExport.ImportMode ) ) )
                    _ModeList.Items.Add( new ListItem( Mode.ToString() ) );
            }

            ExcelFileUploadLiteral = new Literal();
            ExcelFileUploadLiteral.Text = "Excel File:";
            
            _ExcelFileUpload = new FileUpload();
            _ExcelFileUpload.EnableViewState = true;

            ProcessButton = new Button();
            ProcessButton.Click += new EventHandler( Process_Click );
            ProcessButton.Text = "Do Import";

            AllResultsLiteral = new Literal();
            AllResultsLiteral.Text = "Results:";
            AllResultsLiteral.Visible = false;

            ResultLiteral = new Literal();
            ResultLiteral.Visible = false;

            ErrorLogMemo = new TextBox();
            ErrorLogMemo.ID = "ErrorLogMemo";
            ErrorLogMemo.CssClass = "textinput";
            ErrorLogMemo.TextMode = TextBoxMode.MultiLine;
            ErrorLogMemo.Rows = 10;
            ErrorLogMemo.Columns = 80;
            ErrorLogMemo.Visible = false;

            //Literal Spacer = new Literal();
            //Spacer.Text = "&nbsp;";

            LoadViewLink = new LinkButton();
            LoadViewLink.ID = "LoadViewLink";
            LoadViewLink.Text = "Load Imported View";
            LoadViewLink.Visible = false;
            LoadViewLink.Click += new EventHandler( LoadViewLink_Click );

            LoadViewXml = new HiddenField();
            LoadViewXml.ID = "LoadViewXml";

            LoadNodeTypeLink = new LinkButton();
            LoadNodeTypeLink.ID = "LoadNodeTypeLink";
            LoadNodeTypeLink.Text = "Load Imported NodeType";
            LoadNodeTypeLink.Visible = false;
            LoadNodeTypeLink.Click += new EventHandler( LoadNodeTypeLink_Click );

            LoadNodeTypeId = new HiddenField();
            LoadNodeTypeId.ID = "LoadNodeTypeId";


            Int32 row = 0;
            Int32 headercol = 0;
            Int32 namecol = 1;
            Int32 valuecol = 2;
            //Table.addControl( row++, headercol, DownloadLiteral );
            //Table.addControl( row++, namecol, NodeTypesCBALiteral );
            //Table.addControl( row++, valuecol, NodeTypesCBA );
            //Table.addControl( row++, valuecol, DownloadButton );
            //Table.addControl( row++, headercol, Spacer );
            Table.addControl( row++, headercol, ImportLiteral );
            Table.addControl( row, namecol, HHFileUploadLiteral );
            Table.addControl( row++, valuecol, _HHFileUpload );
            if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.NBTManager ) )
            {
                Table.addControl( row, namecol, ModeLiteral );
                Table.addControl( row++, valuecol, _ModeList );
            }
            //Table.addControl( row, namecol, ExcelFileUploadLiteral );
            //Table.addControl( row++, valuecol, _ExcelFileUpload );
            Table.addControl( row++, valuecol, ProcessButton );
            Table.addControl( row++, headercol, AllResultsLiteral );
            Table.addControl( row++, valuecol, ResultLiteral );
            Table.addControl( row++, valuecol, ErrorLogMemo );
            Table.addControl( row, valuecol, LoadViewLink );
            Table.addControl( row++, valuecol, LoadViewXml );
            Table.addControl( row, valuecol, LoadNodeTypeLink );
            Table.addControl( row++, valuecol, LoadNodeTypeId );
            base.CreateChildControls();
        }

        
        private void initNodeTypesCBA()
        {
            DataTable Data = new CswDataTable("nodetypescbadatatable","");
            Data.Columns.Add("NodeType Name", typeof(string));
            Data.Columns.Add("nodetypeid", typeof(int));
            Data.Columns.Add("Include", typeof(bool));

            foreach (CswNbtMetaDataNodeType NodeType in Master.CswNbtResources.MetaData.LatestVersionNodeTypes)
            {
                DataRow NTRow = Data.NewRow();
                NTRow["NodeType Name"] = NodeType.NodeTypeName;          // Latest name
                NTRow["nodetypeid"] = NodeType.FirstVersionNodeTypeId;   // First ID
                NTRow["Include"] = false;
                Data.Rows.Add(NTRow);
            }
            NodeTypesCBA.CreateCheckBoxes(Data, "NodeType Name", "nodetypeid");
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                //if( ResultMemo.Text != string.Empty )
                //{
                //    ResultMemo.Visible = true;
                //    ResultLiteral.Visible = true;
                //    AllResultsLiteral.Visible = true;
                //}

                //if( ErrorLogMemo.Text != string.Empty )
                //{
                //    ErrorLogMemo.Visible = true;
                //    ErrorLiteral.Visible = true;
                //    AllResultsLiteral.Visible = true;
                //}
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnPreRender( e );
        }


        #endregion Page Lifecycle

        #region Events


        void DownloadButton_Click( object sender, EventArgs e )
        {
            try
            {

                //string TempFileName = "temp/template_" + Master.CswNbtResources.CurrentUser.Username.Text.ToString() + "_" + DateTime.Now.Ticks.ToString() + ".csv";
                //string TempFileFullName = Server.MapPath( "" ) + "/" + TempFileName;

                //Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                //if( ExcelApp == null )
                //    throw new CswDniException( "Excel did not start" );

                //ExcelApp.Workbooks.Add( Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet );
                //Microsoft.Office.Interop.Excel.Worksheet Worksheet = ExcelApp.Workbooks[0].Sheets[0] as Microsoft.Office.Interop.Excel.Worksheet;
                //Int32 DemoNodeTypeId = 8;
                //CswNbtMetaDataNodeType DemoNodeType = Master.CswNbtResources.MetaData.getNodeType( DemoNodeTypeId );
                //Worksheet.Name = DemoNodeType.NodeTypeName;
                //Int32 row = 0;
                //Int32 col = 0;
                //foreach( CswNbtMetaDataNodeTypeProp Prop in DemoNodeType.NodeTypeProps )
                //{
                //    Worksheet.Cells[row, col] = Prop.PropName;
                //}
                //ExcelApp.Save( TempFileFullName );
                
                string CSVTemplate = string.Empty;
                Int32 DemoNodeTypeId = 8;
                CswNbtMetaDataNodeType DemoNodeType = Master.CswNbtResources.MetaData.getNodeType( DemoNodeTypeId );
                CSVTemplate += DemoNodeType.NodeTypeName + "\r\n"; 
                foreach( CswNbtMetaDataNodeTypeProp Prop in DemoNodeType.NodeTypeProps )
                {
                    if(Prop.FieldType.IsSimpleType())
                        CSVTemplate += Prop.PropName + ",";
                }
                CSVTemplate += "\r\n";

                Response.ClearContent();
                Response.AddHeader( "content-disposition", "attachment;filename=template.csv" );
                Response.ContentType = "application/vnd.ms-excel";
                Response.Write( CSVTemplate);
                Response.End();

            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

        }



        private Int32 _GarbageCharacterOffset = 0;
        protected void Process_Click( object sender, EventArgs e )
        {
            try
            {
                if( _HHFileUpload.HasFile )
                {
                    Byte[] Data = new Byte[_HHFileUpload.FileContent.Length];
                    _HHFileUpload.FileContent.Read( Data, 0, (int) _HHFileUpload.FileContent.Length );

                    // BZ 10179 - Use Latin-9 encoding
                    //ASCIIEncoding ASCIIEncoding = new ASCIIEncoding();
                    //string ImportXml = ASCIIEncoding.GetString( Data, _GarbageCharacterOffset, (int) _HHFileUpload.FileContent.Length - _GarbageCharacterOffset );
                    Encoding Latin9Encoding = Encoding.GetEncoding( "iso-8859-15" );
                    string ImportXml = Latin9Encoding.GetString( Data, _GarbageCharacterOffset, (int) _HHFileUpload.FileContent.Length - _GarbageCharacterOffset );

                    CswNbtImportExport.ImportMode ImportMode = CswNbtImportExport.ImportMode.Update;
                    if( Master.CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.NBTManager ) )
                        ImportMode = (CswNbtImportExport.ImportMode) Enum.Parse( typeof( CswNbtImportExport.ImportMode ), _ModeList.SelectedValue );

                    _DoImport( ImportMode, ImportXml );
                }
                else if( _ExcelFileUpload.HasFile )
                {
                    if( _ExcelFileUpload.PostedFile.FileName.EndsWith(".csv"))
                    {
                        Byte[] CSVBuffer = new Byte[_ExcelFileUpload.FileContent.Length];
                        _ExcelFileUpload.FileContent.Read( CSVBuffer, 0, (int) _ExcelFileUpload.FileContent.Length );

                        string CSVContents = CswTools.ByteArrayToString(CSVBuffer);
                        CSVContents = CSVContents.Replace( "\r", string.Empty );
                        string[] lines = CSVContents.Split( '\n' );
                        string NodeTypeName = lines[0].Substring(0, lines[0].IndexOf(','));

                        CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( NodeTypeName );

                        Hashtable ColumnReference = new Hashtable();
                        string[] propnamevalues = lines[1].Split( ',' );
                        Int32 currentcolumn = 0;
                        foreach( string propname in propnamevalues )
                        {
                            ColumnReference.Add( currentcolumn, NodeType.getNodeTypeProp( propname ) );
                            currentcolumn++;
                        }

                        string XmlString = "<Nodes>";
                        Int32 FakeNodeId = -1;
                        for( int i = 2; i < lines.Length; i++ )
                        {
                            if( lines[i] != string.Empty )
                            {
                                XmlString += "<Node nodeid=\"" + FakeNodeId.ToString() + "\" nodetypeid=\"" + NodeType.NodeTypeId.ToString() + "\">";
                                FakeNodeId--;
                                currentcolumn = 0;
                                string[] columnvalues = lines[i].Split( ',' );
                                foreach( string value in columnvalues )
                                {
                                    if( value != string.Empty )
                                    {
                                        CswNbtMetaDataNodeTypeProp Prop = ColumnReference[currentcolumn] as CswNbtMetaDataNodeTypeProp;
                                        string SubFieldName = Prop.FieldTypeRule.SubFields.Default.ToXmlNodeName();
                                        XmlString += "<PropValue nodetypepropid=\"" + Prop.PropId.ToString() + "\">";
                                        XmlString += "<" + SubFieldName + ">";
                                        XmlString += value;
                                        XmlString += "</" + SubFieldName + ">";
                                        XmlString += "</PropValue>";
                                    }
                                    currentcolumn++;
                                }
                                XmlString += "</Node>";
                            }
                        }
                        XmlString += "</Nodes>";

                        _DoImport( CswNbtImportExport.ImportMode.Duplicate, XmlString );
                    }
                    else  //xls
                    {  
                        string TempFileName = "temp/excelupload_" + Master.CswNbtResources.CurrentUser.Username + "_" + DateTime.Now.Ticks.ToString();
                        string TempFileFullName = Server.MapPath( "" ) + "/" + TempFileName;
                        _ExcelFileUpload.SaveAs( TempFileFullName );

                        string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + TempFileFullName + ";Extended PRoperties=Excel 8.0;";
                        OleDbConnection ExcelConn = new OleDbConnection( ConnStr );
                        ExcelConn.Open();

                        OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                        OleDbCommand Command1 = new OleDbCommand( "SELECT * FROM [Sheet1$]", ExcelConn );
                        DataAdapter.SelectCommand = Command1;

                        DataSet ExcelDS = new DataSet();
                        DataAdapter.Fill( ExcelDS );
                        DataTable FirstSheet = ExcelDS.Tables[0];

                        // Convert to import-compatible XML format
                        Int32 DemoNodeTypeId = 8;
                        CswNbtMetaDataNodeType DemoNodeType = Master.CswNbtResources.MetaData.getNodeType( DemoNodeTypeId );
                        string XmlString = "<Nodes>";
                        Int32 FakeNodeId = -1;
                        foreach( DataRow ExcelRow in FirstSheet.Rows )
                        {
                            XmlString += "<Node nodeid=\"" + FakeNodeId.ToString() + "\" nodetypeid=\"" + DemoNodeTypeId.ToString() + "\">";
                            FakeNodeId--;
                            foreach( DataColumn PropColumn in FirstSheet.Columns )
                            {
                                CswNbtMetaDataNodeTypeProp ThisProp = DemoNodeType.getNodeTypeProp( PropColumn.ColumnName );
                                string SubFieldName = ThisProp.FieldTypeRule.SubFields.Default.ToXmlNodeName();
                                XmlString += "<PropValue nodetypepropid=\"" + ThisProp.PropId.ToString() + "\">";
                                XmlString += "<" + SubFieldName + ">";
                                XmlString += ExcelRow[PropColumn].ToString();
                                XmlString += "</" + SubFieldName + ">";
                                XmlString += "</PropValue>";
                            }
                            XmlString += "</Node>";
                        }
                        XmlString += "</Nodes>";

                        _DoImport( CswNbtImportExport.ImportMode.Duplicate, XmlString );
                    }
                } // else if( _ExcelFileUpload.HasFile )
                else
                {
                    throw new CswDniException( "Select a file to import", "The FileUpload control could not detect a file to upload" );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        private void _DoImport(CswNbtImportExport.ImportMode ImportMode, string ImportXml )
        {
            CswNbtImportExport Importer = new CswNbtImportExport( Master.CswNbtResources ); 
            
            string ResultXml = string.Empty;
            string ErrorLog = string.Empty;
            string ViewXml = string.Empty;

            Importer.ImportXml( ImportMode, ImportXml, ref ViewXml, ref ResultXml, ref ErrorLog );

            if( ErrorLog != string.Empty )
            {
                ErrorLogMemo.Visible = true;
                ErrorLogMemo.Text = ErrorLog;
                ResultLiteral.Visible = true;
                ResultLiteral.Text = "Import had errors:";
            }
            else
            {
                ResultLiteral.Visible = true;
                ResultLiteral.Text = "Import was successful";
            }

            if( ViewXml != string.Empty )
            {
                LoadViewLink.Visible = true;
                LoadViewXml.Value = ViewXml;
            }
            else
            {
                XmlDocument ResultXmlDoc = new XmlDocument();
                ResultXmlDoc.LoadXml( ResultXml );
                if( ResultXmlDoc.FirstChild != null &&
                    ResultXmlDoc.FirstChild.FirstChild != null &&
                    ResultXmlDoc.FirstChild.FirstChild.FirstChild != null )
                {
                    XmlNode DestNodeTypeNode = ResultXmlDoc.FirstChild.FirstChild.FirstChild.SelectSingleNode( "destnodetypeid" );
                    if( DestNodeTypeNode != null && CswTools.IsInteger( DestNodeTypeNode.InnerText ) )
                    {
                        Int32 NodeTypeId = CswConvert.ToInt32( DestNodeTypeNode.InnerText );
                        LoadNodeTypeId.Value = NodeTypeId.ToString();
                        LoadNodeTypeLink.Visible = true;
                    }
                }
            }
        }

        void LoadViewLink_Click( object sender, EventArgs e )
        {
            try
            {
                if( LoadViewXml.Value != string.Empty )
                {
                    Master.setViewXml( LoadViewXml.Value.ToString() );
                    Master.Redirect( "Main.aspx" );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void LoadNodeTypeLink_Click( object sender, EventArgs e )
        {
            try
            {
                if( LoadNodeTypeId.Value != string.Empty )
                {
                    Session["Design_ForceReselect"] = "true";
                    Session["Design_SelectedType"] = CswNodeTypeTree.NodeTypeTreeSelectedType.NodeType;
                    Session["Design_SelectedValue"] = LoadNodeTypeId.Value;
                    Master.Redirect( "Design.aspx" );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }
        

        #endregion Events

    }//Act_ImportData

}//ChemSW.Nbt.WebPages