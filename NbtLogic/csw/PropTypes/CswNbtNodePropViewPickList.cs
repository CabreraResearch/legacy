using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropViewPickList : CswNbtNodeProp
    {
        public static char delimiter = ',';

        public CswNbtNodePropViewPickList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.ViewPickList )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropViewPickList() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _SelectedViewIdsSubField = ( (CswNbtFieldTypeRuleViewPickList) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SelectedViewIdsSubField;
            _CachedViewNameSubField = ( (CswNbtFieldTypeRuleViewPickList) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CachedViewNameSubField;

        }//generic

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
        public bool ContainsViewId( Int32 ViewIdToFind )
        {
            return ( ( "," + SelectedViewIds + "," ).Contains( "," + ViewIdToFind + "," ) );
        }

        /// <summary>
        /// Removes a ViewId from the SelectedViewIds
        /// </summary>
        public void RemoveViewId( Int32 ViewIdToRemove )
        {
            SelectedViewIds.Remove( ViewIdToRemove.ToString() );
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
                    DataTable ViewTable = _CswNbtResources.ViewSelect.getView( CswConvert.ToInt32( SelectedViewIds[0] ) );
                    if( ViewTable != null && ViewTable.Rows.Count > 0 )
                        CachedViewNames.Add( ViewTable.Rows[0]["viewname"].ToString() );
                }
                else
                {
                    Collection<Int32> SelectedViewIdCollection = SelectedViewIds.ToIntCollection();
                    foreach( Int32 ViewId in SelectedViewIdCollection )
                    {
                        DataTable ViewTable = _CswNbtResources.ViewSelect.getView( CswConvert.ToInt32( SelectedViewIds[0] ) );
                        if( ViewTable != null && ViewTable.Rows.Count > 0 )
                            CachedViewNames.Add( ViewTable.Rows[0]["viewname"].ToString() );
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

        /// <summary>
        /// Collection of Views to select
        /// </summary>
        public Collection<CswNbtView> Views
        {
            get
            {
                Collection<CswNbtView> Views = new Collection<CswNbtView>();
                //CswStaticSelect ViewsSelect = _CswNbtResources.makeCswStaticSelect( "ViewsSelect", "getVisibleViewInfo" );
                //ViewsSelect.S4Parameters.Add( "orderbyclause", "lower(v.viewname)" );
                if( NodeId != null )
                {
                    // Use the User's visible views
                    Views = _CswNbtResources.ViewSelect.getVisibleViews( User, false );
                }
                // else // Creating a new user, don't pick a default view (BZ 7055)

                return Views;
            }
        }

        public const string NameColumn = "View Name";
        public const string KeyColumn = "nodeviewid";
        public const string ValueColumn = "Include";

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

            string searchstr = CswNbtNodePropViewPickList.delimiter.ToString() + SelectedViewIds + CswNbtNodePropViewPickList.delimiter.ToString();
            bool first = true;
            foreach( CswNbtView View in Views )
            {
                DataRow NewViewRow = _ViewsForCBA.NewRow();
                NewViewRow[NameColumn] = View.ViewName;
                NewViewRow[KeyColumn] = View.ViewId;
                //NewViewRow[ValueColumn] = ( searchstr.IndexOf( CswNbtNodePropViewPickList.delimiter.ToString() + ViewRow["nodeviewid"].ToString() + CswNbtNodePropViewPickList.delimiter.ToString() ) >= 0 );
                NewViewRow[ValueColumn] = ( ( searchstr.IndexOf( CswNbtNodePropNodeTypeSelect.delimiter.ToString() + View.ViewId + CswNbtNodePropNodeTypeSelect.delimiter.ToString() ) >= 0 ) ||
                                          ( first && Required && SelectedViewIds.Count == 0 ) );
                first = false;
                _ViewsForCBA.Rows.Add( NewViewRow );
            }
            return _ViewsForCBA;
        } // ViewsForCBA

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SelectedViewIdsNode = CswXmlDocument.AppendXmlNode( ParentNode, _SelectedViewIdsSubField.ToXmlNodeName(), SelectedViewIds.ToString() );
            CswXmlDocument.AppendXmlAttribute( SelectedViewIdsNode, "SelectMode", SelectMode.ToString() );
            XmlNode CachedViewNameNode = CswXmlDocument.AppendXmlNode( ParentNode, _CachedViewNameSubField.ToXmlNodeName(), CachedViewNames.ToString() );
            XmlNode OptionsNode = CswXmlDocument.AppendXmlNode( ParentNode, "options" );
            DataTable ViewsTable = ViewsForCBA();
            foreach( DataRow ViewRow in ViewsTable.Rows )
            {
                XmlNode ViewNode = CswXmlDocument.AppendXmlNode( OptionsNode, "user" );

                XmlNode NameNode = CswXmlDocument.AppendXmlNode( ViewNode, "column" );
                CswXmlDocument.AppendXmlAttribute( NameNode, "field", NameColumn );
                CswXmlDocument.AppendXmlAttribute( NameNode, "value", ViewRow[NameColumn].ToString() );

                XmlNode KeyNode = CswXmlDocument.AppendXmlNode( ViewNode, "column" );
                CswXmlDocument.AppendXmlAttribute( KeyNode, "field", KeyColumn );
                CswXmlDocument.AppendXmlAttribute( KeyNode, "value", ViewRow[KeyColumn].ToString() );

                XmlNode IncludeNode = CswXmlDocument.AppendXmlNode( ViewNode, "column" );
                CswXmlDocument.AppendXmlAttribute( IncludeNode, "field", ValueColumn );
                CswXmlDocument.AppendXmlAttribute( IncludeNode, "value", ViewRow[ValueColumn].ToString() );
            }
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //SelectedViewIds.FromString( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SelectedViewIdsSubField.ToXmlNodeName() ) );
            //PendingUpdate = true;

            CswCommaDelimitedString NewSelectedViewIds = new CswCommaDelimitedString();

            foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, "options" ).ChildNodes )
            {
                string key = string.Empty;
                string name = string.Empty;
                bool value = false;
                foreach( XmlNode ColumnNode in ItemNode.ChildNodes )
                {
                    if( KeyColumn == ColumnNode.Attributes["field"].Value )
                        key = ColumnNode.Attributes["value"].Value;
                    if( NameColumn == ColumnNode.Attributes["field"].Value )
                        name = ColumnNode.Attributes["value"].Value;
                    if( ValueColumn == ColumnNode.Attributes["field"].Value )
                        value = CswConvert.ToBoolean( ColumnNode.Attributes["value"].Value );
                }
                if( value )
                {
                    NewSelectedViewIds.Add( key );
                }
            } // foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, "Options" ).ChildNodes )

            SelectedViewIds = NewSelectedViewIds;
            RefreshViewName();
        }

        public override void ToXElement( XElement ParentNode )
        {
            throw new NotImplementedException();
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            throw new NotImplementedException();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            SelectedViewIds.FromString( CswTools.XmlRealAttributeName( PropRow[_SelectedViewIdsSubField.ToXmlNodeName()].ToString() ) );
            PendingUpdate = true;
        }



    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
