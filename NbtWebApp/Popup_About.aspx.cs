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
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string BrandPageTitle = Master.CswNbtResources.getConfigVariableValue("brand_pagetitle");
            if (BrandPageTitle != string.Empty)
            {
                TitleContentLiteral.Text = "About " + BrandPageTitle;
                //LeftHeaderContentLiteral.Text = "About " + BrandPageTitle;
            }

            AboutText.Text = makeVersionString();
        }

        private string makeVersionString()
        {
            string ret = string.Empty;

            string AssemblyFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/_Assembly.txt";
            if (File.Exists(AssemblyFilePath))
            {
                TextReader AssemblyFileReader = new StreamReader(AssemblyFilePath);
                ret += "NBT Assembly Version: " + AssemblyFileReader.ReadLine();
                AssemblyFileReader.Close();
            }

            ret += "<br><br>";

            ret += "<table>";

            string VersionFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/_Version.txt";
            if (File.Exists(VersionFilePath))
            {
                TextReader VersionFileReader = new StreamReader(VersionFilePath);
                ret += "<tr><td>NbtWebApp</td><td>" + VersionFileReader.ReadLine() + "</td><td>Copyright &copy; ChemSW, Inc. 2005-2009</td></tr>";
                VersionFileReader.Close();
            }

            ArrayList Assemblies = new ArrayList();
            Assemblies.Add( "CswCommon" );
            Assemblies.Add( "CswWebControls" );
            Assemblies.Add( "FarPoint.Web.Spread" );
            Assemblies.Add( "NbtBase" );
            Assemblies.Add( "NbtConfig" );
            Assemblies.Add( "NbtLogic" );
            Assemblies.Add( "NbtWebControls" );
            Assemblies.Add( "Telerik.Web.UI" );

            foreach (string AssemblyName in Assemblies)
            {
                ret += "<tr><td>" + AssemblyName + "</td>";
                Assembly AssemblyInfo = Assembly.Load(AssemblyName);
                object[] AssemblyAttributes = (object[])AssemblyInfo.GetCustomAttributes(true);

                string Version = AssemblyInfo.GetName().Version.ToString();
                string Copyright = string.Empty;
                foreach (object AssemblyAttribute in AssemblyAttributes)
                {
                    //if (AssemblyAttribute is AssemblyFileVersionAttribute)
                    //{
                    //    Version = ( (AssemblyFileVersionAttribute) AssemblyAttribute ).Version;
                    //}
                    if (AssemblyAttribute is AssemblyCopyrightAttribute)
                    {
                        Copyright = ( (AssemblyCopyrightAttribute) AssemblyAttribute ).Copyright;
                    }
                }
                ret += "<td>" + Version + "</td><td>" + Copyright + "</td></tr>";
            }

            //CswTableCaddy ConfigVarsTableCaddy = Master.CswNbtResources.makeCswTableCaddy("configuration_variables");
            //ConfigVarsTableCaddy.WhereClause = "where variablename = 'schemaversion'";
            //DataTable ConfigVarsTable = ConfigVarsTableCaddy.Table;

            ret += "<tr><td>Schema</td><td>" + Master.CswNbtResources.getConfigVariableValue("schemaversion") + "</td><td></td></tr>";

            ret += "</table>";
            return ret;
        }
    }
}
