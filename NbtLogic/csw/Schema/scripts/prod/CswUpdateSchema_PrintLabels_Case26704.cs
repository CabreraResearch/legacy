using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for PrintLabels_Case26704
    /// </summary>
    public class CswUpdateSchema_PrintLabels_Case26704 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp ControlTypeOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PrintLabelOc )
                {
                    PropName = CswNbtObjClassPrintLabel.PropertyName.ControlType,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    ListOptions = CswNbtObjClassPrintLabel.ControlTypes.Options.ToString(),
                    IsRequired = true
                } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ControlTypeOcp, ControlTypeOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtObjClassPrintLabel.ControlTypes.jZebra );

            foreach( CswNbtObjClassPrintLabel PrintLabel in PrintLabelOc.getNodes( forceReInit: true, includeSystemNodes: false ) )
            {
                if( null != PrintLabel )
                {
                    PrintLabel.ControlType.Value = CswNbtObjClassPrintLabel.ControlTypes.jZebra;
                    PrintLabel.postChanges( ForceUpdate: false );
                }
            }

            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ControlTypeOcp, ControlTypeOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtObjClassPrintLabel.ControlTypes.jZebra );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PrintLabelOc )
                {
                    PropName = CswNbtObjClassPrintLabel.PropertyName.LabelName,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = true
                } );

            foreach( CswNbtObjClassPrintLabel PrintLabel in PrintLabelOc.getNodes( forceReInit: true, includeSystemNodes: false ) )
            {
                if( null != PrintLabel )
                {
                    PrintLabel.ControlType.Value = CswNbtObjClassPrintLabel.ControlTypes.jZebra;
                }
            }

            CswNbtMetaDataNodeType PrintLabelNt = PrintLabelOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != PrintLabelNt )
            {
                CswNbtObjClassPrintLabel PrintLabel = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( PrintLabelNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, OverrideUniqueValidation: false );
                if( null != PrintLabel )
                {
                    PrintLabel.ControlType.Value = CswNbtObjClassPrintLabel.ControlTypes.jZebra;
                    //Whitespace is treated literally
                    PrintLabel.EplText.Text = @"I8,1,001
q664
S2
OD
JF
WN
D7
ZB
Q300,37
N
B41,85,0,3,3,8,50,N,""{Barcode}""
A41,140,0,3,1,1,N,""{Barcode}""
P1";
                    try
                    {
                        PrintLabel.LabelName.Text = "Default Barcode Label";
                    }
                    catch( Exception )
                    {
                        CswNbtMetaDataNodeTypeProp LabelNameNtp = PrintLabelNt.getNodeTypeProp( "Label Name" );
                        if( null != LabelNameNtp )
                        {
                            PrintLabel.Node.Properties[LabelNameNtp].AsText.Text = "Default Barcode Label";
                        }
                    }
                    CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
                    foreach( Int32 ContainerId in ContainerOc.getNodeTypeIds() )
                    {
                        PrintLabel.NodeTypes.SelectedNodeTypeIds.Add( ContainerId.ToString() );
                    }
                    PrintLabel.postChanges( ForceUpdate: false );
                }
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema




