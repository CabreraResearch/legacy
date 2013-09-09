CREATE OR REPLACE TRIGGER TRG_IMPRT_locations_level5 AFTER INSERT OR DELETE OR UPDATE OF locationlevel5name,locationcode,locationlevel4id,controlzoneid,inventorygroupid,locationlevel5id,deleted ON locations_level5@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.locationlevel5id, 'locations_level5', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel5id, 'locations_level5', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel5id, 'locations_level5', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.locationlevel5id, 'locations_level5', '', '');      END IF
    
                                END IF;
  
                                END;