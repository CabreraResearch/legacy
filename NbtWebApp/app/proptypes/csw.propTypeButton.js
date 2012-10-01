/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.button = Csw.properties.button ||
        Csw.properties.register('button',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {
                    size: 'small'
                };
                var cswPublic = {
                    data: propertyOption
                };
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propDiv = cswPublic.data.propDiv;

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.value = Csw.string(cswPrivate.propVals.text, cswPublic.data.propData.name);
                    cswPrivate.mode = Csw.string(cswPrivate.propVals.mode, 'button');
                    cswPrivate.menuoptions = cswPrivate.propVals.menuoptions.split(',');
                    cswPrivate.state = cswPrivate.propVals.state;
                    cswPrivate.text = cswPrivate.propVals.text;
                    cswPrivate.selectedText = cswPrivate.propVals.selectedText;
                    cswPrivate.selectedText = cswPrivate.propVals.selectedText;

                    
                    //                    cswPrivate.onClickSuccess = function(data) {
                    //                        var isRefresh = data.action == Csw.enums.nbtButtonAction.refresh;
                    //                        if (isRefresh) { //cases 26201, 26107 
                    //                            Csw.tryExec(cswPublic.data.onReload,
                    //                                (function(messagedivid) {
                    //                                    return function() {
                    //                                        if (false === Csw.isNullOrEmpty(data.message)) {
                    //                                            var $newmessagediv = $('#' + messagedivid);
                    //                                            $newmessagediv.text(data.message);
                    //                                        }
                    //                                    };
                    //                                })(cswPublic.control.messageDiv.getId())
                    //                            );
                    //                        }
                    //                        return false === isRefresh;
                    //                    };

                    cswPublic.control = cswPrivate.propDiv.nodeButton({
                        ID: Csw.makeId(cswPublic.data.propid, cswPrivate.text, 'btn'),
                        value: cswPrivate.value,
                        size: cswPublic.data.size,
                        mode: cswPrivate.mode,
                        state: cswPrivate.state,
                        menuOptions: cswPrivate.menuoptions,
                        selectedText: cswPrivate.selectedText,
                        confirmmessage: cswPrivate.propVals.confirmmessage,
                        propId: cswPublic.data.propid,
                        ReadOnly: Csw.bool(cswPublic.data.isReadOnly()),
                        onClickSuccess: cswPrivate.onClickSuccess,
                        disabled: cswPublic.data.isEnabled()
                    });
                };

                cswPublic.data.bindRender(render);

                return cswPublic;
            }));
} ());
