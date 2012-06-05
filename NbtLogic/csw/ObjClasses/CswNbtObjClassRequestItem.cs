using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Mail;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequestItem : CswNbtObjClass
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
            public static readonly PropertyName Request = new PropertyName( "Request" );
            public static readonly PropertyName Type = new PropertyName( "Type" );
            public static readonly PropertyName Quantity = new PropertyName( "Quantity" );
            public static readonly PropertyName Size = new PropertyName( "Size" );
            public static readonly PropertyName Count = new PropertyName( "Count" );
            public static readonly PropertyName Material = new PropertyName( "Material" );
            public static readonly PropertyName Container = new PropertyName( "Container" );
            public static readonly PropertyName Comments = new PropertyName( "Comments" );
            public static readonly PropertyName Status = new PropertyName( "Status" );
            public static readonly PropertyName Number = new PropertyName( "Number" );
            public static readonly PropertyName ExternalOrderNumber = new PropertyName( "External Order Number" );
            public static readonly PropertyName Location = new PropertyName( "Location" );
            public static readonly PropertyName Unknown = new PropertyName( "Unknown" );
        }

        public sealed class Types : CswEnum<Types>
        {
            private Types( String Name ) : base( Name ) { }
            public static IEnumerable<Types> all { get { return All; } }
            public static explicit operator Types( string Str )
            {
                Types Ret = Parse( Str );
                return Ret ?? Unknown;
            }
            public static readonly Types Dispense = new Types( "Dispense" );
            public static readonly Types RequestBySize = new Types( "Request by Size" );
            public static readonly Types RequestByBulk = new Types( "Request by Bulk" );
            public static readonly Types Move = new Types( "Move" );
            public static readonly Types Dispose = new Types( "Dispose" );
            public static readonly Types Unknown = new Types( "Unknown" );
        }
        public static readonly CswCommaDelimitedString TypeOptions = new CswCommaDelimitedString
                                                                         {
                                                                             Types.Dispense.ToString(), Types.RequestByBulk.ToString(), Types.RequestBySize.ToString(), Types.Move.ToString(), Types.Dispose.ToString()
                                                                         };

        public sealed class Statuses : CswEnum<Statuses>
        {
            private Statuses( String Name ) : base( Name ) { }
            public static IEnumerable<Statuses> all { get { return All; } }
            public static explicit operator Statuses( string Str )
            {
                Statuses Ret = Parse( Str );
                return Ret ?? Unknown;
            }
            public static readonly Statuses Pending = new Statuses( "Pending" );
            public static readonly Statuses Submitted = new Statuses( "Submitted" );
            public static readonly Statuses Ordered = new Statuses( "Ordered" );
            public static readonly Statuses Received = new Statuses( "Received" );
            public static readonly Statuses Dispensed = new Statuses( "Dispensed" );
            public static readonly Statuses Completed = new Statuses( "Completed" );
            public static readonly Statuses Unknown = new Statuses( "Unknown" );
        }
        public static readonly CswCommaDelimitedString StatusOptions = new CswCommaDelimitedString
                                                                           {
                                                                               Statuses.Pending.ToString(),Statuses.Submitted.ToString(),Statuses.Ordered.ToString(),Statuses.Received.ToString(),Statuses.Dispensed.ToString(),Statuses.Completed.ToString()
                                                                           };

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public static implicit operator CswNbtObjClassRequestItem( CswNbtNode Node )
        {
            CswNbtObjClassRequestItem ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass ) )
            {
                ret = (CswNbtObjClassRequestItem) Node.ObjClass;
            }
            return ret;
        }

        public CswNbtObjClassRequestItem( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        private string _makeNotificationMessage()
        {
            string MessageText = "The Status for this Request Item has changed to: " + Status.Value + ". /n";

            CswNbtObjClassMaterial NodeAsMaterial = _CswNbtResources.Nodes.GetNode( Material.RelatedNodeId );
            if( null != NodeAsMaterial )
            {
                MessageText += "Material: " + NodeAsMaterial.TradeName + "/n";
            }

            CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
            if( null != NodeAsContainer )
            {
                MessageText += "Container: " + NodeAsContainer.Node.NodeName + "/n";
            }

            if( Quantity.Quantity > 0 )
            {
                MessageText += "Quantity: " + Quantity.Quantity + ", Unit: " + Quantity.CachedUnitName + "/n";
            }
            if( false == string.IsNullOrEmpty( Size.CachedNodeName ) )
            {
                MessageText += "Size: " + Size.CachedNodeName + "/n";
            }
            if( Count.Quantity > 0 )
            {
                MessageText += "Count: " + Count.Quantity + "/n";
            }
            CswNbtObjClassLocation NodeAsLocation = _CswNbtResources.Nodes.GetNode( Location.RelatedNodeId );
            if( null != NodeAsLocation )
            {
                MessageText += "Location: " + NodeAsLocation.Location + CswNbtNodePropLocation.PathDelimiter +
                                NodeAsLocation.Name + "/n";
            }


            return MessageText;
        }

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );

            CswNbtObjClassRequest NodeAsRequest = _CswNbtResources.Nodes.GetNode( Request.RelatedNodeId );

            if( ( Type.Value == Types.Dispense.ToString() ||
                Type.Value == Types.Move.ToString() ||
                Type.Value == Types.Dispose.ToString() ) &&
                null != Container.RelatedNodeId &&
                null != NodeAsRequest &&
                null != NodeAsRequest.InventoryGroup.RelatedNodeId )
            {
                CswNbtObjClassContainer NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                if( null == NodeAsContainer )
                {
                    throw new CswDniException( ErrorType.Error, "A " + Type.Value + " type of Request Item requires a valid Container.", "Attempted to edit node without a valid Container relationship." );
                }
                CswNbtObjClassLocation NodeAsLocation = _CswNbtResources.Nodes.GetNode( NodeAsContainer.Location.NodeId );
                if( null != NodeAsLocation && NodeAsRequest.InventoryGroup.RelatedNodeId != NodeAsLocation.InventoryGroup.RelatedNodeId )
                {
                    throw new CswDniException( ErrorType.Error, "For a " + Type.Value + " type of Request Item, the Inventory Group of the Request must match the Inventory Group of the Container's Location.", "Attempted to edit node without matching Container and Request Inventory Group relationships." );
                }
            }

            if( Status.WasModified &&
                Status.Value != Statuses.Pending.ToString() &&
                null != NodeAsRequest &&
                null != NodeAsRequest.Requestor.RelatedNodeId )
            {
                CswNbtObjClassUser RequestorAsUser = _CswNbtResources.Nodes.GetNode( NodeAsRequest.Requestor.RelatedNodeId );
                if( null != RequestorAsUser )
                {
                    string Subject = Node.NodeName + "'s Request Item Status has Changed to " + Status.Value;
                    string Message = _makeNotificationMessage();
                    string Recipient = RequestorAsUser.Email;
                    Collection<CswMailMessage> EmailMessage = _CswNbtResources.makeMailMessages( Subject, Message,
                                                                                                Recipient );
                    _CswNbtResources.sendEmailNotification( EmailMessage );
                }
            }

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
            CswNbtMetaDataObjectClassProp StatusOcp = ObjectClass.getObjectClassProp( PropertyName.Status.ToString() );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, StatusOcp, Statuses.Pending.ToString() );

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

        public CswNbtNodePropRelationship Request
        {
            get { return _CswNbtNode.Properties[PropertyName.Request.ToString()]; }
        }

        public CswNbtNodePropList Type
        {
            get { return _CswNbtNode.Properties[PropertyName.Type.ToString()]; }
        }

        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity.ToString()]; }
        }

        public CswNbtNodePropRelationship Size
        {
            get { return _CswNbtNode.Properties[PropertyName.Size.ToString()]; }
        }

        public CswNbtNodePropQuantity Count
        {
            get { return _CswNbtNode.Properties[PropertyName.Count.ToString()]; }
        }

        public CswNbtNodePropRelationship Material
        {
            get { return _CswNbtNode.Properties[PropertyName.Material.ToString()]; }
        }

        public CswNbtNodePropRelationship Container
        {
            get { return _CswNbtNode.Properties[PropertyName.Container.ToString()]; }
        }

        public CswNbtNodePropRelationship Location
        {
            get { return _CswNbtNode.Properties[PropertyName.Location.ToString()]; }
        }

        public CswNbtNodePropComments Comments
        {
            get { return _CswNbtNode.Properties[PropertyName.Comments.ToString()]; }
        }

        public CswNbtNodePropList Status
        {
            get { return _CswNbtNode.Properties[PropertyName.Status.ToString()]; }
        }

        public CswNbtNodePropSequence Number
        {
            get { return _CswNbtNode.Properties[PropertyName.Number.ToString()]; }
        }

        public CswNbtNodePropText ExternalOrderNumber
        {
            get { return _CswNbtNode.Properties[PropertyName.ExternalOrderNumber.ToString()]; }
        }

        #endregion
    }//CswNbtObjClassRequestItem

}//namespace ChemSW.Nbt.ObjClasses
