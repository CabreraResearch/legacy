using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
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

			Dictionary<Int32, Int32> NodeCounts = _CswNbtActQuotas.GetNodeCounts();

			ret["canedit"] = _CanEditQuotas.ToString().ToLower();
			ret["objectclasses"] = new JObject();
			foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
			{
				string OCId = "oc_" + ObjectClass.ObjectClassId.ToString();
				ret["objectclasses"][OCId] = new JObject();
				ret["objectclasses"][OCId]["objectclass"] = ObjectClass.ObjectClass.ToString();
				ret["objectclasses"][OCId]["objectclassid"] = ObjectClass.ObjectClassId.ToString();
				if( NodeCounts.ContainsKey( ObjectClass.ObjectClassId ) )
				{
					ret["objectclasses"][OCId]["currentusage"] = NodeCounts[ObjectClass.ObjectClassId];
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
				ret["objectclasses"][OCId]["nodetypecount"] = ObjectClass.NodeTypes.Count.ToString();

				ret["objectclasses"][OCId]["nodetypes"] = new JObject();
				foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
				{
					if( NodeType.IsLatestVersion )
					{
						string NTId = "nt_" + NodeType.FirstVersionNodeTypeId.ToString();
						ret["objectclasses"][OCId]["nodetypes"][NTId] = new JObject();
						ret["objectclasses"][OCId]["nodetypes"][NTId]["nodetypename"] = NodeType.NodeTypeName;
						ret["objectclasses"][OCId]["nodetypes"][NTId]["nodetypeid"] = NodeType.NodeTypeId;
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
				foreach(JObject JObjClass in inQuotasJson["objectclasses"].Children().Values())
				{
					Int32 ObjectClassId = CswConvert.ToInt32( JObjClass["objectclassid"] );
					Int32 NewQuota = CswConvert.ToInt32( JObjClass["quota"] );
					_CswNbtActQuotas.SetQuota( ObjectClassId, NewQuota );

				} // foreach( DataRow OCRow in OCTable.Rows )
				return true;
			} // if( CanEditQuotas )
			else
			{
				return false;
			}
		} // SaveQuotas()

		public Double GetQuotaPercent()
		{
			return _CswNbtActQuotas.GetQuotaPercent();
		}

		public bool CheckQuota( Int32 NodeTypeId )
		{
			return _CswNbtActQuotas.CheckQuotaNT( NodeTypeId );
		}

	} // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

