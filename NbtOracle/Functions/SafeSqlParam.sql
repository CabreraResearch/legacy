-- ****** Object: Function NBT.SAFESQLPARAM Script Date: 10/22/2012 10:28:11 AM ******
CREATE function safeSqlParam(inputStr in varchar2) return varchar2 is
  Result varchar2(1000);
begin

  Result := regexp_replace(inputStr,'''', '''''' );

  return(Result);
end safeSqlParam;
/
