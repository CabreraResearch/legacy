using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropMetaDataList : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropMetaDataList( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsMetaDataList;
        }

        public CswNbtNodePropMetaDataList( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _TypeSubField = ( (CswNbtFieldTypeRuleMetaDataList) _FieldTypeRule ).TypeSubField;
            _IdSubField = ( (CswNbtFieldTypeRuleMetaDataList) _FieldTypeRule ).IdSubField;
            _TextSubField = ( (CswNbtFieldTypeRuleMetaDataList) _FieldTypeRule ).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _TypeSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Type, x => Type = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, x => Text = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _IdSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Id,
                                                                                          delegate( dynamic x )
                                                                                          {
                                                                                              if( CswTools.IsInteger( x ) )
                                                                                              {
                                                                                                  if( Type == CswEnumNbtViewRelatedIdType.NodeTypeId )
                                                                                                  {
                                                                                                      setValue( NodeTypePrefix + CswConvert.ToString( x ) );
                                                                                                  }
                                                                                                  else if( Type == CswEnumNbtViewRelatedIdType.ObjectClassId )
                                                                                                  {
                                                                                                      setValue( ObjectClassPrefix + CswConvert.ToString( x ) );
                                                                                                  }
                                                                                                  else if( Type == CswEnumNbtViewRelatedIdType.PropertySetId )
                                                                                                  {
                                                                                                      setValue( PropertySetPrefix + CswConvert.ToString( x ) );
                                                                                                  }
                                                                                              }
                                                                                              else
                                                                                              {
                                                                                                  setValue( CswConvert.ToString( x ) );
                                                                                              }
                                                                                          } ) );
        }

        private CswNbtSubField _TypeSubField;
        private CswNbtSubField _IdSubField;
        private CswNbtSubField _TextSubField;

        override public bool Empty
        {
            get { return ( 0 == Gestalt.Length && Int32.MinValue == Id ); }
        }

        override public string Gestalt
        {
            get { return _CswNbtNodePropData.Gestalt; }
        }

        public CswEnumNbtViewRelatedIdType Type
        {
            get { return _CswNbtNodePropData.GetPropRowValue( _TypeSubField.Column ); }
            private set { _CswNbtNodePropData.SetPropRowValue( _TypeSubField.Column, value.ToString() ); }
        }

        public Int32 Id
        {
            get { return CswConvert.ToInt32( _CswNbtNodePropData.GetPropRowValue( _IdSubField.Column ) ); }
            private set { _CswNbtNodePropData.SetPropRowValue( _IdSubField.Column, value ); }
        }

        public string Text
        {
            get { return _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column ); }
            private set { _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, Text ); }
        }

        private void setValue( CswEnumNbtViewRelatedIdType inType, Int32 inId, string inText )
        {
            Type = inType;
            Id = inId;
            Text = inText;
            SyncGestalt();
        }

        public void setValue( CswNbtNodeTypePropListOption selOption )
        {
            if( null != selOption )
            {
                if( selOption.Value.StartsWith( NodeTypePrefix ) )
                {
                    setValue( CswEnumNbtViewRelatedIdType.NodeTypeId, CswConvert.ToInt32( selOption.Value.Substring( NodeTypePrefix.Length ) ), selOption.Text );
                }
                else if( selOption.Value.StartsWith( ObjectClassPrefix ) )
                {
                    setValue( CswEnumNbtViewRelatedIdType.ObjectClassId, CswConvert.ToInt32( selOption.Value.Substring( ObjectClassPrefix.Length ) ), selOption.Text );
                }
                else if( selOption.Value.StartsWith( PropertySetPrefix ) )
                {
                    setValue( CswEnumNbtViewRelatedIdType.PropertySetId, CswConvert.ToInt32( selOption.Value.Substring( PropertySetPrefix.Length ) ), selOption.Text );
                }
                //else
                //{
                //    throw new CswDniException( CswEnumErrorType.Error, "Invalid option: " + selOption.Text, "MetaDataList got an unrecognized value: " + selOption.Value );
                //}
            }
        }//setValue( CswNbtNodeTypePropListOption selOption )

        public void setValue( string selOptionValue )
        {
            setValue( Options.FindByValue( selOptionValue ) );
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public const string NodeTypePrefix = "nt_";
        public const string ObjectClassPrefix = "oc_";
        public const string PropertySetPrefix = "ps_";


        private CswNbtNodeTypePropListOptions _CswNbtNodeTypePropListOptions = null;
        public CswNbtNodeTypePropListOptions Options
        {
            get
            {
                if( null == _CswNbtNodeTypePropListOptions )
                {
                    //_CswNbtNodeTypePropListOptions = new CswNbtNodeTypePropListOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                    _CswNbtNodeTypePropListOptions = new CswNbtNodeTypePropListOptions( _CswNbtResources, "", Int32.MinValue,
                                                                                        CswConvert.ToBoolean( _CswNbtNodePropData[CswEnumNbtPropertyAttributeName.Required] ) );


                    Collection<CswNbtNodeTypePropListOption> newOptions = new Collection<CswNbtNodeTypePropListOption>();
                    // NodeTypes
                    Dictionary<Int32, string> ntDict = _CswNbtResources.MetaData.getNodeTypeIds();
                    foreach( Int32 ntid in ntDict.Keys )
                    {
                        newOptions.Add( new CswNbtNodeTypePropListOption( ntDict[ntid], NodeTypePrefix + ntid ) );
                    }
                    // Object Classes
                    Dictionary<Int32, CswEnumNbtObjectClass> ocDict = _CswNbtResources.MetaData.getObjectClassIds();
                    foreach( Int32 ocid in ocDict.Keys )
                    {
                        newOptions.Add( new CswNbtNodeTypePropListOption( ocDict[ocid], ObjectClassPrefix + ocid ) );
                    }
                    // Property Sets
                    Dictionary<Int32, CswEnumNbtPropertySetName> psDict = _CswNbtResources.MetaData.getPropertySetIds();
                    foreach( Int32 psid in psDict.Keys )
                    {
                        newOptions.Add( new CswNbtNodeTypePropListOption( psDict[psid], PropertySetPrefix + psid ) );
                    }
                    _CswNbtNodeTypePropListOptions.Override( newOptions );
                }

                return ( _CswNbtNodeTypePropListOptions );

            }//get

        }//Options

        public string SelectedValue
        {
            get
            {
                string ret = string.Empty;
                if( CswEnumNbtViewRelatedIdType.NodeTypeId == Type )
                {
                    ret = NodeTypePrefix + Id;
                }
                else if( CswEnumNbtViewRelatedIdType.ObjectClassId == Type )
                {
                    ret = ObjectClassPrefix + Id;
                }
                else if( CswEnumNbtViewRelatedIdType.PropertySetId == Type )
                {
                    ret = PropertySetPrefix + Id;
                }
                return ret;
            }
        }

        override public void onNodePropRowFilled()
        {
        }

        public override void ToJSON( JObject ParentObject )
        {
            //ParentObject[_TypeSubField.ToXmlNodeName( true )] = Type.ToString();
            //ParentObject[_IdSubField.ToXmlNodeName( true )] = Id;
            ParentObject["selectedvalue"] = SelectedValue;

            JArray OptionsArr = new JArray();
            JObject foundValue = null;
            foreach( CswNbtNodeTypePropListOption o in Options.Options )
            {
                JObject Opt = new JObject();
                Opt["text"] = o.Text;
                Opt["value"] = o.Value;
                OptionsArr.Add( Opt );
                if( o.Value == SelectedValue )
                {
                    foundValue = Opt;
                }
            }
            // Make sure the selected value is in the list of options (originally case 28020)
            if( null == foundValue )
            {
                JObject Opt = new JObject();
                Opt["text"] = Text;
                Opt["value"] = SelectedValue;
                OptionsArr.Add( Opt );
                foundValue = Opt;
            }
            ParentObject["options"] = OptionsArr;

            ParentObject[_TextSubField.ToXmlNodeName( true )] = foundValue["text"];
        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject["selectedvalue"] )
            {
                // Decode the actual value from the option selected
                //CswNbtNodeTypePropListOption selOption = Options.FindByValue( JObject["selectedvalue"].ToString() );
                //if( null != selOption )
                //{
                //    setValue( selOption );
                //}
                setValue( JObject["selectedvalue"].ToString() );
            }
        }

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Text );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
