/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.moreDiv = Csw.controls.moreDiv ||
        Csw.controls.register('moreDiv', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var cswPublic;

            cswPrivate.moreDiv = cswParent.div();
            cswPublic = Csw.dom({}, cswPrivate.moreDiv);

            cswPublic.shownDiv = cswPrivate.moreDiv.div();

            cswPublic.hiddenDiv = cswPrivate.moreDiv.div().hide();

            cswPublic.moreLink = cswPrivate.moreDiv.a({
                text: cswPrivate.moretext,
                cssclass: 'morelink',
                onClick: function () {
                    if (cswPublic.moreLink.toggleState === Csw.enums.toggleState.on) {
                        cswPublic.showHidden();
                    } else {
                        cswPublic.hideHidden();
                    }
                    return false;
                } // onClick()
            });

            cswPublic.showHidden = function() {
                if (false === Csw.isNullOrEmpty(cswPrivate.lesstext)) {
                    cswPublic.moreLink.text(cswPrivate.lesstext);
                } else {
                    cswPublic.moreLink.hide();
                }
                cswPublic.hiddenDiv.show();
            };

            cswPublic.hideHidden = function() {
                cswPublic.moreLink.text(cswPrivate.moretext);
                cswPublic.hiddenDiv.hide();
            };

            return cswPublic;
        });

} ());
