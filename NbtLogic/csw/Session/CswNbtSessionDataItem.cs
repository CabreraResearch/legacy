using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Search;

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
            /// Link to a Search
            /// </summary>
            Search,
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
            get
            {
                SessionDataType ret = SessionDataType.Unknown;
                Enum.TryParse( _SessionDataRow[CswNbtSessionDataMgr.SessionDataColumn_SessionDataType].ToString(), out ret );
                return ret;
            }
        }

        public CswNbtSessionDataId DataId
        {
            get
            {
                return new CswNbtSessionDataId( CswConvert.ToInt32( _SessionDataRow[CswNbtSessionDataMgr.SessionDataColumn_PrimaryKey] ) );
            }
        }

        public string Name
        {
            get
            {
                return _SessionDataRow[CswNbtSessionDataMgr.SessionDataColumn_Name].ToString();
            }
        }

        public CswNbtView View
        {
            get
            {
                CswNbtView View = new CswNbtView( _CswNbtResources );
                if( DataType == SessionDataType.View )
                {
                    View.LoadXml( _SessionDataRow[CswNbtSessionDataMgr.SessionDataColumn_ViewXml].ToString() );
                    View.ViewId = new CswNbtViewId( CswConvert.ToInt32( _SessionDataRow[CswNbtSessionDataMgr.SessionDataColumn_ViewId] ) );
                    View.SessionViewId = DataId;
                }
                return View;
            }
        }

        public Int32 ActionId
        {
            get
            {
                return CswConvert.ToInt32( _SessionDataRow[CswNbtSessionDataMgr.SessionDataColumn_ActionId] );
            }
        }

        public CswNbtAction Action
        {
            get
            {
                CswNbtAction ret = null;
                if( DataType == SessionDataType.Action )
                {
                    ret = _CswNbtResources.Actions[ActionId];
                }
                return ret;
            }
        }

        public CswNbtSearch Search
        {
            get
            {
                CswNbtSearch ret = null;
                if( DataType == SessionDataType.Search )
                {
                    ret = new CswNbtSearch( _CswNbtResources );
                    ret.FromSessionData( _SessionDataRow );
                }
                return ret;
            }
        }

    } // CswNbtSessionDataItem
} // namespace ChemSW.Nbt
