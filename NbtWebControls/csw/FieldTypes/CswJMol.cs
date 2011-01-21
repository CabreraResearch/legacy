using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    [ToolboxData("<{0}:CswJMol runat=server></{0}:CswJMol>")]
    public class CswJMol : CswFieldTypeWebControl
    {
        private TextBox molbox = new TextBox();

        public CswJMol( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswJMol_DataBinding);
        }

        private void CswJMol_DataBinding( object sender, EventArgs e )
        {
            if( Prop != null )
                Mol = Prop.AsMol.Mol;
        }

        public override void Save()
        {
            if( !ReadOnly )
                Prop.AsMol.Mol = Mol;
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            Mol = string.Empty;
        }
        
        public string Mol
        {
            get
            {
                String s = (String)ViewState["Mol"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Mol"] = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            molbox.TextMode = TextBoxMode.MultiLine;
            molbox.Style.Add(HtmlTextWriterStyle.Display, "none");
            molbox.ID = this.ID + "_molbox";
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            String JS = "<script src=\"./jmol/Jmol.js\"></script>\n";
            JS += "<script language=\"JavaScript\" type=\"text/JavaScript\">\n";
            JS += "jmolInitialize(\"./jmol\");\n";
            JS += "setTimeout(\"jmolLoadInline(document.getElementById('" + this.ID + "_molbox').value);\", 100);\n";
            JS += "</script>\n";

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.ID + "_JMolBlock", JS);
            
            //Use ScriptManager statically instead of Page.ClientScript because it works with AJAX
            System.Web.UI.ScriptManager.RegisterClientScriptBlock(this, this.GetType(), this.UniqueID + "_JMolBlock", JS, false);
            
            base.OnPreRender(e);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            molbox.Text = Mol;
            molbox.RenderControl(writer);
            
            String ret = "";

            ret += "<applet name='jmolApplet0' id='jmolApplet0'\n";
            ret += " code='JmolApplet' archive='JmolApplet.jar'\n";
            ret += " codebase='./jmol'\n";
            ret += " width='300' height='300'\n";
            ret += " mayscript='true'>\n";
            ret += "  <param name='progressbar' value='true' />\n";
            ret += "  <param name='progresscolor' value='blue' />\n";
            ret += "  <param name='boxmessage' value='Downloading JmolApplet ...' />\n";
            ret += "  <param name='boxbgcolor' value='black' />\n";
            ret += "  <param name='boxfgcolor' value='white' />\n";
            ret += "  <param name='ReadyCallback' value='_jmolReadyCallback' />\n";
            ret += "</applet>\n";
            
            writer.Write(ret);
        }
    }
}
