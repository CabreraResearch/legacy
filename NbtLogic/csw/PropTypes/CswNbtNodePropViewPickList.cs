using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropViewPickList : CswNbtNodeProp
    {
        public static char delimiter = ',';

        public static implicit operator CswNbtNodePropViewPickList( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsViewPickList;
        }

        public CswNbtNodePropViewPickList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _SelectedViewIdsSubField = ( (CswNbtFieldTypeRuleViewPickList) _FieldTypeRule ).SelectedViewIdsSubField;
            _CachedViewNameSubField = ( (CswNbtFieldTypeRuleViewPickList) _FieldTypeRule ).CachedViewNameSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _SelectedViewIdsSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => SelectedViewIds, x => SelectedViewIds.FromString( CswConvert.ToString( x ) ) ) );
            _SubFieldMethods.Add( _CachedViewNameSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => CachedViewNames, x => CachedViewNames.FromString( CswConvert.ToString( x ) ) ) );
        }

        private CswNbtSubField _SelectedViewIdsSubField;
        private CswNbtSubField _CachedViewNameSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == SelectedViewIds.Count );
            }
        }//Empty

        private CswCommaDelimitedString _SelectedViewIds = null;
        /// <summary>
        /// Comma-separated list of Selected ViewIds
        /// </summary>
        public CswCommaDelimitedString SelectedViewIds
        {
            get
            {
                if( _SelectedViewIds == null )
                {
                    _SelectedViewIds = new CswCommaDelimitedString();
                    _SelectedViewIds.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _SelectedViewIds_OnChange );
                    _SelectedViewIds.FromString( GetPropRowValue( _SelectedViewIdsSubField ) );
                }
                return _SelectedViewIds;
            }
            set
            {
                _SelectedViewIds = value;
                _SelectedViewIds.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _SelectedViewIds_OnChange );
                _SelectedViewIds_OnChange();
            }
        }

        // This event handler allows us to save changes made directly to _SelectedNodeTypeIds (like .Add() )
        void _SelectedViewIds_OnChange()
        {
            if( SetPropRowValue( _SelectedViewIdsSubField, _SelectedViewIds.ToString() ) )
                PendingUpdate = true;
        }

        /// <summary>
        /// True if the SelectedViewIds contains the given ViewId
        /// </summary>
        public bool ContainsViewId( CswNbtViewId ViewIdToFind )
        {
            return SelectedViewIds.Contains( ViewIdToFind.get().ToString() );
        }

        /// <summary>
        /// Removes a ViewId from the SelectedViewIds
        /// </summary>
        public void RemoveViewId( CswNbtViewId ViewIdToRemove )
        {
            SelectedViewIds.Remove( ViewIdToRemove.get().ToString() );
        }

        /// <summary>
        /// Mode of operation for this property
        /// </summary>
        public CswEnumNbtPropertySelectMode SelectMode
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.Multi;
            }
        }

        private CswCommaDelimitedString _CachedViewNames = null;
        public CswCommaDelimitedString CachedViewNames
        {
            get
            {
                if( _CachedViewNames == null )
                {
                    _CachedViewNames = new CswCommaDelimitedString();
                    _CachedViewNames.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _CachedViewNames_OnChange );
                    _CachedViewNames.FromString( GetPropRowValue( _CachedViewNameSubField ) );
                }
                return _CachedViewNames;
            }
            set
            {
                _CachedViewNames = value;
                _CachedViewNames.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _CachedViewNames_OnChange );
                _CachedViewNames_OnChange();
            }
        }

        // This event handler allows us to save changes made directly to _SelectedNodeTypeIds (like .Add() )
        void _CachedViewNames_OnChange()
        {
            SetPropRowValue( _CachedViewNameSubField, _CachedViewNames.ToString() );
        }

        public void RefreshViewName()
        {
            CachedViewNames.Clear();
            if( SelectedViewIds.Count > 0 )
            {
                if( SelectMode != CswEnumNbtPropertySelectMode.Multiple && CswConvert.ToInt32( SelectedViewIds[0] ) > 0 )
                {
                    CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( SelectedViewIds[0] ) ) );
                    if( null != ThisView )
                    {
                        CachedViewNames.Add( ThisView.ViewName );
                    }
                }
                else
                {
                    Collection<Int32> SelectedViewIdCollection = SelectedViewIds.ToIntCollection();
                    foreach( Int32 ViewId in SelectedViewIdCollection )
                    {
                        CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( ViewId ) );
                        if( null != ThisView )
                        {
                            CachedViewNames.Add( ThisView.ViewName );
                        }
                    } // foreach( Int32 ViewId in SelectedViewIdCollection )
                }
            } // if( SelectedViewIds.Count > 0 )

            this.PendingUpdate = false;
        } // RefreshViewName()

        private ICswNbtUser _User = null;
        /// <summary>
        /// Sets the user to which to orient the views to select
        /// </summary>
        public ICswNbtUser User
        {
            get
            {
                if( _User == null )
                    _User = _CswNbtResources.CurrentNbtUser;
                return _User;
            }
            set { _User = value; }
        }

        private Dictionary<CswNbtViewId, CswNbtView> _Views = null;
        /// <summary>
        /// Collection of Views to select
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> Views
        {
            get
            {
                if( _Views == null )
                {
                    if( NodeId != null )
                    {
                        // Use the User's visible views
                        _Views = _CswNbtResources.ViewSelect.getVisibleViews( User, false );
                    }
                    else
                    {
                        // We're creating a new user, don't pick a default view
                        _Views = new Dictionary<CswNbtViewId, CswNbtView>();
                    }
                }
                return _Views;
            }
        }

        /// <summary>
        /// Collection of Views to select
        /// </summary>
        public Dictionary<CswNbtViewId, CswNbtView> SelectedViews
        {
            get
            {
                Dictionary<CswNbtViewId, CswNbtView> _SelectedViews = new Dictionary<CswNbtViewId, CswNbtView>();
                if( NodeId != null )
                {
                    // Use the User's visible, quicklaunch views
                    CswCommaDelimitedString ViewIds = new CswCommaDelimitedString( SelectedViewIds.Count, "'" );
                    ViewIds.FromDelimitedString( SelectedViewIds );
                    _SelectedViews = _CswNbtResources.ViewSelect.getVisibleViews( User, false, ViewIds );
                }
                return _SelectedViews;
            }
        }

        public const string NameColumn = "label";
        public const string KeyColumn = "key";
        public const string ValueColumn = "value";
        public const string ElemName_Options = "options";

        public DataTable ViewsForCBA()
        {
            DataTable _ViewsForCBA = new CswDataTable( "viewpicklistdatatable", "" );
            _ViewsForCBA.Columns.Add( KeyColumn, typeof( Int32 ) );
            _ViewsForCBA.Columns.Add( NameColumn, typeof( string ) );
            _ViewsForCBA.Columns.Add( ValueColumn, typeof( bool ) );

            foreach( CswNbtView ThisView in Views.Values )
            {
                DataRow NewViewRow = _ViewsForCBA.NewRow();
                NewViewRow[NameColumn] = ThisView.ViewName;
                NewViewRow[KeyColumn] = ThisView.ViewId.get();
                NewViewRow[ValueColumn] = ( ( SelectedViewIds.Contains( ThisView.ViewId.get() ) ) ||
                                          ( ( Views.Values.First() == ThisView ) && Required && SelectedViewIds.Count == 0 ) );
                _ViewsForCBA.Rows.Add( NewViewRow );
            }
            return _ViewsForCBA;
        } // ViewsForCBA

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_SelectedViewIdsSubField.ToXmlNodeName()] = SelectedViewIds.ToString();
            ParentObject["selectmode"] = SelectMode.ToString();
            ParentObject[_CachedViewNameSubField.ToXmlNodeName()] = CachedViewNames.ToString();
            ParentObject[ElemName_Options] = new JObject();

            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();
            CBAOptions.Columns.Add( "Include" );

            DataTable ViewsTable = ViewsForCBA();
            foreach( DataRow ViewRow in ViewsTable.Rows )
            {
                CswCheckBoxArrayOptions.Option Option = new CswCheckBoxArrayOptions.Option();
                Option.Key = ViewRow[KeyColumn].ToString();
                Option.Label = ViewRow[NameColumn].ToString();
                Option.Values.Add( CswConvert.ToBoolean( ViewRow[ValueColumn] ) );
                CBAOptions.Options.Add( Option );
            }
            CBAOptions.ToJSON( (JObject) ParentObject[ElemName_Options] );
        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            SelectedViewIds.FromString( CswTools.XmlRealAttributeName( PropRow[_SelectedViewIdsSubField.ToXmlNodeName()].ToString() ) );
            PendingUpdate = true;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswCommaDelimitedString NewSelectedViewIds = new CswCommaDelimitedString();

            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();
            if( null != JObject[ElemName_Options] )
            {
                CBAOptions.ReadJson( (JObject) JObject[ElemName_Options] );
            }
            foreach( CswCheckBoxArrayOptions.Option Option in CBAOptions.Options )
            {
                if( Option.Values.Count > 0 && true == Option.Values[0] )
                {
                    NewSelectedViewIds.Add( Option.Key );
                }
            }
            SelectedViewIds = NewSelectedViewIds;
            RefreshViewName();
        } // ReadJSON()

        public override void SyncGestalt()
        {

        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes