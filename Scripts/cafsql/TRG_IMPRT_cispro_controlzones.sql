CREATE OR REPLACE TRIGGER TRG_IMPRT_cispro_controlzones AFTER INSERT OR DELETE OR UPDATE OF controlzonename,exemptqtyfactor,controlzoneid,deleted ON cispro_controlzones@CAFLINK FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.controlzoneid, 'cispro_controlzones@CAFLINK', '', '');  ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.controlzoneid, 'cispro_controlzones@CAFLINK', '', '');  ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.controlzoneid, 'cispro_controlzones@CAFLINK', '', '');      ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.controlzoneid, 'cispro_controlzones@CAFLINK', '', '');      END IF
    
                                END IF;
  
                                END;