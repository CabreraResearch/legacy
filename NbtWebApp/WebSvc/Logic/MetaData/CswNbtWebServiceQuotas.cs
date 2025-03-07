using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for object class quota management
    /// </summary>
    public class CswNbtWebServiceQuotas
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtActQuotas _CswNbtActQuotas;

        public CswNbtWebServiceQuotas( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtActQuotas = new CswNbtActQuotas( _CswNbtResources );
        }

        private bool _CanEditQuotas
        {
            get
            {
                return ( _CswNbtActQuotas.UserCanEditQuotas( _CswNbtResources.CurrentNbtUser ) );
            }
        }

        public JObject GetQuotas()
        {
            JObject ret = new JObject();

            Dictionary<Int32, Int32> NodeCountsForNodeType;
            Dictionary<Int32, Int32> NodeCountsForObjectClass;
            _CswNbtActQuotas.GetNodeCounts( out NodeCountsForNodeType, out NodeCountsForObjectClass );

            ret["canedit"] = _CanEditQuotas.ToString().ToLower();
            ret["objectclasses"] = new JObject();
            foreach( CswNbtMetaDataObjectClass ObjectClass in from _ObjectClass in _CswNbtResources.MetaData.getObjectClasses() orderby _ObjectClass.ObjectClass select _ObjectClass )
            {
                string OCId = "oc_" + ObjectClass.ObjectClassId.ToString();
                ret["objectclasses"][OCId] = new JObject();
                ret["objectclasses"][OCId]["objectclass"] = ObjectClass.ObjectClass.ToString();
                ret["objectclasses"][OCId]["objectclassid"] = ObjectClass.ObjectClassId.ToString();
                if( NodeCountsForObjectClass.ContainsKey( ObjectClass.ObjectClassId ) )
                {
                    ret["objectclasses"][OCId]["currentusage"] = NodeCountsForObjectClass[ObjectClass.ObjectClassId];
                }
                else
                {
                    ret["objectclasses"][OCId]["currentusage"] = "0";
                }
                if( ObjectClass.Quota != Int32.MinValue )
                {
                    ret["objectclasses"][OCId]["quota"] = ObjectClass.Quota;
                }
                else
                {
                    ret["objectclasses"][OCId]["quota"] = "";
                }
                ret["objectclasses"][OCId]["nodetypecount"] = ObjectClass.getNodeTypes().Count().ToString();
                ret["objectclasses"][OCId]["excludeinquotabar"] = ObjectClass.ExcludeInQuotaBar;
                bool readOnly = ObjectClass.ObjectClass == CswEnumNbtObjectClass.InventoryGroupClass &&
                    false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MultiInventoryGroup ); //case 29298
                ret["objectclasses"][OCId]["readonly"] = readOnly;

                ret["objectclasses"][OCId]["nodetypes"] = new JObject();
                foreach( CswNbtMetaDataNodeType NodeType in from _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
                {
                    if( NodeType.IsLatestVersion() )
                    {
                        Int32 NodeTypeId = NodeType.FirstVersionNodeTypeId;
                        string NodeTypeName = NodeType.NodeTypeName;
                        Int32 Quota = NodeType.getFirstVersionNodeType().Quota;
                        string NTId = "nt_" + NodeType.FirstVersionNodeTypeId.ToString();

                        ret["objectclasses"][OCId]["nodetypes"][NTId] = new JObject();
                        ret["objectclasses"][OCId]["nodetypes"][NTId]["nodetypename"] = NodeTypeName;
                        ret["objectclasses"][OCId]["nodetypes"][NTId]["nodetypeid"] = NodeTypeId;
                        ret["objectclasses"][OCId]["nodetypes"][NTId]["excludeinquotabar"] = NodeType.ExcludeInQuotaBar;
                        ret["objectclasses"][OCId]["nodetypes"][NTId]["readonly"] = readOnly ||
                                ( NodeTypeName.Equals( "Site" ) && false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MultiSite ) ); //case 29298

                        if( NodeCountsForNodeType.ContainsKey( NodeTypeId ) )
                        {
                            ret["objectclasses"][OCId]["nodetypes"][NTId]["currentusage"] = NodeCountsForNodeType[NodeTypeId];
                        }
                        else
                        {
                            ret["objectclasses"][OCId]["nodetypes"][NTId]["currentusage"] = "0";
                        }
                        if( Quota != Int32.MinValue )
                        {
                            ret["objectclasses"][OCId]["nodetypes"][NTId]["quota"] = Quota;
                        }
                        else
                        {
                            ret["objectclasses"][OCId]["nodetypes"][NTId]["quota"] = "";
                        }
                    } // if( NodeType.IsLatestVersion )
                } // foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
            } // foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )

            return ret;
        } // GetQuotas()


        public bool SaveQuotas( string inQuotas )
        {
            JObject inQuotasJson = JObject.Parse( inQuotas );
            if( _CanEditQuotas )
            {
                foreach( JObject JObjClass in inQuotasJson["objectclasses"].Children().Values() )
                {
                    Int32 ObjectClassId = CswConvert.ToInt32( JObjClass["objectclassid"] );
                    Int32 NewOCQuota = CswConvert.ToInt32( JObjClass["quota"] );
                    bool ExcludeInQuotaBarOC = CswConvert.ToBoolean( JObjClass["excludeinquotabar"] );
                    _CswNbtActQuotas.SetQuotaForObjectClass( ObjectClassId, NewOCQuota, ExcludeInQuotaBarOC );

                    foreach( JObject JNodeType in JObjClass["nodetypes"].Children().Values() )
                    {
                        Int32 NodeTypeId = CswConvert.ToInt32( JNodeType["nodetypeid"] );
                        Int32 NewNTQuota = CswConvert.ToInt32( JNodeType["quota"] );
                        bool ExcludeInQuotaBarNT = CswConvert.ToBoolean( JNodeType["excludeinquotabar"] );
                        _CswNbtActQuotas.SetQuotaForNodeType( NodeTypeId, NewNTQuota, ExcludeInQuotaBarNT );
                    } // foreach( DataRow OCRow in OCTable.Rows )

                } // foreach( DataRow OCRow in OCTable.Rows )
                return true;
            } // if( CanEditQuotas )
            else
            {
                return false;
            }
        } // SaveQuotas()

        public Double GetTotalQuotaPercent()
        {
            return _CswNbtActQuotas.GetTotalQuotaPercent();
        }

        public Double GetHighestQuotaPercent()
        {
            return _CswNbtActQuotas.GetHighestQuotaPercent();
        }

        /// <summary>
        /// Returns true if there is a quota limit on any NodeType or ObjClass
        /// </summary>
        public bool IsQuotaSet()
        {
            bool ret = false;

            ret = _CswNbtResources.MetaData.getObjectClasses().Any( ObjClass => ObjClass.Quota > Int32.MinValue && false == ObjClass.ExcludeInQuotaBar );
            if( false == ret )
            {
                ret = _CswNbtResources.MetaData.getNodeTypes().Any( NodeType => NodeType.Quota > Int32.MinValue && false == NodeType.ExcludeInQuotaBar );
            }

            return ret;
        }

        [DataContract]
        public class CswNbtQuotaResponse: CswWebSvcReturn
        {
            public CswNbtQuotaResponse()
            {
                Data = new CswNbtActQuotas.Quota();
            }
            [DataMember]
            public CswNbtActQuotas.Quota Data;
        }

        [DataContract]
        public class QuotaRequest
        {
            [DataMember( IsRequired = false )]
            public int NodeTypeId;
            [DataMember( IsRequired = false )]
            public int ObjectClassId;
            [DataMember( IsRequired = false )]
            public string NodeKey;
            [DataMember( IsRequired = false )]
            public string NodeId;
        }

        public static void getQuota( ICswResources Resources, CswNbtQuotaResponse Response, QuotaRequest Request )
        {
            if( null != Resources )
            {
                CswNbtResources NbtResources = (CswNbtResources) Resources;
                CswNbtActQuotas ActQuotas = new CswNbtActQuotas( NbtResources );
                int NodeTypeId = Request.NodeTypeId;
                int ObjectClassId = Request.ObjectClassId;
                if( NodeTypeId <= 0 )
                {
                    CswNbtNodeKey Key = wsNBT.getNodeKey( Request.NodeKey );
                    if( null != Key )
                    {
                        NodeTypeId = Key.NodeTypeId;
                        ObjectClassId = Key.ObjectClassId;
                    }
                }
                if( NodeTypeId <= 0 )
                {
                    CswNbtNode Node = NbtResources.Nodes[Request.NodeId];
                    if( null != Node )
                    {
                        NodeTypeId = Node.NodeTypeId;
                        ObjectClassId = Node.getObjectClassId();
                    }
                }
                Response.Data = ActQuotas.CheckQuota( NodeTypeId, ObjectClassId );
            }
        }

    } // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

