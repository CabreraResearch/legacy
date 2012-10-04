/// <reference path="../../NbtWebApp/app/CswApp-vsdoc.js" />
/// <reference path="../../NbtWebApp/app/ChemSW.js" />
/// <reference path="../../NbtWebApp/app/Main.js" />


module('setup');
test('ChemSW internetExplorerVersionNo', function () {
    var ienum = internetExplorerVersionNo;
    ok(ienum === -1, 'I can see ChemSW.internetExplorerVersionNo.');
});

test('initChemSW', function () {
    var chemsw = ChemSW;
    ok(chemsw, 'I can see ChemSW.');
    var csw = Csw;
    ok(csw, 'If I can see ChemSW, I should also be able to see Csw.');
    ok(chemsw.tryExec !== null, 'I can see ChemSW\'s cswPublic methods.');
});

test('initMain', function () {
    ok(initMain, 'I can see initMain.');
    var main = window.initMain();//...but this fails because Csw is undefined
    ok(main !== null, 'main undefined');
});

test('enum', function () {
    var edit = Csw.enums.editMode.Add; //this fails because Csw is undefined
    equals(edit, 'Add', 'Subtracting 1 from 5 gives 4');
});