using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;
using System.Data;
using ChemSW.DB;
using ChemSW.StructureSearch;
using NbtWebApp.WebSvc.Logic.CISPro;
using ChemSW.Nbt.MetaData;
using System.Collections.Generic;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMols
    {
        #region Ctor

        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceMols( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Ctor

        #region DataContract

        [DataContract]
        public class MolDataReturn : CswWebSvcReturn
        {
            public MolDataReturn()
            {
                Data = new MolData();
            }
            [DataMember]
            public MolData Data;
        }

        #endregion

        #region Public

        public static void getMolImg( ICswResources CswResources, MolDataReturn Return, MolData.MolImgData ImgData )
        {
            string molData = ImgData.molString;
            string nodeId = ImgData.nodeId;
            string base64String = "";

            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            if( String.IsNullOrEmpty( molData ) && false == String.IsNullOrEmpty( nodeId ) ) //if we only have a nodeid, get the mol text from the mol property if there is one
            {
                //----------------------THIS NEEDS TO BE BETTER ------------INSTANCE NODE, ITERATE PROPS, GET MOL PROP------
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( nodeId );
                CswTableSelect ts = NbtResources.makeCswTableSelect( "getMolProp", "jct_nodes_props" );
                DataTable dt = ts.getTable( "where nodeid = " + pk.PrimaryKey + " and field1 = 'mol.jpeg'" );
                if( dt.Rows.Count > 0 )
                {
                    molData = dt.Rows[0]["clobdata"].ToString();
                }
                //----------------------THIS NEEDS TO BE BETTER ------------------------------------------------------------
            }

            if( false == String.IsNullOrEmpty( molData ) )
            {
                byte[] bytes = CswStructureSearch.GetImage( molData );
                base64String = Convert.ToBase64String( bytes );
            }

            ImgData.molImgAsBase64String = base64String;
            ImgData.molString = molData;
            Return.Data.MolImgDataCollection.Add( ImgData );
        }

        public static void RunStructureSearch( ICswResources CswResources, MolDataReturn Return, MolData.StructureSearchViewData StructureSearchData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            string molData = StructureSearchData.molString;
            bool exact = StructureSearchData.exact;

            Dictionary<int, string> results = NbtResources.StructureSearchManager.RunSearch( molData, exact );
            CswNbtView searchView = new CswNbtView( NbtResources );
            searchView.SetViewMode( NbtViewRenderingMode.Table );
            searchView.Category = "Recent";
            searchView.ViewName = "Structure Search Results";

            if( results.Count > 0 )
            {
                CswNbtMetaDataObjectClass materialOC = NbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtViewRelationship parent = searchView.AddViewRelationship( materialOC, false );

                foreach( int nodeId in results.Keys )
                {
                    CswPrimaryKey pk = new CswPrimaryKey( "nodes", nodeId );
                    parent.NodeIdsToFilterIn.Add( pk );
                }
            }

            searchView.SaveToCache( false );

            StructureSearchData.viewId = searchView.SessionViewId.ToString();
            StructureSearchData.viewMode = searchView.ViewMode.ToString();
            Return.Data.StructureSearchViewDataCollection.Add( StructureSearchData );
        }

        #endregion
    }

}