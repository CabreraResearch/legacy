using System;
using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30561 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Make Deficient Inspections an SI Report"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30561; }
        }

        public override string ScriptName
        {
            get { return "Case30561"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ReportClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtObjClassReport DoomedNode = null;

            CswNbtMetaDataNodeType SiReportNt = ReportClass.getNodeTypes().FirstOrDefault( NodeType => NodeType.NodeTypeName == "SI Report" );
            if( null != SiReportNt )
            {
                foreach( CswNbtObjClassReport Report in ReportClass.getNodes( true, false, false, false ) )
                {
                    if( Report.NodeType != SiReportNt && Report.ReportName.Text == "Deficient Inspections (Demo)" )
                    {
                        DoomedNode = Report;
                    }
                }

                if( null != DoomedNode )
                {
                    CswNbtObjClassReport NewNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SiReportNt.NodeTypeId );
                    foreach( CswNbtNodePropWrapper NodePropWrapper in DoomedNode.Node.Properties )
                    {
                        if( Int32.MinValue != NodePropWrapper.ObjectClassPropId )
                        {
                            JObject OldVals = new JObject();
                            NodePropWrapper.ToJSON( OldVals );
                            NewNode.Node.Properties[NodePropWrapper.ObjectClassPropName].ReadJSON( OldVals, null, null );
                        }
                    }
                    NewNode.postChanges( ForceUpdate: false );
                    DoomedNode.Node.delete( DeleteAllRequiredRelatedNodes: true, OverridePermissions: true );
                }
            }
        }
    }
}