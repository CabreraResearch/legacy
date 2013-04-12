using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public sealed class CswEnumNbtRequestItemType
    {
        public readonly string Value;

        public CswEnumNbtRequestItemType( string ItemName = Container )
        {
            switch( ItemName )
            {
                case Material:
                    Value = Material;
                    break;
                default:
                    Value = Container;
                    break;
            }
        }

        public const string Material = "Material";
        public const string Container = "Container";
    }

} // namespace