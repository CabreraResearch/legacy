using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassNotification : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Event = "Event";
            public const string TargetType = "Target Type";
            public const string Property = "Property";
            public const string Value = "Value";
            public const string SubscribedUsers = "Subscribed Users";
            public const string Subject = "Subject";
            public const string Message = "Message";
        }


        public enum EventOption
        {
            Create,
            Edit,
            Delete
        }

        public const string MessageNodeNameReplacement = "[Name]";
        public const string MessagePropertyValueReplacement = "[NewValue]";


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassNotification( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassNotification
        /// </summary>
        public static implicit operator CswNbtObjClassNotification( CswNbtNode Node )
        {
            CswNbtObjClassNotification ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass ) )
            {
                ret = (CswNbtObjClassNotification) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        private void _setDefaultMessage()
        {
            if( null != TargetNodeType )
            {
                switch( SelectedEvent )
                {
                    case EventOption.Create:
                        Message.Text = "New " + TargetNodeType.NodeTypeName + " has been created: " +
                                       MessageNodeNameReplacement + ".\r\n";
                        break;
                    case EventOption.Edit:
                        Message.Text = TargetNodeType.NodeTypeName + " has been edited: " + MessageNodeNameReplacement +
                                       ".\r\n";
                        if( Property.Value != string.Empty )
                            Message.Text += Property.Value + " changed to: " + MessagePropertyValueReplacement;
                        break;
                    case EventOption.Delete:
                        Message.Text = TargetNodeType.NodeTypeName + " has been deleted: " + MessageNodeNameReplacement +
                                       ".\r\n";
                        break;
                }
            }
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            // If TargetType changes or Event is set to Create or Delete, 
            // clear value for Property and Value
            if( TargetType.WasModified || Event.Value != CswNbtObjClassNotification.EventOption.Edit.ToString() )
            {
                Property.Value = string.Empty;
                Value.Text = string.Empty;
            }

            if( TargetType.WasModified || Property.WasModified || Event.WasModified )
                _setDefaultMessage();

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            // BZ 10094 - Reset cache
            _CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            // BZ 10094 - Reset cache
            _CswNbtResources.ConfigVbls.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();

            // List options for 'Property' depend on TargetType
            CswCommaDelimitedString PropertyOptions = new CswCommaDelimitedString();
            if( CswTools.IsInteger( TargetType.SelectedNodeTypeIds[0] ) )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( TargetType.SelectedNodeTypeIds[0] ) );
                if( NodeType != null )
                {
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps() )
                    {
                        PropertyOptions.Add( Prop.PropName );
                    }
                    Property.Options.Override( PropertyOptions );
                } // if( NodeType != null )
            } // if( CswTools.IsInteger( TargetType.SelectedNodeTypeIds[0] ) )

        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Event
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Event].AsList );
            }
        }
        public EventOption SelectedEvent
        {
            get
            {
                return ( (EventOption) Enum.Parse( typeof( EventOption ), Event.Value ) );
            }
        }
        public CswNbtNodePropList Property
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Property].AsList );
            }
        }
        public CswNbtNodePropNodeTypeSelect TargetType
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.TargetType].AsNodeTypeSelect );
            }
        }
        public CswNbtMetaDataNodeType TargetNodeType
        {
            get
            {
                return ( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( TargetType.SelectedNodeTypeIds[0] ) ) );
            }
        }
        public CswNbtNodePropText Value
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Value].AsText );
            }
        }
        public CswNbtNodePropUserSelect SubscribedUsers
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SubscribedUsers].AsUserSelect );
            }
        }
        public CswNbtNodePropText Subject
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Subject].AsText );
            }
        }
        public CswNbtNodePropMemo Message
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Message].AsMemo );
            }
        }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
