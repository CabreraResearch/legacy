using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Session;

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
                                         string.Empty,
                                         CswNbtResources.CswNbtModule.IMCS ) );

            DashIcons.Add( new DashIcon( "SI - Site Inspection",
                                         "dash_si",
                                         string.Empty,
                                         CswNbtResources.CswNbtModule.SI ) );
            // Case 24091
            //DashIcons.Add( new DashIcon( "STIS - Sample Tracking and Inventory System",
            //                             "dash_stis",
            //                             "http://www.chemswlive.com/19002.htm",
            //                             CswNbtResources.CswNbtModule.STIS ) );
            DashIcons.Add( new DashIcon( "CISPro - Chemical Inventory System",
                                         "dash_cispro",
                                         "",                              //"http://www.chemswlive.com/19002.htm",
                                         CswNbtResources.CswNbtModule.CISPro ) );
            //DashIcons.Add( new DashIcon( "CCPro - Control Charts",
            //                             "dash_ccpro",
            //                             "http://www.chemswlive.com/19002.htm",
            //                             CswNbtResources.CswNbtModule.CCPro ) );
            //DashIcons.Add( new DashIcon( "BioSafety",
            //                             "dash_biosafety",
            //                             "http://www.chemswlive.com/19002.htm",
            //                             CswNbtResources.CswNbtModule.BioSafety ) );
            //DashIcons.Add( new DashIcon( "Mobile",
            //                             "dash_hh",
            //                             "http://www.chemswlive.com/cis-pro-mobile.htm",
            //                             CswNbtResources.CswNbtModule.Mobile ) );
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

        private bool _SchemaHasDemoData()
        {
            CswTableSelect DemoNodesSelect = _CswNbtResources.makeCswTableSelect( "AdminMenuDemoSelect", "nodes" );
            //CswTableSelect DemoNodesSelect = new CswTableSelect( _CswNbtResources.CswResources, "AdminMenuDemoSelect", "nodes" );
            DataTable DemoNodesTable = DemoNodesSelect.getTable( new CswCommaDelimitedString { "nodeid" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
            return DemoNodesTable.Rows.Count > 0;
        }

        public JObject getHeaderMenu( CswSessionResourcesNbt CswSessionResources )
        {
            JObject Ret = new JObject();

            Ret["Home"] = new JObject( new JProperty( "action", "Home" ) );
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() || CswSessionResources.CswSessionManager.isImpersonating() )
            {
                Ret["Admin"] = new JObject();
                Ret["Admin"]["haschildren"] = true;
                if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                {
                    Ret["Admin"]["Current User List"] = new JObject();
                    Ret["Admin"]["Current User List"]["action"] = "Sessions";
                    Ret["Admin"]["Edit Config Vars"] = new JObject();
                    Ret["Admin"]["Edit Config Vars"]["href"] = "ConfigVars.aspx";
                    //Ret["Admin"]["Statistics"] = new JObject();
                    //Ret["Admin"]["Statistics"]["href"] = "Statistics.aspx";
                    Ret["Admin"]["Quotas"] = new JObject();
                    Ret["Admin"]["Quotas"]["action"] = "Quotas";
                    if( false == CswSessionResources.CswSessionManager.isImpersonating() )
                    {
                        Ret["Admin"]["Impersonate"] = new JObject();
                        Ret["Admin"]["Impersonate"]["action"] = "Impersonate";
                    }

                    if( _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername )
                    {
                        //Ret["Admin"]["View Log"] = new JObject();
                        //Ret["Admin"]["View Log"]["href"] = "DisplayLog.aspx";

                        Ret["Admin"]["Modules"] = new JObject();
                        Ret["Admin"]["Modules"]["action"] = "Modules";
                    }

                    if( _CswNbtResources.CurrentNbtUser.IsAdministrator() &&
                        _SchemaHasDemoData() )
                    {
                        Ret["Admin"]["Delete Demo Data"] = new JObject();
                        Ret["Admin"]["Delete Demo Data"]["action"] = "DeleteDemoNodes";
                    }
                } // if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )

                if( CswSessionResources.CswSessionManager.isImpersonating() )
                {
                    Ret["Admin"]["End Impersonation"] = new JObject();
                    Ret["Admin"]["End Impersonation"]["action"] = "EndImpersonation";
                }
            } // if( _CswNbtResources.CurrentNbtUser.IsAdministrator() || CswSessionResources.CswSessionManager.isImpersonating() )

            Ret["Preferences"] = new JObject(
                                    new JProperty( "haschildren", true ),
                                    new JProperty( "Profile", new JObject(
                                        new JProperty( "action", "Profile" ),
                                        new JProperty( "userid", _CswNbtResources.CurrentNbtUser.UserId.ToString() )
                                    ) ),
                                    new JProperty( "Subscriptions", new JObject(//TODO - case 27046 - remove once action below is finished
                                        new JProperty( "href", "Subscriptions.aspx" )
                                    ) )
                //new JProperty( "Subscriptions", new JObject(
                //    new JProperty( "action", "Subscriptions" )
                //) )
                        );

            if( _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                CswNbtActSubmitRequest RequestAction = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: false );
                if( RequestAction.CartContentCount > 0 )
                {
                    Ret["Cart (" + RequestAction.CartContentCount + ")"] = new JObject();
                    Ret["Cart (" + RequestAction.CartContentCount + ")"]["action"] = "Submit_Request";
                }
            }

            Ret["Help"] = new JObject();
            Ret["Help"]["haschildren"] = true;
            Ret["Help"]["Help"] = new JObject();
            Ret["Help"]["Help"]["popup"] = "help/index.htm";
            Ret["Help"]["Clear Cache"] = new JObject();
            Ret["Help"]["Clear Cache"]["action"] = "Clear Cache";
            Ret["Help"]["About"] = new JObject();
            Ret["Help"]["About"]["action"] = "About";
            CswNbtMetaDataObjectClass feedbackOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.FeedbackClass );
            IEnumerable<CswNbtMetaDataNodeType> feedbackNodeTypes = feedbackOC.getNodeTypes();
            if( feedbackNodeTypes.Any() )
            {
                Ret["Help"]["Give Feedback"] = new JObject();
                //Ret["Help"]["Give Feedback"]["action"] = "AddNode";
                Ret["Help"]["Give Feedback"]["action"] = "AddFeedback";
                CswNbtMetaDataNodeType feedbackNodeType = feedbackNodeTypes.First();
                Ret["Help"]["Give Feedback"]["nodetypeid"] = feedbackNodeType.NodeTypeId;
            }

            Ret["Logout"] = new JObject( new JProperty( "action", "Logout" ) );

            return Ret;
        }

        public JObject makeVersionJson( CswSessionResourcesNbt _CswSessionResources )
        {
            JObject ret = new JObject();

            string AssemblyFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/_Assembly.txt";
            if( File.Exists( AssemblyFilePath ) )
            {
                TextReader AssemblyFileReader = new StreamReader( AssemblyFilePath );
                ret.Add( new JProperty( "assembly", AssemblyFileReader.ReadLine() ) );
                AssemblyFileReader.Close();
            }

            JObject ComponentObj = new JObject();
            JProperty ComponentsProp = new JProperty( "components", ComponentObj );
            ret.Add( ComponentsProp );

            string VersionFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "/_Version.txt";
            if( File.Exists( VersionFilePath ) )
            {
                TextReader VersionFileReader = new StreamReader( VersionFilePath );
                ComponentObj.Add( new JProperty( "NbtWebApp",
                                        new JObject(
                                            new JProperty( "name", "NbtWebApp" ),
                                            new JProperty( "version", VersionFileReader.ReadLine() ),
                                            new JProperty( "copyright", "Copyright &copy; ChemSW, Inc. 2005-2011" )
                                            )
                             ) );
                VersionFileReader.Close();
            }

            ArrayList Assemblies = new ArrayList();
            Assemblies.Add( "CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" );
            Assemblies.Add( "CswCommon" );
            Assemblies.Add( "CswWebControls" );
            Assemblies.Add( "NbtConfig" );
            Assemblies.Add( "NbtLogic" );
            Assemblies.Add( "NbtWebControls" );
            Assemblies.Add( "Telerik.Web.UI" );

            foreach( string AssemblyName in Assemblies )
            {
                string name = string.Empty;
                name = AssemblyName.Contains( "," ) ? AssemblyName.Substring( 0, AssemblyName.IndexOf( ',' ) ) : AssemblyName;

                JObject AssemObj = new JObject();
                ComponentObj.Add( new JProperty( name, AssemObj ) );

                Assembly AssemblyInfo = Assembly.Load( AssemblyName );
                object[] AssemblyAttributes = (object[]) AssemblyInfo.GetCustomAttributes( true );

                string Version = AssemblyInfo.GetName().Version.ToString();
                string Copyright = string.Empty;
                foreach( AssemblyCopyrightAttribute AssemblyAttribute in AssemblyAttributes.OfType<AssemblyCopyrightAttribute>() )
                {
                    Copyright = ( AssemblyAttribute ).Copyright;
                }
                AssemObj.Add( new JProperty( "name", name ) );
                AssemObj.Add( new JProperty( "version", Version ) );
                AssemObj.Add( new JProperty( "copyright", Copyright ) );
            }

            ComponentObj.Add( new JProperty( "Schema",
                                    new JObject(
                                        new JProperty( "name", "Schema" ),
                                        new JProperty( "version", _CswNbtResources.ConfigVbls.getConfigVariableValue( "schemaversion" ) ),
                                        new JProperty( "copyright", "Copyright &copy; ChemSW, Inc. 2005-2011" )
                                        )

                         ) );

            SortedList<string, CswSessionsListEntry> sessions = _CswSessionResources.CswSessionManager.SessionsList.AllSessions;
            CswDateTime loginDate = new CswDateTime( _CswNbtResources );
            foreach( var entry in sessions )
            {
                CswSessionsListEntry sessionEntry = entry.Value;
                if( sessionEntry.UserName.Equals( _CswNbtResources.CurrentUser.Username ) )
                {
                    loginDate.FromClientDateString( sessionEntry.LoginDate.ToString() );
                }
            }

            JObject UserObj = new JObject();
            UserObj["customerid"] = new JObject( new JProperty( "componentName", "Customer ID:" ), new JProperty( "value", _CswNbtResources.AccessId ) );
            UserObj["loggedinas"] = new JObject( new JProperty( "componentName", "Logged in as:" ), new JProperty( "value", _CswNbtResources.CurrentUser.Username ) );
            UserObj["sessionsince"] = new JObject( new JProperty( "componentName", "Session since:" ), new JProperty( "value", loginDate.ToDateTime().ToString() ) );

            ret.Add( new JProperty( "userProps", UserObj ) );

            return ret;
        } // makeVersionJson()



    } // class CswNbtWebServiceHeader

} // namespace ChemSW.Nbt.WebServices
