CREATE OR REPLACE TRIGGER TRG_IMPRT_locations AFTER INSERT OR DELETE OR UPDATE OF locationlevel1name,locationcode,siteid,controlzoneid,inventorygroupid,locationid,deleted ON locations@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.locationid, 'locations', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationid, 'locations', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationid, 'locations', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.locationid, 'locations', '', '');      END IF
    
                                END IF;
  
                                END;