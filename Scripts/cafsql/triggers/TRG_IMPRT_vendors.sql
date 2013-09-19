CREATE OR REPLACE TRIGGER TRG_IMPRT_vendors AFTER INSERT OR DELETE OR UPDATE OF accountno,city,contactname,fax,phone,state,street1,street2,vendorname,zip,vendorid,deleted ON vendors@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.vendorid, 'vendors', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.vendorid, 'vendors', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.vendorid, 'vendors', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.vendorid, 'vendors', '', '');      END IF
    
                                END IF;
  
                                END;