using System;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using Telerik.Web.UI;

namespace ChemSW.Nbt.WebPages
{
    public partial class PopupLayout : System.Web.UI.MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            Master.OnError += new CswErrorHandler(HandleError);
            base.OnInit(e);
        }

        public string AccessID
        {
            get { return Master.AccessId; }
            set { Master.AccessId = value; }
        }

        public CswNbtResources CswNbtResources
        {
            get { return Master.CswNbtResources; }
        }

        public CswNbtView CswNbtView
        {
            get { return Master.CswNbtView; }
        }
        public string LogoutPath
        {
            get { return Master.LogoutPath; }
            set { Master.LogoutPath = value; }
        }
        public RadAjaxManager AjaxManager
        {
            get { return Master.AjaxManager; }
        }

        //public CswAuthenticator CswAuthenticator
        //{
        //    get { return Master.CswAuthenticator; }
        //}

        //public ItemLookCollection ItemLooks
        //{
        //    get { return Master.ItemLooks; }
        //}

        public void HandleAddNode( CswNbtNode Node )
        {
            Master.HandleAddNode( Node );
        }
        public void HandleLoadReport( CswPrimaryKey ReportId )
        {
            Master.HandleLoadReport( ReportId );
        }
        public void HandleCopyNode( CswNbtNode OldNode, CswNbtNode NewNode )
        {
            Master.HandleCopyNode( OldNode, NewNode );
        }
        public void HandleMultiModeEnabled( CswNbtView View )
        {
            Master.HandleMultiModeEnabled(View);
        }
        public void HandleViewEditorFinish(CswNbtView View)
        {
            Master.HandleViewEditorFinish(View);
        }
        public void EndSession( string SessionId )
        {
            Master.EndSession( SessionId );
        }

        public RadWindow DesignDeleteDialogWindow
        {
            get { return Master.DesignDeleteDialogWindow; }
        }

        public void setViewId(Int32 ViewId)
        {
            Master.setViewId(ViewId);
        }
        public void setViewId(Int32 ViewId, bool ForceReload)
        {
            Master.setViewId(ViewId, ForceReload);
        }
        public void setSessionViewId( Int32 SessionViewId, bool ForceReload )
        {
            Master.setSessionViewId( SessionViewId, ForceReload );
        }//setSessionViewId()

        public void setSessionViewId( Int32 SessionViewId )
        {
            Master.setSessionViewId( SessionViewId );
        }//setSessionViewId()

        public void setViewXml(string ViewXml)
        {
            Master.setViewXml(ViewXml);
        }
        public void setViewXml(string ViewXml, bool ForceReload)
        {
            Master.setViewXml(ViewXml, ForceReload);
        }
        public void clearView()
        {
            Master.clearView();
        }

        public void ReleaseAll()
        {
            Master.ReleaseAll();
        }

        public void Redirect(string url)
        {
            Master.Redirect(url);
        }
        public void GoHome() { Master.GoHome(); }

        public void LogMessage(string Message)
        {
            Master.LogMessage(Message);
        }
        public void LogTimerResult(string Message, string TimerResult)
        {
            Master.LogTimerResult(Message, TimerResult);
        }
        
        public Int32 ActionId { get { return Master.ActionId; } }
        public void setAction( Int32 ActionId ) { Master.setAction( ActionId ); }
        public object PreviousView1 { get { return Master.PreviousView1; } }
        public object PreviousView2 { get { return Master.PreviousView2; } }
        public object PreviousView3 { get { return Master.PreviousView3; } }
        public object PreviousView4 { get { return Master.PreviousView4; } }
        public object PreviousView5 { get { return Master.PreviousView5; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if( Master.CswNbtResources.getConfigVariableValue( "showloadbox" ) != "1" )
                    ProgressDiv.Visible = false;
                else
                    ProgressDiv.Visible = true;

                //if( SearchTextBox.Text == string.Empty && null != Session[ "SearchString" ] != null )
                //    Session.Remove( "SearchString" );

                if (Master.IsAuthenticated())
                {
                    UserLabel.Text = Master.CswNbtResources.CurrentUser.Username;

                    //CswTableCaddy ConfigVarsTableCaddy = Master.CswNbtResources.makeCswTableCaddy("configuration_variables");
                    //ConfigVarsTableCaddy.WhereClause = "where variablename like 'brand_%'";
                    //DataTable ConfigVarsTable = ConfigVarsTableCaddy.Table;

                    //BrandTitle.Visible = false;
                    //BrandTitle2.Visible = false;
                    //BrandIcon.Visible = false;
                    //TitleSpacer.Visible = false;
                    //TitleSpacer2.Visible = false;

                    //foreach (DataRow Row in ConfigVarsTable.Rows)
                    //{
                    //    if (Row["variablename"].ToString() == "brand_pagetitle" && Row["variablevalue"].ToString() != string.Empty)
                    //    {
                    //string BrandPageTitle = Master.CswNbtResources.getConfigVariableValue("brand_pagetitle");
                    //if (BrandPageTitle != string.Empty)
                    //{
                    //    BrandTitle.Text = BrandPageTitle;
                    //    BrandTitle.Visible = true;
                    //    BrandTitle2.Text = BrandPageTitle;
                    //    BrandTitle2.Visible = true;
                    //    TitleSpacer.Visible = true;
                    //    TitleSpacer2.Visible = true;
                    //}
                    //if (Row["variablename"].ToString() == "brand_pageicon" && Row["variablevalue"].ToString() != string.Empty)
                    //{
                    //string BrandPageIcon = Master.CswNbtResources.getConfigVariableValue("brand_pageicon");
                    //if (BrandPageIcon != string.Empty)
                    //{
                    //    BrandIcon.ImageUrl = BrandPageIcon;
                    //    BrandIcon.Visible = true;
                    //}
                    //}

                    //DesignButton.Visible = false;
                    //if (CswNbtResources != null && CswNbtResources.CurrentUser.IsAdministrator())
                    //    DesignButton.Visible = true;

                }
                else
                {
                    UserLabel.Visible = false;
                    //LogoutButton.Visible = false;
                    //HomeButton.Visible = false;
                    //DesignButton.Visible = false;
                    //SearchTextBox.Visible = false;
                    //SearchButton.Visible = false;
                    //BrandTitle.Visible = false;
                    //BrandTitle2.Visible = false;
                    //BrandIcon.Visible = false;
                    //TitleSpacer.Visible = false;
                    //TitleSpacer2.Visible = false;

                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }


        }

        public void HandleError( Exception ex )
        {
            // See BZ 6662
            if( !( ex is System.Threading.ThreadAbortException ) )
            {
                if( CswNbtResources != null )
                {
                    CswNbtResources.AnErrorOccurred = true;
                    CswNbtResources.logError( ex );

                    // Display the error in the ErrorBox
                    if( CswNbtResources.getConfigVariableValue( "displayerrorsinui" ) != "0" )
                    {
                        string Title;
                        string Message;
                        if( ex is CswDniException )
                        {
                            Title = "Error: " + ( (CswDniException) ex ).MsgFriendly;
                            Message = ( (CswDniException) ex ).MsgEscoteric;
                        }
                        else
                        {
                            Title = "Error: An internal error occurred.";
                            Message = ex.Message;
                        }
                        Message += "<br/>Time: " + DateTime.Now.ToString();
                        Message += "<br/>Stack Trace: <br/>" + ex.StackTrace.Replace( "\n", "<br/>\n" ) + "<br/><br/>";

                        //Title = DateTime.Now.ToString() + ": " + Title;

                        ErrorBox.addMessage( Title, Message );
                    }
                }

                //if (Session["errors"] == null)
                //{
                //    Session.Add("errors", ex.Message);
                //}
                //else
                //{
                //    Session["errors"] = Session["errors"].ToString() + "|" + ex.Message;
                //}

                if( ex.InnerException != null )
                {
                    HandleError( ex.InnerException );
                }
            }
            else
            {
                throw ex;
            }
            Master.HandleAfterError( ex );
        }

    }
}
