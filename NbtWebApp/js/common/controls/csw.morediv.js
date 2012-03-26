/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswMoreDiv() {
    'use strict';

    Csw.literals.moreDiv = Csw.literals.moreDiv ||
        Csw.literals.register('moreDiv', function (options) {

            var internal = {
                ID: '',
                $parent: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) $.extend(internal, options);

            var external = {};

            external.shownDiv = Csw.literals.div({
                ID: Csw.makeId(internal.ID, '', '_shwn'),
                $parent: internal.$parent
            });

            external.hiddenDiv = Csw.literals.div({
                ID: Csw.makeId(internal.ID, '', '_hddn'),
                $parent: internal.$parent
            }).hide();

            external.moreLink = Csw.literals.link({
                ID: Csw.makeId(internal.ID, '', '_more'),
                $parent: internal.$parent,
                text: internal.moretext,
                cssclass: 'morelink',
                onClick: function () {
                    if (external.moreLink.toggleState === Csw.enums.toggleState.on) {
                        external.moreLink.text(internal.lesstext);
                        external.hiddenDiv.show();
                    } else {
                        external.moreLink.text(internal.moretext);
                        external.hiddenDiv.hide();
                    }
                    return false;
                } // onClick()
            });

            return external;
        });

} ());
