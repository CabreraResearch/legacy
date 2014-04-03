using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using NbtWebApp.WebSvc.Returns;
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
            public CswEnumNbtModuleName Module;

            public DashIcon( string inText, string inId, string inHref, CswEnumNbtModuleName inModule )
            {
                Text = inText;
                Id = inId;
                Href = inHref;
                Module = inModule;
            }
        }

        [DataContract]
        public class HeaderUsername : CswWebSvcReturn
        {
            [DataMember]
            public string Data = string.Empty;
        }

        public JObject getDashboard()
        {
            JObject Ret = new JObject();
            Collection<DashIcon> DashIcons = new Collection<DashIcon>();

            DashIcons.Add( new DashIcon( "IMCS - Instrument Maintenance and Calibration",
                                         "dash_imcs",
                                         string.Empty,
                                         CswEnumNbtModuleName.IMCS ) );

            DashIcons.Add( new DashIcon( "SI - Site Inspection",
                                         "dash_si",
                                         string.Empty,
                                         CswEnumNbtModuleName.SI ) );
            // Case 24091
            //DashIcons.Add( new DashIcon( "STIS - Sample Tracking and Inventory System",
            //                             "dash_stis",
            //                             "http://www.chemswlive.com/19002.htm",
            //                             CswNbtModule.STIS ) );
            DashIcons.Add( new DashIcon( "CISPro - Chemical Inventory System",
                                         "dash_cispro",
                                         "",                              //"http://www.chemswlive.com/19002.htm",
                                         CswEnumNbtModuleName.CISPro ) );
            //DashIcons.Add( new DashIcon( "CCPro - Control Charts",
            //                             "dash_ccpro",
            //                             "http://www.chemswlive.com/19002.htm",
            //                             CswNbtModule.CCPro ) );
            //DashIcons.Add( new DashIcon( "BioSafety",
            //                             "dash_biosafety",
            //                             "http://www.chemswlive.com/19002.htm",
            //                             CswNbtModule.BioSafety ) );
            //DashIcons.Add( new DashIcon( "Mobile",
            //                             "dash_hh",
            //                             "http://www.chemswlive.com/cis-pro-mobile.htm",
            //                             CswNbtModule.Mobile ) );
            DashIcons.Add( new DashIcon( "NBTManager",
                                         "dash_nbtmgr",
                                         "",
                                         CswEnumNbtModuleName.NBTManager ) );

            foreach( DashIcon DashIcon in DashIcons.Where( DashIcon => _CswNbtResources.Modules.IsModuleEnabled( DashIcon.Module ) || DashIcon.Module != CswEnumNbtModuleName.NBTManager ) )
            {
                Ret.Add( new JProperty( ( _CswNbtResources.Modules.IsModuleEnabled( DashIcon.Module ) ) ? DashIcon.Id : DashIcon.Id + "_off",
                                        new JObject(
                                            new JProperty( "text", DashIcon.Text ),
                                            new JProperty( "href", DashIcon.Href )
                                            )
                             ) );
            }

            return Ret;
        } // getDashboard()

        private Int32 _getSchemaDemoDataCount()
        {
            Int32 Ret = 0;

            //Nodes
            CswTableSelect DemoNodesSelect = _CswNbtResources.makeCswTableSelect( "AdminMenuDemoSelect", "nodes" );
            DataTable DemoNodesTable = DemoNodesSelect.getTable( new CswCommaDelimitedString { "nodeid" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
            Ret += DemoNodesTable.Rows.Count;

            //Views
            CswTableSelect DemoViewsSelect = _CswNbtResources.makeCswTableSelect( "AdminMenuDemoSelect", "node_views" );
            DataTable DemoViewsTable = DemoViewsSelect.getTable( new CswCommaDelimitedString { "nodeviewid" }, " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
            Ret += DemoViewsTable.Rows.Count;

            return Ret;
        }

        private bool _isChemSWAdminImpersonating( CswSessionResourcesNbt CswSessionResources )
        {
            bool Ret = false;

            if( CswSessionResources.CswSessionManager.isImpersonating() )
            {
                Ret = _CswNbtResources.CswResources.AuditUsername.Equals( CswNbtObjClassUser.ChemSWAdminUsername );
            }

            return Ret;
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
                    Ret["Admin"]["Config Vars"] = new JObject();
                    Ret["Admin"]["Config Vars"]["action"] = "Config Vars";
                    //Ret["Admin"]["Statistics"] = new JObject();
                    //Ret["Admin"]["Statistics"]["href"] = "Statistics.aspx";
                    Ret["Admin"]["Quotas"] = new JObject();
                    Ret["Admin"]["Quotas"]["action"] = "Quotas";
                    Ret["Admin"]["Login Data"] = new JObject();
                    Ret["Admin"]["Login Data"]["action"] = "Login Data";

                    //Removed condition for Case 29185
                    Ret["Admin"]["Impersonate"] = new JObject();
                    Ret["Admin"]["Impersonate"]["action"] = "Impersonate";

                    if( _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername )
                    {
                        //Ret["Admin"]["View Log"] = new JObject();
                        //Ret["Admin"]["View Log"]["href"] = "DisplayLog.aspx";

                        Ret["Admin"]["Modules"] = new JObject();
                        Ret["Admin"]["Modules"]["action"] = "Modules";
                    }

                    //if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
                    //{
                    //    Int32 DemoCount = _getSchemaDemoDataCount();
                    //    if( DemoCount > 0 )
                    //    {
                    //        Ret["Admin"]["Delete Demo Data (" + DemoCount + ")"] = new JObject();
                    //        Ret["Admin"]["Delete Demo Data (" + DemoCount + ")"]["action"] = "DeleteDemoNodes";
                    //    }
                    //}
                } // if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )

                if( CswSessionResources.CswSessionManager.isImpersonating() )
                {
                    Ret["Admin"]["End Impersonation"] = new JObject();
                    Ret["Admin"]["End Impersonation"]["action"] = "EndImpersonation";
                }

                if( ( CswSessionResources.CswSessionManager.didChangeSchema() ) )
                {
                    if( _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername || _isChemSWAdminImpersonating( CswSessionResources ) )
                    {
                        Ret["Admin"]["Return to NbtManager"] = new JObject();
                        Ret["Admin"]["Return to NbtManager"]["action"] = "NbtManager";
                    }
                }

            } // if( _CswNbtResources.CurrentNbtUser.IsAdministrator() || CswSessionResources.CswSessionManager.isImpersonating() )

            Ret["Preferences"] = new JObject(
                                    new JProperty( "haschildren", true ),
                                    new JProperty( "Profile", new JObject(
                                        new JProperty( "action", "Profile" ),
                                        new JProperty( "userid", _CswNbtResources.CurrentNbtUser.UserId.ToString() )
                                    ) ),
                                    new JProperty( "Subscriptions", new JObject(
                                        new JProperty( "action", "Subscriptions" )
                                    ) )
                                );

            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Requesting ) && _CswNbtResources.Permit.can( CswEnumNbtActionName.Submit_Request ) )
            {
                CswNbtActRequesting RequestAction = new CswNbtActRequesting( _CswNbtResources );
                Int32 CartCount = RequestAction.getCartContentCount();
                Ret["Cart (" + CartCount + ")"] = new JObject();
                Ret["Cart (" + CartCount + ")"]["action"] = "Submit_Request";
            }

            Ret["Help"] = new JObject();
            Ret["Help"]["haschildren"] = true;
            Ret["Help"]["Help"] = new JObject();
            Ret["Help"]["Help"]["popup"] = "help/index.htm";
            Ret["Help"]["Clear Cache"] = new JObject();
            Ret["Help"]["Clear Cache"]["action"] = "Clear Cache";
            CswNbtMetaDataObjectClass feedbackOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.FeedbackClass );
            IEnumerable<CswNbtMetaDataNodeType> feedbackNodeTypes = feedbackOC.getNodeTypes();
            if( feedbackNodeTypes.Any() )
            {
                if( _CswNbtResources.Permit.canNodeType( Security.CswEnumNbtNodeTypePermission.Create, feedbackNodeTypes.First() ) )
                {
                    Ret["Help"]["Give Feedback"] = new JObject();
                    //Ret["Help"]["Give Feedback"]["action"] = "AddNode";
                    Ret["Help"]["Give Feedback"]["action"] = "AddFeedback";
                    CswNbtMetaDataNodeType feedbackNodeType = feedbackNodeTypes.First();
                    Ret["Help"]["Give Feedback"]["nodetypeid"] = feedbackNodeType.NodeTypeId;
                }
            }
            Ret["Help"]["Printing Labels"] = new JObject();
            Ret["Help"]["Printing Labels"]["popup"] = "PrintingLabels.html";
            Ret["Help"]["Reading Balances"] = new JObject();
            Ret["Help"]["Reading Balances"]["popup"] = "ReadingBalances.html";
            Ret["Help"]["About"] = new JObject();
            Ret["Help"]["About"]["action"] = "About";

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
            string ThisYear = DateTime.Now.Year.ToString();

            ArrayList CswAssemblies = new ArrayList();
            CswAssemblies.Add( "NbtWebApp" );
            CswAssemblies.Add( "CswCommon" );
            CswAssemblies.Add( "CswWebControls" );
            CswAssemblies.Add( "NbtLogic" );
            CswAssemblies.Add( "NbtWebControls" );

            foreach( string AssemblyName in CswAssemblies )
            {
                string name = AssemblyName.Contains( "," ) ? AssemblyName.Substring( 0, AssemblyName.IndexOf( ',' ) ) : AssemblyName;

                JObject AssemObj = new JObject();
                ComponentObj.Add( new JProperty( name, AssemObj ) );

                Assembly AssemblyInfo = Assembly.Load( AssemblyName );
                string Version = AssemblyInfo.GetName().Version.ToString();
                AssemObj.Add( new JProperty( "name", name ) );
                AssemObj.Add( new JProperty( "version", Version ) );
                AssemObj.Add( new JProperty( "copyright", "Copyright &copy; ChemSW, Inc. 2005-" + ThisYear ) );
            }

            ArrayList ThirdAssemblies = new ArrayList();
            ThirdAssemblies.Add( "CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" );
            ThirdAssemblies.Add( "Telerik.Web.UI" );

            foreach( string AssemblyName in ThirdAssemblies )
            {
                string name = AssemblyName.Contains( "," ) ? AssemblyName.Substring( 0, AssemblyName.IndexOf( ',' ) ) : AssemblyName;

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
                                        new JProperty( "copyright", "Copyright &copy; ChemSW, Inc. 2005-" + ThisYear )
                                        )

                         ) );

            #region ChemCatCentral Products Versions
            // Add ChemCatCentral version to the About dialog: Case 29380
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3Products ) )
            {
                CswC3Params CswC3Params = new CswC3Params();
                CswNbtC3ClientManager C3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
                SearchClient C3SearchClient = C3ClientManager.initializeC3Client();
                if( null != C3SearchClient )
                {
                    string C3Version = C3ClientManager.getCurrentC3Version();
                    ComponentObj.Add( new JProperty( "ChemCatCentral", new JObject(
                                                       new JProperty( "name", "ChemCatCentral" ),
                                                       new JProperty( "version", Regex.Replace( C3Version, "_", " " ) ),
                                                       new JProperty( "copyright", "Copyright &copy; ChemSW, Inc. 2005-" + ThisYear ) ) ) );

                    // Add the datasource import dates (if they are available to the user and/or the module is enabled
                    CswC3ServiceLogicGetDataSourcesDataSource[] DataSourceDates = C3ClientManager.getDataSourceDates();
                    if( null != DataSourceDates )
                    {
                        JObject DSDatesObj = new JObject();
                        foreach( CswC3ServiceLogicGetDataSourcesDataSource ds in DataSourceDates )
                        {
                            if( ds.DataType == "Extra Chemical" )
                            {
                                switch( ds.DataSourceName )
                                {
                                    case "FireDb":
                                        if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.FireDbSync ) )
                                        {
                                            DSDatesObj[ds.DataSourceName] = new JObject( new JProperty( "componentName", ds.DataSourceName ), new JProperty( "value", ds.ImportDate ) );
                                        }
                                        break;
                                    case "PCID":
                                        if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.PCIDSync ) )
                                        {
                                            DSDatesObj[ds.DataSourceName] = new JObject( new JProperty( "componentName", ds.DataSourceName ), new JProperty( "value", ds.ImportDate ) );
                                        }
                                        break;
                                } //switch
                            }
                            else
                            {
                                DSDatesObj[ds.DataSourceName] = new JObject( new JProperty( "componentName", ds.DataSourceName ), new JProperty( "value", ds.ImportDate ) );
                            }
                        }
                        ret.Add( new JProperty( "dsDates", DSDatesObj ) );
                    }

                }//if( C3ClientManager.checkC3ServiceReferenceStatus() )
            }
            #endregion ChemCatCentral Products Versions

            #region ACD Version
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3ACD ) )
            {
                CswC3Params CswC3Params = new CswC3Params();
                CswNbtC3ClientManager C3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3Params );
                SearchClient C3SearchClient = C3ClientManager.initializeC3Client();
                if( null != C3SearchClient )
                {
                    CswRetObjSearchResults Results = C3SearchClient.getACDVersion( CswC3Params );
                    if( null != Results )
                    {
                        ComponentObj.Add( new JProperty( "ACD", new JObject(
                                                       new JProperty( "name", "Available Chemicals Directory (ACD)" ),
                                                       new JProperty( "version", Regex.Replace( Results.ACDVersion, "_", " " ) ),
                                                       new JProperty( "copyright", "Copyright &copy; Accelrys, Inc. 1983-" + ThisYear ) ) ) );
                    }
                }
            }
            #endregion ACD Version


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

        public static void getHeaderUsername( ICswResources CswResources, HeaderUsername Return, object Request )
        {
            if( string.IsNullOrEmpty( CswResources.CswSessionManager.OriginalUserName ) )
            {
                Return.Data += CswResources.CurrentUser.Username + "@" + CswResources.AccessId;
            }
            else
            {
                Return.Data = CswResources.CswSessionManager.OriginalUserName + " as " + CswResources.CurrentUser.Username + "@" + CswResources.AccessId;
            }
        }

    } // class CswNbtWebServiceHeader

} // namespace ChemSW.Nbt.WebServices
