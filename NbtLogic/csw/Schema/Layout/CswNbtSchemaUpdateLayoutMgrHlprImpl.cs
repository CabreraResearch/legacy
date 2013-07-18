using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using System;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.csw.Schema
{
    struct CswNbtSchemaUpdateLayoutMgrHlprImpl : ICswNbtSchemaUpdateLayoutMgrHlpr
    {
        private CswNbtSchemaUpdateLayoutMgr _LayoutMgr;
        private CswNbtSchemaUpdateLayoutMgr.getTab _getTab;
        
        public CswNbtSchemaUpdateLayoutMgrHlprImpl( CswNbtSchemaUpdateLayoutMgr LayoutMgr, CswNbtSchemaUpdateLayoutMgr.getTab GetTab )
        {
            _LayoutMgr = LayoutMgr;
            _getTab = GetTab;
        }

        private CswNbtSchemaUpdateLayoutMgr.LayoutProp _vetLayoutOp( string PropName, bool DoNotTrack, CswEnumNbtLayoutType LayoutType = null )
        {
            LayoutType = LayoutType ?? _LayoutMgr.LayoutType;
            CswNbtMetaDataObjectClassProp Ocp = _LayoutMgr.ObjectClass.getObjectClassProp( PropName );
            if ( null == Ocp )
            {
                throw new CswDniException( "Cannot copy a property that is not an Object Class prop. Object Class Prop {" + PropName + "} was null." );
            }

            CswNbtSchemaUpdateLayoutMgr.LayoutProp Ret = new CswNbtSchemaUpdateLayoutMgr.LayoutProp( Ocp, LayoutType, "" );
            //I originally thought we could prevent copy/paste errors by tracking which props had been positioned where, but this turns out to be non trivial.
            //It's not a bad idea, but not worth the overhead to do correctly right now.

            //if ( _LayoutMgr.Props.Contains( Ret ) )
            //{
            //    if ( DoNotTrack )
            //    {
            //        _LayoutMgr.Props.Remove( Ret );
            //    }
            //    else
            //    {
            //        throw new CswDniException( "This property has already been placed in this layout. Prop: {" + PropName + "}, LayoutType: {" + LayoutType + "}, Tab: {" + CurrentTabName + "}." );
            //    }
            //}

            //if ( false == DoNotTrack )
            //{
            //    _LayoutMgr.Props.Add( Ret );
            //}
            return Ret;
        }

        private void _doLayoutUpdate( string PropName, Int32 Row, Int32 Column, bool DoMove, bool DoNotTrack, CswEnumNbtLayoutType LayoutType = null )
        {
            CswNbtSchemaUpdateLayoutMgr.LayoutProp Prop = _vetLayoutOp( PropName, DoNotTrack, LayoutType );

            foreach ( CswNbtMetaDataNodeType NodeType in _LayoutMgr.LatestVersionNodeTypes )
            {
                CswNbtMetaDataNodeTypeTab Tab = _getTab( _LayoutMgr.SchemaModTrnsctn, NodeType );
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
        public void copyProp( string PropName, Int32 Row, Int32 Column, CswEnumNbtLayoutType LayoutType = null )
        {
            _doLayoutUpdate( PropName, Row, Column, DoMove: false, DoNotTrack: false, LayoutType: LayoutType );
        }

        /// <summary>
        /// Moves a property on the "first" tab, which should be the tab of the same name as the NodeType else the first tab by sequence.
        /// </summary>
        public void moveProp( string PropName, Int32 Row, Int32 Column, CswEnumNbtLayoutType LayoutType = null )
        {
            _doLayoutUpdate( PropName, Row, Column, DoMove: true, DoNotTrack: true, LayoutType: LayoutType );
        }

        /// <summary>
        /// Remove a property from the tab of a specific layout or from all tabs of a layout if the current tab is null.
        /// </summary>
        public void removeProp( string PropName, CswEnumNbtLayoutType LayoutType = null )
        {
            CswNbtSchemaUpdateLayoutMgr.LayoutProp Prop = _vetLayoutOp( PropName, true, LayoutType );

            foreach ( CswNbtMetaDataNodeType NodeType in _LayoutMgr.LatestVersionNodeTypes )
            {
                CswNbtMetaDataNodeTypeTab Tab = _getTab( _LayoutMgr.SchemaModTrnsctn, NodeType );
                if ( null != Tab )
                {
                    CswNbtMetaDataNodeTypeProp Ntp = NodeType.getNodeTypePropByObjectClassProp( Prop.Prop );
                    Ntp.removeFromLayout( Prop.Layout, Tab.TabId );
                }
            }
        }

        public void setPermit( CswEnumNbtNodeTypeTabPermission Permission, bool Value, Collection<CswNbtObjClassRole> Roles )
        {
            foreach ( CswNbtMetaDataNodeType NodeType in _LayoutMgr.LatestVersionNodeTypes )
            {
                CswNbtMetaDataNodeTypeTab Tab = _getTab( _LayoutMgr.SchemaModTrnsctn, NodeType );
                if ( null != Tab )
                {
                    foreach ( CswNbtObjClassRole Role in Roles )
                    {
                        _LayoutMgr.SchemaModTrnsctn.Permit.set( Permission, Tab, Role, Value );
                    }
                }
            }
        }
    }
}
