using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
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

        public CswNbtNodePropViewPickList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            //if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.ViewPickList )
            //{
            //    throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
            //                                "CswNbtNodePropViewPickList() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            //}
            _FieldTypeRule = (CswNbtFieldTypeRuleViewPickList) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _SelectedViewIdsSubField = _FieldTypeRule.SelectedViewIdsSubField;
            _CachedViewNameSubField = _FieldTypeRule.CachedViewNameSubField;

        }//generic
        private CswNbtFieldTypeRuleViewPickList _FieldTypeRule;
        private CswNbtSubField _SelectedViewIdsSubField;
        private CswNbtSubField _CachedViewNameSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == SelectedViewIds.Count );
            }
        }//Empty


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }
        }//Gestalt


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
                    _SelectedViewIds.FromString( _CswNbtNodePropData.GetPropRowValue( _SelectedViewIdsSubField.Column ) );
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
            if( _CswNbtNodePropData.SetPropRowValue( _SelectedViewIdsSubField.Column, _SelectedViewIds.ToString() ) )
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
        public PropertySelectMode SelectMode
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
                    _CachedViewNames.FromString( _CswNbtNodePropData.GetPropRowValue( _CachedViewNameSubField.Column ) );
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
            _CswNbtNodePropData.SetPropRowValue( _CachedViewNameSubField.Column, _CachedViewNames.ToString() );
        }

        public void RefreshViewName()
        {
            //bz # 8758
            CachedViewNames.Clear();
            if( SelectedViewIds.Count > 0 )
            {
                if( SelectMode != PropertySelectMode.Multiple && CswConvert.ToInt32( SelectedViewIds[0] ) > 0 )
                {
                    //    DataTable ViewTable = _CswNbtResources.ViewSelect.getView( CswConvert.ToInt32( SelectedViewIds[0] ) );
                    //    if( ViewTable != null && ViewTable.Rows.Count > 0 )
                    //        CachedViewNames.Add( ViewTable.Rows[0]["viewname"].ToString() );
                    CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( SelectedViewIds[0] ) ) );
                    CachedViewNames.Add( ThisView.ViewName );
                }
                else
                {
                    Collection<Int32> SelectedViewIdCollection = SelectedViewIds.ToIntCollection();
                    foreach( Int32 ViewId in SelectedViewIdCollection )
                    {
                        //DataTable ViewTable = _CswNbtResources.ViewSelect.getView( CswConvert.ToInt32( SelectedViewIds[0] ) );
                        //if( ViewTable != null && ViewTable.Rows.Count > 0 )
                        //    CachedViewNames.Add( ViewTable.Rows[0]["viewname"].ToString() );
                        CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( SelectedViewIds[0] ) ) );
                        CachedViewNames.Add( ThisView.ViewName );
                    } // foreach( Int32 ViewId in SelectedViewIdCollection )

                } // if-else( SelectMode != PropertySelectMode.Multiple && CswConvert.ToInt32( SelectedViewIds[0] ) > 0 )
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
                        // else 
                        // Creating a new user, don't pick a default view (BZ 7055)
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
                    CswCommaDelimitedString ViewIds = new CswCommaDelimitedString( SelectedViewIds.Count, true );
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

            //if( SelectMode != PropertySelectMode.Multiple && !Required )
            //{
            //    DataRow NoneRow = _ViewsForCBA.NewRow();
            //    NoneRow[NameColumn] = "[none]";
            //    NoneRow[KeyColumn] = CswConvert.ToDbVal( Int32.MinValue );
            //    NoneRow[ValueColumn] = ( SelectedViewIds.Count == 0 );
            //    _ViewsForCBA.Rows.Add( NoneRow );
            //}

            foreach( CswNbtView ThisView in Views.Values )
            {
                DataRow NewViewRow = _ViewsForCBA.NewRow();
                NewViewRow[NameColumn] = ThisView.ViewName;
                NewViewRow[KeyColumn] = ThisView.ViewId.get();
                //NewViewRow[ValueColumn] = ( searchstr.IndexOf( CswNbtNodePropViewPickList.delimiter.ToString() + ViewRow["nodeviewid"].ToString() + CswNbtNodePropViewPickList.delimiter.ToString() ) >= 0 );
                NewViewRow[ValueColumn] = ( ( SelectedViewIds.Contains( ThisView.ViewId.get() ) ) ||
                                          ( ( Views.Values.First() == ThisView ) && Required && SelectedViewIds.Count == 0 ) );
                _ViewsForCBA.Rows.Add( NewViewRow );
            }
            return _ViewsForCBA;
        } // ViewsForCBA

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

        }

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

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
