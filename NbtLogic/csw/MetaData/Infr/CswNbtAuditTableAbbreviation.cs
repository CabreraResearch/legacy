using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Updates the schema for PL/SQL object changes
    /// </summary>
    public class CswNbtAuditTableAbbreviation
    {
        private static Dictionary<string, string> _AuditTables = new Dictionary<string, string>()
            {
                {"bda", "blob_data_audit"},
                {"jnpa", "jct_nodes_props_audit"},
                {"laa", "license_accept_audit"},
                {"na", "nodes_audit"},
                {"nta", "nodetypes_audit"},
                {"ntpa", "nodetype_props_audit"},
                {"ntla", "nodetype_layout_audit"},
                {"ntta", "nodetype_tabset_audit"},
                {"nva", "node_views_audit"},
                {"oca", "object_class_audit"},
                {"ocpa", "object_class_props_audit"}
            };

        public static ICollection<string> Abbreviations
        {
            get { return _AuditTables.Keys; }
        }

        public static string getAbbreviation( string AuditTableName )
        {
            return _AuditTables.FirstOrDefault( kvp => kvp.Value == AuditTableName ).Key;
        }

        public static string getAbbreviationForRealTable( string RealTableName )
        {
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            return getAbbreviation( CswAuditMetaData.makeAuditTableName( RealTableName ) );
        }

        public static string getAuditTableName( string Abbreviation )
        {
            return _AuditTables[Abbreviation];
        }

        public static string getAuditLookupFunctionName( string AuditTableName )
        {
            return "AuditLookup_" + getAbbreviation( AuditTableName );
        }

        public static string getAuditLookupFunctionNameForRealTable( string RealTableName )
        {
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            return getAuditLookupFunctionName( CswAuditMetaData.makeAuditTableName( RealTableName ) );
        }

        public static string getAuditTableSql( CswNbtResources CswNbtResources, string RealTableName, CswDateTime Date, Int32 NodeId = Int32.MinValue )
        {
            return getAuditTableSql( CswNbtResources, RealTableName, Date.ToDateTime(), NodeId );
        }

        public static bool requiresNodeId( string RealTableName )
        {
            // case 30982 - require a 'nodeid' param for some audit tables
            return ( RealTableName == "jct_nodes_props" ||
                     RealTableName == "nodes" );
        }

        public static string getAuditTableSql( CswNbtResources CswNbtResources, string RealTableName, DateTime Date, Int32 NodeId = Int32.MinValue )
        {
            string ret = "TABLE(" + getAuditLookupFunctionNameForRealTable( RealTableName );
            ret += "(" + CswNbtResources.getDbNativeDate( Date.AddSeconds( 1 ) );

            if( requiresNodeId( RealTableName ) )
            {
                if( NodeId < 0 )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid query", "NodeId is required to extract data from " + RealTableName + " audit table" );
                }
                ret += ", " + NodeId;
            }
            ret += "))";
            return ret;
        }
    }
}