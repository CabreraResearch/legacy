using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DBindingCollection : Collection<CswNbt2DBinding>
    {
        public IEnumerable<CswNbt2DBinding> byProp( string Instance, CswNbtMetaDataNodeTypeProp Prop, CswNbtSubField Subfield = null )
        {
            return this.Where( b => b.DestProperty == Prop &&
                                    ( Subfield == null || b.DestSubfield == Subfield ) &&
                                    ( b.Instance == Instance ) );
        }
    }
}
