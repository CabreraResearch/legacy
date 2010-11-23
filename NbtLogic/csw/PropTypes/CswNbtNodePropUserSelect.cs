using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Prop Class for UserSelect Properties
    /// </summary>
    public class CswNbtNodePropUserSelect : CswNbtNodeProp
    {
        public static char delimiter = ',';

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodePropUserSelect( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _SelectedUserIdsSubField = ( (CswNbtFieldTypeRuleUserSelect) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SelectedUserIdsSubField;
        }//ctor

        private CswNbtSubField _SelectedUserIdsSubField;

        /// <summary>
        /// Returns whether the property value is empty
        /// </summary>
        override public bool Empty
        {
            get
            {
                return ( 0 == SelectedUserIds.Length );
            }
        }

        /// <summary>
        /// Text value of property
        /// </summary>
        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        /// <summary>
        /// Comma-separated list of Selected UserIds
        /// </summary>
        public string SelectedUserIds
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _SelectedUserIdsSubField.Column );
            }
            set
            {
                if( _CswNbtNodePropData.SetPropRowValue( _SelectedUserIdsSubField.Column, value ) )
                    PendingUpdate = true;
            }
        }

        /// <summary>
        /// Subscribes a user by adding the userid to the SelectedUserIds list
        /// </summary>
        public void AddUser( CswPrimaryKey UserId )
        {
            string SearchStr = delimiter.ToString() + SelectedUserIds + delimiter.ToString();
            if( !SearchStr.Contains( delimiter.ToString() + UserId.PrimaryKey.ToString() + delimiter.ToString() ) )
            {
                SelectedUserIds = SelectedUserIds + delimiter + UserId.PrimaryKey.ToString();
            }
        }

        /// <summary>
        /// Unsubscribes a user by removing the userid from the SelectedUserIds list
        /// </summary>
        public void RemoveUser( CswPrimaryKey UserId )
        {
            string SearchStr = delimiter.ToString() + SelectedUserIds + delimiter.ToString();
            if( SearchStr.Contains( delimiter.ToString() + UserId.PrimaryKey.ToString() + delimiter.ToString() ) )
            {
                Collection<Int32> SelectedIds = CswTools.DelimitedStringToIntCollection( SelectedUserIds, delimiter );
                SelectedIds.Remove( UserId.PrimaryKey );
                SelectedUserIds = CswTools.IntCollectionToDelimitedString( SelectedIds, delimiter, false );
            }
        }


        /// <summary>
        /// Refresh the names of the selected users
        /// </summary>
        public void RefreshSelectedUserNames()
        {
            _CswNbtNodePropData.Gestalt = SelectedUsersToString();
            PendingUpdate = false;
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SelectedUsersNode = CswXmlDocument.AppendXmlNode( ParentNode, _SelectedUserIdsSubField.ToXmlNodeName(), SelectedUserIds );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string UserIds = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SelectedUserIdsSubField.ToXmlNodeName() );
            string[] UserIdsArray = UserIds.Split( delimiter );
            string NewSelectedUserIds = string.Empty;
            foreach( string UserIdString in UserIdsArray )
            {
                if( CswTools.IsInteger( UserIdString ) )
                {
                    Int32 UserId = Convert.ToInt32( UserIdString );
                    if( NodeMap != null && NodeMap.ContainsKey( UserId ) )
                        UserId = NodeMap[UserId];
                    if( NewSelectedUserIds != string.Empty ) NewSelectedUserIds += delimiter.ToString();
                    NewSelectedUserIds += UserId.ToString();
                }
            }
            PendingUpdate = true;
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string UserIds = CswTools.XmlRealAttributeName( PropRow[_SelectedUserIdsSubField.ToXmlNodeName()].ToString() );
            string[] UserIdsArray = UserIds.Split( delimiter );
            string NewSelectedUserIds = string.Empty;
            foreach( string UserIdString in UserIdsArray )
            {
                Int32 UserId = Int32.MinValue;
                if( NodeMap != null && NodeMap.ContainsKey( UserIdString.ToLower() ) )
                    UserId = NodeMap[UserIdString.ToLower()];
                else if( CswTools.IsInteger( UserIdString ) )
                    UserId = Convert.ToInt32( UserIdString );
                if( UserId != Int32.MinValue )
                {
                    if( NewSelectedUserIds != string.Empty ) NewSelectedUserIds += delimiter.ToString();
                    NewSelectedUserIds += UserId.ToString();
                }
            }
            PendingUpdate = true;
        }

        public string SelectedUsersToString()
        {
            string[] UserIdArray = SelectedUserIds.Split( delimiter );
            string[] SelectedUserNames = new string[UserIdArray.Length];
            ICswNbtTree UsersTree = _CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )
            {
                UsersTree.goToNthChild( c );

                CswPrimaryKey ThisUserId = UsersTree.getNodeIdForCurrentPosition();
                string ThisUserName = UsersTree.getNodeNameForCurrentPosition();

                for( int i = 0; i < UserIdArray.Length; i++ )
                {
                    if( UserIdArray[i].ToString() != string.Empty )
                    {
                        if( ThisUserId.PrimaryKey.ToString() == UserIdArray[i].ToString() )
                        {
                            SelectedUserNames[i] = ThisUserName;
                        }
                    }
                }

                UsersTree.goToParentNode();
            }

            // Sort alphabetically
            Array.Sort( SelectedUserNames );

            string SelectedUsersString = string.Empty;
            for( int i = 0; i < SelectedUserNames.Length; i++ )
            {
                if( SelectedUsersString != string.Empty )
                    SelectedUsersString += ", ";
                SelectedUsersString += SelectedUserNames[i];
            }
            return SelectedUsersString;
        }

    }//CswNbtNodePropUserSelect
}//namespace ChemSW.Nbt.PropTypes
