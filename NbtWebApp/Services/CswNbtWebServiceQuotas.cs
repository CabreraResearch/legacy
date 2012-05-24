using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
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
            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClasses() )
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

                ret["objectclasses"][OCId]["nodetypes"] = new JObject();
                foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
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
                    _CswNbtActQuotas.SetQuotaForObjectClass( ObjectClassId, NewOCQuota );

                    foreach( JObject JNodeType in JObjClass["nodetypes"].Children().Values() )
                    {
                        Int32 NodeTypeId = CswConvert.ToInt32( JNodeType["nodetypeid"] );
                        Int32 NewNTQuota = CswConvert.ToInt32( JNodeType["quota"] );
                        _CswNbtActQuotas.SetQuotaForNodeType( NodeTypeId, NewNTQuota );
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

        public bool CheckQuota( Int32 NodeTypeId )
        {
            return _CswNbtActQuotas.CheckQuotaNT( NodeTypeId );
        }

        public bool CheckQuota( CswNbtMetaDataNodeType NodeType )
        {
            return _CswNbtActQuotas.CheckQuotaNT( NodeType );
        }

    } // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

