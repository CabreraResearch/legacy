CREATE OR REPLACE TRIGGER TRG_IMPRT_sites AFTER INSERT OR DELETE OR UPDATE OF sitename,sitecode,controlzoneid,siteid,deleted ON sites@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.siteid, 'sites', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.siteid, 'sites', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.siteid, 'sites', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.siteid, 'sites', '', '');      END IF
    
                                END IF;
  
                                END;