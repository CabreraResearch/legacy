using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequest : CswNbtObjClass
    {

        public sealed class PropertyName : CswEnum<PropertyName>
        {
            private PropertyName( String Name ) : base( Name ) { }
            public static IEnumerable<PropertyName> all { get { return All; } }
            public static explicit operator PropertyName( string Str )
            {
                PropertyName Ret = Parse( Str );
                return Ret ?? Unknown;
            }
            public static readonly PropertyName Requestor = new PropertyName( "Requestor" );
            public static readonly PropertyName Name = new PropertyName( "Name" );
            public static readonly PropertyName SubmittedDate = new PropertyName( "Submitted Date" );
            public static readonly PropertyName CompletedDate = new PropertyName( "Completed Date" );
            public static readonly PropertyName InventoryGroup = new PropertyName( "Inventory Group" );
            public static readonly PropertyName Unknown = new PropertyName( "Unknown" );
        }

        public static implicit operator CswNbtObjClassRequest( CswNbtNode Node )
        {
            CswNbtObjClassRequest ret = null;
            if( _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass ) )
            {
                ret = (CswNbtObjClassRequest) Node.ObjClass;
            }
            return ret;
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRequest( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Requestor
        {
            get { return _CswNbtNode.Properties[PropertyName.Requestor.ToString()]; }
        }

        public CswNbtNodePropRelationship InventoryGroup
        {
            get { return _CswNbtNode.Properties[PropertyName.InventoryGroup.ToString()]; }
        }

        public CswNbtNodePropText Name
        {
            get { return _CswNbtNode.Properties[PropertyName.Name.ToString()]; }
        }

        public CswNbtNodePropDateTime SubmittedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.SubmittedDate.ToString()]; }
        }

        public CswNbtNodePropDateTime CompletedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.CompletedDate.ToString()]; }
        }

        #endregion
    }//CswNbtObjClassRequest

}//namespace ChemSW.Nbt.ObjClasses
