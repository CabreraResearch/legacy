using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Prop Class for UserSelect Properties
    /// </summary>
    public class CswNbtNodePropUserSelect : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropUserSelect( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsUserSelect;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodePropUserSelect( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleUserSelect) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _SelectedUserIdsSubField = _FieldTypeRule.SelectedUserIdsSubField;
        }//ctor

        private CswNbtFieldTypeRuleUserSelect _FieldTypeRule;
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
                    _SelectedUserIds.OnChange += _SelectedUserIds_OnChange;
                    _SelectedUserIds.FromString( _CswNbtNodePropData.GetPropRowValue( _SelectedUserIdsSubField.Column ) );
                }
                //removed archived or invalid users
                Collection<Int32> UserIdsToRemove = new Collection<Int32>();
                foreach( Int32 UserId in _SelectedUserIds.ToIntCollection() )
                {
                    bool Remove = true;
                    if( Int32.MinValue != UserId )
                    {
                        CswPrimaryKey pk = new CswPrimaryKey( "nodes", UserId );
                        CswNbtObjClassUser node = _CswNbtResources.Nodes.GetNode( pk );
                        if( null != node && false == node.IsArchived() )
                        {
                            Remove = false;
                        }
                    }
                    if( Remove )
                    {
                        UserIdsToRemove.Add( UserId );
                    }
                }
                foreach( Int32 DoomedUserId in UserIdsToRemove )
                {
                    _SelectedUserIds.Remove( DoomedUserId.ToString() );
                }
                return _SelectedUserIds;
            }
            set
            {
                _SelectedUserIds = value;
                _SelectedUserIds.OnChange += _SelectedUserIds_OnChange;
                _SelectedUserIds_OnChange();
            }
        }

        // This event handler allows us to save changes made directly to _SelectedNodeTypeIds (like .Add() )
        private void _SelectedUserIds_OnChange()
        {
            if( _CswNbtNodePropData.SetPropRowValue( _SelectedUserIdsSubField.Column, _SelectedUserIds.ToString() ) )
            {
                PendingUpdate = true;
            }
        }

        /// <summary>
        /// True if user is subscribed
        /// </summary>
        private bool _IsSubscribed( Int32 UserId )
        {
            bool ret = Int32.MinValue != UserId && SelectedUserIds.Contains( UserId.ToString() );
            return ret;
        }

        /// <summary>
        /// True if user is subscribed
        /// </summary>
        public bool IsSubscribed( CswPrimaryKey UserId )
        {
            return _IsSubscribed( UserId.PrimaryKey );
        }

        /// <summary>
        /// Subscribes a user by adding the userid to the SelectedUserIds list
        /// </summary>
        public void AddUser( CswPrimaryKey UserId )
        {
            if( !IsSubscribed( UserId ) )
            {
                SelectedUserIds.Add( UserId.PrimaryKey.ToString() );
            }
        }

        /// <summary>
        /// Gets a CswNbtObjClassUser from the SelectedUserIds list
        /// </summary>
        public CswNbtObjClassUser GetUser( Int32 UserId )
        {
            CswNbtNode Ret = null;
            if( _IsSubscribed( UserId ) )
            {
                CswPrimaryKey UserPk = new CswPrimaryKey( "nodes", UserId );
                Ret = _CswNbtResources.Nodes.GetNode( UserPk );
            }
            return Ret;
        }

        /// <summary>
        /// Unsubscribes a user by removing the userid from the SelectedUserIds list
        /// </summary>
        public void RemoveUser( CswPrimaryKey UserId )
        {
            if( IsSubscribed( UserId ) )
            {
                SelectedUserIds.Remove( UserId.PrimaryKey.ToString() );
            }
        }

        /// <summary>
        /// Refresh the names of the selected users
        /// </summary>
        public void RefreshSelectedUserNames()
        {
            _CswNbtNodePropData.Gestalt = SelectedUserNames().ToString();
            PendingUpdate = false;
        }

        public DataTable getUserOptions()
        {
            DataTable Data = new CswDataTable( "Userselectdatatable", "" );
            Data.Columns.Add( NameColumn, typeof( string ) );
            Data.Columns.Add( KeyColumn, typeof( int ) );
            Data.Columns.Add( StringKeyColumn, typeof( string ) );
            Data.Columns.Add( ValueColumn, typeof( bool ) );

            bool first = true;
            //ICswNbtTree UsersTree = _CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass );
            //for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )
            //{
            //    UsersTree.goToNthChild( c );
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            foreach( CswNbtNode UserNode in UserOC.getNodes( false, false ) )
            {
                if( Tristate.True != UserNode.Properties[CswNbtObjClassUser.PropertyName.Archived].AsLogical.Checked )
                {
                    DataRow NTRow = Data.NewRow();
                    NTRow[NameColumn] = UserNode.NodeName; // UsersTree.getNodeNameForCurrentPosition();
                    NTRow[KeyColumn] = UserNode.NodeId.PrimaryKey; //  UsersTree.getNodeIdForCurrentPosition().PrimaryKey;
                    NTRow[StringKeyColumn] = UserNode.NodeId.ToString(); //  UsersTree.getNodeIdForCurrentPosition().PrimaryKey;
                    NTRow[ValueColumn] = ( SelectedUserIds.Contains( UserNode.NodeId.PrimaryKey.ToString() ) ||  //UsersTree.getNodeIdForCurrentPosition().PrimaryKey.ToString() ) ) ||
                                           ( first && Required && SelectedUserIds.Count == 0 ) );
                    Data.Rows.Add( NTRow );
                    first = false;

                    //UsersTree.goToParentNode();
                }
            }
            return Data;
        } // UserOptions()

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public const string NameColumn = "label";
        public const string KeyColumn = "key";
        public const string StringKeyColumn = "UserIdString";
        public const string ValueColumn = "value";
        public const string ElemName_Options = "options";

        // ToXml()

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_SelectedUserIdsSubField.ToXmlNodeName()] = SelectedUserIds.ToString();
            ParentObject[ElemName_Options] = new JObject();

            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();
            CBAOptions.Columns.Add( "Include" );

            DataTable Data = getUserOptions();
            foreach( DataRow Row in Data.Rows )
            {
                CswCheckBoxArrayOptions.Option Option = new CswCheckBoxArrayOptions.Option();
                Option.Key = Row[KeyColumn].ToString();
                Option.Label = Row[NameColumn].ToString();
                Option.Values.Add( CswConvert.ToBoolean( Row[ValueColumn] ) );
                CBAOptions.Options.Add( Option );
            }

            CBAOptions.ToJSON( (JObject) ParentObject[ElemName_Options] );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswCommaDelimitedString NewSelectedUserIds = new CswCommaDelimitedString();

            CswCheckBoxArrayOptions CBAOptions = new CswCheckBoxArrayOptions();
            if( null != JObject[ElemName_Options] )
            {
                CBAOptions.ReadJson( (JObject) JObject[ElemName_Options] );
            }
            foreach( CswCheckBoxArrayOptions.Option Option in CBAOptions.Options )
            {
                if( Option.Values.Count > 0 && true == Option.Values[0] )
                {
                    NewSelectedUserIds.Add( Option.Key );
                }
            }
            SelectedUserIds = NewSelectedUserIds;
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
                    UserId = CswConvert.ToInt32( UserIdString );
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
            //ICswNbtTree UsersTree = _CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass );
            //for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )
            //{
            //    UsersTree.goToNthChild( c );
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
            foreach( CswNbtNode UserNode in UserOC.getNodes( false, false ) )
            {
                CswPrimaryKey ThisUserId = UserNode.NodeId;  //UsersTree.getNodeIdForCurrentPosition();
                string ThisUserName = UserNode.NodeName; // UsersTree.getNodeNameForCurrentPosition();

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

                //UsersTree.goToParentNode();
            } // for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )

            // Sort alphabetically
            SelectedUserNames.Sort();

            return SelectedUserNames;
        } // SelectedUserNames()

        public Collection<CswNbtObjClassUser> SelectedUsers()
        {
            Collection<CswNbtObjClassUser> Ret = new Collection<CswNbtObjClassUser>();
            foreach( Int32 UserId in SelectedUserIds.ToIntCollection() )
            {
                CswNbtObjClassUser User = GetUser( UserId );
                if( null != User )
                {
                    Ret.Add( User );
                }
            }
            return Ret;
        } // SelectedUserNames()

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.Gestalt = SelectedUserNames().ToString();
        }

    }//CswNbtNodePropUserSelect
}//namespace ChemSW.Nbt.PropTypes
