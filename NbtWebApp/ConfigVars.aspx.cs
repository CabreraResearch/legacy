using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.CswWebControls;
using ChemSW.Core;
using ChemSW.Config;

namespace ChemSW.Nbt.WebPages
{
    public partial class ConfigVars : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EnsureChildControls();
            if (!Master.CswNbtResources.CurrentNbtUser.IsAdministrator())
            {
                //Master.Redirect("Main.aspx");
                Master.GoHome();
            }

            CswAutoTable AutoTable = new CswAutoTable();
            ph.Controls.Add(AutoTable);
            Int32 Row = 0;
            foreach( CswConfigVariable ConfigVar in Master.CswNbtResources.ConfigVbls.ConfigVariables )
            {
                if( !ConfigVar.IsSystem )
                {
                    Literal NameLabel = new Literal();
                    NameLabel.Text = ConfigVar.VariableName;
                    AutoTable.addControl( Row, 0, NameLabel );

                    TextBox ValueTextBox = new TextBox();
                    ValueTextBox.ID = ConfigVar.VariableName;
                    ValueTextBox.CssClass = "textinput";
                    ValueTextBox.Width = Unit.Parse( "20em" );
                    ValueTextBox.Text = ConfigVar.VariableValue;
                    AutoTable.addControl( Row, 1, ValueTextBox );

                    Literal DescriptionLabel = new Literal();
                    DescriptionLabel.Text = ConfigVar.Description;
                    AutoTable.addControl( Row, 2, DescriptionLabel );
                    Row++;
                }
            }

            Button SaveButton = new Button();
            SaveButton.ID = "save";
            SaveButton.Text = "Save";
            SaveButton.CssClass = "Button";
            SaveButton.Click += new EventHandler(SaveButton_Click);
            ph.Controls.Add(SaveButton);
        }

        void SaveButton_Click(object sender, EventArgs e)
        {
            GetConfigVarValues(ph.Controls);
            // Master.CswNbtResources.saveConfigVariables(); // finalize should take care of this
        }

        private void GetConfigVarValues(ControlCollection Controls)
        {
            foreach (Control Control in Controls)
            {
                if (Control is TextBox)
                {
                    Master.CswNbtResources.ConfigVbls.setConfigVariableValue(((TextBox)Control).ID, ((TextBox)Control).Text);
                }
                if (Control.Controls.Count > 0)
                    GetConfigVarValues(Control.Controls);
            }
        }
    }
}