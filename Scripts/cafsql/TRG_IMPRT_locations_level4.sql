CREATE OR REPLACE TRIGGER TRG_IMPRT_locations_level4 AFTER INSERT OR DELETE OR UPDATE OF locationlevel4name,locationcode,locationlevel3id,controlzoneid,inventorygroupid,locationlevel4id,deleted ON locations_level4@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.locationlevel4id, 'locations_level4', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel4id, 'locations_level4', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel4id, 'locations_level4', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.locationlevel4id, 'locations_level4', '', '');      END IF
    
                                END IF;
  
                                END;