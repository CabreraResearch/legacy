using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassNotification : CswNbtObjClass
    {
        public static string EventPropertyName { get { return "Event"; } }
        public static string TargetTypePropertyName { get { return "Target Type"; } }
        public static string PropertyPropertyName { get { return "Property"; } }
        public static string ValuePropertyName { get { return "Value"; } }
        public static string SubscribedUsersPropertyName { get { return "Subscribed Users"; } }
        public static string SubjectPropertyName { get { return "Subject"; } }
        public static string MessagePropertyName { get { return "Message"; } }

        public enum EventOption
        {
            Create,
            Edit,
            Delete
        }

        public static string MessageNodeNameReplacement = "[Name]";
        public static string MessagePropertyValueReplacement = "[NewValue]";


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassNotification( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public CswNbtObjClassNotification( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass ); }
        }

        #region Inherited Events

        public override void beforeCreateNode()
        {
            _setDefaultMessage();

            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        private void _setDefaultMessage()
        {
            if (null != TargetNodeType)
            {
                switch (SelectedEvent)
                {
                    case EventOption.Create:
                        Message.Text = "New " + TargetNodeType.NodeTypeName + " has been created: " +
                                       MessageNodeNameReplacement + ".\r\n";
                        break;
                    case EventOption.Edit:
                        Message.Text = TargetNodeType.NodeTypeName + " has been edited: " + MessageNodeNameReplacement +
                                       ".\r\n";
                        if (Property.Value != string.Empty)
                            Message.Text += Property.Value + " changed to: " + MessagePropertyValueReplacement;
                        break;
                    case EventOption.Delete:
                        Message.Text = TargetNodeType.NodeTypeName + " has been deleted: " + MessageNodeNameReplacement +
                                       ".\r\n";
                        break;
                }
            }
        }
        
        public override void afterCreateNode()
        {
            // BZ 10094 - Reset cache
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
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

            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            // BZ 10094 - Reset cache
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            // BZ 10094 - Reset cache
            _CswNbtResources.setConfigVariableValue( "cache_lastupdated", DateTime.Now.ToString() );

            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();

            // List options for 'Property' depend on TargetType
            string PropertyOptions = string.Empty;
            if( CswTools.IsInteger( TargetType.SelectedNodeTypeIds[0] ) )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( TargetType.SelectedNodeTypeIds[0] ) );
                if( NodeType != null )
                {
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.NodeTypeProps )
                    {
                        if( PropertyOptions != string.Empty ) PropertyOptions += ",";
                        PropertyOptions += Prop.PropName;
                    }
                    Property.Options.Override( PropertyOptions );
                }
            }

        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Event
        {
            get
            {
                return ( _CswNbtNode.Properties[EventPropertyName].AsList );
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
                return ( _CswNbtNode.Properties[PropertyPropertyName].AsList );
            }
        }
        public CswNbtNodePropNodeTypeSelect TargetType
        {
            get
            {
                return ( _CswNbtNode.Properties[TargetTypePropertyName].AsNodeTypeSelect );
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
                return ( _CswNbtNode.Properties[ValuePropertyName].AsText );
            }
        }
        public CswNbtNodePropUserSelect SubscribedUsers
        {
            get
            {
                return ( _CswNbtNode.Properties[SubscribedUsersPropertyName].AsUserSelect );
            }
        }
        public CswNbtNodePropText Subject
        {
            get
            {
                return ( _CswNbtNode.Properties[SubjectPropertyName].AsText );
            }
        }
        public CswNbtNodePropMemo Message
        {
            get
            {
                return ( _CswNbtNode.Properties[MessagePropertyName].AsMemo );
            }
        }

        #endregion



    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
