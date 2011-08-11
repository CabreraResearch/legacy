using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceHeader
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtWebServiceHeader( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private class DashIcon
        {
            public string Text;
            public string Id;
            public string Href;
            public CswNbtResources.CswNbtModule Module;

            public DashIcon( string inText, string inId, string inHref, CswNbtResources.CswNbtModule inModule )
            {
                Text = inText;
                Id = inId;
                Href = inHref;
                Module = inModule;
            }
        }

        public JObject getDashboard()
        {
            JObject Ret = new JObject();
            Collection<DashIcon> DashIcons = new Collection<DashIcon>();
            DashIcons.Add( new DashIcon( "IMCS - Instrument Maintenance and Calibration",
                                         "dash_imcs",
                                         "http://www.chemswlive.com/19013.htm",
                                         CswNbtResources.CswNbtModule.IMCS ) );
            DashIcons.Add( new DashIcon( "FE - Fire Extinguisher Inspection",
                                         "dash_fe",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.FE ) );
            DashIcons.Add( new DashIcon( "SI - Site Inspection",
                                         "dash_si",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.SI ) );
            DashIcons.Add( new DashIcon( "STIS - Sample Tracking and Inventory System",
                                         "dash_stis",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.STIS ) );
            DashIcons.Add( new DashIcon( "CISPro - Chemical Inventory System",
                                         "dash_cispro",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.CISPro ) );
            DashIcons.Add( new DashIcon( "CCPro - Control Charts",
                                         "dash_ccpro",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.CCPro ) );
            DashIcons.Add( new DashIcon( "BioSafety",
                                         "dash_biosafety",
                                         "http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.BioSafety ) );
            DashIcons.Add( new DashIcon( "Mobile",
                                         "dash_hh",
                                         "http://www.chemswlive.com/cis-pro-mobile.htm",
                                         CswNbtResources.CswNbtModule.Mobile ) );
            DashIcons.Add( new DashIcon( "NBTManager",
                                         "dash_nbtmgr",
                                         "",
                                         CswNbtResources.CswNbtModule.NBTManager ) );

            foreach( DashIcon DashIcon in DashIcons.Where( DashIcon => _CswNbtResources.IsModuleEnabled( DashIcon.Module ) || DashIcon.Module != CswNbtResources.CswNbtModule.NBTManager ) )
            {
                Ret.Add( new JProperty( ( _CswNbtResources.IsModuleEnabled( DashIcon.Module ) ) ? DashIcon.Id : DashIcon.Id + "_off",
                                        new JObject(
                                            new JProperty( "text", DashIcon.Text ),
                                            new JProperty( "href", DashIcon.Href )
                                            )
                             ) );
            }

            return Ret;
        } // getDashboard()


        public JObject getHeaderMenu()
        {
            JObject Ret = new JObject();

            Ret["Home"] = new JObject( new JProperty( "action", "Home" ) );
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                Ret["Admin"] = new JObject(
                                new JProperty( "haschildren", true ),
                                new JProperty( "Current User List", new JObject(
                                    new JProperty( "href", "UserList.aspx" )
                                ) ),
                                new JProperty( "View Log", new JObject(
                                    new JProperty( "href", "DisplayLog.aspx" )
                                ) ),
                                new JProperty( "Edit Config Vars", new JObject(
                                    new JProperty( "href", "ConfigVars.aspx" )
                                ) ),
                                new JProperty( "Statistics", new JObject(
                                    new JProperty( "href", "Statistics.aspx" )
                                ) )
                        );
            }
            Ret["Preferences"] = new JObject(
                                    new JProperty( "haschildren", true ),
                                    new JProperty( "Profile", new JObject(
                                        new JProperty( "action", "Profile" ),
                                        new JProperty( "userid", _CswNbtResources.CurrentNbtUser.UserNode.NodeId.ToString() )
                                    ) ),
                                    new JProperty( "Subscriptions", new JObject(
                                        new JProperty( "href", "Subscriptions.aspx" )
                                    ) )
                        );
            Ret["Help"] = new JObject(
                                new JProperty( "haschildren", true ),
                                new JProperty( "Help", new JObject(
                                    new JProperty( "popup", "help/index.htm" )
                                ) ),
                                new JProperty( "About", new JObject(
                                    new JProperty( "action", "About" )
                                ) )
                        );
            Ret["Logout"] = new JObject( new JProperty( "action", "Logout" ) );

            return Ret;
        }


        public string makeVersionXml()
        {
            string ret = string.Empty;

            string AssemblyFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/_Assembly.txt";
            if( File.Exists( AssemblyFilePath ) )
            {
                TextReader AssemblyFileReader = new StreamReader( AssemblyFilePath );
                ret += "<assembly>" + AssemblyFileReader.ReadLine() + "</assembly>";
                AssemblyFileReader.Close();
            }

            string VersionFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/_Version.txt";
            if( File.Exists( VersionFilePath ) )
            {
                TextReader VersionFileReader = new StreamReader( VersionFilePath );
                ret += "<component>";
                ret += " <name>NbtWebApp</name>";
                ret += " <version>" + VersionFileReader.ReadLine() + "</version>";
                ret += " <copyright>Copyright &copy; ChemSW, Inc. 2005-2011</copyright>";
                ret += "</component>";
                VersionFileReader.Close();
            }

            ArrayList Assemblies = new ArrayList();
            Assemblies.Add( "CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" );
            Assemblies.Add( "CswCommon" );
            Assemblies.Add( "CswWebControls" );
            Assemblies.Add( "FarPoint.Web.Spread" );
            Assemblies.Add( "NbtConfig" );
            Assemblies.Add( "NbtLogic" );
            Assemblies.Add( "NbtWebControls" );
            Assemblies.Add( "Telerik.Web.UI" );

            foreach( string AssemblyName in Assemblies )
            {
                ret += "<component>";
                if( AssemblyName.Contains( "," ) )
                    ret += "<name>" + AssemblyName.Substring( 0, AssemblyName.IndexOf( ',' ) ) + "</name>";
                else
                    ret += "<name>" + AssemblyName + "</name>";
                Assembly AssemblyInfo = Assembly.Load( AssemblyName );
                object[] AssemblyAttributes = (object[]) AssemblyInfo.GetCustomAttributes( true );

                string Version = AssemblyInfo.GetName().Version.ToString();
                string Copyright = string.Empty;
                foreach( object AssemblyAttribute in AssemblyAttributes )
                {
                    //if (AssemblyAttribute is AssemblyFileVersionAttribute)
                    //{
                    //    Version = ( (AssemblyFileVersionAttribute) AssemblyAttribute ).Version;
                    //}
                    if( AssemblyAttribute is AssemblyCopyrightAttribute )
                    {
                        Copyright = ( (AssemblyCopyrightAttribute) AssemblyAttribute ).Copyright;
                    }
                }
                ret += "<version>" + Version + "</version>";
                ret += "<copyright>" + Copyright + "</copyright>";
                ret += "</component>";
            }

            //CswTableCaddy ConfigVarsTableCaddy = Master.CswNbtResources.makeCswTableCaddy("configuration_variables");
            //ConfigVarsTableCaddy.WhereClause = "where variablename = 'schemaversion'";
            //DataTable ConfigVarsTable = ConfigVarsTableCaddy.Table;

            ret += "<component>";
            ret += " <name>Schema</name>";
            ret += " <version>" + _CswNbtResources.getConfigVariableValue( "schemaversion" ) + "</version>";
            ret += " <copyright>Copyright &copy; ChemSW, Inc. 2005-2011</copyright>";
            ret += "</component>";

            return "<versions>" + ret + "</versions>";
        } // makeVersionXml()



    } // class CswNbtWebServiceHeader

} // namespace ChemSW.Nbt.WebServices
