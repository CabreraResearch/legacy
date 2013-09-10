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

    public class CswNbtNodePropComments: CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropComments( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsComments;
        }

        public CswNbtNodePropComments( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _CommentSubField = ( (CswNbtFieldTypeRuleComments) _FieldTypeRule ).CommentSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _CommentSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => CommentsJson, x => CommentsJson = CswConvert.ToJArray(x) ) ); // not sure if this should be AddComment()
        }

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
                return GetPropRowValue( CswEnumNbtPropColumn.Gestalt );
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
                JArray Ret = new JArray();
                
                try
                {
                    string Json = GetPropRowValue( _CommentSubField.Column );
                    if( false == string.IsNullOrEmpty( Json ) )
                    {
                        Ret = CswConvert.ToJArray( Json );
                        //TODO: Order these DateTime descending
                        //foreach( JObject Comment in from _Comment in obj orderby CswConvert.ToDateTime( _Comment["datetime"] ) descending select (JObject) _Comment )
                        //{
                        //    Ret.Add( Comment );
                        //}
                    }
                }
                catch( Exception e )
                {
                    _CswNbtResources.logError( e );
                }
                return ( Ret );
            }
            set
            {
                SetPropRowValue( _CommentSubField.Column, value.ToString() );
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

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
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

                //TODO: AddFirst()
                _CommentsJson.Add( new JObject(
                    new JProperty( "datetime", dateSubmitted ),
                    new JProperty( "commenter", commenter ),
                    new JProperty( "message", message ) ) );


                //Remove exceess comments
                Int32 CommentsTruncationLimit = 10;
                if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswEnumNbtConfigurationVariables.total_comments_lines.ToString() ) )
                {

                    CommentsTruncationLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.total_comments_lines.ToString() ) );
                }

                while( _CommentsJson.Count > CommentsTruncationLimit )
                {
                    _CommentsJson.RemoveAt( 0 );
                }


                CommentsJson = _CommentsJson;
                SyncGestalt();
            }
        }

        public override void SyncGestalt()
        {
            if( CommentsJson.Count > 0 )
            {
                JToken lastComment = CommentsJson[CommentsJson.Count-1];
                string commenter = lastComment["commenter"].ToString();
                string dateSubmitted = lastComment["datetime"].ToString();
                string message = lastComment["message"].ToString();

                SetPropRowValue( CswEnumNbtPropColumn.Gestalt, commenter + " on " + dateSubmitted.ToString() + ": " + message );
            }
        }

    }//CswNbtNodePropComments

}//namespace ChemSW.Nbt.PropTypes
