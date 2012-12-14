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
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.PropTypes;

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

        [DataContract]
        public class StructureSearchDataReturn : CswWebSvcReturn
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
            Return.Data = StructureSearchData;
        }

        public static void SaveMolPropFile( ICswResources CswResources, MolDataReturn Return, MolData ImgData )
        {
            CswNbtResources NBTResources = (CswNbtResources) CswResources;
            CswPropIdAttr PropId = new CswPropIdAttr( ImgData.propId );
            CswNbtMetaDataNodeTypeProp MetaDataProp = NBTResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( Int32.MinValue != PropId.NodeId.PrimaryKey )
            {
                CswNbtNode Node = NBTResources.Nodes[PropId.NodeId];
                if( null != Node )
                {
                    CswNbtNodePropMol PropMol = Node.Properties[MetaDataProp];
                    if( null != PropMol )
                    {
                        // Do the update directly
                        CswTableUpdate JctUpdate = NBTResources.makeCswTableUpdate( "Clobber_save_update", "jct_nodes_props" );
                        if( PropMol.JctNodePropId > 0 )
                        {
                            DataTable JctTable = JctUpdate.getTable( "jctnodepropid", PropMol.JctNodePropId );
                            JctTable.Rows[0]["clobdata"] = ImgData.molString;
                            JctUpdate.update( JctTable );
                        }
                        else
                        {
                            DataTable JctTable = JctUpdate.getEmptyTable();
                            DataRow JRow = JctTable.NewRow();
                            JRow["nodetypepropid"] = CswConvert.ToDbVal( PropId.NodeTypePropId );
                            JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                            JRow["nodeidtablename"] = Node.NodeId.TableName;
                            JRow["clobdata"] = ImgData.molString;
                            JctTable.Rows.Add( JRow );
                            JctUpdate.update( JctTable );
                        }
                        byte[] molImage = CswStructureSearch.GetImage( ImgData.molString );
                        string Href;
                        CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( NBTResources, null, false, false );
                        ws.SetPropBlobValue( molImage, "mol.jpeg", "image/jpeg", ImgData.propId, "blobdata", out Href );
                        ImgData.href = Href;
                    }
                }
            }
            Return.Data = ImgData;
        }

        #endregion
    }

}