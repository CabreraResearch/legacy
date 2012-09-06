/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.button = Csw.properties.button ||
        Csw.properties.register('button',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                var render = function(o) {
                    'use strict';
                    o = o || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propDiv = o.propDiv;
                    
                    cswPrivate.propVals = o.propData.values;
                    cswPrivate.value = Csw.string(cswPrivate.propVals.text, o.propData.name);
                    cswPrivate.mode = Csw.string(cswPrivate.propVals.mode, 'button');
                    cswPrivate.menuoptions = cswPrivate.propVals.menuoptions.split(',');
                    cswPrivate.state = cswPrivate.propVals.state;
                    cswPrivate.text = cswPrivate.propVals.text;
                    cswPrivate.selectedText = cswPrivate.propVals.selectedText;

                    cswPrivate.onClickSuccess = function(data) {
                        var isRefresh = data.action == Csw.enums.nbtButtonAction.refresh;
                        if (isRefresh) { //cases 26201, 26107 
                            Csw.tryExec(o.onReload,
                                (function(messagedivid) {
                                    return function() {
                                        if (false === Csw.isNullOrEmpty(data.message)) {
                                            var $newmessagediv = $('#' + messagedivid);
                                            $newmessagediv.text(data.message);
                                        }
                                    };
                                })(cswPublic.control.messageDiv.getId())
                            );
                        }
                        return false === isRefresh;
                    };

                    cswPublic.control = cswPrivate.propDiv.nodeButton({
                        ID: Csw.makeId(o.propid, cswPrivate.text, 'btn'),
                        value: cswPrivate.value,
                        mode: cswPrivate.mode,
                        state: cswPrivate.state,
                        menuOptions: cswPrivate.menuoptions,
                        selectedText: cswPrivate.selectedText,
                        confirmmessage: cswPrivate.propVals.confirmmessage,
                        propId: o.propid,
                        ReadOnly: Csw.bool(o.ReadOnly),
                        Required: Csw.bool(o.Required),
                        onClickSuccess: cswPrivate.onClickSuccess
                    });
                };

                propertyOption.render(render);

                return cswPublic;
            }));
}());
