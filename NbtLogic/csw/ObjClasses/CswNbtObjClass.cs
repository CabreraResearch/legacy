using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public abstract class CswNbtObjClass
    {
        //protected CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        protected CswNbtNode _CswNbtNode = null;
        protected CswNbtResources _CswNbtResources = null;

        private delegate void ifPermiited();
        

        private bool canSave(Int32 TabId)
        {
            bool Ret = false;
            switch( _CswNbtResources.EditMode )
            {
                case NodeEditMode.Temp:
                case NodeEditMode.Add:
                    if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, this.NodeType ) )
                    {
                        Ret = true;
                    }
                    break;
                case NodeEditMode.EditInPopup:
                case NodeEditMode.Edit:
                    if( TabId > 0 )
                    {
                        CswNbtMetaDataNodeTypeTab Tab = this.NodeType.getNodeTypeTab( TabId );
                        if( null != Tab )
                        {
                            Ret = _CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.Edit, this.NodeType, Tab );
                        }
                    }
                    else
                    {
                        Ret = _CswNbtResources.Permit.canAnyTab( CswNbtPermit.NodeTypePermission.Edit, this.NodeType );
                    }
                    break;
            }

            return Ret;
        }

        /// <summary>
        /// Constructor for when we have a node instance
        /// </summary>
        public CswNbtObjClass( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
        {
            _CswNbtNode = CswNbtNode;
            _CswNbtResources = CswNbtResources;
            //We don't have a context for which Tab is going to render, but we can eliminate the base conditions for displaying the Save button here.
            if( false == canSave( TabId: Int32.MinValue ) )
            {
                Save.setHidden( value: true, SaveToDb: false );
            }
        }//ctor()

        /// <summary>
        /// Post node property changes to the database
        /// </summary>
        /// <param name="ForceUpdate">If true, an update will happen whether properties have been modified or not</param>
        public void postChanges( bool ForceUpdate ) //bz# 5446
        {
            _CswNbtNode.postChanges( ForceUpdate );
        }//postChanges()

        public abstract CswNbtMetaDataObjectClass ObjectClass { get; }
        public abstract void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation );
        public abstract void afterWriteNode();
        public abstract void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false );
        public abstract void afterDeleteNode();
        public abstract void afterPopulateProps();
        
        public bool triggerOnButtonClick( NbtButtonData ButtonData )
        {
            if( ButtonData.TabId > 0 && null != ButtonData.Props && ButtonData.Props.HasValues )
            {
                if( canSave( ButtonData.TabId ) )
                {
                    CswNbtSdTabsAndProps Sd = new CswNbtSdTabsAndProps( _CswNbtResources );
                    Sd.saveProps( this.NodeId, ButtonData.TabId, ButtonData.Props, this.NodeTypeId, null, false );
                }
            }
            return onButtonClick( ButtonData );
        }

        protected abstract bool onButtonClick( NbtButtonData ButtonData );
        
        public abstract void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship );
        public virtual CswNbtNode CopyNode()
        {
            CswNbtNode CopiedNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedNode.copyPropertyValues( Node );
            CopiedNode.postChanges( true, true );
            return CopiedNode;
        }

        public abstract class PropertyName
        {
            public const string Save = "Save";
        }

        public virtual CswNbtNodePropButton Save { get { return _CswNbtNode.Properties[PropertyName.Save]; } }

        public Int32 NodeTypeId { get { return _CswNbtNode.NodeTypeId; } }
        public CswNbtMetaDataNodeType NodeType { get { return _CswNbtResources.MetaData.getNodeType( _CswNbtNode.NodeTypeId ); } }
        public string NodeName { get { return _CswNbtNode.NodeName; } }
        public CswPrimaryKey NodeId { get { return _CswNbtNode.NodeId; } }
        public CswNbtNode Node { get { return _CswNbtNode; } }
        public bool IsDemo { get { return _CswNbtNode.IsDemo; } set { _CswNbtNode.IsDemo = value; } }
        public bool IsTemp { get { return _CswNbtNode.IsTemp; } set { _CswNbtNode.IsTemp = value; } }
        public class NbtButtonData
        {
            public NbtButtonData( CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            {
                Data = new JObject();
                Action = NbtButtonAction.Unknown;

                Debug.Assert( null != CswNbtMetaDataNodeTypeProp, "CswNbtMetaDataNodeTypeProp is null." );
                if( null == CswNbtMetaDataNodeTypeProp )
                {
                    throw new CswDniException( "Property is unknown." );
                }
                NodeTypeProp = CswNbtMetaDataNodeTypeProp;
            }
            public void clone( NbtButtonData DataToCopy )
            {
                if( null != DataToCopy )
                {
                    if( null != DataToCopy.Action )
                    {
                        Action = DataToCopy.Action;
                    }
                    if( null != DataToCopy.SelectedText )
                    {
                        SelectedText = DataToCopy.SelectedText;
                    }
                    if( null != DataToCopy.Data )
                    {
                        Data = DataToCopy.Data;
                    }
                    if( null != DataToCopy.Message )
                    {
                        Message = DataToCopy.Message;
                    }
                }
            }

            public NbtButtonAction Action;
            public string SelectedText;
            public CswNbtMetaDataNodeTypeProp NodeTypeProp;
            public JObject Data;
            public JObject Props;
            public Int32 TabId;
            public string Message;

        }

        /// <summary>
        /// Button Actions
        /// </summary>
        public sealed class NbtButtonAction : CswEnum<NbtButtonAction>
        {
            private NbtButtonAction( string Name ) : base( Name ) { }
            public static IEnumerable<NbtButtonAction> _All { get { return All; } }
            public static implicit operator NbtButtonAction( string str )
            {
                NbtButtonAction ret = Parse( str );
                return ret ?? Unknown;
            }
            public static readonly NbtButtonAction Unknown = new NbtButtonAction( "Unknown" );

            public static readonly NbtButtonAction editprop = new NbtButtonAction( "editprop" );
            public static readonly NbtButtonAction creatematerial = new NbtButtonAction( "creatematerial" );
            public static readonly NbtButtonAction dispense = new NbtButtonAction( "dispense" );
            public static readonly NbtButtonAction move = new NbtButtonAction( "move" );
            public static readonly NbtButtonAction reauthenticate = new NbtButtonAction( "reauthenticate" );
            public static readonly NbtButtonAction refresh = new NbtButtonAction( "refresh" );
            public static readonly NbtButtonAction receive = new NbtButtonAction( "receive" );
            public static readonly NbtButtonAction request = new NbtButtonAction( "request" );
            public static readonly NbtButtonAction popup = new NbtButtonAction( "popup" );
            public static readonly NbtButtonAction landingpage = new NbtButtonAction( "landingpage" );
            public static readonly NbtButtonAction loadView = new NbtButtonAction( "loadview" );
            public static readonly NbtButtonAction nothing = new NbtButtonAction( "nothing" );
            public static readonly NbtButtonAction griddialog = new NbtButtonAction( "griddialog" );
        }

        // For validating object class casting
        protected static bool _Validate( CswNbtNode Node, NbtObjectClass TargetObjectClass )
        {
            if( Node == null )
            {
                throw new CswDniException( ErrorType.Error, "Invalid node", "CswNbtObjClass._Validate was given a null node as a parameter" );
            }

            if( !( Node.getObjectClass().ObjectClass == TargetObjectClass ) )
            {
                throw ( new CswDniException( ErrorType.Error, "Invalid cast", "Can't cast current object class as " + TargetObjectClass.ToString() + "; Current object class is " + Node.getObjectClass().ObjectClass.ToString() ) );
            }
            return true;
        }


    }//CswNbtObjClass

}//namespace ChemSW.Nbt.ObjClasses
