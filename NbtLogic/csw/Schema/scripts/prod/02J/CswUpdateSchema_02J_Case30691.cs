using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case30691 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30691; }
        }

        public override string Title
        {
            get { return "Add aliases to units of measure"; }
        }

        public override string AppendToScriptName()
        {
            return "V3";
        }

        public override void update()
        {
            // Set aliases for Units of Measurement nodes
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            foreach( CswNbtObjClassUnitOfMeasure UoMNode in UoMOC.getNodes( false, false ) )
            {
                switch( UoMNode.Name.Text )
                {
                    case "Each":
                        _updateAliasesValue( UoMNode, "each,ea,EA" );
                        break;
                    case "fluid ounces":
                        _updateAliasesValue( UoMNode, "fl oz,fl.oz." );
                        break;
                    case "g":
                        _updateAliasesValue( UoMNode, "G,gm,GM" );
                        break;
                    case "gal":
                        _updateAliasesValue( UoMNode, "Gal,GL,GA" );
                        break;
                    case "kg":
                        _updateAliasesValue( UoMNode, "KG" );
                        break;
                    case "Liters":
                        _updateAliasesValue( UoMNode, "L,LT" );
                        break;
                    case "mg":
                        _updateAliasesValue( UoMNode, "MG" );
                        break;
                    case "mL":
                        _updateAliasesValue( UoMNode, "ml,ML" );
                        break;
                    case "ounces":
                        _updateAliasesValue( UoMNode, "oz,OZ" );
                        break;
                    case "µL":
                        _updateAliasesValue( UoMNode, "microliter,microL,UL,uL" );
                        break;
                }
            }

        } // update()

        private void _updateAliasesValue( CswNbtObjClassUnitOfMeasure UoMNode, string NewAliases )
        {
            CswCommaDelimitedString UpdatedAliases = UoMNode.AliasesAsDelimitedString;
            CswCommaDelimitedString NewAliasesCommaDelimited = new CswCommaDelimitedString( NewAliases );
            foreach( string Alias in NewAliasesCommaDelimited )
            {
                if( false == UoMNode.AliasesAsDelimitedString.Contains( Alias ) )
                {
                    UpdatedAliases.Add( Alias );
                }
            }

            // Update the property value
            UoMNode.Aliases.Text = CswConvert.ToString( UpdatedAliases );
            UoMNode.postChanges( false );
        }//_updateAliasesValue()

    }

}//namespace ChemSW.Nbt.Schema