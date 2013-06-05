using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.ViewEditor
{
    public sealed class CswEnumNbtViewEditorRuleName: CswEnum<CswEnumNbtViewEditorRuleName>
    {
        private CswEnumNbtViewEditorRuleName( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewEditorRuleName> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewEditorRuleName( string str )
        {
            CswEnumNbtViewEditorRuleName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewEditorRuleName Unknown = new CswEnumNbtViewEditorRuleName( "Unknown" );

        public static readonly CswEnumNbtViewEditorRuleName ChooseView = new CswEnumNbtViewEditorRuleName( "Choose a View" );
        public static readonly CswEnumNbtViewEditorRuleName BuildView = new CswEnumNbtViewEditorRuleName( "Build a View" );
        public static readonly CswEnumNbtViewEditorRuleName AddToView = new CswEnumNbtViewEditorRuleName( "Add to View" );
        public static readonly CswEnumNbtViewEditorRuleName SetFilters = new CswEnumNbtViewEditorRuleName( "Set Filters" );
        public static readonly CswEnumNbtViewEditorRuleName ViewAttributes = new CswEnumNbtViewEditorRuleName( "View Attributes" );
        public static readonly CswEnumNbtViewEditorRuleName FineTuning = new CswEnumNbtViewEditorRuleName( "Fine Tuning (Advanced)" );
    }
}
