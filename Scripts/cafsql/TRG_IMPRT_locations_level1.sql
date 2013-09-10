CREATE OR REPLACE TRIGGER TRG_IMPRT_locations_level1 AFTER INSERT OR DELETE OR UPDATE OF locationlevel1name,locationcode,siteid,controlzoneid,inventorygroupid,locationlevel1id,deleted ON locations_level1@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.locationlevel1id, 'locations_level1', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel1id, 'locations_level1', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel1id, 'locations_level1', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.locationlevel1id, 'locations_level1', '', '');      END IF
    
                                END IF;
  
                                END;