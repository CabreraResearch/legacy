using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Helper class for structuring data for CheckBoxArray proptypes (UserSelect, ViewPickList, LogicalSet, NodeTypeSelect)
    /// </summary>
    public class CswCheckBoxArrayOptions
    {
        public CswCheckBoxArrayOptions()
        {
        }

        public StringCollection Columns = new StringCollection();
        public Collection<Option> Options = new Collection<Option>();
        public class Option
        {
            public string Label;
            public string Key;
            public Collection<bool> Values = new Collection<bool>();
        }

        // Everything below should be replaced by WCF eventually

        public string ElemName_Data = "data";
        public string ElemName_Columns = "columns";
        public string ElemName_Label = "label";
        public string ElemName_Key = "key";
        public string ElemName_Values = "values";

        public void ToJSON( JObject ParentObject )
        {
            JArray DataArray = new JArray();
            ParentObject[ElemName_Data] = DataArray;
            foreach( Option o in Options )
            {
                JObject ItemNodeObj = new JObject();
                DataArray.Add( ItemNodeObj );
                
                ItemNodeObj[ElemName_Label] = o.Label;
                ItemNodeObj[ElemName_Key] = o.Key;

                JArray ValuesArr = new JArray();
                foreach( bool v in o.Values )
                {
                    ValuesArr.Add( v );
                }
                ItemNodeObj[ElemName_Values] = ValuesArr;
            }

            JArray ColumnArray = new JArray();
            foreach( string c in Columns )
            {
                ColumnArray.Add( c );
            }
            ParentObject[ElemName_Columns] = ColumnArray;

        } // ToJSON()

        public void ReadJson( JObject ParentObject )
        {
            if( null != ParentObject[ElemName_Data] && null != ParentObject[ElemName_Columns] )
            {
                Columns = new StringCollection();
                JArray ColumnsAry = CswConvert.ToJArray( ParentObject[ElemName_Columns] );
                if( null != ColumnsAry )
                {
                    foreach( string c in ColumnsAry.Values<string>() )
                    {
                        Columns.Add( c );
                    }
                } // if( null != ColumnsAry )

                JArray Data = CswConvert.ToJArray( ParentObject[ElemName_Data] );
                if( null != Data )
                {
                    foreach( JObject ItemObj in Data )
                    {
                        Option o = new Option();
                        o.Key = CswConvert.ToString( ItemObj[ElemName_Key] );
                        o.Label = CswConvert.ToString( ItemObj[ElemName_Label] );

                        JArray Values = CswConvert.ToJArray( ItemObj["values"] );
                        if( null != Values )
                        {
                            foreach(bool v in Values.Values<bool>() )
                            {
                                o.Values.Add( v );
                            } // for( Int32 i = 0; i < ColumnNames.Count; i++ )
                        } // if( null != Values )
                    } // foreach( JObject ItemObj in Data )
                } // if( null != Data )
            } // if( null != ParentObject[ElemName_Data] && null != ParentObject[ElemName_Columns] )
        } // ReadJson()

    }//class CswCheckBoxArrayOptions

}//namespace ChemSW.Nbt.PropTypes
