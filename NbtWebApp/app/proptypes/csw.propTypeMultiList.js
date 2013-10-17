/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.register('multiList', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.gestalt = nodeProperty.propData.gestalt;
            cswPrivate.options = nodeProperty.propData.values.options;

            if (nodeProperty.isReadOnly()) {
                nodeProperty.propDiv.append(cswPrivate.gestalt);
            } else {
                var errorDiv = nodeProperty.propDiv.div();

                var doValidation = function (val) {
                    var ret = true;
                    if (nodeProperty.isRequired()) {
                        if (Csw.isNullOrEmpty(val)) {
                            cswPrivate.validatorCheckBox.val(false);
                            ret = false;
                        } else {
                            cswPrivate.validatorCheckBox.val(true);
                            ret = true;
                        }
                        cswPrivate.validatorCheckBox.$.valid();
                    }
                    return ret;
                };

                /* Select Box */
                var multiSel = nodeProperty.propDiv.multiSelect({
                    name: nodeProperty.name,
                    cssclass: 'selectinput',
                    values: cswPrivate.options,
                    valStr: nodeProperty.propData.values.value,
                    readonlyless: nodeProperty.propData.values.readonlyless,
                    readonlymore: nodeProperty.propData.values.readonlymore,
                    isMultiEdit: nodeProperty.isMulti(),
                    EditMode: nodeProperty.tabState.EditMode,
                    required: nodeProperty.isRequired(),
                    onChange: function (val) {
                        var valStr = val.join(',');

                        var ret = doValidation(valStr);

                        //Case 29390: no sync for Multi List
                        nodeProperty.propData.values.value = valStr;
                        nodeProperty.broadcastPropChange(valStr);

                        //without this the changes dialog will show up when clicking save in the edit dialog
                        Csw.clientChanges.unsetChanged();

                        return ret;
                    }
                });

                //Validation
                var validator = Csw.validator(errorDiv, multiSel, {
                    cssOptions: { 'visibility': 'hidden', 'width': '20px' },
                    errorMsg: 'At least one value must be selected',
                    onValidation: function (isValid) {
                        if (isValid) {
                            errorDiv.hide();
                        } else {
                            errorDiv.show();
                        }
                    }
                });

                cswPrivate.validatorCheckBox = validator.input;
                if (Csw.enums.editMode.Add === nodeProperty.tabState.EditMode) {
                    doValidation(nodeProperty.propData.values.value);
                } else {
                    errorDiv.hide();
                }
            }

        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());
