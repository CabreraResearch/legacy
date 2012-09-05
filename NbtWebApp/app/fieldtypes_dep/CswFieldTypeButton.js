///// <reference path="~app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeButton';

//    var methods = {
//        init: function (o) {

//            var propDiv = o.propDiv;
//            propDiv.empty();

//            var propVals = o.propData.values,
//                value = Csw.string(propVals.text, o.propData.name),
//                mode = Csw.string(propVals.mode, 'button'),
//                menuoptions, state, text, selectedText;

//            menuoptions = propVals.menuoptions.split(',');
//            state = propVals.state;
//            text = propVals.text;
//            selectedText = propVals.selectedText;

//            var onClickSuccess = function(data) {
//                var isRefresh = data.action == Csw.enums.nbtButtonAction.refresh;
//                if (isRefresh) { //cases 26201, 26107 
//                    Csw.tryExec(o.onReload,
//                        (function (messagedivid) {
//                            return function () {
//                                if (false === Csw.isNullOrEmpty(data.message)) {
//                                    var $newmessagediv = $('#' + messagedivid);
//                                    $newmessagediv.text(data.message);
//                                }
//                            };
//                        })(nodeButton.messageDiv.getId())
//                    );
//                }
//                return false === isRefresh;
//            };

//            var nodeButton = propDiv.nodeButton({
//                ID: Csw.makeId(o.propid, text, 'btn'),
//                value: value,
//                mode: mode,
//                state: state,
//                menuOptions: menuoptions,
//                selectedText: selectedText,
//                confirmmessage: propVals.confirmmessage,
//                propId: o.propid,
//                ReadOnly: Csw.bool(o.ReadOnly),
//                Required: Csw.bool(o.Required),
//                onClickSuccess: onClickSuccess
//            });

//        },
//        save: function (o) {
//            Csw.preparePropJsonForSave(o.propData);
//        }
//    };

//    $.fn.CswFieldTypeButton = function (method) {
//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }
//    };
//})(jQuery);
