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
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PictogramsNTP = ChemicalNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.Pictograms );

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
                PictogramsNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleImageList.AttributeName.ImageUrls].AsMemo.Text = PictoPaths.ToString();

                foreach( CswNbtObjClassChemical ChemicalNode in ChemicalNT.getNodes( false, true ) )
                {
                    CswDelimitedString oldVals = ChemicalNode.Pictograms.Value;
                    CswDelimitedString newVals = new CswDelimitedString( CswNbtNodePropImageList.Delimiter );
                    foreach( string oldVal in oldVals )
                    {
                        if( oldVal.IndexOf( "/DSD/" ) >= 0 )
                        {
                            newVals.Add( oldVal.Replace( "/DSD/", "/DSD/512/" ) );
                        }
                    }
                    ChemicalNode.Pictograms.Value = newVals;
                    ChemicalNode.postChanges( false );
                } // foreach( CswNbtObjClassChemical ChemicalNode in ChemicalNT.getNodes( false, true ) )
            } // foreach( CswNbtMetaDataNodeType ChemicalNT in ChemicalOC.getNodeTypes() )

        } // update()
    } // class CswUpdateSchema_02K_Case31396
} // namespace