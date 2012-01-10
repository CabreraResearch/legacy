var loadResource = function(filename, filetype, useJquery) {
    var fileref, type = filetype || 'js';
    switch (type) {
        case 'js':
            if (jQuery && useJquery) {
                $.ajax({
                    url: '/NbtWebApp/' + filename,
                    dataType: 'script',
                    success: function() {
                        if(initMain) {
                            initMain();
                        }
                    }
                });
            } else {
                fileref = document.createElement('script');
                fileref.setAttribute("type", "text/javascript");
                fileref.setAttribute("src", filename);
            }
            break;
        case 'css':
            fileref = document.createElement("link");
            fileref.setAttribute("rel", "stylesheet");
            fileref.setAttribute("type", "text/css");
            fileref.setAttribute("href", filename);
            break;
    }
    if (fileref) {
        document.getElementsByTagName("head")[0].appendChild(fileref);
    }
};

// ChemSW 
// Globals: load first (because order matters)
loadResource('js/globals/CswEnums.js');
loadResource('js/globals/CswGlobalTools.js');
loadResource('js/globals/CswPrototypeExtensions.js');
loadResource('js/globals/Global.js');

// Actions
loadResource('js/main/actions/CswAuditHistoryGrid.js');
loadResource('js/main/actions/CswInspectionStatus.js');
loadResource('js/main/actions/CswInspectionDesign.js');
loadResource('js/main/actions/CswQuotas.js');
loadResource('js/main/actions/CswQuotaImage.js');
loadResource('js/main/actions/CswScheduledRulesGrid.js');
loadResource('js/main/actions/CswSessions.js');

// Controls
loadResource('js/main/controls/CswDateTimePicker.js');
loadResource('js/main/controls/CswDiv.js');
loadResource('js/main/controls/CswGrid.js');
loadResource('js/main/controls/CswInput.js');
loadResource('js/main/controls/CswLink.js');
loadResource('js/main/controls/CswMultiSelect.js');
loadResource('js/main/controls/CswSelect.js');
loadResource('js/main/controls/CswSpan.js');
loadResource('js/main/controls/CswButton.js');
loadResource('js/main/controls/CswCheckBoxArray.js');
loadResource('js/main/controls/CswComboBox.js');
loadResource('js/main/controls/CswLayoutTable.js');
loadResource('js/main/controls/CswList.js');
loadResource('js/main/controls/CswImageButton.js');
loadResource('js/main/controls/CswNodeTypeSelect.js');
loadResource('js/main/controls/CswNumberTextBox.js');
loadResource('js/main/controls/CswTimeInterval.js');
loadResource('js/main/controls/CswTable.js');
loadResource('js/main/controls/CswTristateCheckBox.js');

// Tools
loadResource('js/main/tools/CswAttr.js');
loadResource('js/main/tools/CswCookie.js');
loadResource('js/main/tools/CswTools.js');
loadResource('js/main/tools/CswQueryString.js');
loadResource('js/main/tools/CswString.js');
loadResource('js/main/tools/CswPrint.js');
loadResource('js/main/tools/CswClientDb.js');
loadResource('js/main/tools/CswProfileMethod.js');
loadResource('js/main/tools/CswPubSub.js');

// PageCmp 
loadResource('js/main/pagecmp/CswDashboard.js');
loadResource('js/main/pagecmp/CswDialog.js');
loadResource('js/main/pagecmp/CswErrorMessage.js');
loadResource('js/main/pagecmp/CswLogin.js');
loadResource('js/main/pagecmp/CswMenu.js');
loadResource('js/main/pagecmp/CswMenuHeader.js');
loadResource('js/main/pagecmp/CswMenuMain.js');
loadResource('js/main/pagecmp/CswQuickLaunch.js');
loadResource('js/main/pagecmp/CswSearch.js');
loadResource('js/main/pagecmp/CswWelcome.js');
loadResource('js/main/pagecmp/CswWizard.js');

// Node
loadResource('js/main/node/CswNodeGrid.js');
loadResource('js/main/node/CswNodePreview.js');
loadResource('js/main/node/CswNodeSelect.js');
loadResource('js/main/node/CswNodeTabs.js');
loadResource('js/main/node/CswNodeTable.js');
loadResource('js/main/node/CswNodeTree.js');

// View
loadResource('js/main/view/CswViewContentTree.js');
loadResource('js/main/view/CswViewEditor.js');
loadResource('js/main/view/CswViewListTree.js');
loadResource('js/main/view/CswViewPropFilter.js');
loadResource('js/main/view/CswViewSelect.js');

// Field Types
loadResource('js/main/Fieldtypes/_CswFieldTypeFactory.js');
loadResource('js/main/Fieldtypes/CswFieldTypeAuditHistoryGrid.js');
loadResource('js/main/Fieldtypes/CswFieldTypeBarcode.js');
loadResource('js/main/Fieldtypes/CswFieldTypeButton.js');
loadResource('js/main/Fieldtypes/CswFieldTypeComposite.js');
loadResource('js/main/Fieldtypes/CswFieldTypeDateTime.js');
loadResource('js/main/Fieldtypes/CswFieldTypeFile.js');
loadResource('js/main/Fieldtypes/CswFieldTypeGrid.js');
loadResource('js/main/Fieldtypes/CswFieldTypeImage.js');
loadResource('js/main/Fieldtypes/CswFieldTypeImageList.js');
loadResource('js/main/Fieldtypes/CswFieldTypeLink.js');
loadResource('js/main/Fieldtypes/CswFieldTypeList.js');
loadResource('js/main/Fieldtypes/CswFieldTypeLocation.js');
loadResource('js/main/Fieldtypes/CswFieldTypeLocationContents.js');
loadResource('js/main/Fieldtypes/CswFieldTypeLogical.js');
loadResource('js/main/Fieldtypes/CswFieldTypeLogicalSet.js');
loadResource('js/main/Fieldtypes/CswFieldTypeMemo.js');
loadResource('js/main/Fieldtypes/CswFieldTypeMol.js');
loadResource('js/main/Fieldtypes/CswFieldTypeMultiList.js');
loadResource('js/main/Fieldtypes/CswFieldTypeMTBF.js');
loadResource('js/main/Fieldtypes/CswFieldTypeNFPA.js');
loadResource('js/main/Fieldtypes/CswFieldTypeNodeTypeSelect.js');
loadResource('js/main/Fieldtypes/CswFieldTypeNumber.js');
loadResource('js/main/Fieldtypes/CswFieldTypePassword.js');
loadResource('js/main/Fieldtypes/CswFieldTypePropertyReference.js');
loadResource('js/main/Fieldtypes/CswFieldTypeQuantity.js');
loadResource('js/main/Fieldtypes/CswFieldTypeQuestion.js');
loadResource('js/main/Fieldtypes/CswFieldTypeRelationship.js');
loadResource('js/main/Fieldtypes/CswFieldTypeScientific.js');
loadResource('js/main/Fieldtypes/CswFieldTypeSequence.js');
loadResource('js/main/Fieldtypes/CswFieldTypeStatic.js');
loadResource('js/main/Fieldtypes/CswFieldTypeText.js');
loadResource('js/main/Fieldtypes/CswFieldTypeTimeInterval.js');
loadResource('js/main/Fieldtypes/CswFieldTypeUserSelect.js');
loadResource('js/main/Fieldtypes/CswFieldTypeViewPickList.js');
loadResource('js/main/Fieldtypes/CswFieldTypeViewReference.js');

loadResource('js/main/Main.js', 'js', true);
