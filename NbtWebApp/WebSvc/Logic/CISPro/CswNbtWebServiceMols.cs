using System;
using System.Collections.Generic;
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
                    molData = node.Properties[molNTP].AsMol.Mol;
                }
            }

            if( false == String.IsNullOrEmpty( molData ) )
            {
                byte[] bytes = CswStructureSearch.GetImage( molData );
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

            Dictionary<int, string> results = NbtResources.StructureSearchManager.RunSearch( molData, exact );
            CswNbtView searchView = new CswNbtView( NbtResources );
            searchView.SetViewMode( CswEnumNbtViewRenderingMode.Table );
            searchView.Category = "Recent";
            searchView.ViewName = "Structure Search Results";

            if( results.Count > 0 )
            {
                CswNbtMetaDataObjectClass materialOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
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
            Return.Data = StructureSearchData;
        }

        public static void SaveMolPropFile( ICswResources CswResources, MolDataReturn Return, MolData ImgData )
        {
            CswNbtResources NBTResources = (CswNbtResources) CswResources;

            string Href;
            CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( NBTResources );
            SdBlobData.saveMol( ImgData.molString, ImgData.propId, out Href );
            ImgData.href = Href;

            Return.Data = ImgData;
        }

        public static void ClearMolFingerprint( ICswResources CswResources, MolDataReturn Return, MolData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey pk = new CswPrimaryKey();
            pk.FromString( Request.nodeId );
            NbtResources.StructureSearchManager.DeleteFingerprintRecord( pk.PrimaryKey );
        }

        #endregion
    }

}