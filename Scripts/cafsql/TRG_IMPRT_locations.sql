CREATE OR REPLACE TRIGGER TRG_IMPRT_locations AFTER INSERT OR DELETE OR UPDATE OF sitename,sitecode,siteid,locationlevel1name,locationcode,locationid,locationlevel2name,locationlevel3name,locationlevel4name,locationlevel5name,deleted ON locations@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.locationid, 'locations', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationid, 'locations', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationid, 'locations', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.locationid, 'locations', '', '');      END IF
    
                                END IF;
  
                                END;