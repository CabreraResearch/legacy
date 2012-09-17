/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.jsTreeGetSelected = Csw.jsTreeGetSelected ||
        Csw.register('jsTreeGetSelected', function ($treediv) {
            var idPrefix = $treediv.CswAttrDom('id');
            var $SelectedItem = $treediv.jstree('get_selected');
            var ret = {
                'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
                'id': Csw.string($SelectedItem.CswAttrDom('id')).substring(idPrefix.length),
                'text': Csw.string($SelectedItem.children('a').first().text()).trim(),
                '$item': $SelectedItem
            };
            return ret;
        });
        
} ());
