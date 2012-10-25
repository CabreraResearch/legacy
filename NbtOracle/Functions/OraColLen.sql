-- ****** Object: Function NBT.ORACOLLEN Script Date: 10/22/2012 10:28:08 AM ******
CREATE function OraColLen(aprefix in varchar2,inputStr in varchar2,asuffix in varchar2) return varchar2 is
  Result varchar2(100);
  maxlen number(3);
begin
  --dbms_output.put_line(aprefix || inputstr || asuffix);
  maxlen := 28- nvl(length(aprefix),0) - nvl(length(asuffix),0);
  --dbms_output.put_line(maxlen);
  Result := aprefix || substr(trim(inputStr),1,maxlen) || asuffix;
  --dbms_output.put_line(Result);
  return(Result);
end OraColLen;
/
