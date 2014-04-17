/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.register('permission', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.permissionsData = [];
            //for Magnolia Permissions are locked to the data format of Multilist, so we need to do a bit of massaging
            if (nodeProperty.propData.values.options) {
                Csw.each(nodeProperty.propData.values.options, function(permission) {
                    cswPrivate.permissionsData.push(permission);
                });
            }

//            if (nodeProperty.isReadOnly()) {
//                nodeProperty.propDiv.append(cswPrivate.gestalt);
//            } else {
                var errorDiv = nodeProperty.propDiv.div();

                /* Select Box */
                var permissionsGrid = nodeProperty.propDiv.permissionsGrid({
                    name: nodeProperty.name,
                    propname: nodeProperty.propData.name,
                    cssclass: 'selectinput',
                    //values: cswPrivate.options,
                    //valStr: nodeProperty.propData.values.value,
                    dataModel: { name: "", children: cswPrivate.permissionsData },
                    isMultiEdit: nodeProperty.isMulti(),
                    EditMode: nodeProperty.tabState.EditMode,
                    required: nodeProperty.isRequired(),
                    nodeId: nodeProperty.tabState.nodeid,
                    onChange: function () {
                        var valStr = permissionsGrid.val();
                        
                        nodeProperty.propData.values.value = valStr;
                        nodeProperty.broadcastPropChange(valStr);

                    }
                });

                //Validation
                var validator = Csw.validator(errorDiv, permissionsGrid, {
                    cssOptions: { 'visibility': 'hidden', 'width': '20px' },
                    errorMsg: 'At least one value must be selected',
                    className: 'multiListValidator' + nodeProperty.propData.id,
                    onValidation: function (isValid) {
                        if (isValid || Csw.enums.editMode.Edit === nodeProperty.tabState.EditMode) {
                            errorDiv.hide();
                        } else {
                            errorDiv.show();
                        }
                    }
                });

                cswPrivate.validatorCheckBox = validator.input;
                cswPrivate.validatorCheckBox.val(true); //if this is a required property this will get set accordingly , for now set it to true
                if (Csw.enums.editMode.Add === nodeProperty.tabState.EditMode) {
                    doValidation(nodeProperty.propData.values.value);
                } else {
                    errorDiv.hide();
                }
//            }

        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());
