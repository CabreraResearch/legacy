/// <reference path="~/app/CswApp-vsdoc.js" />
(function () { 
    Csw.layouts.dialog = Csw.layouts.dialog ||
        Csw.layouts.register('dialog', function (cswPrivate) {
            'use strict';
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate = cswPrivate || {};
                cswPrivate.title = cswPrivate.title || '';
                cswPrivate.width = cswPrivate.width || 500;
                cswPrivate.height = cswPrivate.height || 500;
                
                cswPrivate.onOpen = cswPrivate.onOpen || function () { };
                cswPrivate.onBeforeClose = cswPrivate.onBeforeClose || function () {};
                cswPrivate.onClose = cswPrivate.onClose || function () { };
            }());
            
            cswPublic.div = Csw.literals.div({
                name: cswPrivate.title + 'DialogDiv'
            });
            cswPublic.close = function () {
                cswPublic.div.$.dialog('close');
            };
            
            cswPublic.open = function () {
                'use strict';
                $('<div id="DialogErrorDiv" style="display: none;"></div>').prependTo(cswPublic.div.$);
                Csw.tryExec(cswPublic.div.$.dialog, 'close');
                var incrPosBy = 30;
                var posX = (document.documentElement.clientWidth / 2) - (cswPrivate.width / 2) + (incrPosBy * Csw.dialogsCount());
                var posY = (document.documentElement.clientHeight / 2) - (cswPrivate.height / 2) + (incrPosBy * Csw.dialogsCount());

                Csw.subscribe(Csw.enums.events.main.clear, function _close() {
                    Csw.tryExec(cswPublic.div.remove);
                    Csw.tryExec(cswPrivate.onClose);
                    unbindEvents();
                    Csw.unsubscribe(Csw.enums.events.main.clear, _close);
                });

                cswPublic.div.$.dialog({
                    modal: true,
                    width: cswPrivate.width,
                    height: cswPrivate.height,
                    title: cswPrivate.title,
                    position: [posX, posY],
                    beforeClose: function () {
                        var ret = Csw.clientChanges.manuallyCheckChanges();
                        if (Csw.isFunction(cswPrivate.onBeforeClose)) {
                            ret = ret && cswPrivate.onBeforeClose();
                        }
                        return ret;
                    },
                    close: function () {
                        Csw.dialogsCount(-1);
                        Csw.tryExec(cswPrivate.onClose);
                        unbindEvents();
                        cswPublic.div.remove();
                    },
                    open: function () {
                        Csw.dialogsCount(1);
                        Csw.tryExec(cswPrivate.onOpen, cswPublic.div);
                        cswPublic.div.$.parent().find(' :button').blur();
                    }
                });

                var doClose = function (func) {
                    if (!func || true === func()) {
                        Csw.tryExec(cswPrivate.onClose);
                        cswPublic.close();
                        unbindEvents();
                    }
                };
                var unbindEvents = function () {
                    Csw.publish('onAnyNodeButtonClickFinish', true);
                    Csw.unsubscribe(Csw.enums.events.afterObjectClassButtonClick, doClose);
                    Csw.unsubscribe('initGlobalEventTeardown', doClose);
                };
                Csw.subscribe(Csw.enums.events.afterObjectClassButtonClick, doClose);
                Csw.subscribe('initGlobalEventTeardown', doClose);
            };

            (function _postCtor() {}());
            
            return cswPublic;
        });
    }
());