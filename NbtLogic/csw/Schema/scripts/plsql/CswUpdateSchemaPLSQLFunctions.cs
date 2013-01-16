using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CswUpdateSchemaPLSQLFunctions
    /// </summary>    
    public class CswUpdateSchemaPLSQLFunctions
    {
        public sealed class Functions : CswEnum<Functions>
        {
            #region Properties and ctor

            private Functions( string Dev, Int32 CaseNo, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<Functions> _All { get { return All; } }
            public static implicit operator Functions( string str )
            {
                Functions ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region ALNUMONLY

            public static readonly Functions ALNUMONLY = new Functions( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE function alnumOnly(inputStr in varchar2, replaceWith in varchar2) return varchar2 is
        Result varchar2(1000);
    begin

        Result := regexp_replace(inputStr,'[[:space:]]', null );

        Result := regexp_replace(trim(Result), '[^a-zA-Z0-9_]', replaceWith );

        return(Result);
    end alnumOnly;" );

            #endregion ALNUMONLY

            #region ISNUMERIC

            public static readonly Functions ISNUMERIC = new Functions( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE function isnumeric (param in varchar2) return boolean as
       dummy number;
    begin
       dummy:=to_number(param);
       return(true);
    exception
       when others then
           return (false);
    end;" );

            #endregion ISNUMERIC

            #region ORACOLLEN

            public static readonly Functions ORACOLLEN = new Functions( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE function OraColLen(aprefix in varchar2,inputStr in varchar2,asuffix in varchar2) return varchar2 is
      Result varchar2(100);
      maxlen number(3);
    begin
      --dbms_output.put_line(aprefix || inputstr || asuffix);
      maxlen := 28- nvl(length(aprefix),0) - nvl(length(asuffix),0);
      --dbms_output.put_line(maxlen);
      Result := aprefix || substr(trim(inputStr),1,maxlen) || asuffix;
      --dbms_output.put_line(Result);
      return(Result);
    end OraColLen;" );

            #endregion ORACOLLEN

            #region SAFESQLPARAM

            public static readonly Functions SAFESQLPARAM = new Functions( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE function safeSqlParam(inputStr in varchar2) return varchar2 is
      Result varchar2(1000);
    begin

      Result := regexp_replace(inputStr,'''', '''''' );

      return(Result);
    end safeSqlParam;" );

            #endregion SAFESQLPARAM
        }

    }//class CswUpdateSchemaPLSQLFunctions

}//namespace ChemSW.Nbt.Schema