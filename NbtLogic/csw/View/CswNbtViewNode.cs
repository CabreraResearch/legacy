using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{
    [DataContract]
    public abstract class CswNbtViewNode: System.IEquatable<CswNbtViewNode>
    {
        public abstract CswEnumNbtViewNodeType ViewNodeType { get; set; }

        protected CswNbtView _View;
        protected CswNbtResources _CswNbtResources
        {
            get
            {
                CswNbtResources ret = null;
                if( null != _View )
                {
                    ret = _View._CswNbtResources;
                }
                return ret;
            }
        }

        public string UniqueId;

        public CswNbtView View
        {
            get { return _View; }
            set { CswNbtView DummyView = value; }
        }

        public CswNbtViewNode( CswNbtResources CswNbtResources, CswNbtView View )
        {
            //_CswNbtResources = CswNbtResources;
            _View = View;
            UniqueId = View.GenerateUniqueId();
        }

        public static CswNbtViewNode makeViewNode( CswNbtResources CswNbtResources, CswNbtView View, CswDelimitedString ViewNodeString )
        {
            CswNbtViewNode newNode = null;
            CswEnumNbtViewNodeType type = (CswEnumNbtViewNodeType) ViewNodeString[0];
            if( type == CswEnumNbtViewNodeType.CswNbtViewRelationship )
            {
                newNode = new CswNbtViewRelationship( CswNbtResources, View, ViewNodeString );
            }
            else if( type == CswEnumNbtViewNodeType.CswNbtViewProperty )
            {
                newNode = new CswNbtViewProperty( CswNbtResources, View, ViewNodeString );
            }
            else if( type == CswEnumNbtViewNodeType.CswNbtViewPropertyFilter )
            {
                newNode = new CswNbtViewPropertyFilter( CswNbtResources, View, ViewNodeString );
            }
            else if( type == CswEnumNbtViewNodeType.CswNbtViewRoot )
            {
                newNode = new CswNbtViewRoot( CswNbtResources, View, ViewNodeString );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid ViewNode", "CswNbtViewNode.makeViewNode() got an invalid ViewNodeString: " + ViewNodeString.ToString() );
            }
            return newNode;
        } // makeViewNode()

        public abstract override string ToString();

        [DataMember]
        public abstract string ArbitraryId
        {
            get;
            set;
        }

        public abstract CswNbtViewNode Parent
        {
            get;
            set;
        }

        [DataMember]
        public abstract string TextLabel
        {
            get;
            set;
        }

        [DataMember]
        public abstract string IconFileName
        {
            get;
            set;
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
                throw new CswDniException( CswEnumErrorType.Error, "Invalid Operation", "CswNbtViewNode.RemoveChild attempted to perform an invalid RemoveChild" );
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
                throw new CswDniException( CswEnumErrorType.Error, "Invalid Operation", "CswNbtViewNode attempted to perform an invalid AddChild" );
        }


        public ICollection GetChildrenOfType( CswEnumNbtViewNodeType ChildrenViewNodeType )
        {
            ICollection ret = null;
            bool bError = true;
            if( this is CswNbtViewRoot && ChildrenViewNodeType == CswEnumNbtViewNodeType.CswNbtViewRelationship )
            {
                bError = false;
                ret = ( (CswNbtViewRoot) this ).ChildRelationships;
            }
            else if( this is CswNbtViewRelationship )
            {
                if( ChildrenViewNodeType == CswEnumNbtViewNodeType.CswNbtViewRelationship )
                {
                    bError = false;
                    ret = ( (CswNbtViewRelationship) this ).ChildRelationships;
                }
                else if( ChildrenViewNodeType == CswEnumNbtViewNodeType.CswNbtViewProperty )
                {
                    bError = false;
                    ret = ( (CswNbtViewRelationship) this ).Properties;
                }
            }
            else if( this is CswNbtViewProperty && ChildrenViewNodeType == CswEnumNbtViewNodeType.CswNbtViewPropertyFilter )
            {
                bError = false;
                ret = ( (CswNbtViewProperty) this ).Filters;
            }

            if( bError )
                throw new CswDniException( CswEnumErrorType.Error, "Invalid Operation", "CswNbtViewNode attempted to perform an invalid GetChildrenOfType with parameter: " + ChildrenViewNodeType.ToString() + " on ViewNode: " + this.ToString() );

            return ret;
        }

        /// <summary>
        /// Returns all children of a given ViewNodeType beneath this node in the view, recursively
        /// </summary>
        public ArrayList GetAllChildrenOfType( CswEnumNbtViewNodeType ChildrenViewNodeType )
        {
            ArrayList Results = new ArrayList();
            GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, this, ref Results );
            return Results;
        }

        private void GetAllChildrenOfTypeRecursive( CswEnumNbtViewNodeType ChildrenViewNodeType, CswNbtViewNode CurrentViewNode, ref ArrayList Results )
        {
            if( CurrentViewNode.ViewNodeType == ChildrenViewNodeType )
                Results.Add( CurrentViewNode );

            // Recurse
            if( CurrentViewNode.ViewNodeType == CswEnumNbtViewNodeType.CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) CurrentViewNode ).ChildRelationships )
                {
                    GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                }
            }
            else if( CurrentViewNode.ViewNodeType == CswEnumNbtViewNodeType.CswNbtViewRelationship )
            {
                foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) CurrentViewNode ).ChildRelationships )
                {
                    GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                }
                foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) CurrentViewNode ).Properties )
                {
                    GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                }
            }
            else if( CurrentViewNode.ViewNodeType == CswEnumNbtViewNodeType.CswNbtViewProperty )
            {
                foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) CurrentViewNode ).Filters )
                {
                    GetAllChildrenOfTypeRecursive( ChildrenViewNodeType, Child, ref Results );
                }
            }
        } // GetAllChildrenOfTypeRecursive()

        public Collection<CswNbtViewAddNodeTypeEntry> AllowedChildNodeTypes( bool LimitToFirstGeneration )
        {
            Collection<CswNbtViewAddNodeTypeEntry> ret = new Collection<CswNbtViewAddNodeTypeEntry>();

            ArrayList ChildRelationships = new ArrayList();
            if( LimitToFirstGeneration && this is CswNbtViewRoot )
            {
                ChildRelationships.AddRange( ( (CswNbtViewRoot) this ).ChildRelationships );
            }
            else if( LimitToFirstGeneration && this is CswNbtViewRelationship )
            {
                ChildRelationships.AddRange( ( (CswNbtViewRelationship) this ).ChildRelationships );
            }
            else
            {
                ChildRelationships = _View.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewRelationship );
            }

            foreach( CswNbtViewRelationship ChildRelationship in ChildRelationships )
            {

                if( ChildRelationship.ShowInTree &&                                          // BZ 8296
                    ChildRelationship.NodeIdsToFilterIn.Count == 0 &&                        // BZ 8022
                    ChildRelationship.AllowAdd )                                             // 28663
                {
                    Collection<CswNbtMetaDataNodeType> PotentialNodeTypes = new Collection<CswNbtMetaDataNodeType>();
                    if( ChildRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( ChildRelationship.SecondId );
                        if( NodeType != null )
                            PotentialNodeTypes.Add( NodeType.getFirstVersionNodeType() );
                    }
                    else if( ChildRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                    {
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ChildRelationship.SecondId );
                        if( ObjectClass != null )
                        {
                            foreach( CswNbtMetaDataNodeType PotentialNodeType in ObjectClass.getNodeTypes() )
                            {
                                PotentialNodeTypes.Add( PotentialNodeType.getFirstVersionNodeType() );
                            }
                        }
                    }
                    else if( ChildRelationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                    {
                        foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClassesByPropertySetId( ChildRelationship.SecondId ) )
                        {
                            foreach( CswNbtMetaDataNodeType PotentialNodeType in ObjectClass.getNodeTypes() )
                            {
                                PotentialNodeTypes.Add( PotentialNodeType.getFirstVersionNodeType() );
                            }
                        }
                    }

                    foreach( CswNbtMetaDataNodeType FirstVersionNodeType in PotentialNodeTypes )
                    {
                        CswNbtViewAddNodeTypeEntry NewEntry = new CswNbtViewAddNodeTypeEntry( FirstVersionNodeType.getNodeTypeLatestVersion(), ChildRelationship );
                        CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.getObjectClass();
                        if( ObjectClass.CanAdd &&
                            _CswNbtResources.Permit.canNodeType( Security.CswEnumNbtNodeTypePermission.Create, FirstVersionNodeType ) )
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
                    }
                }
            }

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
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeID );
                foreach( CswNbtViewRelationship CurrentRelationship in Relationships )
                {
                    //if( ( NbtViewRelatedIdType.NodeTypeId == CurrentRelationship.SecondType ) &&
                    //    ( CurrentRelationship.SecondId == NodeTypeID ) ) ||
                    //  ( ( NbtViewRelatedIdType.ObjectClassId == CurrentRelationship.SecondType ) &&
                    //    ( CurrentRelationship.SecondId == _CswNbtResources.MetaData.getNodeType( NodeTypeID ).ObjectClassId ) ) )
                    if( CurrentRelationship.SecondMatches( NodeType ) )
                    {
                        ReturnVal = true;
                        break;
                    }
                }
            }
            return ( ReturnVal );
        }

        /// <summary>
        /// Sets the internal View
        /// </summary>
        /// <remarks>
        /// We can't add a [DataMember] tag to "_View" because it will cause an infinite loop during serialization.
        /// Our handler for Wcf is responsible for setting this to the incoming view.
        /// </remarks>
        public void SetViewRootView( CswNbtView View )
        {
            if( null == _View )
            {
                _View = View;
            }
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
