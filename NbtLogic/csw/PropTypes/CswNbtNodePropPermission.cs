using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropPermission : CswNbtNodeProp
    {

        public static implicit operator CswNbtNodePropPermission( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsPermission;
        }

        public CswNbtNodePropPermission( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _ValueSubField = ( (CswNbtFieldTypeRulePermission) _FieldTypeRule ).ValueSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _ValueSubField.Name, new Tuple<Func<dynamic>, Action<dynamic>>( () => Value, x => Value = new CswCommaDelimitedString( CswConvert.ToString( x ) ) ) );
        }

        private CswNbtSubField _ValueSubField;

        #region Attributes

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length && Value.IsEmpty );
            }//
        }

        /// <summary>
        /// The Delimiter used to concatenate all selected options
        /// </summary>
        public string ReadOnlyDelimiter
        {
            get
            {
                //string _readOnlyDelimiter = "<br />";
                //if( false == String.IsNullOrEmpty( _CswNbtMetaDataNodeTypeProp.Extended ) )
                //{
                //    _readOnlyDelimiter = _CswNbtMetaDataNodeTypeProp.Extended;
                //}
                string _readOnlyDelimiter = _CswNbtNodePropData[CswNbtFieldTypeRulePermission.AttributeName.ReadOnlyDelimiter];
                if( string.IsNullOrEmpty( _readOnlyDelimiter ) )
                {
                    _readOnlyDelimiter = "<br />";
                }
                return _readOnlyDelimiter;
            }
        }

        /// <summary>
        /// The number of selected options to display in readonly mode.
        /// </summary>
        public int HideThreshold
        {
            get
            {
                //int _hideThreshold = 5;
                //if( CswTools.IsDouble( _CswNbtMetaDataNodeTypeProp.MaxValue ) )
                //{
                //    _hideThreshold = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.MaxValue );
                //}
                //return _hideThreshold;
                Int32 ret = CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRulePermission.AttributeName.ReadOnlyHideThreshold] );
                if( Int32.MinValue == ret )
                {
                    ret = 5;
                }
                return ret;
            }
        }

        private CswCommaDelimitedString _Value = null;
        public CswCommaDelimitedString Value
        {
            get
            {
                if( _Value == null )
                {
                    _Value = new CswCommaDelimitedString();
                    _Value.FromString( GetPropRowValue( _ValueSubField ) );
                }
                return _Value;
            }
            set
            {
                _Value = value;

                //Case 31236 - trim each selected value to avoid having values like " value1" or "value2 "
                CswCommaDelimitedString trimmmedValues = new CswCommaDelimitedString();
                foreach( string s in value )
                {
                    trimmmedValues.Add( s.Trim() );
                }

                string ValString = trimmmedValues.ToString();
                SetPropRowValue( _ValueSubField, ValString );
                SyncGestalt();
            }
        }

        #endregion Attributes

        /// <summary>
        /// Add a value to the set of selected values
        /// </summary>
        public void AddValue( string ValueToAdd )
        {
            CswCommaDelimitedString myValue = Value;
            myValue.Add( ValueToAdd, IsUnique: true );
            Value = myValue;
        }

        /// <summary>
        /// Check to see if a value is present
        /// </summary>
        public bool CheckValue( string ValueToCheck )
        {
            return Value.Contains( ValueToCheck );
        }


        /// <summary>
        /// Checks to be sure all values assigned are unique and valid against possible options
        /// </summary>
        public void ValidateValues()
        {
            CswCommaDelimitedString newVals = new CswCommaDelimitedString();

            foreach( string aval in Value )
            {
                if( Options.ContainsKey( aval ) && false == newVals.Contains( aval ) )
                {
                    newVals.Add( aval );
                }
            }
            Value = newVals;

        } // ValidateValues() 

        /// <summary>
        /// Remove a value from the set of selected values
        /// </summary>
        public void RemoveValue( string ValueToRemove )
        {
            CswCommaDelimitedString myValue = Value;
            myValue.RemoveAll( ValueToRemove );
            Value = myValue;
        }

        public delegate Dictionary<string, string> InitOptionsHandler();
        public InitOptionsHandler InitOptions = null;

        private Dictionary<string, string> _Options = null;
        public Dictionary<string, string> Options
        {
            get
            {
                if( _Options == null )
                {
                    if( InitOptions != null )
                    {
                        // Override, usually from CswNbtObjClass*
                        _Options = InitOptions();
                    }
                    if( _Options == null )
                    {
                        // Default
                        _Options = new Dictionary<string, string>();
                        CswCommaDelimitedString ListOptions = new CswCommaDelimitedString();
                        //ListOptions.FromString( _CswNbtMetaDataNodeTypeProp.ListOptions );
                        ListOptions.FromString( _CswNbtNodePropData[CswNbtFieldTypeRulePermission.AttributeName.Options] );
                        foreach( string ListOption in ListOptions )
                        {
                            _Options.Add( ListOption, ListOption );
                        }
                    }
                } // if( _Options == null )
                return _Options;
            } // get
            // Use InitOptions handler for performance instead
            //set
            //{
            //    _Options = value;
            //}
        } // Options

        public override void SyncGestalt()
        {
            CswCommaDelimitedString NewGestalt = new CswCommaDelimitedString();
            foreach( string Key in this.Value )
            {
                if( Options.ContainsKey( Key ) )
                {
                    NewGestalt.Add( Options[Key] );
                }
            }
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, NewGestalt.ToString() );
        } // _setGestalt()

        public static string OptionTextField = "Text";
        public static string OptionValueField = "Value";

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        override public void onNodePropRowFilled()
        {
        }


        #region Serialization

        public override void ToJSON( JObject ParentObject )
        {
            //! the data should be stored this way in the DB and not transformed into it, but we can't do this in Magnolia because of design mode and field type shenanigans
            /*
             * data is coming from the server in two dictionaries: a mapping of value to string, and an array of held permissions
             *                          Options: { 
             *                             'nt_1_Create': 'Role', 
             *                             'nt_1_tab_1_View': 'Role, Role', 
             *                             ... 
             *                          }
             *                          Values: [
             *                            'nt_1_Create', 
             *                            'nt_1_tab_5_Create', 
             *                            ...
             *                         ]
             * 
             * We need it in the format: Options: [ 
             *                            { name: 'Role', 
             *                              Create: true, 
             *                              Edit: true, 
             *                              Delete: true, 
             *                              View: true, 
             *                              Children: [
             *                                 { name: 'Role Tab', 
             *                                   Create: true, 
             *                                   Edit: true, 
             *                                   Delete: true, 
             *                                   View: true, 
             *                                   leaf: true
             *                                 }]
             *                             },
             *                             ...
             *                             ]
             * 
             */

            ParentObject[_ValueSubField.ToXmlNodeName( true )] = Value.ToString();

            JObject OptionsArray = new JObject();
            ParentObject["options"] = OptionsArray;

            //first build the skeleton of the object we want to return
            foreach( CswNbtMetaDataNodeType NT in _CswNbtResources.MetaData.getNodeTypes() )
            {
                JObject NewNT = new JObject();
                NewNT["itemname"] = NT.NodeTypeName;
                NewNT["create"] = false;
                NewNT["edit"] = false;
                NewNT["view"] = false;
                NewNT["delete"] = false;
                NewNT["itemid"] = NT.NodeTypeId;
                JArray Children = new JArray();
                foreach( CswNbtMetaDataNodeTypeTab Tab in NT.getNodeTypeTabs() )
                {
                    JObject NewTab = new JObject();
                    NewTab["itemname"] = Tab.TabName + " Tab";
                    NewTab["create"] = false;
                    NewTab["edit"] = false;
                    NewTab["view"] = false;
                    NewTab["delete"] = false;
                    NewTab["itemid"] = Tab.TabId;
                    NewTab["leaf"] = true;

                    Children.Add( NewTab );
                }
                NewNT["children"] = Children;
                OptionsArray[NT.NodeTypeId.ToString()] = NewNT;
            }

            //now iterate the values we've stored and assign them to the correct permissions in the skeleton:
            foreach( string Permission in Value )
            {
                string[] PermissionComponents = Permission.Split( '_' );
                bool IsTab = PermissionComponents.Length == 5;
                string Nodetype = PermissionComponents[1];
                string PermissionCategory = PermissionComponents[PermissionComponents.Length-1].ToLower();

                //our permissions data needs a good cleaning... there are invalid nodetypes in the Values dictionary
                if( ParentObject["options"][Nodetype] != null )
                {
                    if( false == IsTab )
                    {
                        //if we're not dealing with a tab, we can just set the proper permission
                        ParentObject["options"][Nodetype][PermissionCategory] = true;
                    }
                    else
                    {
                        string TabId = PermissionComponents[3];
                        //for tabs we have no choice but to iterate to find the correct tab, because of client-side restrictions
                        foreach( JObject Tab in ParentObject["options"][Nodetype]["children"] )
                        {
                            if( Tab["itemid"].ToString() == TabId.ToString() )
                            {
                                Tab[PermissionCategory] = true;
                            }
                        } //for each tab in the returned JObject
                    } //else -- if the permission is a tab
                }//if this nodetype exists
            }//for each permission stored in the database


        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswCommaDelimitedString NewValue = new CswCommaDelimitedString();
            NewValue.FromString( CswTools.XmlRealAttributeName( PropRow[_ValueSubField.ToXmlNodeName()].ToString() ) );
            Value = NewValue;
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_ValueSubField.ToXmlNodeName( true )] )
            {
                CswCommaDelimitedString NewValue = new CswCommaDelimitedString();
                NewValue.FromString( JObject[_ValueSubField.ToXmlNodeName( true )].ToString() );
                Value = NewValue;
            }
        }

        #endregion Serialization

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
