/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswTBD() {
    'use strict';


    function jsTreeGetSelected($treediv) {
        var idPrefix = $treediv.CswAttrDom('id');
        var $SelectedItem = $treediv.jstree('get_selected');
        var ret = {
            'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
            'id': Csw.string($SelectedItem.CswAttrDom('id')).substring(idPrefix.length),
            'text': Csw.string($SelectedItem.children('a').first().text()).trim(),
            '$item': $SelectedItem
        };
        return ret;
    }
    Csw.register('jsTreeGetSelected', jsTreeGetSelected);
    Csw.jsTreeGetSelected = Csw.jsTreeGetSelected || jsTreeGetSelected;

    function makeViewVisibilitySelect($table, rownum, label) {
        // Used by CswDialog and CswViewEditor
        var $visibilityselect;
        var $visroleselect;
        var $visuserselect;

        Csw.clientSession.isAdministrator({
            'Yes': function() {

                $table.CswTable('cell', rownum, 1).append(label);
                var $parent = $table.CswTable('cell', rownum, 2);
                var id = $table.CswAttrDom('id');

                $visibilityselect = $('<select id="' + id + '_vissel" />')
                    .appendTo($parent);
                $visibilityselect.append('<option value="User">User:</option>');
                $visibilityselect.append('<option value="Role">Role:</option>');
                $visibilityselect.append('<option value="Global">Global</option>');

                $visroleselect = $parent.CswNodeSelect('init', {
                    'ID': id + '_visrolesel',
                    'objectclass': 'RoleClass'
                }).hide();
                $visuserselect = $parent.CswNodeSelect('init', {
                    'ID': id + '_visusersel',
                    'objectclass': 'UserClass'
                });

                $visibilityselect.change(function() {
                    var val = $visibilityselect.val();
                    if (val === 'Role') {
                        $visroleselect.show();
                        $visuserselect.hide();
                    } else if (val === 'User') {
                        $visroleselect.hide();
                        $visuserselect.show();
                    } else {
                        $visroleselect.hide();
                        $visuserselect.hide();
                    }
                }); // change
            } // yes
        }); // IsAdministrator

        return {
            'getvisibilityselect': function() {
                return $visibilityselect;
            },
            'getvisroleselect': function() {
                return $visroleselect;
            },
            'getvisuserselect': function() {
                return $visuserselect;
            }
        };

    } // makeViewVisibilitySelect()
    Csw.register('makeViewVisibilitySelect', makeViewVisibilitySelect);
    Csw.makeViewVisibilitySelect = Csw.makeViewVisibilitySelect || makeViewVisibilitySelect;

    function openPopup(url, height, width) {
        var popup = window.open(url, null, 'height=' + height + ', width=' + width + ', status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes');
        popup.focus();
        return popup;
    }
    Csw.register('openPopup', openPopup);
    Csw.openPopup = Csw.openPopup || openPopup;

}());
