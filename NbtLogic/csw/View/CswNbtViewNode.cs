using System;
using System.Collections;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{
    public abstract class CswNbtViewNode : System.IEquatable<CswNbtViewNode>
    {
        public abstract NbtViewNodeType ViewNodeType { get; }

        protected CswNbtResources _CswNbtResources;
        protected CswNbtView _View;
        public string UniqueId;

        public CswNbtView View
        {
            get { return _View; }
        }

        public CswNbtViewNode( CswNbtResources CswNbtResources, CswNbtView View )
        {
            _CswNbtResources = CswNbtResources;
            _View = View;
            UniqueId = View.GenerateUniqueId();
        }

        public static CswNbtViewNode makeViewNode( CswNbtResources CswNbtResources, CswNbtView View, CswDelimitedString ViewNodeString )
        {
            CswNbtViewNode newNode = null;
            NbtViewNodeType type;
            if( Enum.TryParse<NbtViewNodeType>( ViewNodeString[0], out type ) )
            {
                switch( type )
                {
                    case NbtViewNodeType.CswNbtViewRelationship:
                        newNode = new CswNbtViewRelationship( CswNbtResources, View, ViewNodeString );
                        break;
                    case NbtViewNodeType.CswNbtViewProperty:
                        newNode = new CswNbtViewProperty( CswNbtResources, View, ViewNodeString );
                        break;
                    case NbtViewNodeType.CswNbtViewPropertyFilter:
                        newNode = new CswNbtViewPropertyFilter( CswNbtResources, View, ViewNodeString );
                        break;
                    case NbtViewNodeType.CswNbtViewRoot:
                        newNode = new CswNbtViewRoot( CswNbtResources, View, ViewNodeString );
                        break;
                }
            }
            else
                throw new CswDniException( "Invalid ViewNode", "CswNbtViewNode.makeViewNode() got an invalid ViewNodeString: " + ViewNodeString.ToString() );

            return newNode;
        }

        public abstract override string ToString();

        public abstract string ArbitraryId
        {
            get;
            //set;
        }

        public abstract CswNbtViewNode Parent
        {
            get;
            set;
        }

        public abstract string TextLabel
        {
            get;
        }

        public abstract string IconFileName
        {
            get;
        }

        public void RemoveChild( CswNbtViewNode ChildNode )
        {
            bool bError = true;
            if( this is CswNbtViewRoot )
            {
                if( ChildNode is CswNbtViewRelationship )
                {
                    bError = false;
                    ( (CswNbtViewRoot) this ).removeChildRelationship( ( (CswNbtViewRelationship) ChildNode ) );
                }
            }
            else if( this is CswNbtViewRelationship )
            {
                if( ChildNode is CswNbtViewRelationship )
                {
                    bError = false;
                    ( (CswNbtViewRelationship) this ).removeChildRelationship( ( (CswNbtViewRelationship) ChildNode ) );
                }
                else if( ChildNode is CswNbtViewProperty )
                {
                    bError = false;
                    ( (CswNbtViewRelationship) this ).removeProperty( ( (CswNbtViewProperty) ChildNode ) );
                }
            }
            else if( this is CswNbtViewProperty )
            {
                if( ChildNode is CswNbtViewPropertyFilter )
                {
                    bError = false;
                    ( (CswNbtViewProperty) this ).removeFilter( ( (CswNbtViewPropertyFilter) ChildNode ) );
                }
            }

            if( bError )
                throw new CswDniException( "Invalid Operation", "CswNbtViewNode.RemoveChild attempted to perform an invalid RemoveChild" );
        }



        public void AddChild( CswNbtViewNode ChildNode )
        {
            bool bError = true;
            if( this is CswNbtViewRoot )
            {
                if( ChildNode is CswNbtViewRelationship )
                {
                    bError = false;
                    ( (CswNbtViewRoot) this ).addChildRelationship( ( (CswNbtViewRelationship) ChildNode ) );
                }
            }
            else if( this is CswNbtViewRelationship )
            {
                if( ChildNode is CswNbtViewRelationship )
                {
                    bError = false;
                    ( (CswNbtViewRelationship) this ).addChildRelationship( ( (CswNbtViewRelationship) ChildNode ) );
                }
                else if( ChildNode is CswNbtViewProperty )
                {
                    bError = false;
                    ( (CswNbtViewRelationship) this ).addProperty( ( (CswNbtViewProperty) ChildNode ) );
                }
            }
            else if( this is CswNbtViewProperty )
            {
                if( ChildNode is CswNbtViewPropertyFilter )
                {
                    bError = false;
                    ( (CswNbtViewProperty) this ).addFilter( ( (CswNbtViewPropertyFilter) ChildNode ) );
                }
            }

            if( bError )
                throw new CswDniException( "Invalid Operation", "CswNbtViewNode attempted to perform an invalid AddChild" );
        }


        public ICollection GetChildrenOfType( NbtViewNodeType ChildrenViewNodeType )
        {
            ICollection ret = null;
            bool bError = true;
            if( this is CswNbtViewRoot && ChildrenViewNodeType == NbtViewNodeType.CswNbtViewRelationship )
            {
                bError = false;
                ret = ( (CswNbtViewRoot) this ).ChildRelationships;
            }
            else if( this is CswNbtViewRelationship )
            {
                if( ChildrenViewNodeType == NbtViewNodeType.CswNbtViewRelationship )
                {
                    bError = false;
                    ret = ( (CswNbtViewRelationship) this ).ChildRelationships;
                }
                else if( ChildrenViewNodeType == NbtViewNodeType.CswNbtViewProperty )
                {
                    bError = false;
                    ret = ( (CswNbtViewRelationship) this ).Properties;
                }
            }
            else if( this is CswNbtViewProperty && ChildrenViewNodeType == NbtViewNodeType.CswNbtViewPropertyFilter )
            {
                bError = false;
                ret = ( (CswNbtViewProperty) this ).Filters;
            }

            if( bError )
                throw new CswDniException( "Invalid Operation", "CswNbtViewNode attempted to perform an invalid GetChildrenOfType with parameter: " + ChildrenViewNodeType.ToString() + " on ViewNode: " + this.ToString() );

            return ret;
        }

        /// <summary>
        /// Returns all children of a given ViewNodeType beneath this node in the view, recursively
        /// </summary>
        public ArrayList GetAllChildrenOfType( NbtViewNodeType ChildrenViewNodeType )
        {
            ArrayList Results = new ArrayList();
            GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, this, ref Results );
            return Results;
        }
        private void GetAllChildrenOfTypeRecursive( NbtViewNodeType ChildrenViewNodeType, CswNbtViewNode CurrentViewNode, ref ArrayList Results )
        {
            if( CurrentViewNode.ViewNodeType == ChildrenViewNodeType )
                Results.Add( CurrentViewNode );

            // Recurse
            switch( CurrentViewNode.ViewNodeType )
            {
                case NbtViewNodeType.CswNbtViewRoot:
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) CurrentViewNode ).ChildRelationships )
                        GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                    break;
                case NbtViewNodeType.CswNbtViewRelationship:
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) CurrentViewNode ).ChildRelationships )
                        GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                    foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) CurrentViewNode ).Properties )
                        GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                    break;
                case NbtViewNodeType.CswNbtViewProperty:
                    foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) CurrentViewNode ).Filters )
                        GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                    break;
                case NbtViewNodeType.CswNbtViewPropertyFilter:
                    break;
            }
        }

        //public CswNbtView.AddChildrenSetting getAddChildren()
        //{
        //    CswNbtView.AddChildrenSetting ret = CswNbtView.AddChildrenSetting.None;
        //    if (this is CswNbtViewRoot)
        //    {
        //        ret = ((CswNbtViewRoot)this).AddChildren;
        //    }
        //    else if (this is CswNbtViewRelationship)
        //    {
        //        ret = ((CswNbtViewRelationship)this).AddChildren;
        //    }
        //    return ret;
        //}

        public Collection<CswNbtViewAddNodeTypeEntry> AllowedChildNodeTypes( bool LimitToFirstGeneration )
        {
            Collection<CswNbtViewAddNodeTypeEntry> ret = new Collection<CswNbtViewAddNodeTypeEntry>();

            //NbtViewAddChildrenSetting AddChildrenSetting = NbtViewAddChildrenSetting.None;
            ArrayList ChildRelationships = new ArrayList();
            if( LimitToFirstGeneration && this is CswNbtViewRoot )
            {
                ChildRelationships.AddRange( ( (CswNbtViewRoot) this ).ChildRelationships );
            }
            else if( LimitToFirstGeneration && this is CswNbtViewRelationship )
            {
                //    AddChildrenSetting = ( (CswNbtViewRelationship) this ).AddChildren;
                ChildRelationships.AddRange( ( (CswNbtViewRelationship) this ).ChildRelationships );
            }
            else
            {
                ChildRelationships = _View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
            }

            //if( AddChildrenSetting == NbtViewAddChildrenSetting.InView )
            //{
            //foreach( CswNbtViewRelationship ChildRelationship in ChildRelationships )

            // BZ 10025 - Always show all nodetypes in view
            foreach( CswNbtViewRelationship ChildRelationship in ChildRelationships )
            {

                if( ChildRelationship.ShowInTree &&                   // BZ 8296
                    ChildRelationship.NodeIdsToFilterIn.Count == 0 )  // BZ 8022
                {
                    Collection<CswNbtMetaDataNodeType> PotentialNodeTypes = new Collection<CswNbtMetaDataNodeType>();
                    if( ChildRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( ChildRelationship.SecondId );
                        if( NodeType != null )
                            PotentialNodeTypes.Add( NodeType.FirstVersionNodeType );
                    }
                    else if( ChildRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
                    {
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ChildRelationship.SecondId );
                        if( ObjectClass != null )
                        {
                            foreach( CswNbtMetaDataNodeType PotentialNodeType in ObjectClass.NodeTypes )
                            {
                                PotentialNodeTypes.Add( PotentialNodeType.FirstVersionNodeType );
                            }
                        }
                    }

                    foreach( CswNbtMetaDataNodeType FirstVersionNodeType in PotentialNodeTypes )
                    {
                        CswNbtViewAddNodeTypeEntry NewEntry = new CswNbtViewAddNodeTypeEntry( FirstVersionNodeType.LatestVersionNodeType, ChildRelationship );
                        //if( !ret.Contains( NewEntry ) )
                        //{
                        if( ( ( FirstVersionNodeType.ObjectClass.ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.RoleClass &&
                                FirstVersionNodeType.ObjectClass.ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ) ||
                              _CswNbtResources.CurrentNbtUser.IsAdministrator() ) &&
                            _CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Create, FirstVersionNodeType ) )
                        {
                            // Only use the first view relationship found per nodetype
                            bool FoundMatch = false;
                            foreach( CswNbtViewAddNodeTypeEntry ExistingEntry in ret )
                            {
                                if( ExistingEntry.NodeType.FirstVersionNodeTypeId == NewEntry.NodeType.FirstVersionNodeTypeId )
                                    FoundMatch = true;
                            }
                            if( !FoundMatch )
                                ret.Add( NewEntry );
                        }
                        //}
                    }
                }
            }
            //}

            return ret;
        }

        public class CswNbtViewAddNodeTypeEntry
        {
            public CswNbtMetaDataNodeType NodeType;
            public CswNbtViewRelationship ViewRelationship;

            public CswNbtViewAddNodeTypeEntry( CswNbtMetaDataNodeType NT, CswNbtViewRelationship VR )
            {
                NodeType = NT;
                ViewRelationship = VR;
            }
        }

        public bool ContainsNodeType( Int32 NodeTypeID )
        {
            bool ReturnVal = false;

            Collection<CswNbtViewRelationship> Relationships = null;
            if( this is CswNbtViewRelationship )
                Relationships = ( (CswNbtViewRelationship) this ).ChildRelationships;
            else if( this is CswNbtViewRoot )
                Relationships = ( (CswNbtViewRoot) this ).ChildRelationships;

            if( Relationships != null )
            {
                foreach( CswNbtViewRelationship CurrentRelationship in Relationships )
                {
                    if(//((CswNbtViewRelationship.RelatedIdType.NodeTypeId == CurrentRelationship.FirstType) &&
                        // (CurrentRelationship.FirstId == NodeTypeID)) ||
                        ( ( CswNbtViewRelationship.RelatedIdType.NodeTypeId == CurrentRelationship.SecondType ) &&
                         ( CurrentRelationship.SecondId == NodeTypeID ) ) ||
                        ( ( CswNbtViewRelationship.RelatedIdType.ObjectClassId == CurrentRelationship.SecondType ) &&
                         ( CurrentRelationship.SecondId == _CswNbtResources.MetaData.getNodeType( NodeTypeID ).ObjectClass.ObjectClassId ) ) )
                    {
                        ReturnVal = true;
                        break;
                    }
                }
            }
            return ( ReturnVal );
        }

        #region IEquatable

        public static bool operator ==( CswNbtViewNode vn1, CswNbtViewNode vn2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( vn1, vn2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) vn1 == null ) || ( (object) vn2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            return ( vn1.ToString() == vn2.ToString() );
        }

        public static bool operator !=( CswNbtViewNode vn1, CswNbtViewNode vn2 )
        {
            return !( vn1 == vn2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtViewNode ) )
                return false;
            return this == (CswNbtViewNode) obj;
        }

        public bool Equals( CswNbtViewNode obj )
        {
            return this == (CswNbtViewNode) obj;
        }

        public override int GetHashCode()
        {
            int ret = 1;
            if( this is CswNbtViewRoot )
            {
            }
            else if( this is CswNbtViewRelationship )
            {
                ret = ( (CswNbtViewRelationship) this ).PropId;
            }
            else if( this is CswNbtViewProperty )
            {
                ret = ( (CswNbtViewRelationship) ( (CswNbtViewProperty) this ).Parent ).PropId;
            }
            else if( this is CswNbtViewPropertyFilter )
            {
                ret = ( (CswNbtViewRelationship) ( (CswNbtViewProperty) ( (CswNbtViewPropertyFilter) this ).Parent ).Parent ).PropId;
            }
            return ret;
        }
        #endregion IEquatable

    } // class CswNbtViewNode

} // namespace ChemSW.Nbt
