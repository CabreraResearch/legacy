using System;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    [Serializable()]
    public class CswNbtViewPropertyFilter : CswNbtViewNode
    {
        public override NbtViewNodeType ViewNodeType { get { return NbtViewNodeType.CswNbtViewPropertyFilter; } }

        /// <summary>
        /// For creating a property filter
        /// </summary>
        public CswNbtViewPropertyFilter( CswNbtResources CswNbtResources, CswNbtView View,
                                         CswNbtSubField.SubFieldName inSubFieldName,
                                         CswNbtPropFilterSql.PropertyFilterMode inFilterMode,
                                         string inValue,
                                         bool inCaseSensitive )
            : base( CswNbtResources, View )
        {
            SubfieldName = inSubFieldName;
            FilterMode = inFilterMode;
            Value = inValue;
            CaseSensitive = inCaseSensitive;
        }

        /// <summary>
        /// For loading from a string (created by ToString())
        /// </summary>
        public CswNbtViewPropertyFilter( CswNbtResources CswNbtResources, CswNbtView View, CswDelimitedString FilterString )
            : base( CswNbtResources, View )
        {
            if( FilterString[0] == NbtViewNodeType.CswNbtViewPropertyFilter.ToString() )
            {
                if( FilterString[1] != String.Empty )
                    Conjunction = (CswNbtPropFilterSql.PropertyFilterConjunction) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterConjunction ), FilterString[1].ToString(), true );
                if( FilterString[2] != String.Empty )
                    Value = FilterString[2].ToString();
                if( FilterString[3] != String.Empty )
                    //FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterMode ), FilterString[3].ToString(), true );
                    FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) FilterString[3].ToString();
                if( FilterString[4] != String.Empty )
                    CaseSensitive = Convert.ToBoolean( FilterString[4].ToString() );
                //if( Values[ 5 ] != String.Empty )
                //    ArbitraryId = Values[ 5 ].ToString();
                if( FilterString[6] != String.Empty )
                    //SubfieldName = (CswNbtSubField.SubFieldName) Enum.Parse( typeof( CswNbtSubField.SubFieldName ), FilterString[6].ToString() );
                    SubfieldName = (CswNbtSubField.SubFieldName) FilterString[6].ToString();
                _validate();
            }

        }//ctor

        /// <summary>
        /// For loading from XML
        /// </summary>
        public CswNbtViewPropertyFilter( CswNbtResources CswNbtResources, CswNbtView View, XmlNode FilterNode )
            : base( CswNbtResources, View )
        {
            try
            {
                if( FilterNode.Attributes["value"] != null )
                {
                    Value = FilterNode.Attributes["value"].Value;
                }
                if( FilterNode.Attributes["filtermode"] != null )
                {
                    //FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterMode ), FilterNode.Attributes["filtermode"].Value, true );
                    FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) FilterNode.Attributes["filtermode"].Value;
                }
                if( FilterNode.Attributes["casesensitive"] != null )
                {
                    CaseSensitive = Convert.ToBoolean( FilterNode.Attributes["casesensitive"].Value );
                }
                //if( FilterNode.Attributes[ "arbitraryid" ] != null )
                //    ArbitraryId = FilterNode.Attributes[ "arbitraryid" ].Value;
                if( FilterNode.Attributes["subfieldname"] != null )
                {
                    //SubfieldName = (CswNbtSubField.SubFieldName) Enum.Parse( typeof( CswNbtSubField.SubFieldName ), FilterNode.Attributes["subfieldname"].Value );
                    SubfieldName = (CswNbtSubField.SubFieldName) FilterNode.Attributes["subfieldname"].Value;
                }

                _validate();

            }//try

            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswViewPropertyFilterValue",
                                          "CswViewPropertyFilterValue.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }//catch

        }//ctor

        /// <summary>
        /// For loading from JSON
        /// </summary>
        public CswNbtViewPropertyFilter( CswNbtResources CswNbtResources, CswNbtView View, JObject FilterObj )
            : base( CswNbtResources, View )
        {
            try
            {
                string _Value = CswConvert.ToString( FilterObj["value"] );
                if( !string.IsNullOrEmpty( _Value ) )
                {
                    Value = _Value;
                }

                string _FilterMode = CswConvert.ToString( FilterObj["filtermode"] );
                if( !string.IsNullOrEmpty( _FilterMode ) )
                {
                    //FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) Enum.Parse( typeof( CswNbtPropFilterSql.PropertyFilterMode ), _FilterMode, true );
                    FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) _FilterMode;
                }

                if( FilterObj["casesensitive"] != null )
                {
                    bool _CaseSensitive = CswConvert.ToBoolean( FilterObj["casesensitive"] );
                    CaseSensitive = _CaseSensitive;
                }

                string _SfName = CswConvert.ToString( FilterObj["subfieldname"] );
                if( !string.IsNullOrEmpty( _SfName ) )
                {
                    //SubfieldName = (CswNbtSubField.SubFieldName) Enum.Parse( typeof( CswNbtSubField.SubFieldName ), _SfName );
                    SubfieldName = (CswNbtSubField.SubFieldName) _SfName;
                }

                _validate();

            }//try

            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Misconfigured CswViewPropertyFilterValue",
                                          "CswViewPropertyFilterValue.constructor(xmlnode) encountered an invalid attribute value",
                                          ex );
            }//catch

        }//ctor

        private CswNbtViewProperty _Parent;
        public override CswNbtViewNode Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                if( value == null )
                    _Parent = null;
                else if( value is CswNbtViewProperty )
                    _Parent = (CswNbtViewProperty) value;
                else
                    throw new CswDniException( "Illegal parent assignment on CswNbtViewPropertyFilter" );

                if( SubfieldName == CswNbtSubField.SubFieldName.Unknown )
                {
                    // Set the subfield to be the default subfield for the new parent's field type:
                    if( _Parent.Type == NbtViewPropType.NodeTypePropId )
                        SubfieldName = _Parent.NodeTypeProp.getFieldTypeRule().SubFields.Default.Name;
                    else if( _Parent.Type == NbtViewPropType.ObjectClassPropId )
                        SubfieldName = _Parent.ObjectClassProp.getFieldTypeRule().SubFields.Default.Name;
                }
            }
        }

        //private string _ArbitraryId = "";
        //public override string ArbitraryId
        //{
        //    get { return _ArbitraryId; }
        //    set { _ArbitraryId = value; }
        //}

        public override string ArbitraryId
        {
            get
            {
                string ArbId = string.Empty;
                if( Parent != null )
                    ArbId += Parent.ArbitraryId + "_";
                ArbId += this.SubfieldName.ToString() + "_" + this.FilterMode.ToString() + "_" + this.Value;
                return ArbId;
            }
        }



        public CswNbtPropFilterSql.PropertyFilterConjunction Conjunction = CswNbtPropFilterSql.PropertyFilterConjunction.And;
        public string Value;

        private CswNbtSubField.SubFieldName _SubfieldName = CswNbtSubField.SubFieldName.Unknown;
        public CswNbtSubField.SubFieldName SubfieldName
        {
            set
            {
                _SubfieldName = value;
            }

            get
            {
                return ( _SubfieldName );
            }
        }//

        public CswNbtPropFilterSql.PropertyFilterMode FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Unknown;
        public bool CaseSensitive;

        public override string IconFileName
        {
            get { return "Images/view/filter.gif"; }
        }


        private void _validate()
        {
            if( CswNbtPropFilterSql.PropertyFilterMode.Unknown == FilterMode )
                throw ( new CswDniException( "Illegal filter definition: A filter mode setting is required" ) );


            //if( String.Empty == SubfieldName )
            //    throw ( new CswDniException( "Illegal filter definition: A subfield name is missing" ) );
        }//validate() 

        public XmlNode ToXml( XmlDocument XmlDoc )
        {
            XmlNode PropFilterNode = XmlDoc.CreateNode( XmlNodeType.Element, NbtViewXmlNodeName.Filter.ToString(), "" );

            XmlAttribute FilterValueAttribute = XmlDoc.CreateAttribute( "value" );
            FilterValueAttribute.Value = Value;
            PropFilterNode.Attributes.Append( FilterValueAttribute );

            XmlAttribute FilterModeAttribute = XmlDoc.CreateAttribute( "filtermode" );
            FilterModeAttribute.Value = FilterMode.ToString();
            PropFilterNode.Attributes.Append( FilterModeAttribute );

            XmlAttribute CaseSensitiveAttribute = XmlDoc.CreateAttribute( "casesensitive" );
            CaseSensitiveAttribute.Value = CaseSensitive.ToString();
            PropFilterNode.Attributes.Append( CaseSensitiveAttribute );

            XmlAttribute ArbitraryIdAttribute = XmlDoc.CreateAttribute( "arbitraryid" );
            ArbitraryIdAttribute.Value = ArbitraryId.ToString();
            PropFilterNode.Attributes.Append( ArbitraryIdAttribute );

            XmlAttribute SubfieldNameAttribute = XmlDoc.CreateAttribute( "subfieldname" );
            SubfieldNameAttribute.Value = SubfieldName.ToString();
            PropFilterNode.Attributes.Append( SubfieldNameAttribute );

            return PropFilterNode;
        }

        public XElement ToXElement()
        {
            XElement PropFilter = new XElement( NbtViewXmlNodeName.Filter.ToString(),
                                     new XAttribute( "value", Value ),
                                     new XAttribute( "filtermode", FilterMode.ToString() ),
                                     new XAttribute( "casesensitive", CaseSensitive.ToString() ),
                                     new XAttribute( "arbitraryid", ArbitraryId ),
                                     new XAttribute( "subfieldname", SubfieldName.ToString() )
                );
            return PropFilter;
        }

        public JProperty ToJson()
        {
            JProperty PropFilter = new JProperty( NbtViewXmlNodeName.Filter.ToString() + "_" + ArbitraryId,
                                            new JObject(
                                                new JProperty( "nodename", NbtViewXmlNodeName.Filter.ToString().ToLower() ),
                                                new JProperty( "value", Value ),
                                                new JProperty( "filtermode", FilterMode.ToString() ),
                                                new JProperty( "casesensitive", CaseSensitive.ToString() ),
                                                new JProperty( "arbitraryid", ArbitraryId ),
                                                new JProperty( "subfieldname", SubfieldName.ToString() )
                                                )

                );
            return PropFilter;
        }

        public override string ToString()
        {
            return ToDelimitedString().ToString();
        }

        public CswDelimitedString ToDelimitedString()
        {
            CswDelimitedString ret = new CswDelimitedString( CswNbtView.delimiter );
            ret.Add( NbtViewNodeType.CswNbtViewPropertyFilter.ToString() );
            ret.Add( Conjunction.ToString() );
            ret.Add( Value );
            ret.Add( FilterMode.ToString() );
            ret.Add( CaseSensitive.ToString() );
            ret.Add( ArbitraryId.ToString() );
            ret.Add( SubfieldName.ToString() );

            return ret;
        }

        public override string TextLabel
        {
            get
            {
                return _SubfieldName + " " + FilterMode.ToString() + " " + Value;
            }
        }

    } // class CswViewPropertyFilterValue

} // namespace ChemSW.Nbt

