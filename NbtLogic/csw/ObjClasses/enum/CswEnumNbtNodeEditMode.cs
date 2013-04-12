using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt
{
    /// <summary>
    /// Editing and Display mode for Nodes
    /// </summary>
    public sealed class CswEnumNbtNodeEditMode : IEquatable<CswEnumNbtNodeEditMode>
    {
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Edit, Edit },
                                                                       { Add, Add },
                                                                       { EditInPopup, EditInPopup },
                                                                       { Demo, Demo },
                                                                       { PrintReport, PrintReport },
                                                                       { DefaultValue, DefaultValue },
                                                                       { AuditHistoryInPopup, AuditHistoryInPopup },
                                                                       { Preview, Preview },
                                                                       { Table, Table },
                                                                       { Temp, Temp }
                                                                   };
        /// <summary>
        /// 
        /// </summary>
        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CswEnumNbtNodeEditMode( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit case to Enum
        /// </summary>
        public static implicit operator CswEnumNbtNodeEditMode( string Val )
        {
            return new CswEnumNbtNodeEditMode( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumNbtNodeEditMode item )
        {
            return item.Value;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Regular editing
        /// </summary>
        public const string Edit = "Edit";

        /// <summary>
        /// Adding a new node in a popup
        /// </summary>
        public const string Add = "Add";

        /// <summary>
        /// Adding/Editing a temporary node
        /// </summary>
        public const string Temp = "Temp";

        /// <summary>
        /// Editing a node in a popup
        /// </summary>
        public const string EditInPopup = "EditInPopup";

        /// <summary>
        /// Editing fake property values (as in Design mode)
        /// </summary>
        public const string Demo = "Demo";

        /// <summary>
        /// Displaying values for a print report
        /// </summary>
        public const string PrintReport = "PrintReport";

        /// <summary>
        /// Editing the default value of a property (in Design)
        /// </summary>
        public const string DefaultValue = "DefaultValue";

        /// <summary>
        /// Showing node audit history in a popup
        /// </summary>
        public const string AuditHistoryInPopup = "AuditHistoryInPopup";

        /// <summary>
        /// A preview of the node, displayed when hovering
        /// </summary>
        public const string Preview = "Preview";

        /// <summary>
        /// Properties of a node displayed in a Table Layout
        /// </summary>
        public const string Table = "Table";

        #region IEquatable (CswEnum)

        public static bool operator ==( CswEnumNbtNodeEditMode ft1, CswEnumNbtNodeEditMode ft2 )
        {
            //do a string comparison on the fieldtypes
            return ft1.ToString() == ft2.ToString();
        }

        public static bool operator !=( CswEnumNbtNodeEditMode ft1, CswEnumNbtNodeEditMode ft2 )
        {
            return !( ft1 == ft2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtNodeEditMode ) )
                return false;
            return this == (CswEnumNbtNodeEditMode) obj;
        }

        public bool Equals( CswEnumNbtNodeEditMode obj )
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = ( ret * prime ) + Value.GetHashCode();
            ret = ( ret * prime ) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (NodeEditMode)


    }; // NodeEditMode
} // namespace ChemSW.Nbt
