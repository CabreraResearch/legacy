using System;
using System.Collections.Generic;
using ChemSW.Core;
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
        public abstract void beforeCreateNode( bool OverrideUniqueValidation );
        public abstract void afterCreateNode();
        public abstract void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation );
        public abstract void afterWriteNode();
        public abstract void beforeDeleteNode();
        public abstract void afterDeleteNode();
        public abstract void afterPopulateProps();
        public abstract bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message );
        public abstract void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship );

        public Int32 NodeTypeId { get { return _CswNbtNode.NodeTypeId; } }
        public CswNbtMetaDataNodeType NodeType { get { return _CswNbtResources.MetaData.getNodeType( _CswNbtNode.NodeTypeId ); } }
        public CswPrimaryKey NodeId { get { return _CswNbtNode.NodeId; } }
        public CswNbtNode Node { get { return _CswNbtNode; } }


        /// <summary>
        /// Button Actions
        /// </summary>
        public sealed class NbtButtonAction : CswEnum<NbtButtonAction>
        {
            private NbtButtonAction( string Name ) : base( Name ) { }
            public static IEnumerable<NbtButtonAction> _All { get { return CswEnum<NbtButtonAction>.All; } }
            public static explicit operator NbtButtonAction( string str )
            {
                NbtButtonAction ret = Parse( str );
                return ( ret != null ) ? ret : NbtButtonAction.Unknown;
            }
            public static readonly NbtButtonAction Unknown = new NbtButtonAction( "Unknown" );

            public static readonly NbtButtonAction reauthenticate = new NbtButtonAction( "reauthenticate" );
            public static readonly NbtButtonAction refresh = new NbtButtonAction( "refresh" );
            public static readonly NbtButtonAction popup = new NbtButtonAction( "popup" );
        }

    }//CswNbtObjClass

}//namespace ChemSW.Nbt.ObjClasses
