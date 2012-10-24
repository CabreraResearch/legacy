-- ****** Object: Function NBT.ALNUMONLY Script Date: 10/22/2012 10:28:03 AM ******
CREATE function alnumOnly(inputStr in varchar2, replaceWith in varchar2) return varchar2 is
  Result varchar2(1000);
begin

  Result := regexp_replace(inputStr,'[[:space:]]', null );

  Result := regexp_replace(trim(Result), '[^a-zA-Z0-9_]', replaceWith );

  return(Result);
end alnumOnly;
/
