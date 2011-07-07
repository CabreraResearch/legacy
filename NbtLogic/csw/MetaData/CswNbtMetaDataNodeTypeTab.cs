using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtMetaDataNodeTypeTab : ICswNbtMetaDataObject, IEquatable<CswNbtMetaDataNodeTypeTab>
    {
        private CswNbtMetaDataResources _CswNbtMetaDataResources;
        private DataRow _NodeTypeTabRow;

        public CswNbtMetaDataNodeTypeTab( CswNbtMetaDataResources CswNbtMetaDataResources, DataRow Row )
        {
            _CswNbtMetaDataResources = CswNbtMetaDataResources;
            Reassign( Row );
        }

        public DataRow _DataRow
        {
            get { return _NodeTypeTabRow; }
            //set { _NodeTypeTabRow = value; }
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
            //set { _UniqueId = value; }
        }

        public string UniqueIdFieldName { get { return "nodetypetabsetid"; } }

        public void Reassign( DataRow NewRow )
        {
            _NodeTypeTabRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }


        public ICollection NodeTypePropIds { get { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropIdsByTab( TabId ); } }
        public ICollection NodeTypeProps { get { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropsByTab( TabId ); } }
        public ICollection NodeTypePropsByDisplayOrder { get { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropsByDisplayOrder( TabId ); } }

        public CswNbtMetaDataNodeTypeProp FirstPropByDisplayOrder()
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            ICollection Props = NodeTypePropsByDisplayOrder;
            // Weird, I know.
            foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
            {
                ret = Prop;
                break;
            }
            return ret;
        }

        public CswNbtMetaDataNodeTypeProp getNextPropByDisplayOrder( CswNbtMetaDataNodeTypeProp PreviousProp )
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            ICollection Props = NodeTypePropsByDisplayOrder;
            bool GetNext = false;
            foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
            {
                if( GetNext )
                {
                    ret = Prop;
                    break;
                }
                if( Prop == PreviousProp )
                    GetNext = true;
            }
            return ret;
        }
        public CswNbtMetaDataNodeTypeProp getPreviousPropByDisplayOrder( CswNbtMetaDataNodeTypeProp NextProp )
        {
            CswNbtMetaDataNodeTypeProp ret = null;
            ICollection Props = NodeTypePropsByDisplayOrder;
            bool GetNext = false;
            foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
            {
                if( Prop == NextProp )
                    break;
                ret = Prop;
            }
            return ret;
        }

        public Int32 TabId
        {
            get { return CswConvert.ToInt32( _NodeTypeTabRow[ "nodetypetabsetid" ].ToString() ); }
        }
        public string TabName
        {
            get { return _NodeTypeTabRow[ "tabname" ].ToString(); }
            set
            {
                if( value == string.Empty )
					throw new CswDniException( ErrorType.Warning, "Tab name cannot be empty", "User attempted to save a null tabname for tabid " + TabId );

                if( _NodeTypeTabRow["tabname"].ToString() != value )
                {
                    _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this.NodeType );

                    _CswNbtMetaDataResources.NodeTypeTabsCollection.Deregister( this );
                    _NodeTypeTabRow["tabname"] = value;
                    _CswNbtMetaDataResources.NodeTypeTabsCollection.RegisterExisting( this );
                }
            }
        }
        public Int32 TabOrder
        {
            get { return CswConvert.ToInt32( _NodeTypeTabRow[ "taborder" ] ); }
            set
            {
                if( CswConvert.ToInt32( _NodeTypeTabRow["taborder"] ) != value )
                {
                    _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this.NodeType );

                    _CswNbtMetaDataResources.NodeTypeTabsCollection.Deregister( this );
                    _NodeTypeTabRow["taborder"] = CswConvert.ToDbVal( value );
                    _CswNbtMetaDataResources.NodeTypeTabsCollection.RegisterExisting( this );
                }
            }
        }

        /// <summary>
        /// Whether to include properties on this tab in the node report.
        /// This does not trigger versioning on purpose, see BZ 7936.
        /// </summary>
        public bool IncludeInNodeReport
        {
            get { return CswConvert.ToBoolean( _NodeTypeTabRow[ "includeinnodereport" ] ); }
            set { _NodeTypeTabRow[ "includeinnodereport" ] = CswConvert.ToDbVal( value ); }
        }

        // For Inspection Design
        public Int32 SectionNo
        {
            get { return TabOrder; }
            private set { TabOrder = value; }
        }

        public CswNbtMetaDataNodeType NodeType
        {
            get { return _CswNbtMetaDataResources.CswNbtMetaData.getNodeType( CswConvert.ToInt32( _NodeTypeTabRow[ "nodetypeid" ].ToString() ) ); }
        }


        public bool HasConditionalProps
        {
            get
            {
                bool ret = false;
                foreach( CswNbtMetaDataNodeTypeProp Prop in this.NodeTypeProps )
                    ret = ret || Prop.hasFilter();
                return ret;
            }
        }

        public void CopyTabToNewNodeTypeTabRow( DataRow NewTabRow )
        {
            foreach ( DataColumn TabColumn in NewTabRow.Table.Columns )
            {
                if ( _NodeTypeTabRow[ TabColumn.ColumnName ].ToString() != String.Empty )
                {
                    if ( TabColumn.ColumnName.ToLower() != "nodetypeid" &&
                         TabColumn.ColumnName.ToLower() != "nodetypetabsetid" )
                    {
                        NewTabRow[ TabColumn.ColumnName ] = _NodeTypeTabRow[ TabColumn.ColumnName ].ToString();
                    }
                }
            }
        }

        public Int32 GetPropDisplayOrder( CswNbtMetaDataNodeTypeProp Prop )
        {
            return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropDisplayOrder( TabId, Prop );
        }

        public Int32 getCurrentMaxDisplayRow()
        {
            Int32 Max = 0;
            foreach( CswNbtMetaDataNodeTypeProp Prop in NodeTypeProps )
            {
                if( Prop.DisplayRow > Max )
                    Max = Prop.DisplayRow;
            }
            return Max;
        }

        public static string _Element_MetaDataNodeTypeTab = "MetaDataNodeTypeTab";
        public static string _Attribute_TabId = "tabid";
        public static string _Attribute_TabName = "tabname";
        public static string _Attribute_Order = "order";

        public XmlNode ToXml( CswNbtView View, XmlDocument XmlDoc, bool ForMobile, bool PropsInViewOnly )
        {
            XmlNode TabNode = XmlDoc.CreateNode( XmlNodeType.Element, _Element_MetaDataNodeTypeTab, "" );

            XmlAttribute TabIdAttr = XmlDoc.CreateAttribute( _Attribute_TabId );
            TabIdAttr.Value = TabId.ToString();
            TabNode.Attributes.Append( TabIdAttr );

            XmlAttribute TabNameAttr = XmlDoc.CreateAttribute( _Attribute_TabName );
            TabNameAttr.Value = TabName;
            TabNode.Attributes.Append( TabNameAttr );

            XmlAttribute TabOrderAttr = XmlDoc.CreateAttribute( _Attribute_Order );
            TabOrderAttr.Value = TabOrder.ToString();
            TabNode.Attributes.Append( TabOrderAttr );

            bool bAtLeastOneProp = false;
            foreach ( CswNbtMetaDataNodeTypeProp Prop in this.NodeTypeProps )
            {
                if( View == null || !PropsInViewOnly || View.ContainsNodeTypeProp( Prop ) )
                {
                    bAtLeastOneProp = true;
                    TabNode.AppendChild( Prop.ToXml( XmlDoc, ForMobile ) );
                }
            }

            if ( bAtLeastOneProp )
                return TabNode;
            else
                return null;
        }

        #region IEquatable

        public static bool operator ==( CswNbtMetaDataNodeTypeTab tab1, CswNbtMetaDataNodeTypeTab tab2 )
        {
            // If both are null, or both are same instance, return true.
            if ( System.Object.ReferenceEquals( tab1, tab2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ( ( ( object )tab1 == null ) || ( ( object )tab2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if ( tab1.UniqueId == tab2.UniqueId )
                return true;
            else
                return false;
        }

        public static bool operator !=( CswNbtMetaDataNodeTypeTab tab1, CswNbtMetaDataNodeTypeTab tab2 )
        {
            return !( tab1 == tab2 );
        }

        public override bool Equals( object obj )
        {
            if ( !( obj is CswNbtMetaDataNodeTypeTab ) )
                return false;
            return this == ( CswNbtMetaDataNodeTypeTab )obj;
        }

        public bool Equals( CswNbtMetaDataNodeTypeTab obj )
        {
            return this == ( CswNbtMetaDataNodeTypeTab )obj;
        }

        public override int GetHashCode()
        {
            return this.TabId;
        }

        #endregion IEquatable
    }
}
