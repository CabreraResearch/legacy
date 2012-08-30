using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public abstract class CswNbtObjClass
    {
        //protected CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        protected CswNbtNode _CswNbtNode = null;
        protected CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// Constructor for when we have a node instance
        /// </summary>
        public CswNbtObjClass( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
        {
            _CswNbtNode = CswNbtNode;
            _CswNbtResources = CswNbtResources;
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
        public abstract bool onButtonClick( NbtButtonData ButtonData );
        public abstract void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship );

        public Int32 NodeTypeId { get { return _CswNbtNode.NodeTypeId; } }
        public CswNbtMetaDataNodeType NodeType { get { return _CswNbtResources.MetaData.getNodeType( _CswNbtNode.NodeTypeId ); } }
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
            public static readonly NbtButtonAction dispense = new NbtButtonAction( "dispense" );
            public static readonly NbtButtonAction reauthenticate = new NbtButtonAction( "reauthenticate" );
            public static readonly NbtButtonAction refresh = new NbtButtonAction( "refresh" );
            public static readonly NbtButtonAction receive = new NbtButtonAction( "receive" );
            public static readonly NbtButtonAction request = new NbtButtonAction( "request" );
            public static readonly NbtButtonAction popup = new NbtButtonAction( "popup" );
            public static readonly NbtButtonAction loadView = new NbtButtonAction( "loadView" );
            public static readonly NbtButtonAction nothing = new NbtButtonAction( "nothing" );
        }

        // For validating object class casting
        protected static bool _Validate( CswNbtNode Node, CswNbtMetaDataObjectClass.NbtObjectClass TargetObjectClass )
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
