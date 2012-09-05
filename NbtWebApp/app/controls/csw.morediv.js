/// <reference path="~app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.moreDiv = Csw.controls.moreDiv ||
        Csw.controls.register('moreDiv', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var cswPublic;

            cswPrivate.moreDiv = cswParent.div();
            cswPublic = Csw.dom({}, cswPrivate.moreDiv);

            cswPublic.shownDiv = cswPrivate.moreDiv.div({
                ID: Csw.makeId(cswPrivate.ID, '', '_shwn')
            });

            cswPublic.hiddenDiv = cswPrivate.moreDiv.div({
                ID: Csw.makeId(cswPrivate.ID, '', '_hddn')
            }).hide();

            cswPublic.moreLink = cswPrivate.moreDiv.a({
                ID: Csw.makeId(cswPrivate.ID, '', '_more'),
                text: cswPrivate.moretext,
                cssclass: 'morelink',
                onClick: function () {
                    if (cswPublic.moreLink.toggleState === Csw.enums.toggleState.on) {
                        cswPublic.moreLink.text(cswPrivate.lesstext);
                        cswPublic.hiddenDiv.show();
                    } else {
                        cswPublic.moreLink.text(cswPrivate.moretext);
                        cswPublic.hiddenDiv.hide();
                    }
                    return false;
                } // onClick()
            });

            return cswPublic;
        });

} ());
