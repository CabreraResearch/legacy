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
                return ( 0 == SelectedUserIds.Count );
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

        public static char delimiter = ',';

        /// <summary>
        /// Comma-separated list of Selected UserIds
        /// </summary>
        private CswCommaDelimitedString _SelectedUserIds = null;
        public CswCommaDelimitedString SelectedUserIds
        {
            get
            {
                if( _SelectedUserIds == null )
                {
                    _SelectedUserIds = new CswCommaDelimitedString();
                    _SelectedUserIds.OnChange += new CswDelimitedString.DelimitedStringChangeHandler( _SelectedUserIds_OnChange );
                    _SelectedUserIds.FromString( _CswNbtNodePropData.GetPropRowValue( _SelectedUserIdsSubField.Column ) );
                }
                return _SelectedUserIds;
            }
            set
            {
                _SelectedUserIds = value;
                _SelectedUserIds_OnChange();
            }
        }

        // This event handler allows us to save changes made directly to _SelectedNodeTypeIds (like .Add() )
        private void _SelectedUserIds_OnChange()
        {
            if( _CswNbtNodePropData.SetPropRowValue( _SelectedUserIdsSubField.Column, _SelectedUserIds.ToString() ) )
                PendingUpdate = true;
        }

        /// <summary>
        /// Subscribes a user by adding the userid to the SelectedUserIds list
        /// </summary>
        public void AddUser( CswPrimaryKey UserId )
        {
            if( !SelectedUserIds.Contains( UserId.PrimaryKey.ToString() ) )
                SelectedUserIds.Add( UserId.PrimaryKey.ToString() );
        }

        /// <summary>
        /// Unsubscribes a user by removing the userid from the SelectedUserIds list
        /// </summary>
        public void RemoveUser( CswPrimaryKey UserId )
        {
            SelectedUserIds.Remove( UserId.PrimaryKey.ToString() );
        }

        /// <summary>
        /// Refresh the names of the selected users
        /// </summary>
        public void RefreshSelectedUserNames()
        {
            _CswNbtNodePropData.Gestalt = SelectedUserNames().ToString();
            PendingUpdate = false;
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode SelectedUsersNode = CswXmlDocument.AppendXmlNode( ParentNode, _SelectedUserIdsSubField.ToXmlNodeName(), SelectedUserIds.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string UserIds = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SelectedUserIdsSubField.ToXmlNodeName() );
            SelectedUserIds.FromString( UserIds );

            foreach( string UserIdString in SelectedUserIds )
            {
                if( CswTools.IsInteger( UserIdString ) )
                {
                    Int32 UserId = Convert.ToInt32( UserIdString );
                    if( NodeMap != null && NodeMap.ContainsKey( UserId ) )
                        SelectedUserIds.Replace( UserIdString, NodeMap[UserId].ToString() );
                }
            }
            PendingUpdate = true;
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string UserIds = CswTools.XmlRealAttributeName( PropRow[_SelectedUserIdsSubField.ToXmlNodeName()].ToString() );
            SelectedUserIds.FromString( UserIds );

            foreach( string UserIdString in SelectedUserIds )
            {
                Int32 UserId = Int32.MinValue;
                if( NodeMap != null && NodeMap.ContainsKey( UserIdString.ToLower() ) )
                    UserId = NodeMap[UserIdString.ToLower()];
                else if( CswTools.IsInteger( UserIdString ) )
                    UserId = Convert.ToInt32( UserIdString );
                if( UserId != Int32.MinValue )
                {
                    SelectedUserIds.Replace( UserIdString, UserId.ToString() );
                }
            }
            PendingUpdate = true;
        }

        public CswCommaDelimitedString SelectedUserNames()
        {
            CswCommaDelimitedString SelectedUserNames = new CswCommaDelimitedString();
            ICswNbtTree UsersTree = _CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )
            {
                UsersTree.goToNthChild( c );
                CswPrimaryKey ThisUserId = UsersTree.getNodeIdForCurrentPosition();
                string ThisUserName = UsersTree.getNodeNameForCurrentPosition();

                foreach( Int32 UserId in SelectedUserIds.ToIntCollection() )
                {
                    if( UserId != Int32.MinValue )
                    {
                        if( ThisUserId.PrimaryKey == UserId )
                        {
                            SelectedUserNames.Add( ThisUserName );
                        }
                    }
                } // foreach( Int32 UserId in SelectedUserIds.ToIntCollection() )

                UsersTree.goToParentNode();
            } // for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )

            // Sort alphabetically
            SelectedUserNames.Sort();

            return SelectedUserNames;
        } // SelectedUserNames()

    }//CswNbtNodePropUserSelect
}//namespace ChemSW.Nbt.PropTypes
