using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31396 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31396; }
        }

        public override string AppendToScriptName()
        {
            return "DSD Picto paths";
        }

        public override void update()
        {
            // Fix DSD Picto Paths
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {

                CswDelimitedString PictoPaths = new CswDelimitedString( '\n' )
                    {
                        "Images/cispro/DSD/512/none.gif",
                        "Images/cispro/DSD/512/e.gif",
                        "Images/cispro/DSD/512/o.gif",
                        "Images/cispro/DSD/512/f.gif",
                        "Images/cispro/DSD/512/f_plus.gif",
                        "Images/cispro/DSD/512/t.gif",
                        "Images/cispro/DSD/512/t_plus.gif",
                        "Images/cispro/DSD/512/xn.gif",
                        "Images/cispro/DSD/512/xi.gif",
                        "Images/cispro/DSD/512/c.gif",
                        "Images/cispro/DSD/512/n.gif"
                    };
                CswNbtMetaDataNodeTypeProp PictogramsNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.Pictograms );
                PictogramsNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleImageList.AttributeName.ImageUrls].AsMemo.Text = PictoPaths.ToString();
                PictogramsNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleImageList.AttributeName.HeightInPixels].AsNumber.Value = 77;
                PictogramsNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleImageList.AttributeName.WidthInPixels].AsNumber.Value = 77;
                PictogramsNTP.DesignNode.postChanges( false );

                foreach( CswNbtObjClassChemical ChemicalNode in ChemicalNT.getNodes( false, true ) )
                {
                    CswDelimitedString oldVals = ChemicalNode.Pictograms.Value;
                    CswDelimitedString newVals = new CswDelimitedString( CswNbtNodePropImageList.Delimiter );
                    foreach( string oldVal in oldVals )
                    {
                        if( oldVal.IndexOf( "/DSD/" ) >= 0 && oldVal.IndexOf( "/DSD/512" ) == 0 )
                        {
                            newVals.Add( oldVal.Replace( "/DSD/", "/DSD/512/" ) );
                        }
                        else
                        {
                            newVals.Add( oldVal );
                        }
                    }
                    ChemicalNode.Pictograms.Value = newVals;
                    ChemicalNode.postChanges( false );
                } // foreach( CswNbtObjClassChemical ChemicalNode in ChemicalNT.getNodes( false, true ) )
            } // foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )

            // Example DSD Print Label
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass PrintLabelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
            if( null != PrintLabelOC )
            {
                CswNbtMetaDataNodeType PrintLabelNT = PrintLabelOC.FirstNodeType;
                if( null != PrintLabelNT )
                {
                    CswNbtObjClassPrintLabel DsdLabel = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( PrintLabelNT.NodeTypeId );
                    DsdLabel.LabelName.Text = "Example DSD Label";
                    if( null != ContainerOC )
                    {
                        foreach(CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes())
                        {
                            DsdLabel.NodeTypes.SelectedNodeTypeIds.Add( ContainerNT.NodeTypeId.ToString() );
                        }
                    }
                    DsdLabel.EplText.Text = @"I8,0,001
S2
OD
JF
WN
D12
ZB
q375
N
A25,15,0,2,1,1,N,""{Material}""
B25,45,0,1,3,6,40,N,""{Barcode}""
A25,80,0,2,1,1,N,""{Barcode}""
A25,130,0,2,1,1,N,""{NBTDSDB}""
GW0,160,{NBTDSDPICTOS}
GW128,160,{NBTDSDPICTOS_2}
GW256,160,{NBTDSDPICTOS_3}
P1";
                    DsdLabel.postChanges( false );
                }
            }


        } // update()
    } // class CswUpdateSchema_02K_Case31396
} // namespace