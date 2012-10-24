-- ****** Object: Function NBT.ISNUMERIC Script Date: 10/22/2012 10:28:05 AM ******
CREATE function isnumeric (param in varchar2) return boolean as
   dummy number;
begin
   dummy:=to_number(param);
   return(true);
exception
   when others then
       return (false);
end;


 
 
 
 
 
 
 
 
 
 
 
 

/
