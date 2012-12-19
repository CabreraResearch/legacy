using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CswUpdateSchemaPLSQLTypes
    /// </summary>    
    public class CswUpdateSchemaPLSQLTypes
    {
        public sealed class TypeHeaders : CswEnum<TypeHeaders>
        {
            #region Properties and ctor

            private TypeHeaders( string Dev, Int32 CaseNo, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<TypeHeaders> _All { get { return All; } }
            public static implicit operator TypeHeaders( string str )
            {
                TypeHeaders ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region TIER_II_LOCATION

            public static readonly TypeHeaders TIER_II_LOCATION = new TypeHeaders( CswDeveloper.BV, 28247,
            @"create or replace
TYPE TIER_II_LOCATION AS OBJECT 
(
  LOCATIONID number,
  PARENTLOCATIONID number
)" );

            #endregion TIER_II_LOCATION

            #region TIER_II_MATERIAL

            public static readonly TypeHeaders TIER_II_MATERIAL = new TypeHeaders( CswDeveloper.BV, 28247,
            @"create or replace
TYPE TIER_II_MATERIAL AS OBJECT 
(
  MATERIALID number,
  CASNO varchar2(20),
  QUANTITY number,
  UNITID number,
  TOTALQUANTITY number
)" );

            #endregion TIER_II_MATERIAL
        }

        public sealed class NestedTables : CswEnum<NestedTables>
        {
            #region Properties and ctor

            private NestedTables( string Dev, Int32 CaseNo, string Name )
                : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<NestedTables> _All { get { return All; } }
            public static implicit operator NestedTables( string str )
            {
                NestedTables ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region NUM_TABLE

            public static readonly NestedTables NUM_TABLE = new NestedTables( CswDeveloper.BV, 28247,
            @"create or replace
type num_table as table of number;" );

            #endregion NUM_TABLE

            #region TIER_II_LOCATION_TABLE

            public static readonly NestedTables TIER_II_LOCATION_TABLE = new NestedTables( CswDeveloper.BV, 28247,
            @"create or replace
type TIER_II_LOCATION_TABLE as table of TIER_II_LOCATION;" );

            #endregion TIER_II_LOCATION_TABLE

            #region TIER_II_MATERIAL_TABLE

            public static readonly NestedTables TIER_II_MATERIAL_TABLE = new NestedTables( CswDeveloper.BV, 28247,
            @"create or replace
type TIER_II_MATERIAL_TABLE as table of TIER_II_MATERIAL;" );

            #endregion TIER_II_MATERIAL_TABLE
        }

    }//class CswUpdateSchemaPLSQLTypes

}//namespace ChemSW.Nbt.Schema