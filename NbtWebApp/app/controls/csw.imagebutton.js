/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.controls.imageButton = Csw.controls.imageButton ||
        Csw.controls.register('imageButton', function(cswParent, options) {
            ///<summary>Generates an imageButton</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach imageButton to.</param>
            ///<param name="options" type="Object">Object defining paramaters for imageButton construction.</param>
            ///<returns type="Csw.controls.imageButton">Object representing an imageButton</returns>
            'use strict';
            var cswPrivate = {
                ButtonType: Csw.enums.imageButton_ButtonType.None,
                Active: false,
                AlternateText: '',
                name: '',
                cssclass: '',
                Required: false,
                onClick: null
            };
            var cswPublic = { };

            (function() {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                cswPrivate.imageButton = cswParent.div(cswPrivate);
                cswPublic = Csw.dom({ }, cswPrivate.imageButton);
            }());

            cswPublic.setButtonType = function(newButtonType) {
                var multiplier = -18;
                //Case 24112: IE7 processes url() using https but randles the response as http--prompting the security dialog.
//                var port = document.location.port;
//                var prefix = document.location.protocol + "//" + document.location.hostname;
//                if (false === Csw.isNullOrEmpty(port) && port !== 80) {
//                    prefix += ':' + port;
//                }
//                prefix += '/NbtWebApp';
                if (newButtonType !== undefined && newButtonType !== Csw.enums.imageButton_ButtonType.None) {

                    var offset = 0;
                    if (cswPrivate.Active) {
                        offset = -36;
                    }
                    cswPublic.$.get(0).style.background = 'url(\'Images/buttons/buttons18.gif\') ' + offset + 'px ' + newButtonType * multiplier + 'px no-repeat';

                    cswPublic.unbind('mouseover');
                    cswPublic.unbind('mouseout');
                    cswPublic.unbind('mousedown');
                    cswPublic.unbind('mouseup');
                    cswPublic.bind('mouseover', function() {
                        cswPublic.css('background-position', '-18px ' + newButtonType * multiplier + 'px');
                    });
                    cswPublic.bind('mouseout', function() {
                        cswPublic.css('background-position', offset + 'px ' + newButtonType * multiplier + 'px');
                    });
                    cswPublic.bind('mousedown', function() {
                        cswPublic.css('background-position', '-36px ' + newButtonType * multiplier + 'px');
                    });
                    cswPublic.bind('mouseup', function() {
                        cswPublic.css('background-position', '-18px ' + newButtonType * multiplier + 'px');
                    });
                }
                cswPrivate.ButtonType = newButtonType;
                return false;
            };

            cswPublic.getButtonType = function() {
                return cswPrivate.ButtonType;
            };


            cswPublic.click = function(newButtonType, func) {
                if (Csw.isFunction(func)) {
                    return cswPublic.bind('click', func);
                } else {
                    return cswPublic.setButtonType(newButtonType);
                }
            };

            (function() {
                cswPublic.addClass('divbutton');
                cswPublic.propNonDom('title', cswPrivate.AlternateText);
                cswPublic.css('display', 'inline-block');
                cswPublic.setButtonType(cswPrivate.ButtonType);
            }());
            return cswPublic;
        });


} ());
