/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.button = Csw.properties.button ||
        Csw.properties.register('button',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPublic = {
                    data: propertyOption
                };
                var render = function(o) {
                    'use strict';
                    var propDiv = o.propDiv;
                    propDiv.empty();

                    var propVals = o.propData.values,
                        value = Csw.string(propVals.text, o.propData.name),
                        mode = Csw.string(propVals.mode, 'button'),
                        menuoptions, state, text, selectedText;

                    menuoptions = propVals.menuoptions.split(',');
                    state = propVals.state;
                    text = propVals.text;
                    selectedText = propVals.selectedText;

                    var onClickSuccess = function(data) {
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

                    cswPublic.control = propDiv.nodeButton({
                        ID: Csw.makeId(o.propid, text, 'btn'),
                        value: value,
                        mode: mode,
                        state: state,
                        menuOptions: menuoptions,
                        selectedText: selectedText,
                        confirmmessage: propVals.confirmmessage,
                        propId: o.propid,
                        ReadOnly: Csw.bool(o.ReadOnly),
                        Required: Csw.bool(o.Required),
                        onClickSuccess: onClickSuccess
                    });
                };

                propertyOption.render(render);

                return cswPublic;
            }));
}());
