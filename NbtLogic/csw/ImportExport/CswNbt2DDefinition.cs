using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DDefinition
    {
        private string _SheetName;
        public string SheetName
        {
            get { return _SheetName; }
            set
            {
                _SheetName = value;
                if( false == _SheetName.EndsWith( "$" ) )
                {
                    _SheetName += "$";
                }
            }
        }
        public string ImportDataTableName;
        public CswNbt2DBindingCollection Bindings = new CswNbt2DBindingCollection();
        public SortedList<Int32, CswNbt2DOrder> ImportOrder = new SortedList<int, CswNbt2DOrder>();
        public Collection<CswNbt2DRowRelationship> RowRelationships = new Collection<CswNbt2DRowRelationship>();
    }
}
