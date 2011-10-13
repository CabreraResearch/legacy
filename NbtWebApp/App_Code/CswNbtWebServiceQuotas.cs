using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
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

		public CswNbtWebServiceQuotas( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

		private bool CanEditQuotas { get { return ( _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername ); } }

		public JObject GetQuotas()
		{
			JObject ret = new JObject();
			ret["canedit"] = CanEditQuotas.ToString().ToLower();
			ret["objectclasses"] = new JObject();
			foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.ObjectClasses )
			{
				string OCId = "oc_" + ObjectClass.ObjectClassId.ToString();
				ret["objectclasses"][OCId] = new JObject();
				ret["objectclasses"][OCId]["objectclass"] = ObjectClass.ObjectClass.ToString();
				ret["objectclasses"][OCId]["objectclassid"] = ObjectClass.ObjectClassId.ToString();
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
			if( CanEditQuotas )
			{
				CswTableUpdate OCUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtWebServiceQuotas_UpdateOC", "object_class" );
				DataTable OCTable = OCUpdate.getTable();
				foreach( DataRow OCRow in OCTable.Rows )
				{
					string OCId = "oc_" + OCRow["objectclassid"].ToString();
					if( inQuotasJson["objectclasses"][OCId] != null )
					{
						Int32 OldQuota = CswConvert.ToInt32( OCRow["quota"] );
						Int32 NewQuota = CswConvert.ToInt32( inQuotasJson["objectclasses"][OCId]["quota"] );
						if( OldQuota != NewQuota )
						{
							OCRow["quota"] = CswConvert.ToDbVal( NewQuota );
						} // if(OldQuota != NewQuota)
					}
				} // foreach( DataRow OCRow in OCTable.Rows )
				OCUpdate.update( OCTable );

				return true;
			} // if( CanEditQuotas )
			else
			{
				return false;
			}
		} // SaveQuotas()


    } // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

