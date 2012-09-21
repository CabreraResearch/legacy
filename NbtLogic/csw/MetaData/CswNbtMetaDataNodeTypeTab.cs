using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        private Int32 _UniqueId;
        public Int32 UniqueId
        {
            get { return _UniqueId; }
        }

        public string UniqueIdFieldName { get { return "nodetypetabsetid"; } }

        public void Reassign( DataRow NewRow )
        {
            _NodeTypeTabRow = NewRow;
            _UniqueId = CswConvert.ToInt32( NewRow[UniqueIdFieldName] );
        }


        public Collection<Int32> getNodeTypePropIds() { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropIdsByTab( TabId ); }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypeProps() { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropsByTab( TabId ); }
        public IEnumerable<CswNbtMetaDataNodeTypeProp> getNodeTypePropsByDisplayOrder() { return _CswNbtMetaDataResources.NodeTypePropsCollection.getNodeTypePropsByDisplayOrder( NodeTypeId, TabId ); }

        public Int32 TabId
        {
            get { return CswConvert.ToInt32( _NodeTypeTabRow["nodetypetabsetid"].ToString() ); }
        }
        public Int32 FirstTabVersionId
        {
            get
            {
                Int32 ret = CswConvert.ToInt32( _NodeTypeTabRow["firsttabversionid"].ToString() );
                if( ret == Int32.MinValue )
                    ret = TabId;
                return ret;
            }
        }
        public Int32 PriorTabVersionId
        {
            get { return CswConvert.ToInt32( _NodeTypeTabRow["priortabversionid"].ToString() ); }
        }
        public string TabName
        {
            get { return _NodeTypeTabRow["tabname"].ToString(); }
            set
            {
                if( value == string.Empty )
                    throw new CswDniException( ErrorType.Warning, "Tab name cannot be empty", "User attempted to save a null tabname for tabid " + TabId );

                if( _NodeTypeTabRow["tabname"].ToString() != value )
                {
                    _checkVersioningTab();

                    _NodeTypeTabRow["tabname"] = value;
                    _CswNbtMetaDataResources.NodeTypeTabsCollection.clearCache();
                }
            }
        }

        private void _checkVersioningTab()
        {
            CswNbtMetaDataNodeType NewNodeType = _CswNbtMetaDataResources.CswNbtMetaData.CheckVersioning( this.getNodeType() );
            if( NewNodeType.NodeTypeId != NodeTypeId )
            {
                // Get the new tab and reassign myself
                CswNbtMetaDataNodeTypeTab NewTab = _CswNbtMetaDataResources.CswNbtMetaData.getNodeTypeTabVersion( NewNodeType.NodeTypeId, this.TabId );
                this._NodeTypeTabRow = NewTab._DataRow;
            }
        }


        public Int32 TabOrder
        {
            get { return CswConvert.ToInt32( _NodeTypeTabRow["taborder"] ); }
            set
            {
                if( CswConvert.ToInt32( _NodeTypeTabRow["taborder"] ) != value )
                {
                    _checkVersioningTab();

                    _NodeTypeTabRow["taborder"] = CswConvert.ToDbVal( value );
                    _CswNbtMetaDataResources.NodeTypeTabsCollection.clearCache();
                }
            }
        }

        /// <summary>
        /// Whether to include properties on this tab in the node report.
        /// This does not trigger versioning on purpose, see BZ 7936.
        /// </summary>
        public bool IncludeInNodeReport
        {
            get { return CswConvert.ToBoolean( _NodeTypeTabRow["includeinnodereport"] ); }
            set { _NodeTypeTabRow["includeinnodereport"] = CswConvert.ToDbVal( value ); }
        }

        // For Inspection Design
        public Int32 SectionNo
        {
            get { return TabOrder; }
            private set { TabOrder = value; }
        }

        public Int32 NodeTypeId
        {
            get { return CswConvert.ToInt32( _NodeTypeTabRow["nodetypeid"] ); }
        }
        public CswNbtMetaDataNodeType getNodeType()
        {
            return _CswNbtMetaDataResources.CswNbtMetaData.getNodeType( NodeTypeId );
        }


        public bool HasConditionalProps
        {
            get
            {
                bool ret = false;
                foreach( CswNbtMetaDataNodeTypeProp Prop in this.getNodeTypeProps() )
                    ret = ret || Prop.hasFilter();
                return ret;
            }
        }

        public void CopyTabToNewNodeTypeTabRow( DataRow NewTabRow )
        {
            foreach( DataColumn TabColumn in NewTabRow.Table.Columns )
            {
                if( _NodeTypeTabRow[TabColumn.ColumnName].ToString() != String.Empty )
                {
                    if( TabColumn.ColumnName.ToLower() != "nodetypeid" &&
                         TabColumn.ColumnName.ToLower() != "nodetypetabsetid" )
                    {
                        NewTabRow[TabColumn.ColumnName] = _NodeTypeTabRow[TabColumn.ColumnName].ToString();
                    }
                }
            }
        }

        public const string _Element_MetaDataNodeTypeTab = "MetaDataNodeTypeTab";
        public const string _Attribute_TabId = "tabid";
        public const string _Attribute_TabName = "tabname";
        public const string _Attribute_Order = "order";

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
            foreach( CswNbtMetaDataNodeTypeProp Prop in this.getNodeTypeProps() )
            {
                if( View == null || !PropsInViewOnly || View.ContainsNodeTypeProp( Prop ) )
                {
                    bAtLeastOneProp = true;
                    TabNode.AppendChild( Prop.ToXml( XmlDoc, ForMobile ) );
                }
            }

            if( bAtLeastOneProp )
                return TabNode;
            else
                return null;
        }

        #region IEquatable

        public static bool operator ==( CswNbtMetaDataNodeTypeTab tab1, CswNbtMetaDataNodeTypeTab tab2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( tab1, tab2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) tab1 == null ) || ( (object) tab2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( tab1.UniqueId == tab2.UniqueId )
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
            if( !( obj is CswNbtMetaDataNodeTypeTab ) )
                return false;
            return this == (CswNbtMetaDataNodeTypeTab) obj;
        }

        public bool Equals( CswNbtMetaDataNodeTypeTab obj )
        {
            return this == (CswNbtMetaDataNodeTypeTab) obj;
        }

        public override int GetHashCode()
        {
            return this.TabId;
        }

        #endregion IEquatable
    }
}
