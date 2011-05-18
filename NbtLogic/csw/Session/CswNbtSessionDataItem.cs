using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt
{
	public class CswNbtSessionDataItem
	{
		/// <summary>
		/// Types of Session Links
		/// </summary>
		public enum SessionDataType
		{
			/// <summary>
			/// Link to a View
			/// </summary>
			View,
			/// <summary>
			/// Link to an Action
			/// </summary>
			Action,
			/// <summary>
			/// Undefined
			/// </summary>
			Unknown
		}
		
		private CswNbtResources _CswNbtResources;
		private DataRow _SessionDataRow;

		public CswNbtSessionDataItem( CswNbtResources Resources, DataRow SessionDataItemRow )
		{
			_CswNbtResources = Resources;
			_SessionDataRow = SessionDataItemRow;
		} // constructor

		public SessionDataType DataType
		{
			get { 
				SessionDataType ret = SessionDataType.Unknown;
				Enum.TryParse( _SessionDataRow["sessiondatatype"].ToString(), out ret);
				return ret;
			}
		}

		public CswNbtSessionDataId DataId
		{
			get
			{
				CswNbtSessionDataId ret = new CswNbtSessionDataId( CswConvert.ToInt32( _SessionDataRow["sessiondataid"] ) );
				return ret;
			}
		}

		public string Name
		{
			get
			{
				return _SessionDataRow["name"].ToString();
			}
		}

		public CswNbtView View
		{
			get
			{
				CswNbtView View = new CswNbtView( _CswNbtResources );
				if( DataType == SessionDataType.View )
				{
					View.LoadXml( _SessionDataRow["viewxml"].ToString() );
				}
				return View;
			}
		}

		public Int32 ActionId
		{
			get
			{
				return CswConvert.ToInt32( _SessionDataRow["actionid"] );
			}
		}

		public CswNbtAction Action 
		{
			get 
			{
				CswNbtAction ret = null;
				if(DataType == SessionDataType.Action)
				{
					ret = _CswNbtResources.Actions[ActionId];
				}
				return ret;
			}
		}


	} // CswNbtSessionDataItem
} // namespace ChemSW.Nbt
