using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.CswWebControls;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;

namespace ChemSW.Nbt.WebPages
{
    public partial class LowRes_SelectView : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            EnsureChildControls();

            CswNbtViewSelect ViewSelect = new CswNbtViewSelect( Master.CswNbtResources );
            DataTable ViewsTable = ViewSelect.getVisibleViews( false );
            SortedList ViewsList = new SortedList();
            foreach( DataRow ViewRow in ViewsTable.Rows )
            {
                if( ViewRow["category"].ToString() != string.Empty )
                {
                    if( !ViewsList.ContainsKey( ViewRow["category"].ToString().ToLower() ) )
                        ViewsList.Add( ViewRow["category"].ToString().ToLower(), new SortedList() );
                    SortedList CategoryViewsList = (SortedList) ViewsList[ViewRow["category"].ToString().ToLower()];
                    CategoryViewsList.Add( ViewRow["viewname"].ToString().ToLower(), ViewRow );
                }
                else
                {
                    ViewsList.Add( ViewRow["viewname"].ToString().ToLower(), ViewRow );
                }
            }

            foreach( string ViewKey in ViewsList.Keys )
            {
                if( ViewsList[ViewKey] is SortedList )
                {
                    // Category
                    SortedList CategoryViewsList = (SortedList) ViewsList[ViewKey];
                    if( CategoryViewsList.Count > 0 )
                    {
                        bool first = true;
                        foreach( string CategoryViewKey in CategoryViewsList.Keys )
                        {
                            if( first )
                            {
                                // Because the sorted list key is lowercase, we have to get the real category name from the first entry
                                Label CategoryLabel = new Label();
                                CategoryLabel.Text = ( (DataRow) CategoryViewsList[CategoryViewKey] )["category"].ToString();
                                ph.Controls.Add( CategoryLabel );
                                ph.Controls.Add( new CswLiteralBr() );
                                first = false;
                            }
                            // Categorized View
                            _makeViewLink( (DataRow) CategoryViewsList[CategoryViewKey], true );
                        }
                    }
                }
                else
                {
                    // Uncategorized View
                    _makeViewLink( (DataRow) ViewsList[ViewKey], false );
                }
            }
            base.OnInit( e );
        } // OnInit

        private void _makeViewLink( DataRow ViewRow, bool Indent)
        {
            if( Indent )
            {
                Literal IndentLiteral = new Literal();
                IndentLiteral.Text += "&nbsp;&nbsp;&nbsp;";
                ph.Controls.Add( IndentLiteral );
            }

            HyperLink ViewLink = new HyperLink();
            ViewLink.NavigateUrl = Master.MakeLink( "LowRes_SelectNode.aspx", CswConvert.ToInt32( ViewRow["nodeviewid"] ) );
            ViewLink.Text = ViewRow["viewname"].ToString();
            if( ViewRow["nodeviewid"].ToString() == Master.SelectedViewId.ToString() )
                ViewLink.Style.Add( HtmlTextWriterStyle.BackgroundColor, "#ffff00" );
            ph.Controls.Add( ViewLink );
            ph.Controls.Add( new CswLiteralBr() );
        }



        private Label _PageLabel;

        protected override void CreateChildControls()
        {
            _PageLabel = new Label();
            _PageLabel.Text = "Select a View:";
            _PageLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            ph.Controls.Add( _PageLabel );

            ph.Controls.Add( new CswLiteralBr() );

            base.CreateChildControls();
        }

    }
}