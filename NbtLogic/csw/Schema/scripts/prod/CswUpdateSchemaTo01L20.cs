using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-20
    /// </summary>
    public class CswUpdateSchemaTo01L20 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 20 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            #region Case 24929

            CswNbtMetaDataObjectClass MailReportOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            foreach( CswNbtNode ReportNode in MailReportOc.getNodes( true, false ) )
            {
                CswNbtObjClassMailReport NodeAsMailReport = CswNbtNodeCaster.AsMailReport( ReportNode );
                if( NodeAsMailReport.ReportView.SelectedViewIds.Count > 1 )
                {
                    Int32 ValidViewId = Int32.MinValue;
                    foreach( Int32 Vid in NodeAsMailReport.ReportView.SelectedViewIds.ToIntCollection() )
                    {
                        if( Int32.MinValue != Vid )
                        {
                            CswNbtViewId ViewId = new CswNbtViewId();
                            ViewId.set( Vid );
                            CswNbtView View = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( ViewId );
                            if( null != View )
                            {
                                ValidViewId = Vid;
                                break;
                            }
                        }
                    }
                    NodeAsMailReport.ReportView.SelectedViewIds.Clear();
                    if( Int32.MinValue != ValidViewId )
                    {
                        NodeAsMailReport.ReportView.SelectedViewIds.Add( ValidViewId.ToString() );
                    }
                    ReportNode.postChanges( true );
                }
            }

            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            foreach( CswNbtNode GeneratorNode in GeneratorOc.getNodes( true, false ) )
            {
                CswNbtObjClassGenerator NodeAsGenerator = CswNbtNodeCaster.AsGenerator( GeneratorNode );
                if( NodeAsGenerator.ParentType.SelectedNodeTypeIds.Count > 1 )
                {
                    Int32 ValidNodeTypeId = Int32.MinValue;
                    foreach( Int32 Nid in NodeAsGenerator.ParentType.SelectedNodeTypeIds.ToIntCollection() )
                    {
                        if( Int32.MinValue != Nid )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( Nid );
                            if( null != NodeType )
                            {
                                ValidNodeTypeId = Nid;
                                break;
                            }
                        }
                    }
                    NodeAsGenerator.ParentType.SelectedNodeTypeIds.Clear();
                    if( Int32.MinValue != ValidNodeTypeId )
                    {
                        NodeAsGenerator.ParentType.SelectedNodeTypeIds.Add( ValidNodeTypeId.ToString() );
                    }
                    GeneratorNode.postChanges( true );
                }

                if( NodeAsGenerator.TargetType.SelectedNodeTypeIds.Count > 1 )
                {
                    Int32 ValidNodeTypeId = Int32.MinValue;
                    foreach( Int32 Nid in NodeAsGenerator.TargetType.SelectedNodeTypeIds.ToIntCollection() )
                    {
                        if( Int32.MinValue != Nid )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( Nid );
                            if( null != NodeType )
                            {
                                ValidNodeTypeId = Nid;
                                break;
                            }
                        }
                    }
                    NodeAsGenerator.TargetType.SelectedNodeTypeIds.Clear();
                    if( Int32.MinValue != ValidNodeTypeId )
                    {
                        NodeAsGenerator.TargetType.SelectedNodeTypeIds.Add( ValidNodeTypeId.ToString() );
                    }
                    GeneratorNode.postChanges( true );
                }

            }

            #endregion Case 24929


        }//Update()

    }//class CswUpdateSchemaTo01L20

}//namespace ChemSW.Nbt.Schema


