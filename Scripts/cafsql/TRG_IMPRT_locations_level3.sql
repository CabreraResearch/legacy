CREATE OR REPLACE TRIGGER TRG_IMPRT_locations_level3 AFTER INSERT OR DELETE OR UPDATE OF locationlevel3name,locationcode,locationlevel2id,controlzoneid,inventorygroupid,locationlevel3id,deleted ON locations_level3@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.locationlevel3id, 'locations_level3', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel3id, 'locations_level3', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.locationlevel3id, 'locations_level3', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.locationlevel3id, 'locations_level3', '', '');      END IF
    
                                END IF;
  
                                END;