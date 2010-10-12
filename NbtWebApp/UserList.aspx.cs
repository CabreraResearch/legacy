using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.NbtWebControls;
using ChemSW.Security;
using ChemSW.Exceptions;
using ChemSW.Session;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{

    public partial class UserList : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            try
            {
                EnsureChildControls();
                if (!Master.CswNbtResources.CurrentNbtUser.IsAdministrator())
                {
                    //Master.Redirect("Main.aspx");
                    Master.GoHome();
                }
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }
            base.OnInit(e);
        }

        private void initGrid()
        {
            Table.Controls.Clear();

            Literal BootHeading = new Literal();
            BootHeading.ID = "BootHeading";
            BootHeading.Text = "<b>Burn</b>";
            Table.addControl(0, 0, BootHeading);

            Literal UsernameHeading = new Literal();
            UsernameHeading.ID = "UsernameHeading";
            UsernameHeading.Text = "<b>Username</b>";
            Table.addControl(0, 1, UsernameHeading);

            Literal LoginDateHeading = new Literal();
            LoginDateHeading.ID = "LoginDateHeading";
            LoginDateHeading.Text = "<b>Login Date</b>";
            Table.addControl(0, 2, LoginDateHeading);

            Literal TimeoutDateHeading = new Literal();
            TimeoutDateHeading.ID = "TimeoutDateHeading";
            TimeoutDateHeading.Text = "<b>Timeout Date</b>";
            Table.addControl(0, 3, TimeoutDateHeading);

            Literal AccessIdHeading = new Literal();
            AccessIdHeading.ID = "AccessIdHeading";
            AccessIdHeading.Text = "<b>Access Id</b>";
            Table.addControl(0, 4, AccessIdHeading);

            CswAutoTable MiniTable = new CswAutoTable();
            Table.addControl(0, 5, MiniTable);
            MiniTable.Width = Unit.Percentage(100);

            Literal SessionIdHeading = new Literal();
            SessionIdHeading.ID = "SessionIdHeading";
            SessionIdHeading.Text = "<b>Session Id</b>";
            MiniTable.addControl(0, 0, SessionIdHeading);

            TableCell RefreshButtonCell = MiniTable.getCell(0, 1);
            RefreshButtonCell.HorizontalAlign = HorizontalAlign.Right;
            CswImageButton RefreshButton = new CswImageButton(CswImageButton.ButtonType.Refresh);
            RefreshButton.ID = "refresh";
            RefreshButtonCell.Controls.Add(RefreshButton);

            Int32 rownum = 1;
            //CswSessionsList SessionList = Master.SessionList;
            SortedList<string, CswSessionsListEntry> SessionList = Master.SessionList.AllSessions;
            foreach (CswSessionsListEntry Entry in SessionList.Values )
            {
                // Filter to the administrator's access id only
                if (Entry.AccessId == Master.AccessID)
                {
                    CswImageButton BootButton = new CswImageButton(CswImageButton.ButtonType.Fire);
                    BootButton.ID = "boot_" + Entry.SessionId;
                    BootButton.Click += new EventHandler(BootButton_Click);
                    Table.addControl(rownum, 0, BootButton);

                    Literal UsernameLiteral = new Literal();
                    if (Entry.SessionId == Page.Session.SessionID)
                        UsernameLiteral.Text = Entry.UserName + " (you)";
                    else
                        UsernameLiteral.Text = Entry.UserName;
                    Table.addControl(rownum, 1, UsernameLiteral);

                    Literal LoginDateLiteral = new Literal();
                    LoginDateLiteral.Text = Entry.LoginDate.ToString();
                    Table.addControl(rownum, 2, LoginDateLiteral);

                    Literal TimeoutDateLiteral = new Literal();
                    TimeoutDateLiteral.Text = Entry.TimeoutDate.ToString();
                    Table.addControl(rownum, 3, TimeoutDateLiteral);

                    Literal AccessIdLiteral = new Literal();
                    AccessIdLiteral.Text = Entry.AccessId;
                    Table.addControl(rownum, 4, AccessIdLiteral);

                    Literal SessionIdLiteral = new Literal();
                    SessionIdLiteral.Text = Entry.SessionId;
                    Table.addControl(rownum, 5, SessionIdLiteral);

                    rownum++;
                } // if (Entry.AccessId == Master.AccessID)
            } // foreach (CswAuthenticator.SessionListEntry Entry in SessionList.Values)
        } // initGrid()

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                initGrid();
            }
            catch (Exception ex)
            {
                Master.HandleError(ex);
            }
            base.OnLoad(e);
        }

        protected void BootButton_Click(object sender, EventArgs e)
        {
            if (((WebControl)sender).ID.Substring(0, "boot_".Length) == "boot_")
            {
                string SessionId = ((WebControl)sender).ID.Substring("boot_".Length);
                Master.EndSession( SessionId );
                initGrid();
            }
        }


        private CswAutoTable Table;
        protected override void CreateChildControls()
        {
            Table = new CswAutoTable();
            Table.ID = "Table";
            Table.CssClass = "SessionListTable";
            Table.CellPadding = 0;
            Table.CellSpacing = 5;
            ph.Controls.Add(Table);

            base.CreateChildControls();
        }

    }

}