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
        public readonly CswNbtMetaDataObjectClass ObjectClass;
        public  CswEnumNbtLayoutType LayoutType;
        public CswNbtSchemaModTrnsctn SchemaModTrnsctn;

        public IEnumerable<CswNbtMetaDataNodeType> LatestVersionNodeTypes;

        public class LayoutProp : IEquatable<LayoutProp>
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
        public Collection<LayoutProp> Props;
        public CswNbtSchemaUpdateLayoutMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, CswEnumNbtObjectClass ObjectClass, CswEnumNbtLayoutType LayoutType = null )
        {
            if ( null == ObjectClass || CswResources.UnknownEnum == ObjectClass )
            {
                throw new CswDniException( "Cannot instance a Layout Mgr without an Object Class. Object Class was null." );
            }

            this.LayoutType = LayoutType;
            this.SchemaModTrnsctn = SchemaModTrnsctn;
            this.ObjectClass = this.SchemaModTrnsctn.MetaData.getObjectClass( ObjectClass );
            LatestVersionNodeTypes = this.ObjectClass.getLatestVersionNodeTypes();
            Props = new Collection<LayoutProp>();
        }

        public delegate CswNbtMetaDataNodeTypeTab getTab( CswNbtSchemaModTrnsctn SchemaModTrnsctn, CswNbtMetaDataNodeType NodeType );

        public CswNbtSchemaUpdateLayoutMgrHlprImpl this[string TabName]
        {
            get
            {
                getTab GetTab = ( SchemaModTrnsctn, NodeType ) => NodeType.getNodeTypeTab( TabName ) ?? SchemaModTrnsctn.MetaData.makeNewTabDeprecated( NodeType, TabName );
                return new CswNbtSchemaUpdateLayoutMgrHlprImpl( this, GetTab );
            }
        }

        public CswNbtSchemaUpdateLayoutMgrHlprImpl First
        {
            get
            {
                getTab GetTab = ( SchemaModTrnsctn, NodeType ) => NodeType.getNodeTypeTab( NodeType.NodeTypeName ) ?? NodeType.getFirstNodeTypeTab();
                return new CswNbtSchemaUpdateLayoutMgrHlprImpl( this, GetTab );
            }
        }

        public CswNbtSchemaUpdateLayoutMgrHlprImpl Second
        {
            get
            {
                getTab GetTab = ( SchemaModTrnsctn, NodeType ) => NodeType.getSecondNodeTypeTab();
                return new CswNbtSchemaUpdateLayoutMgrHlprImpl( this, GetTab );
            }
        }

        public CswNbtSchemaUpdateLayoutMgrHlprImpl Identity
        {
            get
            {
                getTab GetTab = ( SchemaModTrnsctn, NodeType ) => NodeType.getIdentityTab();
                return new CswNbtSchemaUpdateLayoutMgrHlprImpl( this, GetTab );
            }
        }

    }


}
