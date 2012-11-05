using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Container Dispense Transaction Object Class
    /// </summary>
    public class CswNbtObjClassContainerLocation : CswNbtObjClass
    {
        #region Static Properties

        public sealed class PropertyName
        {
            public const string Container = "Container";
            public const string Location = "Location";
            public const string Type = "Type";
            public const string ContainerScan = "Container Scan";
            public const string LocationScan = "Location Scan";
            public const string ScanDate = "Scan Date";
            public const string Status = "Status";
            public const string Action = "Action";
            public const string ActionApplied = "Action Applied";
            public const string User = "User";
        }

        #endregion

        #region Enums

        public sealed class ActionOptions : CswEnum<ActionOptions>
        {
            private ActionOptions( string Name ) : base( Name ) { }
            public static IEnumerable<ActionOptions> _All { get { return All; } }
            public static implicit operator ActionOptions( string str )
            {
                ActionOptions ret = Parse( str );
                return ret ?? NoAction;
            }
            //Null Option?
            public static readonly ActionOptions NoAction = new ActionOptions( "NoAction" );
            public static readonly ActionOptions Undispose = new ActionOptions( "Undispose" );
            public static readonly ActionOptions MoveToLocation = new ActionOptions( "MoveToLocation" );
            public static readonly ActionOptions UndisposeAndMove = new ActionOptions( "UndisposeAndMove" );
        }

        public sealed class TypeOptions : CswEnum<TypeOptions>
        {
            private TypeOptions( string Name ) : base( Name ) { }
            public static IEnumerable<TypeOptions> _All { get { return All; } }
            public static implicit operator TypeOptions( string str )
            {
                TypeOptions ret = Parse( str );
                return ret ?? Missing;
            }            
            public static readonly TypeOptions Scan = new TypeOptions( "Scan" );
            public static readonly TypeOptions Receipt = new TypeOptions( "Receipt" );
            public static readonly TypeOptions Move = new TypeOptions( "Move" );
            public static readonly TypeOptions Dispense = new TypeOptions( "Dispense" );
            public static readonly TypeOptions Missing = new TypeOptions( "Missing" );
        }

        public sealed class StatusOptions : CswEnum<StatusOptions>
        {
            private StatusOptions( string Name ) : base( Name ) { }
            public static IEnumerable<StatusOptions> _All { get { return All; } }
            public static implicit operator StatusOptions( string str )
            {
                StatusOptions ret = Parse( str );
                return ret ?? Missing;
            }
            public static readonly StatusOptions Correct = new StatusOptions( "Correct" );
            public static readonly StatusOptions Disposed = new StatusOptions( "Disposed" );
            public static readonly StatusOptions WrongLocation = new StatusOptions( "WrongLocation" );
            public static readonly StatusOptions DisposedAtWrongLocation = new StatusOptions( "DisposedAtWrongLocation" );
            public static readonly StatusOptions Missing = new StatusOptions( "Missing" );
        }

        #endregion

        #region ctor

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassContainerLocation( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassContainerLocation
        /// </summary>
        public static implicit operator CswNbtObjClassContainerLocation( CswNbtNode Node )
        {
            CswNbtObjClassContainerLocation ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.ContainerLocationClass ) )
            {
                ret = (CswNbtObjClassContainerLocation) Node.ObjClass;
            }
            return ret;
        }

        #endregion

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
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

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Container
        {
            get { return _CswNbtNode.Properties[PropertyName.Container]; }
        }
        public CswNbtNodePropLocation Location
        {
            get { return _CswNbtNode.Properties[PropertyName.Location]; }
        }
        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[PropertyName.Type]; }
        }
        public CswNbtNodePropText ContainerScan
        {
            get { return _CswNbtNode.Properties[PropertyName.ContainerScan]; }
        }
        public CswNbtNodePropText LocationScan
        {
            get { return _CswNbtNode.Properties[PropertyName.LocationScan]; }
        }
        public CswNbtNodePropDateTime ScanDate
        {
            get { return _CswNbtNode.Properties[PropertyName.ScanDate]; }
        }
        public CswNbtNodePropList Status
        {
            get { return _CswNbtNode.Properties[PropertyName.Status]; }
        }
        public CswNbtNodePropList Action
        {
            get { return _CswNbtNode.Properties[PropertyName.Action]; }
        }
        public CswNbtNodePropLogical ActionApplied
        {
            get { return _CswNbtNode.Properties[PropertyName.ActionApplied]; }
        }
        public CswNbtNodePropRelationship User
        {
            get { return _CswNbtNode.Properties[PropertyName.User]; }
        }

        #endregion

    }//CswNbtObjClassContainerLocation

}//namespace ChemSW.Nbt.ObjClasses

