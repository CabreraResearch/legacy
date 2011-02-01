using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web;
using System.Xml;
using System.Web.Services;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;

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

        public string getDashboard()
        {
            string ret = string.Empty;
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

            foreach( DashIcon DashIcon in DashIcons )
            {
                if( _CswNbtResources.IsModuleEnabled( DashIcon.Module ) )
                {
                    ret += "<dash id=\"" + DashIcon.Id + "\" text=\"" + DashIcon.Text + "\" href=\"" + DashIcon.Href + "\" />";
                }
                else if( DashIcon.Module != CswNbtResources.CswNbtModule.NBTManager )
                {
                    ret += "<dash id=\"" + DashIcon.Id + "_off\" text=\"" + DashIcon.Text + "\" href=\"" + DashIcon.Href + "\" />";
                }
            }

            return "<dashboard>" + ret + "</dashboard>";
        } // getDashboard()


        public string getHeaderMenu()
        {
            string ret = string.Empty;

            ret += "<item text=\"Home\" href=\"NewMain.shtm\" />";
            ret += "<item text=\"Admin\">";
            ret += "  <item text=\"Current User List\" href=\"\" />";
            ret += "  <item text=\"View Log\" href=\"\" />";
            ret += "  <item text=\"Edit Config Vars\" href=\"\" />";
            ret += "  <item text=\"Statistics\" href=\"\" />";
            ret += "</item>";
            ret += "<item text=\"Preferences\">";
            ret += "  <item text=\"Profile\" href=\"\" />";
            ret += "  <item text=\"Subscriptions\" href=\"\" />";
            ret += "</item>";
            ret += "<item text=\"Help\">";
            ret += "  <item text=\"Help\" popup=\"help/index.htm\" />";
            ret += "  <item text=\"About\" popup=\"About.html\" />";
            ret += "</item>";
            ret += "<item text=\"Logout\" />";

            return "<menu>" + ret + "</menu>";
        }


    } // class CswNbtWebServiceHeader

} // namespace ChemSW.Nbt.WebServices
