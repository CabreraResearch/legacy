using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.ImportExport
{


    public class CswImporterExperimental : ICswImporter
    {

        CswNbtResources _CswNbtResources = null;
        CswNbtImportExportFrame _CswNbtImportExportFrame = null;
        public CswImportExportStatusReporter _CswImportExportStatusReporter = null;
        public CswImporterExperimental( CswNbtResources CswNbtResources, CswNbtImportExportFrame CswNbtImportExportFrame, CswImportExportStatusReporter CswImportExportStatusReporter )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtImportExportFrame = CswNbtImportExportFrame;
            _CswImportExportStatusReporter = CswImportExportStatusReporter;
        }


        /// <summary>
        /// Imports data from an Xml String
        /// </summary>
        /// <param name="IMode">Describes how data is to be treated when importing</param>
        /// <param name="XmlStr">Source XML string</param>
        /// <param name="ViewXml">Will be filled with the exported view's XML as String </param>
        /// <param name="ResultXml">Will be filled with an XML String record of new primary keys and references</param>
        /// <param name="ErrorLog">Will be filled with a summary of recoverable errors</param>
        public void ImportXml( ImportMode IMode, string XmlStr, ref string ViewXml, ref string ResultXml, ref string ErrorLog )
        {
            _CswImportExportStatusReporter.reportStatus( "Starting Import -- Experimental" );

            ErrorLog = string.Empty;

            //********** THIS TAKES ABOUT 5-10 MINUTES ON THE CABOT DATA :-( 
            //Dictionary<string, string> NodeTypePropNamesByNodeTypeNames = _CswNbtImportExportFrame.NodeTypePropsByNodeTypes;

            //********** Step one: create nodes (not properties) and update XML doc with nodeids
            foreach( XmlNode CurrentNode in _CswNbtImportExportFrame.Nodes )
            {

                string CurrentNodeTypeName = CurrentNode[CswNbtImportExportFrame._Attribute_NodeTypeName].InnerText;
                CswNbtMetaDataNodeType CurrentNodeType = _CswNbtResources.MetaData.getNodeType( CurrentNodeTypeName );
                if( null != CurrentNodeType )
                {
                }
                else
                {

                }//if-else current node's nodetype exists

                //CswNbtNode Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, true );

            }//iterate xml nodes



        } // ImportXml()



    } // class CswImporterExperimental

} // namespace ChemSW.Nbt
