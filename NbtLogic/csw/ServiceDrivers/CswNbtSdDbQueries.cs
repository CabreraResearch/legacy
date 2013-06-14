using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdDbQueries
    {
        [DataContract]
        public class Tables
        {
            public Tables()
            {
                NodeTypes = new Collection<CswNbtMetaDataNodeType>();
                ObjectClasses = new Collection<CswNbtMetaDataObjectClass>();
            }

            [DataMember]
            public Collection<CswNbtMetaDataNodeType> NodeTypes;
            
            [DataMember]
            public Collection<CswNbtMetaDataObjectClass> ObjectClasses;
        }

        [DataContract]
        public class Column
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string DbName;
            
            [DataMember]
            public string Type;
        }

        [DataContract]
        public class Columns
        {
            public Columns()
            {
                ColumnCol = new Collection<Column>();
            }
            
            [DataMember]
            public string Tablename;
            
            [DataMember( Name = "Columns" )]
            public Collection<Column> ColumnCol;
        }

        private CswNbtResources _CswNbtResources;

        public CswNbtSdDbQueries( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        public static void getTables( ICswResources CswResources, Tables Response, string Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                foreach( CswNbtMetaDataNodeType NodeType in from _NodeType in NbtResources.MetaData.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
                {
                    Response.NodeTypes.Add( NodeType );
                }
                foreach( CswNbtMetaDataObjectClass ObjectClass in from _ObjectClass in NbtResources.MetaData.getObjectClasses() orderby _ObjectClass.ObjectClass select _ObjectClass )
                {
                    Response.ObjectClasses.Add( ObjectClass );
                }
            }
        }

        public static void getOcColumns( ICswResources CswResources, Columns Response, Int32 ObjectClassId )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtMetaDataObjectClass ObjectClass = NbtResources.MetaData.getObjectClass( ObjectClassId );
                if( null != ObjectClass )
                {
                    Response.Tablename = ObjectClass.DbViewName;

                    Column NodeId = new Column();
                    NodeId.Name = ObjectClass.ObjectClass + "Id";
                    NodeId.DbName = "nodeid";
                    NodeId.Type = "number(12,0)";
                    Response.ColumnCol.Add( NodeId );

                    foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in from _ObjectClass in ObjectClass.getObjectClassProps() orderby _ObjectClass.PropName select _ObjectClass )
                    {
                        foreach( Column Column in ObjectClassProp.DbViewColumns )
                        {
                            Response.ColumnCol.Add( Column );
                        }
                    }
                }
            }
        }

    } // public class CswNbtSdDbQueries

} // namespace ChemSW.Nbt.ServiceDrivers
