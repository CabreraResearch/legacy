using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.csw.Schema
{
    class CswNbtSchemaUpdateLayoutMgr
    {
        private readonly CswNbtMetaDataObjectClass _ObjectClass = null;
        private string _TabName = string.Empty;
        private  CswEnumNbtLayoutType _LayoutType = CswEnumNbtLayoutType.Edit;
        private CswNbtSchemaModTrnsctn _SchemaModTrnsctn = null;

        private IEnumerable<CswNbtMetaDataNodeType> _LatestVersionNodeTypes = null;

        private class LayoutProp : IEquatable<LayoutProp>
        {
            public readonly CswNbtMetaDataObjectClassProp Prop;
            public readonly CswEnumNbtLayoutType Layout;
            public readonly string Tab;

            public LayoutProp( CswNbtMetaDataObjectClassProp Prop, CswEnumNbtLayoutType Layout, string Tab )
            {
                this.Prop = Prop;
                this.Layout = Layout;
                this.Tab = Tab;
            }

            public bool Equals( LayoutProp Other )
            {
                return this == Other;
            }

            public static bool operator ==( LayoutProp First, LayoutProp Second )
            {
                return ( null != (object) First && null != (object) Second && Second.Layout == First.Layout && Second.Prop == First.Prop && ( First.Layout != CswEnumNbtLayoutType.Edit || Second.Tab == First.Tab ) );
            }

            public static bool operator !=( LayoutProp First, LayoutProp Second )
            {
                return !( First == Second );
            }

            public override bool Equals( object obj )
            {
                if ( !( obj is LayoutProp ) )
                {
                    return false;
                }
                return this == (LayoutProp) obj;
            }
            public override int GetHashCode()
            {
                int ret = 23, prime = 37;
                ret = ( ret * prime ) + Prop.GetHashCode();
                ret = ( ret * prime ) + Layout.GetHashCode();
                return ret;
            }
        }
        private Collection<LayoutProp> Props = new Collection<LayoutProp>();

        public CswNbtSchemaUpdateLayoutMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, CswEnumNbtObjectClass ObjectClass, string TabName = "", CswEnumNbtLayoutType LayoutType = null )
        {
            if ( null == ObjectClass || CswResources.UnknownEnum == ObjectClass )
            {
                throw new CswDniException( "Cannot instance a Layout Mgr without an Object Class. Object Class was null." );
            }
            _TabName = TabName;
            _LayoutType = LayoutType;
            _SchemaModTrnsctn = SchemaModTrnsctn;
            _ObjectClass = _SchemaModTrnsctn.MetaData.getObjectClass( ObjectClass );
            _LatestVersionNodeTypes = _ObjectClass.getLatestVersionNodeTypes();
        }

        /// <summary>
        /// Get or Set the tab on which this instance will operate.
        /// </summary>
        public string CurrentTabName
        {
            get
            {
                return _TabName;
            }
            set
            {
                _TabName = value;
            }
        }

        /// <summary>
        /// Get or Set the Layout against which this instance will operate.
        /// </summary>
        public CswEnumNbtLayoutType CurrentLayoutType
        {
            get { return _LayoutType; }
            set { _LayoutType = value; }
        }

        private delegate CswNbtMetaDataNodeTypeTab getTab( CswNbtMetaDataNodeType NodeType, string TabName );

        private LayoutProp _vetLayoutOp( string PropName, bool DoNotTrack, string TabName = null, CswEnumNbtLayoutType LayoutType = null )
        {
            TabName = TabName ?? CurrentTabName;
            LayoutType = LayoutType ?? CurrentLayoutType;
            CswNbtMetaDataObjectClassProp Ocp = _ObjectClass.getObjectClassProp( PropName );
            if ( null == Ocp )
            {
                throw new CswDniException( "Cannot copy a property that is not an Object Class prop. Object Class Prop {" + PropName + "} was null." );
            }

            LayoutProp Ret = new LayoutProp( Ocp, LayoutType, TabName );
            if ( Props.Contains( Ret ) )
            {
                if ( DoNotTrack )
                {
                    Props.Remove( Ret );
                }
                else
                {
                    throw new CswDniException( "This property has already been placed in this layout. Prop: {" + PropName + "}, LayoutType: {" + LayoutType + "}, Tab: {" + TabName + "}." );
                }
            }

            if ( false == DoNotTrack )
            {
                Props.Add( Ret );
            }
            return Ret;
        }

        private void _doLayoutUpdate( string PropName, Int32 Row, Int32 Column, bool DoMove, bool DoNotTrack, string TabName = null, CswEnumNbtLayoutType LayoutType = null )
        {
            getTab GetTab = ( NodeType, Tab ) => NodeType.getNodeTypeTab( Tab ) ?? _SchemaModTrnsctn.MetaData.makeNewTabDeprecated( NodeType, Tab );
            _doLayoutUpdate( PropName, Row, Column, DoMove, DoNotTrack, GetTab: GetTab, TabName: TabName, LayoutType: LayoutType );
        }

        private void _doLayoutUpdate( string PropName, Int32 Row, Int32 Column, bool DoMove, bool DoNotTrack, getTab GetTab, string TabName = null, CswEnumNbtLayoutType LayoutType = null )
        {
            LayoutProp Prop = _vetLayoutOp( PropName, DoNotTrack, TabName, LayoutType );

            foreach ( CswNbtMetaDataNodeType NodeType in _LatestVersionNodeTypes )
            {
                CswNbtMetaDataNodeTypeTab Tab = GetTab( NodeType, Prop.Tab );
                CswNbtMetaDataNodeTypeProp Ntp = NodeType.getNodeTypePropByObjectClassProp( Prop.Prop );
                Ntp.updateLayout( Prop.Layout, TabId: Tab.TabId, DoMove: DoMove, DisplayRow: Row, DisplayColumn: Column );
            }
        }

        /// <summary>
        /// Copies a property to the tab. If the tab does not exist, this will create the tab.
        /// </summary>
        /// <param name="PropName"></param>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="TabName">Default to CurrentTabName</param>
        /// <param name="LayoutType">Default to CurrentLayoutType</param>
        public void copyPropToTab( string PropName, Int32 Row, Int32 Column, string TabName = null, CswEnumNbtLayoutType LayoutType = null )
        {
            _doLayoutUpdate( PropName, Row, Column, DoMove: false, DoNotTrack: false, TabName: TabName, LayoutType: LayoutType );
        }

        /// <summary>
        /// Copies a property to the Identity tab, which is only visible in the Edit layout.
        /// </summary>
        public void copyPropToIdentityTab( string PropName, Int32 Row, Int32 Column )
        {
            _doLayoutUpdate( PropName, Row, Column, DoMove: false, DoNotTrack: true, TabName: CswNbtMetaData.IdentityTabName, LayoutType: CswEnumNbtLayoutType.Edit );
        }

        /// <summary>
        /// Copies a property to the "first" tab, which should be the tab of the same name as the NodeType else the first tab by sequence.
        /// </summary>
        public void copyPropToFirstTab( string PropName, Int32 Row, Int32 Column, CswEnumNbtLayoutType LayoutType = null )
        {
            getTab GetTab = ( NodeType, Tab ) => NodeType.getNodeTypeTab( NodeType.NodeTypeName ) ?? NodeType.getFirstNodeTypeTab();
            _doLayoutUpdate( PropName, Row, Column, DoMove: false, DoNotTrack: true, GetTab: GetTab, LayoutType: LayoutType );
        }

        /// <summary>
        /// Moves a property on the "first" tab, which should be the tab of the same name as the NodeType else the first tab by sequence.
        /// </summary>
        public void movePropOnFirstTab( string PropName, Int32 Row, Int32 Column, CswEnumNbtLayoutType LayoutType = null )
        {
            getTab GetTab = ( NodeType, Tab ) => NodeType.getNodeTypeTab( NodeType.NodeTypeName ) ?? NodeType.getFirstNodeTypeTab();
            _doLayoutUpdate( PropName, Row, Column, DoMove: true, DoNotTrack: true, GetTab: GetTab, LayoutType: LayoutType );
        }

        /// <summary>
        /// Remove a property from the tab of a specific layout or from all tabs of a layout if the current tab is null.
        /// </summary>
        public void removePropFromLayout( string PropName, string TabName = null, CswEnumNbtLayoutType LayoutType = null )
        {
            LayoutProp Prop = _vetLayoutOp( PropName, true, TabName, LayoutType );

            foreach ( CswNbtMetaDataNodeType NodeType in _LatestVersionNodeTypes )
            {
                Int32 TabId = Int32.MinValue;
                CswNbtMetaDataNodeTypeTab Tab = NodeType.getNodeTypeTab( Prop.Tab );
                if ( null != Tab )
                {
                    TabId = Tab.TabId;
                }
                CswNbtMetaDataNodeTypeProp Ntp = NodeType.getNodeTypePropByObjectClassProp( Prop.Prop );
                Ntp.removeFromLayout( Prop.Layout, TabId );
            }
        }

        /// <summary>
        /// Remove a property only from the first tab of a specific layout.
        /// </summary>
        public void removePropFromLayoutFirstTab( string PropName, CswEnumNbtLayoutType LayoutType = null )
        {
            LayoutProp Prop = _vetLayoutOp( PropName, true, string.Empty, LayoutType );

            foreach ( CswNbtMetaDataNodeType NodeType in _LatestVersionNodeTypes )
            {
                CswNbtMetaDataNodeTypeTab Tab = NodeType.getFirstNodeTypeTab();
                if ( null != Tab )
                {
                    CswNbtMetaDataNodeTypeProp Ntp = NodeType.getNodeTypePropByObjectClassProp( Prop.Prop );
                    Ntp.removeFromLayout( Prop.Layout, Tab.TabId );
                }
            }
        }

        public void setTabPermission( CswEnumNbtNodeTypeTabPermission Permission, bool Value, Collection<CswNbtObjClassRole> Roles, string TabName = null )
        {
            TabName = TabName ?? CurrentTabName;
            foreach ( CswNbtMetaDataNodeType NodeType in _LatestVersionNodeTypes )
            {
                CswNbtMetaDataNodeTypeTab Tab = NodeType.getNodeTypeTab( TabName );
                if ( null != Tab )
                {
                    foreach ( CswNbtObjClassRole Role in Roles )
                    {
                        _SchemaModTrnsctn.Permit.set( Permission, Tab, Role, Value );
                    }
                }
            }
        }
    }
}
