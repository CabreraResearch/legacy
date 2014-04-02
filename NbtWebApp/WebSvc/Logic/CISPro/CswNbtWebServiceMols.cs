using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.StructureSearch;
using NbtWebApp.WebSvc.Logic.CISPro;
using NbtWebApp.WebSvc.Returns;

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
        public class MolDataReturn: CswWebSvcReturn
        {
            public MolDataReturn()
            {
                Data = new MolData();
            }
            [DataMember]
            public MolData Data;
        }

        [DataContract]
        public class StructureSearchDataReturn: CswWebSvcReturn
        {
            public StructureSearchDataReturn()
            {
                Data = new StructureSearchViewData();
            }
            [DataMember]
            public StructureSearchViewData Data;
        }

        #endregion

        #region Public

        public static void getMolImg( ICswResources CswResources, MolDataReturn Return, MolData ImgData )
        {
            string molData = ImgData.molString;
            string nodeId = ImgData.nodeId;
            string base64String = "";

            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            if( String.IsNullOrEmpty( molData ) && false == String.IsNullOrEmpty( nodeId ) ) //if we only have a nodeid, get the mol text from the mol property if there is one
            {
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( nodeId );
                CswNbtNode node = NbtResources.Nodes[pk];
                CswNbtMetaDataNodeTypeProp molNTP = node.getNodeType().getMolProperty();
                if( null != molNTP )
                {
                    molData = node.Properties[molNTP].AsMol.getMol();
                }
            }

            if( false == String.IsNullOrEmpty( molData ) )
            {
                //If the Direct Structure Search module is enabled, use the AcclDirect methods to generate an image. Otherwise, use the legacy code.
                byte[] bytes =
                    ( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.DirectStructureSearch ) ?
                    NbtResources.AcclDirect.GetImage( molData ) :
                    CswStructureSearch.GetImage( molData ) );

                base64String = Convert.ToBase64String( bytes );
            }

            ImgData.molImgAsBase64String = base64String;
            ImgData.molString = molData;
            Return.Data = ImgData;
        }

        public static void RunStructureSearch( ICswResources CswResources, StructureSearchDataReturn Return, StructureSearchViewData StructureSearchData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            string molData = StructureSearchData.molString;
            bool exact = StructureSearchData.exact;

            Collection<CswPrimaryKey> results = new Collection<CswPrimaryKey>();

            //If the DirectStructureSearch module is enabled, use AcclDirect to run a search. Otherwise use the legacy code
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.DirectStructureSearch ) )
            {
                DataTable resultsTbl = NbtResources.AcclDirect.RunStructureSearch( molData, exact );
                foreach( DataRow row in resultsTbl.Rows )
                {
                    results.Add( new CswPrimaryKey( "nodes", CswConvert.ToInt32( row["nodeid"] ) ) );
                }
            }
            else
            {
                Dictionary<int, string> resultsDict = NbtResources.StructureSearchManager.RunSearch( molData, exact );
                foreach( int nodeidPk in resultsDict.Keys )
                {
                    results.Add( new CswPrimaryKey( "nodes", nodeidPk ) );
                }
            }
            CswNbtView searchView = new CswNbtView( NbtResources );
            searchView.SetViewMode( CswEnumNbtViewRenderingMode.Table );
            searchView.Category = "Recent";
            searchView.ViewName = "Structure Search Results";

            if( results.Count > 0 )
            {
                CswNbtMetaDataObjectClass materialOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
                CswNbtMetaDataObjectClassProp molCOP = materialOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.Structure );
                CswNbtViewRelationship parent = searchView.AddViewRelationship( materialOC, false );
                searchView.AddViewProperty( parent, molCOP );

                foreach( CswPrimaryKey nodeId in results )
                {
                    parent.NodeIdsToFilterIn.Add( nodeId );
                }
            }

            searchView.SaveToCache( false );

            StructureSearchData.viewId = searchView.SessionViewId.ToString();
            StructureSearchData.viewMode = searchView.ViewMode.ToString();
            Return.Data = StructureSearchData;
        }

        public static void SaveMolPropFile( ICswResources CswResources, MolDataReturn Return, MolData ImgData )
        {
            CswNbtResources NBTResources = (CswNbtResources) CswResources;

            string Href;
            CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( NBTResources );
            string FormattedMolString;
            string errorMsg;
            SdBlobData.saveMol( ImgData.molString, ImgData.propId, out Href, out FormattedMolString, out errorMsg );
            ImgData.molString = FormattedMolString;
            ImgData.href = Href;
            ImgData.errorMsg = errorMsg;

            Return.Data = ImgData;
        }

        public static void ClearMolFingerprint( ICswResources CswResources, MolDataReturn Return, MolData Request )
        {
            //TODO: remove me
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey pk = new CswPrimaryKey();
            pk.FromString( Request.nodeId );
        }

        #endregion
    }

}