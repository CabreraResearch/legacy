CREATE OR REPLACE TRIGGER TRG_IMPRT_inventory_groups AFTER INSERT OR DELETE OR UPDATE OF inventorygroupname,iscentralgroup,inventorygroupid,deleted ON inventory_groups@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.inventorygroupid, 'inventory_groups', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.inventorygroupid, 'inventory_groups', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.inventorygroupid, 'inventory_groups', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.inventorygroupid, 'inventory_groups', '', '');      END IF
    
                                END IF;
  
                                END;