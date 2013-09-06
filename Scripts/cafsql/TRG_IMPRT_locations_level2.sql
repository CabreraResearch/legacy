CREATE OR REPLACE TRIGGER TRG_IMPRT_locations_level2 AFTER INSERT OR DELETE OR UPDATE OF locationlevel2name,locationcode,locationlevel1id,controlzoneid,inventorygroupid,locationlevel2id,deleted ON locations_level2@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.locationlevel2id, 'locations_level2', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel2id, 'locations_level2', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel2id, 'locations_level2', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.locationlevel2id, 'locations_level2', '', '');      END IF
    
                                END IF;
  
                                END;