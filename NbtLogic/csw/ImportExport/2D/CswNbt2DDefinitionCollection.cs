using System.Collections.ObjectModel;
using System.Linq;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DDefinitionCollection : Collection<CswNbt2DDefinition>
    {
        public CswNbt2DDefinition bySheetName( string SheetName, bool CreateIfMissing = false )
        {
            if( false == SheetName.EndsWith( "$" ) )
            {
                SheetName += "$";
            }
            CswNbt2DDefinition BindingDef = this.FirstOrDefault( b => b.SheetName.ToLower() == SheetName.ToLower() );
            if( null == BindingDef && CreateIfMissing )
            {
                BindingDef = new CswNbt2DDefinition()
                {
                    SheetName = SheetName
                };
                this.Add( BindingDef );
            }
            return BindingDef;
        }

        public CswNbt2DDefinition byImportDataTableName( string ImportDataTableName )
        {
            return this.FirstOrDefault( b => b.ImportDataTableName.ToLower() == ImportDataTableName.ToLower() );
        }
    }
}
