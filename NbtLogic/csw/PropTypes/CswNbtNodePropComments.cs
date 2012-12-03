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

    public class CswNbtNodePropComments : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropComments( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsComments;
        }

        public CswNbtNodePropComments( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleComments) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _CommentSubField = _FieldTypeRule.CommentSubField;
        }
        private CswNbtFieldTypeRuleComments _FieldTypeRule;
        private CswNbtSubField _CommentSubField;


        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( CswNbtSubField.PropColumn.Gestalt );
            }

        }//Gestalt

        /// <summary>
        /// Gets the last comment
        /// </summary>
        public JObject Last
        {
            get
            {
                return CswConvert.ToJObject( CommentsJson.Last.ToString() );
            }
        }

        public JArray CommentsJson
        {
            get
            {
                JArray obj = new JArray();
                try
                {
                    obj = CswConvert.ToJArray( _CswNbtNodePropData.GetPropRowValue( _CommentSubField.Column ) );
                }
                catch( Exception e )
                {
                    _CswNbtResources.logError( e );
                }
                return ( obj );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _CommentSubField.Column, value.ToString() );
            }
        }

        public Int32 Rows
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaRows == Int32.MinValue )
                    return 4;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaRows = value;
            //}
        }
        public Int32 Columns
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaColumns == Int32.MinValue )
                    return 40;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaColumns = value;
            //}
        }

        public override void ToJSON( JObject ParentObject )
        {
            JArray _CommentsJson = CommentsJson;
            foreach( JObject jr in _CommentsJson )
            {
                //converting each output row datetime to local user display format
                jr["datetime"] = new CswDateTime( _CswNbtResources, CswConvert.ToDateTime( jr["datetime"] ) ).ToClientAsDateTimeString();
            }
            ParentObject[_CommentSubField.ToXmlNodeName( true )] = _CommentsJson;
            ParentObject["rows"] = Rows.ToString();
            ParentObject["columns"] = Columns.ToString();
            ParentObject["newmessage"] = "";
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            AddComment( CswConvert.ToString( JObject["newmessage"] ), CswConvert.ToString( JObject["commenter"] ) );
        }

        public void AddComment( string message, string commenter = "" )
        {
            if( false == String.IsNullOrEmpty( message ) )
            {
                if( _CswNbtResources.CurrentNbtUser != null && true == String.IsNullOrEmpty( commenter ) )
                {
                    commenter = _CswNbtResources.CurrentNbtUser.LastName;
                    if( false == String.IsNullOrEmpty( _CswNbtResources.CurrentNbtUser.FirstName ) )
                    {
                        commenter += "," + _CswNbtResources.CurrentNbtUser.FirstName;
                    }
                    commenter += " (" + _CswNbtResources.CurrentNbtUser.Username + ")";
                }
                JArray _CommentsJson = CommentsJson;
                //comments:  [ { datetime: '12/31/2012', commenter: 'david', message: 'yuck' }, { ... } ]

                var dateSubmitted = CswConvert.ToDbVal( DateTime.Now );

                _CommentsJson.Add( new JObject(
                    new JProperty( "datetime", dateSubmitted ),
                    new JProperty( "commenter", commenter ),
                    new JProperty( "message", message ) ) );


                //Remove exceess comments
                Int32 CommentsTruncationLimit = 10;
                if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswNbtResources.ConfigurationVariables.total_comments_lines.ToString() ) )
                {

                    CommentsTruncationLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.total_comments_lines.ToString() ) );
                }

                while( _CommentsJson.Count > CommentsTruncationLimit )
                {
                    _CommentsJson.RemoveAt( 0 );
                }


                CommentsJson = _CommentsJson;
                _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, commenter + " on " + dateSubmitted.ToString() + ": " + message ); //the caches the last message and sets it to Gestalt


            }

        }
    }//CswNbtNodePropComments

}//namespace ChemSW.Nbt.PropTypes
