using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Audit;

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

    }
}