using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ImportExport
{

    public class CswNbtImportNodeId
    {
        public CswNbtImportNodeId( string ImportNodeId )
        {

            string[] SplitRelatedNodeID = ImportNodeId.Split( new string[] { "--" }, StringSplitOptions.None );
            if( SplitRelatedNodeID.Length >= 2 )
            {
                _NodeNodeIdType = SplitRelatedNodeID[0];
                _NodeNodeType = SplitRelatedNodeID[1];
                _NodeNodeName = SplitRelatedNodeID.Length >= 3 ? SplitRelatedNodeID[2] : string.Empty;
            }

        }//ctor

        private string _NodeNodeIdType = string.Empty;
        private string _NodeNodeType = string.Empty;
        private string _NodeNodeName = string.Empty;

        public string NodeNodeIdType { get { return ( _NodeNodeIdType ); } }
        public string NodeNodeType { get { return ( _NodeNodeType ); } }
        public string NodeNodeName { get { return ( _NodeNodeName ); } }

        public bool IsNull { get { return ( ( false == String.IsNullOrEmpty( _NodeNodeIdType ) ) && ( false == String.IsNullOrEmpty( _NodeNodeName ) ) && ( false == String.IsNullOrEmpty( _NodeNodeType ) ) ); } }


    }//class CswNbtImportNodeId

}//namespace ChemSW.Nbt.ImportExport
