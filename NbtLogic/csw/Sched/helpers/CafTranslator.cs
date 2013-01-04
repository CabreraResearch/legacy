using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    public class CafTranslator
    {
        private Dictionary<string, string> _UnitMappings;
        private Dictionary<string, string> _PhysicalStateMappings;

        public sealed class CafTranslationDictionaryNames
        {
            public const string NONE = "NONE";
            public const string PhysicalState = "Physical State";
            public const string UnitName = "Unit Name";
            public CswCommaDelimitedString Options = new CswCommaDelimitedString { PhysicalState, UnitName };
        }

        /// <summary>
        /// Always Init() before using CafTranslator!!!
        /// </summary>
        public CafTranslator()
        {
            _initUnitMappings();
            _initPhysicalStateMappings();
        }

        private void _initUnitMappings()
        {
            _UnitMappings = new Dictionary<string, string>();

            _UnitMappings.Add( "G", "g" );
            _UnitMappings.Add( "each", "Each" );            //EACH
            _UnitMappings.Add( "PKG", "Boxes" );            //EACH
            _UnitMappings.Add( "GR", "" );                  //EACH
            _UnitMappings.Add( "L", "Liters" );
            _UnitMappings.Add( "PT", "" );                  //VOLUME - pint
            _UnitMappings.Add( "MG", "mg" );
            _UnitMappings.Add( "OZ", "ounces" );
            _UnitMappings.Add( "GAL", "gal" );
            _UnitMappings.Add( "kt", "" );                  //EACH
            _UnitMappings.Add( "oz", "fluid ounces" );
            _UnitMappings.Add( "ML", "mL" );
            _UnitMappings.Add( "GM", "g" );
            _UnitMappings.Add( "k", "" );                   //EACH
            _UnitMappings.Add( "vial", "" );                //EACH
            _UnitMappings.Add( "Bottle", "" );              //EACH
            _UnitMappings.Add( "uCi", "" );                 //EACH
            _UnitMappings.Add( "KG", "kg" );
            _UnitMappings.Add( "QT", "" );                  //VOLUME - quart
            _UnitMappings.Add( "PAK", "" );                 //EACH
            _UnitMappings.Add( "IU", "" );                  //EACH
            _UnitMappings.Add( "ML (each)", "" );           //EACH
            _UnitMappings.Add( "UG", "" );                  //WEIGHT - microgram
            _UnitMappings.Add( "jin", "" );                 //WEIGHT - chinese pound?????
            _UnitMappings.Add( "mCi", "" );                 //EACH - Millicurie, measure of radioactivity???
            _UnitMappings.Add( "KIT", "" );                 //EACH
            _UnitMappings.Add( "item", "" );                //EACH
            _UnitMappings.Add( "LB", "lb" );
            _UnitMappings.Add( "years", "Years" );
        }

        private void _initPhysicalStateMappings()
        {
            _PhysicalStateMappings = new Dictionary<string, string>();

            _PhysicalStateMappings.Add( "L", CswNbtObjClassMaterial.PhysicalStates.Liquid );
            _PhysicalStateMappings.Add( "S", CswNbtObjClassMaterial.PhysicalStates.Solid );
            _PhysicalStateMappings.Add( "G", CswNbtObjClassMaterial.PhysicalStates.Gas );
            _PhysicalStateMappings.Add( "", CswNbtObjClassMaterial.PhysicalStates.NA );
        }

        /// <summary>
        /// Translate a CAF value to an NBT value. If no mapping exists returns the input value
        /// </summary>
        /// <param name="DictionaryName">The name of the translation dictionary</param>
        /// <param name="CafValue">The CAF value to translate</param>
        /// <returns>The NBT equivalent of the CAF value</returns>
        public string Translate( string DictionaryName, string CafValue )
        {
            string ret = CafValue;

            switch( DictionaryName )
            {
                case CafTranslationDictionaryNames.PhysicalState:
                    if( _PhysicalStateMappings.ContainsKey( CafValue ) )
                    {
                        ret = _PhysicalStateMappings[CafValue];
                    }
                    break;
                case CafTranslationDictionaryNames.UnitName:
                    if( _UnitMappings.ContainsKey( CafValue ) )
                    {
                        ret = _UnitMappings[CafValue];
                    }
                    break;
            }
            return ret;
        }
    }
}
